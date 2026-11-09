#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Collections.Generic
Imports System.Drawing
Imports System.IO
Imports System.Runtime.InteropServices

#End Region

#Region " SplatLauncher "

''' <summary>
''' Implements the application workflow, independently of the user interface:
''' parses the command-line arguments, prints the help or the errors, validates and prepares the image,
''' and finally hands it over to a splash screen presenter.
''' </summary>
Friend Module SplatLauncher

#Region " Public Methods "

    ''' <summary>
    ''' Runs the application workflow with the specified command-line arguments.
    ''' </summary>
    '''
    ''' <param name="arguments">
    ''' The command-line arguments, without the executable path.
    ''' </param>
    '''
    ''' <param name="output">
    ''' The writer where the help and the error messages are written.
    ''' </param>
    '''
    ''' <param name="showSplash">
    ''' The presenter that displays the splash screen and returns the exit code.
    ''' It is only invoked when the arguments and the image are valid.
    ''' The <see cref="SplashRequest"/> is disposed when the presenter returns.
    ''' </param>
    '''
    ''' <returns>
    ''' The application exit code.
    ''' </returns>
    '''
    ''' <exception cref="ArgumentNullException">
    ''' Thrown when <paramref name="arguments"/>, <paramref name="output"/> or <paramref name="showSplash"/>
    ''' is <see langword="Nothing"/>.
    ''' </exception>
    Friend Function Run(arguments As IReadOnlyList(Of String), output As TextWriter,
                        showSplash As Func(Of SplashRequest, SplatExitCode)) As SplatExitCode

        If arguments Is Nothing Then
            Throw New ArgumentNullException(NameOf(arguments))
        End If

        If output Is Nothing Then
            Throw New ArgumentNullException(NameOf(output))
        End If

        If showSplash Is Nothing Then
            Throw New ArgumentNullException(NameOf(showSplash))
        End If

        Dim options As SplatOptions
        Try
            options = CommandLineParser.Parse(arguments)

        Catch ex As CommandLineArgumentException
            output.WriteLine(ex.Message)
            output.WriteLine("Use the /? switch to display the help.")
            Return SplatExitCode.InvalidArguments

        End Try

        If options.ShowHelp Then
            output.WriteLine(HelpText.Value)
            Return SplatExitCode.Success
        End If

        If Not File.Exists(options.ImageFilePath) Then
            output.WriteLine($"File ""{options.ImageFilePath}"" does not exist.")
            Return SplatExitCode.FileNotFound
        End If

        Dim request As SplashRequest
        Try
            request = SplatLauncher.CreateRequest(options)

        Catch ex As SplashImageException
            output.WriteLine(ex.Message)
            Return SplatExitCode.InvalidImage

        End Try

        Using request
            Return showSplash(request)
        End Using

    End Function

    ''' <summary>
    ''' Loads and prepares the image specified in the options, and creates the corresponding <see cref="SplashRequest"/>.
    ''' </summary>
    '''
    ''' <param name="options">
    ''' The splash screen options. <see cref="SplatOptions.ImageFilePath"/> must be set.
    ''' </param>
    '''
    ''' <returns>
    ''' The resulting <see cref="SplashRequest"/>. The caller is responsible for disposing it.
    ''' </returns>
    '''
    ''' <exception cref="ArgumentNullException">
    ''' Thrown when <paramref name="options"/> or its <see cref="SplatOptions.ImageFilePath"/> is <see langword="Nothing"/>.
    ''' </exception>
    '''
    ''' <exception cref="SplashImageException">
    ''' Thrown when the image cannot be loaded or processed.
    ''' </exception>
    Friend Function CreateRequest(options As SplatOptions) As SplashRequest

        If options Is Nothing Then
            Throw New ArgumentNullException(NameOf(options))
        End If

        If options.ImageFilePath Is Nothing Then
            Throw New ArgumentNullException(NameOf(options), "The image file path is not set.")
        End If

        Dim image As Image = SplatLauncher.LoadImage(options.ImageFilePath)
        Dim succeeded As Boolean = False
        Try
            Dim request As SplashRequest
            If ImageFileHelper.UsesPerPixelAlpha(options.ImageFilePath) Then
                Dim bitmap As Bitmap = If(options.Size.HasValue,
                                          ImageResizer.Resize(image, options.Size.Value),
                                          ImageResizer.ToArgb32(image))
                image.Dispose()
                request = New SplashRequest(options, bitmap, usesPerPixelAlpha:=True, transparencyKey:=Color.Empty)
            Else
                Dim transparencyKey As Color = TransparencyKeyFinder.FindUnusedColor(image)
                request = New SplashRequest(options, image, usesPerPixelAlpha:=False, transparencyKey:=transparencyKey)
            End If
            succeeded = True
            Return request

        Catch ex As Exception When TypeOf ex Is ArgumentException OrElse
                                   TypeOf ex Is OutOfMemoryException OrElse
                                   TypeOf ex Is ExternalException OrElse
                                   TypeOf ex Is InvalidOperationException

            Throw New SplashImageException($"Unable to process the image file ""{options.ImageFilePath}"": {ex.Message}", ex)

        Finally
            If Not succeeded Then
                image.Dispose()
            End If

        End Try

    End Function

#End Region

#Region " Private Methods "

    ''' <summary>
    ''' Loads the specified image file.
    ''' </summary>
    '''
    ''' <param name="filePath">
    ''' The image file path.
    ''' </param>
    '''
    ''' <returns>
    ''' The loaded <see cref="Image"/>.
    ''' </returns>
    '''
    ''' <exception cref="SplashImageException">
    ''' Thrown when the file is not a valid or supported image, or it cannot be read.
    ''' </exception>
    Private Function LoadImage(filePath As String) As Image

        Try
            Return Image.FromFile(filePath)

        Catch ex As Exception When TypeOf ex Is OutOfMemoryException OrElse ' GDI+ throws this for invalid image files.
                                   TypeOf ex Is IOException OrElse
                                   TypeOf ex Is UnauthorizedAccessException OrElse
                                   TypeOf ex Is ArgumentException OrElse
                                   TypeOf ex Is ExternalException

            Throw New SplashImageException($"File ""{filePath}"" is not a valid or supported image file.", ex)

        End Try

    End Function

#End Region

End Module

#End Region
