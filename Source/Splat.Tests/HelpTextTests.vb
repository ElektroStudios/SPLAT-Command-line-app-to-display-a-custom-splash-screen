#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports Microsoft.VisualStudio.TestTools.UnitTesting

#End Region

#Region " HelpTextTests "

''' <summary>
''' Unit tests for the <see cref="HelpText"/> module.
''' </summary>
<TestClass>
Public NotInheritable Class HelpTextTests

    <TestMethod>
    Public Sub Value_IsNotEmpty()

        Assert.IsFalse(String.IsNullOrWhiteSpace(HelpText.Value))

    End Sub

    <TestMethod>
    <DataRow(CommandLineParser.SwitchDuration)>
    <DataRow(CommandLineParser.SwitchEffectsDuration)>
    <DataRow(CommandLineParser.SwitchFadeIn)>
    <DataRow(CommandLineParser.SwitchFadeOut)>
    <DataRow(CommandLineParser.SwitchResize)>
    <DataRow(CommandLineParser.SwitchLocation)>
    <DataRow(CommandLineParser.SwitchClickable)>
    <DataRow(CommandLineParser.SwitchOnTop)>
    <DataRow("/?")>
    Public Sub Value_DocumentsEverySwitch(switchName As String)

        Assert.Contains(switchName, HelpText.Value)

    End Sub

    <TestMethod>
    Public Sub Value_ShowsRepositoryUrl()

        Assert.Contains(HelpText.RepositoryUrl, HelpText.Value)

    End Sub

    <TestMethod>
    Public Sub Value_ShowsAssemblyVersion()

        Dim version As Version = GetType(SplatOptions).Assembly.GetName().Version
        Assert.Contains($"v{version.Major}.{version.Minor}", HelpText.Value)

    End Sub

    <TestMethod>
    Public Sub Value_DocumentsDefaultDurations()

        Assert.Contains($"(Default: {SplatOptions.DefaultDurationMilliseconds} ms)", HelpText.Value)
        Assert.Contains($"(Default: {SplatOptions.DefaultEffectsDurationMilliseconds} ms)", HelpText.Value)

    End Sub

    <TestMethod>
    Public Sub Value_DocumentsEveryExitCode()

        For Each exitCode As SplatExitCode In [Enum].GetValues(GetType(SplatExitCode))
            Assert.Contains($"    {CInt(exitCode)} | ", HelpText.Value)
        Next

    End Sub

    <TestMethod>
    Public Sub Value_ExamplesAreValidCommandLines()

        ' Every documented example must be accepted by the parser.
        Dim examples As String()() = {
            New String() {"C:\Image.png"},
            New String() {"C:\Image.png", "/Resize", "200x400", "/Location", "100,300"},
            New String() {"C:\Image.jpg", "/Duration", "6000", "/FadeIN", "/FadeOUT", "/FXDuration", "2000"}
        }

        For Each example As String() In examples
            Assert.Contains(String.Join(" ", example).Replace("C:\Image", """C:\Image").Replace(".png", ".png""").Replace(".jpg", ".jpg"""), HelpText.Value)
            Assert.IsFalse(CommandLineParser.Parse(example).ShowHelp)
        Next

    End Sub

End Class

#End Region
