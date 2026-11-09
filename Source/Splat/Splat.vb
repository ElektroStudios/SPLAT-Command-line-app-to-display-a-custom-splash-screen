#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.Drawing
Imports System.Threading
Imports System.Windows.Forms

#End Region

#Region " Splat "

''' <summary>
''' The main window of the application, which displays the splash screen.
''' </summary>
Public Class Splat

#Region " Info "

    ' Splat
    ' A SplashScreen Tool
    ' Version 1.9
    ' 09/11/2026

    ' [+] Changes v1.9:
    ' - Added a unit test project (Splat.Tests).
    ' - Command-line parsing, image preparation and the rest of non-UI logic was moved to testable modules.
    ' - When the parent console cannot be accessed (AttachConsole fails with "Access is denied"), the help and error messages are shown in a message box.
    ' - Added exit codes (0 = Success, 1 = Invalid arguments, 2 = File not found, 3 = Invalid image).
    ' - Bug corrected: "/?" (and the rest of help switches) did not display the help when used as the first argument.
    ' - Bug corrected: Help and argument errors were followed by an error message box ("Object reference not set...").
    ' - Bug corrected: A switch without value at the end of the command-line (e.g. "/Duration") crashed the application.
    ' - Bug corrected: An invalid or corrupt image file crashed the application.
    ' - Bug corrected: "/Location 0,Y" was ignored for PNG/ICO images (the image was centered instead).
    ' - Bug corrected: "/Location" was ignored for JPG/BMP/GIF images when "/Resize" was specified after it.
    ' - Bug corrected: "/OnTop" had no effect for PNG/ICO images.
    ' - Bug corrected: "/Clickable" did not close PNG/ICO images (the click started a window drag instead).
    ' - Bug corrected: Fade effects consumed 100% of a CPU core (busy-wait loop).
    ' - Bug corrected: If "/FXDuration" was greater than "/Duration", the fade-out effect was interrupted.
    ' - Unknown arguments and malformed "/Resize" and "/Location" values are now reported as errors.
    ' - "/FadeOUT" no longer adds a pointless delay for PNG/ICO images (fade effects are not supported for them).
    ' - Improved the image resize quality for PNG/ICO images, and removed the stretching artifact at the image edges.
    ' - The transparency key color is now computed in a single pass over the image pixels (much faster).

    ' [+] Changes v1.8:
    ' - Added support for GIF images.
    ' - Fixed "/Clickable" logic for PNG/ICO images.

    ' [+] Changes v1.7:
    ' - Updated to .NET Framework 4.8.
    ' - Syntax help messages were improved a bit.

    ' [+] Changes v1.1:
    ' - Bug corrected: Image is not correctly centered after a resize operation.
    ' - Bug corrected: Form transparency affect PNG/ICO images. (Images looses pixel colors)
    ' - Added full compatibility with ICO/PNG transparents and with shadows.
    ' - Added "/Ontop" and "/Clickable" switches.
    ' - Added a icon to the application.
    ' - Code optimizations to load faster.
    ' - Mistypsed word corrections in Help section.

    ' [+] ToDo:
    ' - Add FadeIN/FadeOUT compatibility por PNG/ICO images.
    ' - Add more FX effects.

#End Region

#Region " Fields "

    ''' <summary>
    ''' The splash screen request.
    ''' </summary>
    Private ReadOnly request As SplashRequest

    ''' <summary>
    ''' The layered window used to display PNG/ICO images with per-pixel alpha blending.
    ''' </summary>
    Private alphaBlendedForm As AlphaBlendedForm

#End Region

#Region " Constructors "

    ''' <summary>
    ''' Initializes a new instance of the <see cref="Splat"/> class.
    ''' </summary>
    '''
    ''' <param name="request">
    ''' The splash screen request.
    ''' </param>
    '''
    ''' <exception cref="ArgumentNullException">
    ''' Thrown when <paramref name="request"/> is <see langword="Nothing"/>.
    ''' </exception>
    Friend Sub New(request As SplashRequest)

        If request Is Nothing Then
            Throw New ArgumentNullException(NameOf(request))
        End If

        Me.InitializeComponent()
        Me.request = request

    End Sub

#End Region

