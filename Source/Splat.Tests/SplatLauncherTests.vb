#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Collections.Generic
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.IO

Imports Microsoft.VisualStudio.TestTools.UnitTesting

Imports Splat.Tests.TestHelpers

#End Region

#Region " SplatLauncherTests "

''' <summary>
''' Unit tests for the <see cref="SplatLauncher"/> module.
''' <para></para>
''' These tests exercise the whole command-line workflow without displaying any window,
''' by replacing the splash screen presenter with a fake one.
''' </summary>
<TestClass>
Public NotInheritable Class SplatLauncherTests

#Region " Fields "

    ''' <summary>
    ''' The temporary directory for the image files used by the current test.
    ''' </summary>
    Private tempDirectory As TemporaryDirectory

    ''' <summary>
    ''' The output written by the launcher during the current test.
    ''' </summary>
    Private output As StringWriter

    ''' <summary>
    ''' The number of times the fake presenter was invoked during the current test.
    ''' </summary>
    Private presenterCallCount As Integer

    ''' <summary>
    ''' A snapshot of the request received by the fake presenter during the current test.
    ''' </summary>
    Private received As RequestSnapshot

#End Region

#Region " Nested Types "

    ''' <summary>
    ''' A snapshot of the relevant data of a <see cref="SplashRequest"/>,
    ''' taken while the request is still alive (it is disposed when the presenter returns).
    ''' </summary>
    Private NotInheritable Class RequestSnapshot

        Friend Property Request As SplashRequest
        Friend Property Options As SplatOptions
        Friend Property UsesPerPixelAlpha As Boolean
        Friend Property TransparencyKey As Color
        Friend Property ImageSize As Size
        Friend Property PixelFormat As PixelFormat
        Friend Property ImageUsesTransparencyKey As Boolean

    End Class

#End Region

#Region " Setup / Cleanup "

    <TestInitialize>
    Public Sub TestInitialize()

        Me.tempDirectory = New TemporaryDirectory()
        Me.output = New StringWriter()
        Me.presenterCallCount = 0
        Me.received = Nothing

    End Sub

    <TestCleanup>
    Public Sub TestCleanup()

        Me.output.Dispose()
        Me.tempDirectory.Dispose()

    End Sub

#End Region

