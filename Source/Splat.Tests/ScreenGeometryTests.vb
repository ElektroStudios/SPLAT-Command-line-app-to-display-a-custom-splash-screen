#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Drawing

Imports Microsoft.VisualStudio.TestTools.UnitTesting

#End Region

#Region " ScreenGeometryTests "

''' <summary>
''' Unit tests for the <see cref="ScreenGeometry"/> module.
''' </summary>
<TestClass>
Public NotInheritable Class ScreenGeometryTests

    <TestMethod>
    Public Sub GetCenteredLocation_WindowSmallerThanScreen_IsCentered()

        Dim location As Point = ScreenGeometry.GetCenteredLocation(New Rectangle(0, 0, 1920, 1080), New Size(200, 400))

        Assert.AreEqual(New Point(860, 340), location)

    End Sub

    <TestMethod>
    Public Sub GetCenteredLocation_OddRemainder_IsTruncated()

        Dim location As Point = ScreenGeometry.GetCenteredLocation(New Rectangle(0, 0, 1920, 1080), New Size(201, 401))

        Assert.AreEqual(New Point(859, 339), location)

    End Sub

    <TestMethod>
    Public Sub GetCenteredLocation_ScreenWithOffset_IncludesOffset()

        Dim location As Point = ScreenGeometry.GetCenteredLocation(New Rectangle(-1920, 100, 1920, 1080), New Size(200, 400))

        Assert.AreEqual(New Point(-1060, 440), location)

    End Sub

    <TestMethod>
    Public Sub GetCenteredLocation_WindowSameSizeAsScreen_IsAtOrigin()

        Dim location As Point = ScreenGeometry.GetCenteredLocation(New Rectangle(0, 0, 1920, 1080), New Size(1920, 1080))

        Assert.AreEqual(Point.Empty, location)

    End Sub

    <TestMethod>
    Public Sub GetCenteredLocation_WindowLargerThanScreen_IsStillCentered()

        Dim location As Point = ScreenGeometry.GetCenteredLocation(New Rectangle(0, 0, 1000, 1000), New Size(1200, 1100))

        Assert.AreEqual(New Point(-100, -50), location)

    End Sub

End Class

#End Region
