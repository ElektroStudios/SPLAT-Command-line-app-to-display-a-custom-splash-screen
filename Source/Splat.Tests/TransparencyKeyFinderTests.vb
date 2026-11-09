#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Collections.Generic
Imports System.Drawing
Imports System.Drawing.Imaging

Imports Microsoft.VisualStudio.TestTools.UnitTesting

Imports Splat.Tests.TestHelpers

#End Region

#Region " TransparencyKeyFinderTests "

''' <summary>
''' Unit tests for the <see cref="TransparencyKeyFinder"/> module.
''' </summary>
<TestClass>
Public NotInheritable Class TransparencyKeyFinderTests

#Region " Helpers "

    ''' <summary>
    ''' Gets the first opaque <see cref="KnownColor"/>, in enumeration order.
    ''' </summary>
    '''
    ''' <returns>
    ''' The first opaque known color.
    ''' </returns>
    Private Shared Function GetFirstOpaqueKnownColor() As Color

        For Each knownColor As KnownColor In [Enum].GetValues(GetType(KnownColor))
            Dim color As Color = Color.FromKnownColor(knownColor)
            If color.A = Byte.MaxValue Then
                Return color
            End If
        Next
        Throw New InvalidOperationException("No opaque known color found.")

    End Function

    ''' <summary>
    ''' Gets the first opaque color (in RGB order) whose RGB value does not match any <see cref="KnownColor"/>.
    ''' </summary>
    '''
    ''' <returns>
    ''' The first opaque non-known color.
    ''' </returns>
    Private Shared Function GetFirstNonKnownColor() As Color

        Dim knownRgbValues As New HashSet(Of Integer)()
        For Each knownColor As KnownColor In [Enum].GetValues(GetType(KnownColor))
            knownRgbValues.Add(Color.FromKnownColor(knownColor).ToArgb() And &HFFFFFF)
        Next

        For rgb As Integer = 1 To &HFFFFFF
            If Not knownRgbValues.Contains(rgb) Then
                Return Color.FromArgb(Byte.MaxValue, Color.FromArgb(rgb))
            End If
        Next
        Throw New InvalidOperationException("No non-known color found.")

    End Function

    ''' <summary>
    ''' Creates a bitmap that contains one pixel of every distinct opaque <see cref="KnownColor"/> RGB value.
    ''' </summary>
    '''
    ''' <returns>
    ''' The resulting <see cref="Bitmap"/>. The caller is responsible for disposing it.
    ''' </returns>
    Private Shared Function CreateBitmapWithEveryKnownColor() As Bitmap

        Dim rgbValues As New HashSet(Of Integer)()
        For Each knownColor As KnownColor In [Enum].GetValues(GetType(KnownColor))
            Dim color As Color = Color.FromKnownColor(knownColor)
            If color.A = Byte.MaxValue Then
                rgbValues.Add(color.ToArgb() And &HFFFFFF)
            End If
        Next

        Dim bitmap As New Bitmap(rgbValues.Count, 1, PixelFormat.Format32bppArgb)
        Dim x As Integer = 0
        For Each rgb As Integer In rgbValues
            bitmap.SetPixel(x, 0, Color.FromArgb(Byte.MaxValue, Color.FromArgb(rgb)))
            x += 1
        Next
        Return bitmap

    End Function

#End Region

