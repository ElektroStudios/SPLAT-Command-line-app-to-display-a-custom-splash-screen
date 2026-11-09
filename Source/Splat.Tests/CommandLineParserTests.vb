#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Drawing

Imports Microsoft.VisualStudio.TestTools.UnitTesting

#End Region

#Region " CommandLineParserTests "

''' <summary>
''' Unit tests for the <see cref="CommandLineParser"/> module.
''' </summary>
<TestClass>
Public NotInheritable Class CommandLineParserTests

#Region " Constants "

    ''' <summary>
    ''' A sample image file path.
    ''' </summary>
    Private Const ImagePath As String = "C:\Image.png"

#End Region

#Region " Parse: help "

    <TestMethod>
    Public Sub Parse_NoArguments_ShowsHelp()

        Dim options As SplatOptions = CommandLineParser.Parse(Array.Empty(Of String)())

        Assert.IsTrue(options.ShowHelp)
        Assert.IsNull(options.ImageFilePath)

    End Sub

    <TestMethod>
    <DataRow("/?")>
    <DataRow("-?")>
    <DataRow("/h")>
    <DataRow("-h")>
    <DataRow("-H")>
    <DataRow("/help")>
    <DataRow("/HELP")>
    <DataRow("-help")>
    <DataRow("--help")>
    Public Sub Parse_HelpSwitchAsFirstArgument_ShowsHelp(helpSwitch As String)

        Dim options As SplatOptions = CommandLineParser.Parse({helpSwitch})

        Assert.IsTrue(options.ShowHelp)

    End Sub

    <TestMethod>
    Public Sub Parse_HelpSwitchAfterOtherSwitches_ShowsHelp()

        Dim options As SplatOptions = CommandLineParser.Parse({ImagePath, "/FadeIN", "/?"})

        Assert.IsTrue(options.ShowHelp)

    End Sub

    <TestMethod>
    Public Sub Parse_HelpSwitchTogetherWithInvalidArguments_ShowsHelpInsteadOfError()

        Dim options As SplatOptions = CommandLineParser.Parse({ImagePath, "/Unknown", "/Duration", "abc", "/?"})

        Assert.IsTrue(options.ShowHelp)

    End Sub

#End Region

#Region " Parse: image path and defaults "

    <TestMethod>
    Public Sub Parse_OnlyImagePath_ReturnsDefaultOptions()

        Dim options As SplatOptions = CommandLineParser.Parse({ImagePath})

        Assert.IsFalse(options.ShowHelp)
        Assert.AreEqual(ImagePath, options.ImageFilePath)
        Assert.AreEqual(SplatOptions.DefaultDurationMilliseconds, options.DurationMilliseconds)
        Assert.AreEqual(SplatOptions.DefaultEffectsDurationMilliseconds, options.EffectsDurationMilliseconds)
        Assert.IsFalse(options.FadeIn)
        Assert.IsFalse(options.FadeOut)
        Assert.IsFalse(options.Size.HasValue)
        Assert.IsFalse(options.Location.HasValue)
        Assert.IsFalse(options.Clickable)
        Assert.IsFalse(options.OnTop)

    End Sub

    <TestMethod>
    Public Sub Parse_ImagePathWithSurroundingWhitespace_IsTrimmed()

        Dim options As SplatOptions = CommandLineParser.Parse({$"  {ImagePath}  "})

        Assert.AreEqual(ImagePath, options.ImageFilePath)

    End Sub

    <TestMethod>
    <DataRow("")>
    <DataRow("   ")>
    Public Sub Parse_EmptyImagePath_Throws(imagePath As String)

        Dim ex As CommandLineArgumentException =
            Assert.ThrowsExactly(Of CommandLineArgumentException)(Sub() CommandLineParser.Parse({imagePath}))

        Assert.AreEqual("The image file path cannot be empty.", ex.Message)

    End Sub

    <TestMethod>
    Public Sub Parse_NullArguments_Throws()

        Assert.ThrowsExactly(Of ArgumentNullException)(Sub() CommandLineParser.Parse(Nothing))

    End Sub

    <TestMethod>
    Public Sub Parse_NullItemInArguments_Throws()

        Assert.ThrowsExactly(Of ArgumentNullException)(Sub() CommandLineParser.Parse({ImagePath, Nothing}))

    End Sub