#Region " Event Handlers "

    ''' <summary>
    ''' Handles the <see cref="Form.Load"/> event of this form.
    ''' </summary>
    '''
    ''' <param name="sender">
    ''' The source of the event.
    ''' </param>
    '''
    ''' <param name="e">
    ''' The <see cref="EventArgs"/> instance containing the event data.
    ''' </param>
    Private Sub Splat_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Dim options As SplatOptions = Me.request.Options

        Me.Opacity = 1
        Me.SetStyle(ControlStyles.ResizeRedraw, True)
        Me.SetStyle(ControlStyles.AllPaintingInWmPaint, True)
        Me.SetStyle(ControlStyles.Opaque, True)
        Me.SetStyle(ControlStyles.OptimizedDoubleBuffer, True)
        Me.SetStyle(ControlStyles.UserPaint, True)
        Me.Size = New Size(0, 0)
        Me.TopMost = options.OnTop

        Dim primaryScreenBounds As Rectangle = Screen.PrimaryScreen.Bounds

        If Me.request.UsesPerPixelAlpha Then
            ' PNG/ICO: the image is displayed in a separate layered window; this form remains invisible (0x0 size).
            Dim alphaBitmap As Bitmap = DirectCast(Me.request.Image, Bitmap)
            Dim imageLocation As Point = If(options.Location.HasValue,
                                       options.Location.Value,
                                       ScreenGeometry.GetCenteredLocation(primaryScreenBounds, alphaBitmap.Size))

            Me.alphaBlendedForm = New AlphaBlendedForm(alphaBitmap, imageLocation, options.OnTop, options.Clickable)
            Me.alphaBlendedForm.Show()

        Else
            Me.BackColor = Me.request.TransparencyKey
            Me.TransparencyKey = Me.request.TransparencyKey
            Me.PictureBox_Img.Image = Me.request.Image

            Dim windowSize As Size = Me.request.Image.Size
            If options.Size.HasValue Then
                windowSize = options.Size.Value
                Me.PictureBox_Img.SizeMode = PictureBoxSizeMode.Zoom
            End If
            Me.Size = windowSize
            Me.Location = If(options.Location.HasValue,
                             options.Location.Value,
                             ScreenGeometry.GetCenteredLocation(primaryScreenBounds, windowSize))

            If options.Clickable Then
                Me.PictureBox_Img.Cursor = Cursors.Hand
                AddHandler Me.PictureBox_Img.Click, AddressOf Me.PictureBox_Img_Click
            End If

            If options.FadeIn Then
                Me.Opacity = 0
                Me.Show()
                Me.Fade(toOpacity:=1.0R)
            End If
        End If

        If Not Me.IsDisposed Then
            Me.Timer_CloseForm.Interval = options.DurationMilliseconds
            Me.Timer_CloseForm.Start()
        End If

    End Sub

    ''' <summary>
    ''' Handles the <see cref="Control.Click"/> event of the <see cref="Splat.PictureBox_Img"/> control.
    ''' <para></para>
    ''' This handler is only attached when the "/Clickable" switch is specified.
    ''' </summary>
    '''
    ''' <param name="sender">
    ''' The source of the event.
    ''' </param>
    '''
    ''' <param name="e">
    ''' The <see cref="EventArgs"/> instance containing the event data.
    ''' </param>
    Private Sub PictureBox_Img_Click(sender As Object, e As EventArgs)

        Application.Exit()

    End Sub

    ''' <summary>
    ''' Handles the <see cref="System.Windows.Forms.Timer.Tick"/> event of the <see cref="Splat.Timer_CloseForm"/> timer.
    ''' <para></para>
    ''' Closes the splash screen when its duration has elapsed.
    ''' </summary>
    '''
    ''' <param name="sender">
    ''' The source of the event.
    ''' </param>
    '''
    ''' <param name="e">
    ''' The <see cref="EventArgs"/> instance containing the event data.
    ''' </param>
    Private Sub Timer_CloseForm_Tick(sender As Object, e As EventArgs) Handles Timer_CloseForm.Tick

        ' Stop the timer first, otherwise it would tick again (re-entrantly) during the fade-out effect.
        Me.Timer_CloseForm.Stop()

        If Me.request.Options.FadeOut AndAlso Not Me.request.UsesPerPixelAlpha Then
            Me.Fade(toOpacity:=0.0R)
        End If

        Application.Exit()

    End Sub

#End Region

#Region " Private Methods "

    ''' <summary>
    ''' Gradually changes the opacity of this form, from its current opacity to the specified opacity,
    ''' during the effects duration specified in the options.
    ''' <para></para>
    ''' Window messages are processed during the effect, so the form remains responsive.
    ''' </summary>
    '''
    ''' <param name="toOpacity">
    ''' The target opacity, in the range 0.0 to 1.0.
    ''' </param>
    Private Sub Fade(toOpacity As Double)

        Dim schedule As IReadOnlyList(Of FadeStep) =
            FadeScheduler.CreateSchedule(Me.Opacity, toOpacity,
                                         Me.request.Options.EffectsDurationMilliseconds,
                                         FadeScheduler.DefaultStepCount)

        Dim elapsed As Stopwatch = Stopwatch.StartNew()
        For Each currentStep As FadeStep In schedule

            While elapsed.ElapsedMilliseconds < currentStep.DueTimeMilliseconds
                Application.DoEvents()
                If Me.IsDisposed Then
                    Return ' The form was closed (e.g. clicked) during the effect.
                End If
                Thread.Sleep(1)
            End While

            If Me.IsDisposed Then
                Return
            End If
            Me.Opacity = currentStep.Opacity

        Next

    End Sub

#End Region

#Region " Overridden Members "

    ''' <summary>
    ''' Raises the <see cref="Form.FormClosed"/> event.
    ''' <para></para>
    ''' Disposes the layered window used to display PNG/ICO images, if any.
    ''' </summary>
    '''
    ''' <param name="e">
    ''' A <see cref="FormClosedEventArgs"/> that contains the event data.
    ''' </param>
    Protected Overrides Sub OnFormClosed(e As FormClosedEventArgs)

        If Me.alphaBlendedForm IsNot Nothing AndAlso Not Me.alphaBlendedForm.IsDisposed Then
            Me.alphaBlendedForm.Dispose()
        End If
        MyBase.OnFormClosed(e)

    End Sub

#End Region

End Class

#End Region