#Region " FindUnusedColor "

    <TestMethod>
    Public Sub FindUnusedColor_ImageWithoutKnownColors_ReturnsFirstOpaqueKnownColor()

        Dim expected As Color = TransparencyKeyFinderTests.GetFirstOpaqueKnownColor()
        Dim fillColor As Color = TransparencyKeyFinderTests.GetFirstNonKnownColor()

        Using bitmap As Bitmap = TestImages.CreateFilled(8, 8, fillColor)
            Dim result As Color = TransparencyKeyFinder.FindUnusedColor(bitmap)

            Assert.AreEqual(expected.ToArgb(), result.ToArgb())
        End Using

    End Sub

    <TestMethod>
    Public Sub FindUnusedColor_ImageFilledWithFirstKnownColor_ReturnsAnotherColor()

        Dim firstKnownColor As Color = TransparencyKeyFinderTests.GetFirstOpaqueKnownColor()

        Using bitmap As Bitmap = TestImages.CreateFilled(8, 8, firstKnownColor)
            Dim result As Color = TransparencyKeyFinder.FindUnusedColor(bitmap)

            Assert.AreNotEqual(firstKnownColor.ToArgb() And &HFFFFFF, result.ToArgb() And &HFFFFFF)
        End Using

    End Sub

    <TestMethod>
    <DataRow(&HFF000000)>
    <DataRow(&HFFFFFFFF)>
    <DataRow(&HFF9ACD32)>
    <DataRow(&HFF123456)>
    Public Sub FindUnusedColor_ReturnsOpaqueColorNotUsedByImage(argb As Integer)

        Using bitmap As Bitmap = TestImages.CreateFilled(4, 4, Color.FromArgb(argb))
            Dim result As Color = TransparencyKeyFinder.FindUnusedColor(bitmap)
            Dim usedColors As HashSet(Of Integer) = TransparencyKeyFinder.GetVisibleRgbValues(bitmap)

            Assert.AreEqual(Byte.MaxValue, result.A)
            Assert.DoesNotContain(result.ToArgb() And &HFFFFFF, usedColors)
        End Using

    End Sub

    <TestMethod>
    Public Sub FindUnusedColor_FullyTransparentPixels_AreIgnored()

        ' Fully transparent pixels whose RGB matches the first known color must not prevent its use.
        Dim firstKnownColor As Color = TransparencyKeyFinderTests.GetFirstOpaqueKnownColor()
        Dim transparentColor As Color = Color.FromArgb(0, firstKnownColor)

        Using bitmap As Bitmap = TestImages.CreateFilled(8, 8, transparentColor)
            Dim result As Color = TransparencyKeyFinder.FindUnusedColor(bitmap)

            Assert.AreEqual(firstKnownColor.ToArgb(), result.ToArgb())
        End Using

    End Sub

    <TestMethod>
    Public Sub FindUnusedColor_NeverReturnsTransparent()

        Using bitmap As Bitmap = TestImages.CreateFilled(2, 2, Color.Transparent)
            Dim result As Color = TransparencyKeyFinder.FindUnusedColor(bitmap)

            Assert.AreEqual(Byte.MaxValue, result.A)
        End Using

    End Sub

    <TestMethod>
    Public Sub FindUnusedColor_EveryKnownColorUsed_FallsBackToUnusedRgbValue()

        Using bitmap As Bitmap = TransparencyKeyFinderTests.CreateBitmapWithEveryKnownColor()
            Dim result As Color = TransparencyKeyFinder.FindUnusedColor(bitmap)
            Dim usedColors As HashSet(Of Integer) = TransparencyKeyFinder.GetVisibleRgbValues(bitmap)

            Assert.AreEqual(Byte.MaxValue, result.A)
            Assert.DoesNotContain(result.ToArgb() And &HFFFFFF, usedColors)
            Assert.IsFalse(result.IsKnownColor)
        End Using

    End Sub

    <TestMethod>
    Public Sub FindUnusedColor_Null_Throws()

        Assert.ThrowsExactly(Of ArgumentNullException)(Sub() TransparencyKeyFinder.FindUnusedColor(Nothing))

    End Sub

#End Region

#Region " GetVisibleRgbValues "

    <TestMethod>
    Public Sub GetVisibleRgbValues_ReturnsDistinctRgbValuesOfVisiblePixels()

        Using bitmap As New Bitmap(3, 2, PixelFormat.Format32bppArgb)
            bitmap.SetPixel(0, 0, Color.FromArgb(255, 255, 0, 0))
            bitmap.SetPixel(1, 0, Color.FromArgb(255, 255, 0, 0))
            bitmap.SetPixel(2, 0, Color.FromArgb(128, 0, 255, 0))  ' Semi-transparent: visible.
            bitmap.SetPixel(0, 1, Color.FromArgb(0, 0, 0, 255))    ' Fully transparent: ignored.
            bitmap.SetPixel(1, 1, Color.FromArgb(255, 1, 2, 3))
            bitmap.SetPixel(2, 1, Color.FromArgb(255, 1, 2, 3))

            Dim result As HashSet(Of Integer) = TransparencyKeyFinder.GetVisibleRgbValues(bitmap)

            Assert.AreEqual(3, result.Count)
            Assert.Contains(&HFF0000, result)
            Assert.Contains(&HFF00, result)
            Assert.Contains(&H10203, result)
            Assert.DoesNotContain(&HFF, result)
        End Using

    End Sub

    <TestMethod>
    Public Sub GetVisibleRgbValues_ReadsEveryRowAndColumn()

        ' Odd width, to exercise the row stride handling.
        Using bitmap As Bitmap = TestImages.CreateFilled(7, 5, Color.Black)
            bitmap.SetPixel(6, 4, Color.FromArgb(255, 10, 20, 30)) ' Last pixel.
            bitmap.SetPixel(0, 0, Color.FromArgb(255, 40, 50, 60)) ' First pixel.

            Dim result As HashSet(Of Integer) = TransparencyKeyFinder.GetVisibleRgbValues(bitmap)

            Assert.AreEqual(3, result.Count)
            Assert.Contains(&HA141E, result)
            Assert.Contains(&H28323C, result)
        End Using

    End Sub

    <TestMethod>
    Public Sub GetVisibleRgbValues_IgnoresImageResolution()

        ' Regression test: drawing the image with its own DPI could scale it, so pixels were missed.
        Using bitmap As Bitmap = TestImages.CreateFilled(10, 10, Color.Black)
            bitmap.SetPixel(9, 9, Color.FromArgb(255, 1, 1, 1))
            bitmap.SetResolution(300.0F, 300.0F)

            Dim result As HashSet(Of Integer) = TransparencyKeyFinder.GetVisibleRgbValues(bitmap)

            Assert.Contains(&H10101, result)
        End Using

    End Sub

    <TestMethod>
    Public Sub GetVisibleRgbValues_Null_Throws()

        Assert.ThrowsExactly(Of ArgumentNullException)(Sub() TransparencyKeyFinder.GetVisibleRgbValues(Nothing))

    End Sub

#End Region

End Class

#End Region
