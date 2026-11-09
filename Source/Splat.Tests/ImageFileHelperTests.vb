#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports Microsoft.VisualStudio.TestTools.UnitTesting

#End Region

#Region " ImageFileHelperTests "

''' <summary>
''' Unit tests for the <see cref="ImageFileHelper"/> module.
''' </summary>
<TestClass>
Public NotInheritable Class ImageFileHelperTests

    <TestMethod>
    <DataRow("C:\Image.png", True)>
    <DataRow("C:\Image.PNG", True)>
    <DataRow("C:\Image.ico", True)>
    <DataRow("C:\Image.Ico", True)>
    <DataRow("Image.png ", True)>
    <DataRow("C:\Image.jpg", False)>
    <DataRow("C:\Image.jpeg", False)>
    <DataRow("C:\Image.bmp", False)>
    <DataRow("C:\Image.gif", False)>
    <DataRow("C:\Image.tif", False)>
    <DataRow("C:\Image.png.jpg", False)>
    <DataRow("C:\Image.apng", False)>
    <DataRow("C:\png", False)>
    <DataRow("C:\Folder.png\Image", False)>
    <DataRow("", False)>
    Public Sub UsesPerPixelAlpha_ReturnsExpected(filePath As String, expected As Boolean)

        Assert.AreEqual(expected, ImageFileHelper.UsesPerPixelAlpha(filePath))

    End Sub

    <TestMethod>
    Public Sub UsesPerPixelAlpha_Null_Throws()

        Assert.ThrowsExactly(Of ArgumentNullException)(Sub() ImageFileHelper.UsesPerPixelAlpha(Nothing))

    End Sub

End Class

#End Region