#End Region

#Region " Parse: flag switches "

    <TestMethod>
    Public Sub Parse_AllFlagSwitches_AreEnabled()

        Dim options As SplatOptions = CommandLineParser.Parse({ImagePath, "/FadeIN", "/FadeOUT", "/Clickable", "/OnTop"})

        Assert.IsTrue(options.FadeIn)
        Assert.IsTrue(options.FadeOut)
        Assert.IsTrue(options.Clickable)
        Assert.IsTrue(options.OnTop)

    End Sub

    <TestMethod>
    Public Sub Parse_SwitchNames_AreCaseInsensitive()

        Dim options As SplatOptions = CommandLineParser.Parse(
            {ImagePath, "/fadein", "/FADEOUT", "/cLiCkAbLe", "/ontop", "/DURATION", "100", "/fxduration", "200",
             "/RESIZE", "10X20", "/location", "1,2"})

        Assert.IsTrue(options.FadeIn)
        Assert.IsTrue(options.FadeOut)
        Assert.IsTrue(options.Clickable)
        Assert.IsTrue(options.OnTop)
        Assert.AreEqual(100, options.DurationMilliseconds)
        Assert.AreEqual(200, options.EffectsDurationMilliseconds)
        Assert.AreEqual(New Size(10, 20), options.Size.Value)
        Assert.AreEqual(New Point(1, 2), options.Location.Value)

    End Sub

    <TestMethod>
    Public Sub Parse_RepeatedFlagSwitch_IsAccepted()

        Dim options As SplatOptions = CommandLineParser.Parse({ImagePath, "/Clickable", "/Clickable"})

        Assert.IsTrue(options.Clickable)

    End Sub

#End Region

#Region " Parse: valued switches "

    <TestMethod>
    Public Sub Parse_Duration_IsParsed()

        Dim options As SplatOptions = CommandLineParser.Parse({ImagePath, "/Duration", "6000"})

        Assert.AreEqual(6000, options.DurationMilliseconds)

    End Sub

    <TestMethod>
    Public Sub Parse_EffectsDuration_IsParsed()

        Dim options As SplatOptions = CommandLineParser.Parse({ImagePath, "/FXDuration", "2000"})

        Assert.AreEqual(2000, options.EffectsDurationMilliseconds)

    End Sub

    <TestMethod>
    Public Sub Parse_EffectsDurationZero_IsAccepted()

        Dim options As SplatOptions = CommandLineParser.Parse({ImagePath, "/FXDuration", "0"})

        Assert.AreEqual(0, options.EffectsDurationMilliseconds)

    End Sub

    <TestMethod>
    Public Sub Parse_DurationZero_Throws()

        Assert.ThrowsExactly(Of CommandLineArgumentException)(Sub() CommandLineParser.Parse({ImagePath, "/Duration", "0"}))

    End Sub

    <TestMethod>
    Public Sub Parse_Resize_IsParsed()

        Dim options As SplatOptions = CommandLineParser.Parse({ImagePath, "/Resize", "200x400"})

        Assert.AreEqual(New Size(200, 400), options.Size.Value)

    End Sub

    <TestMethod>
    Public Sub Parse_Location_IsParsed()

        Dim options As SplatOptions = CommandLineParser.Parse({ImagePath, "/Location", "100,300"})

        Assert.AreEqual(New Point(100, 300), options.Location.Value)

    End Sub

    <TestMethod>
    Public Sub Parse_LocationWithZeroX_IsKept()

        ' Regression test: "/Location 0,Y" was ignored for PNG/ICO images because 0 was treated as "not set".
        Dim options As SplatOptions = CommandLineParser.Parse({ImagePath, "/Location", "0,300"})

        Assert.IsTrue(options.Location.HasValue)
        Assert.AreEqual(New Point(0, 300), options.Location.Value)

    End Sub

    <TestMethod>
    Public Sub Parse_LocationAndResize_AreIndependentOfTheirOrder()

        ' Regression test: "/Resize" after "/Location" re-centered the window, discarding the location.
        Dim locationFirst As SplatOptions = CommandLineParser.Parse({ImagePath, "/Location", "100,300", "/Resize", "200x400"})
        Dim resizeFirst As SplatOptions = CommandLineParser.Parse({ImagePath, "/Resize", "200x400", "/Location", "100,300"})

        Assert.AreEqual(locationFirst.Location.Value, resizeFirst.Location.Value)
        Assert.AreEqual(locationFirst.Size.Value, resizeFirst.Size.Value)

    End Sub

    <TestMethod>
    Public Sub Parse_RepeatedValuedSwitch_LastValueWins()

        Dim options As SplatOptions = CommandLineParser.Parse({ImagePath, "/Duration", "1000", "/Duration", "2000"})

        Assert.AreEqual(2000, options.DurationMilliseconds)

    End Sub

    <TestMethod>
    Public Sub Parse_AllSwitchesTogether_AreParsed()

        Dim options As SplatOptions = CommandLineParser.Parse(
            {ImagePath, "/Duration", "6000", "/FadeIN", "/FadeOUT", "/FXDuration", "2000",
             "/Resize", "200x400", "/Location", "100,300", "/Clickable", "/OnTop"})

        Assert.AreEqual(ImagePath, options.ImageFilePath)
        Assert.AreEqual(6000, options.DurationMilliseconds)
        Assert.AreEqual(2000, options.EffectsDurationMilliseconds)
        Assert.IsTrue(options.FadeIn)
        Assert.IsTrue(options.FadeOut)
        Assert.AreEqual(New Size(200, 400), options.Size.Value)
        Assert.AreEqual(New Point(100, 300), options.Location.Value)
        Assert.IsTrue(options.Clickable)
        Assert.IsTrue(options.OnTop)

    End Sub

