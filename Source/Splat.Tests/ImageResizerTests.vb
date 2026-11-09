#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Drawing
Imports System.Drawing.Imaging

Imports Microsoft.VisualStudio.TestTools.UnitTesting

Imports Splat.Tests.TestHelpers

#End Region

#Region " ImageResizerTests "

''' <summary>
''' Unit tests for the <see cref="ImageResizer"/> module.
''' </summary>
<TestClass>
Public NotInheritable Class ImageResizerTests

#Region " ToArgb32 "

    <TestMethod>
    Public Sub ToArgb32_PreservesSizeAndPixelsAndReturnsArgbFormat()

        Using source As Bitmap = TestImages.CreateFilled(5, 3, Color.FromArgb(128, 10, 20, 30))
            source.SetPixel(4, 2, Color.FromArgb(0, 0, 0, 0))

            Using result As Bitmap = ImageResizer.ToArgb32(source)
                Assert.AreEqual(PixelFormat.Format32bppArgb, result.PixelFormat)
                Assert.AreEqual(source.Size, result.Size)
                ' GDI+ blends through premultiplied alpha, so semi-transparent channels can differ by a rounding unit.
                Dim pixel As Color = result.GetPixel(0, 0)
                Assert.AreEqual(128, CInt(pixel.A), 1)
                Assert.AreEqual(10, CInt(pixel.R), 1)
                Assert.AreEqual(20, CInt(pixel.G), 1)
                Assert.AreEqual(30, CInt(pixel.B), 1)
                Assert.AreEqual(0, CInt(result.GetPixel(4, 2).A))
            End Using
        End Using

    End Sub

    <TestMethod>
    Public Sub ToArgb32_NonArgbSource_ReturnsArgbFormat()

        Using source As New Bitmap(4, 4, PixelFormat.Format24bppRgb)
            Using result As Bitmap = ImageResizer.ToArgb32(source)
                Assert.AreEqual(PixelFormat.Format32bppArgb, result.PixelFormat)
                Assert.AreEqual(Byte.MaxValue, result.GetPixel(0, 0).A)
            End Using
        End Using

    End Sub

    <TestMethod>
    Public Sub ToArgb32_ReturnsNewInstance()

        Using source As Bitmap = TestImages.CreateFilled(2, 2, Color.Red)
            Using result As Bitmap = ImageResizer.ToArgb32(source)
                Assert.AreNotSame(source, result)
            End Using
        End Using

    End Sub

    <TestMethod>
    Public Sub ToArgb32_Null_Throws()

        Assert.ThrowsExactly(Of ArgumentNullException)(Sub() ImageResizer.ToArgb32(Nothing))

    End Sub

#End Region

#Region " Resize "

    <TestMethod>
    <DataRow(40, 20)>
    <DataRow(5, 50)>
    <DataRow(1, 1)>
    <DataRow(100, 100)>
    Public Sub Resize_ReturnsRequestedSizeAndArgbFormat(width As Integer, height As Integer)

        Using source As Bitmap = TestImages.CreateFilled(10, 10, Color.Red)
            Using result As Bitmap = ImageResizer.Resize(source, New Size(width, height))
                Assert.AreEqual(New Size(width, height), result.Size)
                Assert.AreEqual(PixelFormat.Format32bppArgb, result.PixelFormat)
            End Using
        End Using

    End Sub

    <TestMethod>
    Public Sub Resize_UniformOpaqueImage_HasNoSemiTransparentEdges()

        ' Regression test: scaling with GDI+ default wrap mode blends the image edges with transparent pixels.
        TestEnvironment.RequireWindowsGdiPlus()

        Using source As Bitmap = TestImages.CreateFilled(10, 10, Color.Red)
            Using result As Bitmap = ImageResizer.Resize(source, New Size(37, 23))
                For y As Integer = 0 To result.Height - 1
                    For x As Integer = 0 To result.Width - 1
                        Dim pixel As Color = result.GetPixel(x, y)
                        Assert.AreEqual(Byte.MaxValue, pixel.A, $"Pixel ({x},{y}) is not opaque.")
                        Assert.AreEqual(Color.Red.ToArgb(), pixel.ToArgb(), $"Pixel ({x},{y}) has a wrong color.")
                    Next
                Next
            End Using
        End Using

    End Sub

    <TestMethod>
    Public Sub Resize_FullyTransparentImage_RemainsFullyTransparent()

        Using source As Bitmap = TestImages.CreateFilled(10, 10, Color.Transparent)
            Using result As Bitmap = ImageResizer.Resize(source, New Size(25, 15))
                For y As Integer = 0 To result.Height - 1
                    For x As Integer = 0 To result.Width - 1
                        Assert.AreEqual(0, CInt(result.GetPixel(x, y).A), $"Pixel ({x},{y}) is not transparent.")
                    Next
                Next
            End Using
        End Using

    End Sub

    <TestMethod>
    Public Sub Resize_IgnoresSourceResolution()

        TestEnvironment.RequireWindowsGdiPlus()

        Using source As Bitmap = TestImages.CreateFilled(10, 10, Color.Blue)
            source.SetResolution(300.0F, 300.0F)

            Using result As Bitmap = ImageResizer.Resize(source, New Size(20, 20))
                Assert.AreEqual(Color.Blue.ToArgb(), result.GetPixel(19, 19).ToArgb())
            End Using
        End Using

    End Sub

    <TestMethod>
    <DataRow(0, 10)>
    <DataRow(10, 0)>
    <DataRow(-1, 10)>
    <DataRow(10, -1)>
    Public Sub Resize_InvalidSize_Throws(width As Integer, height As Integer)

        Using source As Bitmap = TestImages.CreateFilled(2, 2, Color.Red)
            Assert.ThrowsExactly(Of ArgumentOutOfRangeException)(Sub() ImageResizer.Resize(source, New Size(width, height)))
        End Using

    End Sub

    <TestMethod>
    Public Sub Resize_Null_Throws()

        Assert.ThrowsExactly(Of ArgumentNullException)(Sub() ImageResizer.Resize(Nothing, New Size(1, 1)))

    End Sub

#End Region

End Class

#End Region