#Region " Helpers "

    ''' <summary>
    ''' A fake splash screen presenter that records the request and returns <see cref="SplatExitCode.Success"/>.
    ''' </summary>
    '''
    ''' <param name="request">
    ''' The splash screen request.
    ''' </param>
    '''
    ''' <returns>
    ''' <see cref="SplatExitCode.Success"/>.
    ''' </returns>
    Private Function FakePresenter(request As SplashRequest) As SplatExitCode

        Me.presenterCallCount += 1

        Dim usesKey As Boolean = False
        If Not request.UsesPerPixelAlpha Then
            usesKey = TransparencyKeyFinder.GetVisibleRgbValues(request.Image).Contains(request.TransparencyKey.ToArgb() And &HFFFFFF)
        End If

        Me.received = New RequestSnapshot With {
            .Request = request,
            .Options = request.Options,
            .UsesPerPixelAlpha = request.UsesPerPixelAlpha,
            .TransparencyKey = request.TransparencyKey,
            .ImageSize = request.Image.Size,
            .PixelFormat = request.Image.PixelFormat,
            .ImageUsesTransparencyKey = usesKey
        }

        Return SplatExitCode.Success

    End Function

    ''' <summary>
    ''' Runs the launcher with the specified arguments and the fake presenter.
    ''' </summary>
    '''
    ''' <param name="arguments">
    ''' The command-line arguments.
    ''' </param>
    '''
    ''' <returns>
    ''' The exit code.
    ''' </returns>
    Private Function Run(ParamArray arguments As String()) As SplatExitCode

        Return SplatLauncher.Run(arguments, Me.output, AddressOf Me.FakePresenter)

    End Function

    ''' <summary>
    ''' Creates a 20x10 test image file with the specified name and format.
    ''' </summary>
    '''
    ''' <param name="fileName">
    ''' The file name.
    ''' </param>
    '''
    ''' <param name="format">
    ''' The image file format.
    ''' </param>
    '''
    ''' <returns>
    ''' The full path of the created file.
    ''' </returns>
    Private Function CreateImageFile(fileName As String, format As ImageFormat) As String

        Dim filePath As String = Me.tempDirectory.GetFilePath(fileName)
        TestImages.SaveFilled(filePath, format, 20, 10, Color.FromArgb(200, 30, 60, 90))
        Return filePath

    End Function

#End Region

#Region " Help "

    <TestMethod>
    Public Sub Run_NoArguments_PrintsHelpAndReturnsSuccess()

        Dim exitCode As SplatExitCode = Me.Run()

        Assert.AreEqual(SplatExitCode.Success, exitCode)
        Assert.Contains(HelpText.Value, Me.output.ToString())
        Assert.AreEqual(0, Me.presenterCallCount)

    End Sub

    <TestMethod>
    Public Sub Run_HelpSwitchAsFirstArgument_PrintsHelpAndDoesNotShowSplash()

        ' Regression test: "Splat.exe /?" printed 'File "/?" don't exist.' instead of the help.
        Dim exitCode As SplatExitCode = Me.Run("/?")

        Assert.AreEqual(SplatExitCode.Success, exitCode)
        Assert.Contains(HelpText.Value, Me.output.ToString())
        Assert.DoesNotContain("File ""/?""", Me.output.ToString())
        Assert.AreEqual(0, Me.presenterCallCount)

    End Sub

    <TestMethod>
    Public Sub Run_HelpSwitchAfterValidImage_PrintsHelpAndDoesNotShowSplash()

        Dim filePath As String = Me.CreateImageFile("Image.png", ImageFormat.Png)

        Dim exitCode As SplatExitCode = Me.Run(filePath, "/?")

        Assert.AreEqual(SplatExitCode.Success, exitCode)
        Assert.AreEqual(0, Me.presenterCallCount)

    End Sub

#End Region

#Region " Errors "

    <TestMethod>
    Public Sub Run_UnknownArgument_ReturnsInvalidArgumentsAndDoesNotShowSplash()

        Dim filePath As String = Me.CreateImageFile("Image.png", ImageFormat.Png)

        Dim exitCode As SplatExitCode = Me.Run(filePath, "/Unknown")

        Assert.AreEqual(SplatExitCode.InvalidArguments, exitCode)
        Assert.Contains("Unknown argument: ""/Unknown"".", Me.output.ToString())
        Assert.Contains("/?", Me.output.ToString())
        Assert.AreEqual(0, Me.presenterCallCount)

    End Sub

    <TestMethod>
    Public Sub Run_SwitchWithoutValue_ReturnsInvalidArgumentsInsteadOfCrashing()

        Dim filePath As String = Me.CreateImageFile("Image.png", ImageFormat.Png)

        Dim exitCode As SplatExitCode = Me.Run(filePath, "/Duration")

        Assert.AreEqual(SplatExitCode.InvalidArguments, exitCode)
        Assert.Contains("Missing value for /Duration.", Me.output.ToString())
        Assert.AreEqual(0, Me.presenterCallCount)

    End Sub

    <TestMethod>
    Public Sub Run_InvalidArgumentsWithMissingFile_ReportsArgumentsFirst()

        Dim exitCode As SplatExitCode = Me.Run(Me.tempDirectory.GetFilePath("Missing.png"), "/Resize", "abc")

        Assert.AreEqual(SplatExitCode.InvalidArguments, exitCode)

    End Sub

    <TestMethod>
    Public Sub Run_FileDoesNotExist_ReturnsFileNotFound()

        Dim filePath As String = Me.tempDirectory.GetFilePath("Missing.png")

        Dim exitCode As SplatExitCode = Me.Run(filePath)

        Assert.AreEqual(SplatExitCode.FileNotFound, exitCode)
        Assert.Contains($"File ""{filePath}"" does not exist.", Me.output.ToString())
        Assert.AreEqual(0, Me.presenterCallCount)

    End Sub

    <TestMethod>
    Public Sub Run_DirectoryInsteadOfFile_ReturnsFileNotFound()

        Dim exitCode As SplatExitCode = Me.Run(Me.tempDirectory.FullPath)

        Assert.AreEqual(SplatExitCode.FileNotFound, exitCode)

    End Sub

    <TestMethod>
    <DataRow("NotAnImage.png")>
    <DataRow("NotAnImage.jpg")>
    <DataRow("NotAnImage.ico")>
    Public Sub Run_InvalidImageFile_ReturnsInvalidImageInsteadOfCrashing(fileName As String)

        Dim filePath As String = Me.tempDirectory.GetFilePath(fileName)
        File.WriteAllText(filePath, "This is not an image.")

        Dim exitCode As SplatExitCode = Me.Run(filePath)

        Assert.AreEqual(SplatExitCode.InvalidImage, exitCode)
        Assert.Contains($"File ""{filePath}"" is not a valid or supported image file.", Me.output.ToString())
        Assert.AreEqual(0, Me.presenterCallCount)

    End Sub

    <TestMethod>
    Public Sub Run_EmptyImageFile_ReturnsInvalidImage()

        Dim filePath As String = Me.tempDirectory.GetFilePath("Empty.png")
        File.WriteAllBytes(filePath, Array.Empty(Of Byte)())

        Dim exitCode As SplatExitCode = Me.Run(filePath)

        Assert.AreEqual(SplatExitCode.InvalidImage, exitCode)

    End Sub

    <TestMethod>
    Public Sub Run_NullArguments_Throw()

        Dim showSplash As Func(Of SplashRequest, SplatExitCode) = AddressOf Me.FakePresenter

        Assert.ThrowsExactly(Of ArgumentNullException)(Sub() SplatLauncher.Run(Nothing, Me.output, showSplash))
        Assert.ThrowsExactly(Of ArgumentNullException)(Sub() SplatLauncher.Run(Array.Empty(Of String)(), Nothing, showSplash))
        Assert.ThrowsExactly(Of ArgumentNullException)(Sub() SplatLauncher.Run(Array.Empty(Of String)(), Me.output, Nothing))

    End Sub

#End Region

#Region " Splash screen "

    <TestMethod>
    Public Sub Run_ValidPng_ShowsSplashWithPerPixelAlphaBitmap()

        Dim filePath As String = Me.CreateImageFile("Image.png", ImageFormat.Png)

        Dim exitCode As SplatExitCode = Me.Run(filePath, "/OnTop", "/Clickable", "/Location", "0,300")

        Assert.AreEqual(SplatExitCode.Success, exitCode)
        Assert.AreEqual(1, Me.presenterCallCount)
        Assert.IsTrue(Me.received.UsesPerPixelAlpha)
        Assert.AreEqual(PixelFormat.Format32bppArgb, Me.received.PixelFormat)
        Assert.AreEqual(New Size(20, 10), Me.received.ImageSize)
        Assert.AreEqual(Color.Empty, Me.received.TransparencyKey)
        Assert.IsTrue(Me.received.Options.OnTop)
        Assert.IsTrue(Me.received.Options.Clickable)
        Assert.AreEqual(New Point(0, 300), Me.received.Options.Location.Value)
        Assert.AreEqual(String.Empty, Me.output.ToString())

    End Sub

    <TestMethod>
    Public Sub Run_ValidPngWithResize_ImageIsResizedBeforeShowing()

        Dim filePath As String = Me.CreateImageFile("Image.PNG", ImageFormat.Png)

        Dim exitCode As SplatExitCode = Me.Run(filePath, "/Resize", "60x45")

        Assert.AreEqual(SplatExitCode.Success, exitCode)
        Assert.IsTrue(Me.received.UsesPerPixelAlpha)
        Assert.AreEqual(New Size(60, 45), Me.received.ImageSize)
        Assert.AreEqual(PixelFormat.Format32bppArgb, Me.received.PixelFormat)

    End Sub

    <TestMethod>
    <DataRow("Image.jpg")>
    <DataRow("Image.bmp")>
    <DataRow("Image.gif")>
    Public Sub Run_ValidNonAlphaImage_ShowsSplashWithUnusedTransparencyKey(fileName As String)

        Dim format As ImageFormat
        Select Case Path.GetExtension(fileName).ToLowerInvariant()
            Case ".jpg" : format = ImageFormat.Jpeg
            Case ".bmp" : format = ImageFormat.Bmp
            Case ".gif" : format = ImageFormat.Gif
            Case Else : Throw New ArgumentException($"Unexpected file name: {fileName}", NameOf(fileName))
        End Select
        Dim filePath As String = Me.CreateImageFile(fileName, format)

        Dim exitCode As SplatExitCode = Me.Run(filePath, "/Resize", "60x45", "/FadeIN", "/FadeOUT")

        Assert.AreEqual(SplatExitCode.Success, exitCode)
        Assert.AreEqual(1, Me.presenterCallCount)
        Assert.IsFalse(Me.received.UsesPerPixelAlpha)
        Assert.AreEqual(Byte.MaxValue, Me.received.TransparencyKey.A)
        Assert.IsFalse(Me.received.ImageUsesTransparencyKey)
        ' Non-alpha images are not resized up-front; the window zooms them.
        Assert.AreEqual(New Size(20, 10), Me.received.ImageSize)
        Assert.AreEqual(New Size(60, 45), Me.received.Options.Size.Value)
        Assert.IsTrue(Me.received.Options.FadeIn)
        Assert.IsTrue(Me.received.Options.FadeOut)

    End Sub

    <TestMethod>
    Public Sub Run_PresenterExitCode_IsReturned()

        Dim filePath As String = Me.CreateImageFile("Image.png", ImageFormat.Png)

        Dim exitCode As SplatExitCode = SplatLauncher.Run({filePath}, Me.output, Function(request As SplashRequest) SplatExitCode.InvalidImage)

        Assert.AreEqual(SplatExitCode.InvalidImage, exitCode)

    End Sub

    <TestMethod>
    Public Sub Run_RequestIsDisposedAfterPresenterReturns()

        Dim filePath As String = Me.CreateImageFile("Image.png", ImageFormat.Png)
        Dim image As Image = Nothing

        SplatLauncher.Run({filePath}, Me.output,
                          Function(request As SplashRequest)
                              image = request.Image
                              Return SplatExitCode.Success
                          End Function)

        Assert.IsNotNull(image)
        Assert.ThrowsExactly(Of ArgumentException)(Function() image.Width)

    End Sub

#End Region

#Region " CreateRequest "

    <TestMethod>
    Public Sub CreateRequest_NullOptions_Throws()

        Assert.ThrowsExactly(Of ArgumentNullException)(Sub() SplatLauncher.CreateRequest(Nothing))

    End Sub

    <TestMethod>
    Public Sub CreateRequest_OptionsWithoutImagePath_Throws()

        Assert.ThrowsExactly(Of ArgumentNullException)(Sub() SplatLauncher.CreateRequest(New SplatOptions()))

    End Sub

    <TestMethod>
    Public Sub CreateRequest_DoesNotLockFileAfterDisposal()

        Dim filePath As String = Me.CreateImageFile("Image.png", ImageFormat.Png)
        Dim options As New SplatOptions With {.ImageFilePath = filePath}

        Using request As SplashRequest = SplatLauncher.CreateRequest(options)
            Assert.IsNotNull(request.Image)
        End Using

        ' The source image must be released (not leaked), so the file can be deleted.
        File.Delete(filePath)
        Assert.IsFalse(File.Exists(filePath))

    End Sub

#End Region

End Class

#End Region
