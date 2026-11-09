#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Drawing

Imports Microsoft.VisualStudio.TestTools.UnitTesting

Imports Splat.Tests.TestHelpers

#End Region

#Region " SplashRequestTests "

''' <summary>
''' Unit tests for the <see cref="SplashRequest"/> class.
''' </summary>
<TestClass>
Public NotInheritable Class SplashRequestTests

    <TestMethod>
    Public Sub Constructor_StoresValues()

        Dim options As New SplatOptions()
        Using bitmap As Bitmap = TestImages.CreateFilled(2, 2, Color.Red)
            Dim request As New SplashRequest(options, bitmap, usesPerPixelAlpha:=False, transparencyKey:=Color.Lime)

            Assert.AreSame(options, request.Options)
            Assert.AreSame(bitmap, request.Image)
            Assert.IsFalse(request.UsesPerPixelAlpha)
            Assert.AreEqual(Color.Lime, request.TransparencyKey)
        End Using

    End Sub

    <TestMethod>
    Public Sub Constructor_NullOptions_Throws()

        Using bitmap As Bitmap = TestImages.CreateFilled(1, 1, Color.Red)
            Assert.ThrowsExactly(Of ArgumentNullException)(Function() New SplashRequest(Nothing, bitmap, False, Color.Empty))
        End Using

    End Sub

    <TestMethod>
    Public Sub Constructor_NullImage_Throws()

        Assert.ThrowsExactly(Of ArgumentNullException)(Function() New SplashRequest(New SplatOptions(), Nothing, False, Color.Empty))

    End Sub

    <TestMethod>
    Public Sub Dispose_DisposesImage()

        Dim bitmap As Bitmap = TestImages.CreateFilled(1, 1, Color.Red)
        Dim request As New SplashRequest(New SplatOptions(), bitmap, False, Color.Empty)

        request.Dispose()

        ' Accessing the size of a disposed GDI+ image throws ArgumentException.
        Assert.ThrowsExactly(Of ArgumentException)(Function() bitmap.Width)

    End Sub

End Class

#End Region