#End Region

#Region " Parse: errors "

    <TestMethod>
    <DataRow("/Duration")>
    <DataRow("/FXDuration")>
    <DataRow("/Resize")>
    <DataRow("/Location")>
    Public Sub Parse_ValuedSwitchWithoutValue_Throws(switchName As String)

        ' Regression test: a valued switch at the end of the command-line crashed the application.
        Dim ex As CommandLineArgumentException =
            Assert.ThrowsExactly(Of CommandLineArgumentException)(Sub() CommandLineParser.Parse({ImagePath, switchName}))

        Assert.AreEqual($"Missing value for {switchName}.", ex.Message)

    End Sub

    <TestMethod>
    Public Sub Parse_ValuedSwitchFollowedByAnotherSwitch_Throws()

        Dim ex As CommandLineArgumentException =
            Assert.ThrowsExactly(Of CommandLineArgumentException)(Sub() CommandLineParser.Parse({ImagePath, "/Duration", "/FadeIN"}))

        Assert.Contains("/Duration", ex.Message)
        Assert.Contains("""/FadeIN""", ex.Message)

    End Sub

    <TestMethod>
    <DataRow("/Unknown")>
    <DataRow("/Durration")>
    <DataRow("extra")>
    <DataRow("5000")>
    Public Sub Parse_UnknownArgument_Throws(argument As String)

        Dim ex As CommandLineArgumentException =
            Assert.ThrowsExactly(Of CommandLineArgumentException)(Sub() CommandLineParser.Parse({ImagePath, argument}))

        Assert.AreEqual($"Unknown argument: ""{argument}"".", ex.Message)

    End Sub

#End Region

#Region " IsHelpSwitch "

    <TestMethod>
    <DataRow("/?", True)>
    <DataRow(" /? ", True)>
    <DataRow("-HELP", True)>
    <DataRow("/Duration", False)>
    <DataRow("?", False)>
    <DataRow("", False)>
    <DataRow("C:\Image.png", False)>
    Public Sub IsHelpSwitch_ReturnsExpected(argument As String, expected As Boolean)

        Assert.AreEqual(expected, CommandLineParser.IsHelpSwitch(argument))

    End Sub

    <TestMethod>
    Public Sub IsHelpSwitch_Null_Throws()

        Assert.ThrowsExactly(Of ArgumentNullException)(Sub() CommandLineParser.IsHelpSwitch(Nothing))

    End Sub

#End Region

#Region " ParseMilliseconds "

    <TestMethod>
    <DataRow("0", 0, 0)>
    <DataRow("1", 1, 1)>
    <DataRow("5000", 1, 5000)>
    <DataRow(" 250 ", 0, 250)>
    <DataRow("2147483647", 1, Integer.MaxValue)>
    Public Sub ParseMilliseconds_ValidValue_ReturnsValue(value As String, minimum As Integer, expected As Integer)

        Assert.AreEqual(expected, CommandLineParser.ParseMilliseconds("/Duration", value, minimum))

    End Sub

    <TestMethod>
    <DataRow("", 0)>
    <DataRow("abc", 0)>
    <DataRow("-1", 0)>
    <DataRow("+5", 0)>
    <DataRow("1.5", 0)>
    <DataRow("1,5", 0)>
    <DataRow("1 000", 0)>
    <DataRow("2147483648", 0)>
    <DataRow("0", 1)>
    <DataRow("99", 100)>
    Public Sub ParseMilliseconds_InvalidValue_Throws(value As String, minimum As Integer)

        Dim ex As CommandLineArgumentException =
            Assert.ThrowsExactly(Of CommandLineArgumentException)(Sub() CommandLineParser.ParseMilliseconds("/Duration", value, minimum))

        Assert.StartsWith($"Wrong value specified for /Duration ""{value}"".", ex.Message)

    End Sub

    <TestMethod>
    Public Sub ParseMilliseconds_NullArguments_Throw()

        Assert.ThrowsExactly(Of ArgumentNullException)(Sub() CommandLineParser.ParseMilliseconds(Nothing, "1", 0))
        Assert.ThrowsExactly(Of ArgumentNullException)(Sub() CommandLineParser.ParseMilliseconds("/Duration", Nothing, 0))

    End Sub

#End Region

#Region " ParseSize "

    <TestMethod>
    <DataRow("200x400", 200, 400)>
    <DataRow("200X400", 200, 400)>
    <DataRow("1x1", 1, 1)>
    <DataRow(" 640x480 ", 640, 480)>
    Public Sub ParseSize_ValidValue_ReturnsSize(value As String, expectedWidth As Integer, expectedHeight As Integer)

        Assert.AreEqual(New Size(expectedWidth, expectedHeight), CommandLineParser.ParseSize("/Resize", value))

    End Sub

    <TestMethod>
    <DataRow("")>
    <DataRow("200")>
    <DataRow("200x")>
    <DataRow("x400")>
    <DataRow("200x400x600")>
    <DataRow("0x400")>
    <DataRow("200x0")>
    <DataRow("-200x400")>
    <DataRow("200*400")>
    <DataRow("200,400")>
    <DataRow("2.5x4")>
    <DataRow("abcxdef")>
    Public Sub ParseSize_InvalidValue_Throws(value As String)

        Dim ex As CommandLineArgumentException =
            Assert.ThrowsExactly(Of CommandLineArgumentException)(Sub() CommandLineParser.ParseSize("/Resize", value))

        Assert.StartsWith($"Wrong value specified for /Resize ""{value}"".", ex.Message)

    End Sub

#End Region

#Region " ParseLocation "

    <TestMethod>
    <DataRow("100,300", 100, 300)>
    <DataRow("0,0", 0, 0)>
    <DataRow("-1920,0", -1920, 0)>
    <DataRow("10,-20", 10, -20)>
    <DataRow(" 5,6 ", 5, 6)>
    Public Sub ParseLocation_ValidValue_ReturnsPoint(value As String, expectedX As Integer, expectedY As Integer)

        Assert.AreEqual(New Point(expectedX, expectedY), CommandLineParser.ParseLocation("/Location", value))

    End Sub

    <TestMethod>
    <DataRow("")>
    <DataRow("100")>
    <DataRow("100,")>
    <DataRow(",300")>
    <DataRow("1,2,3")>
    <DataRow("100x300")>
    <DataRow("100;300")>
    <DataRow("1.5,2")>
    <DataRow("a,b")>
    Public Sub ParseLocation_InvalidValue_Throws(value As String)

        Dim ex As CommandLineArgumentException =
            Assert.ThrowsExactly(Of CommandLineArgumentException)(Sub() CommandLineParser.ParseLocation("/Location", value))

        Assert.StartsWith($"Wrong value specified for /Location ""{value}"".", ex.Message)

    End Sub

#End Region

End Class

#End Region
