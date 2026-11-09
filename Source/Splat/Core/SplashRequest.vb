#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Drawing

#End Region

#Region " SplashRequest "

''' <summary>
''' Represents a validated and ready-to-display splash screen: the options and the prepared image.
''' </summary>
Friend NotInheritable Class SplashRequest : Implements IDisposable

#Region " Properties "

    ''' <summary>
    ''' Gets the options that control how the splash screen is displayed.
    ''' </summary>
    Friend ReadOnly Property Options As SplatOptions

    ''' <summary>
    ''' Gets the image to display.
    ''' <para></para>
    ''' When <see cref="SplashRequest.UsesPerPixelAlpha"/> is <see langword="True"/>, this is a
    ''' <see cref="Bitmap"/> with <see cref="Imaging.PixelFormat.Format32bppArgb"/> pixel format,
    ''' already resized to <see cref="SplatOptions.Size"/> (if specified).
    ''' </summary>
    Friend ReadOnly Property Image As Image

    ''' <summary>
    ''' Gets a value indicating whether the image must be displayed with per-pixel alpha blending (layered window).
    ''' </summary>
    Friend ReadOnly Property UsesPerPixelAlpha As Boolean

    ''' <summary>
    ''' Gets the transparency key color for the splash screen window,
    ''' or <see cref="Color.Empty"/> when <see cref="SplashRequest.UsesPerPixelAlpha"/> is <see langword="True"/>.
    ''' <para></para>
    ''' This color is not used by any visible pixel of the image.
    ''' </summary>
    Friend ReadOnly Property TransparencyKey As Color

#End Region

#Region " Constructors "

    ''' <summary>
    ''' Initializes a new instance of the <see cref="SplashRequest"/> class.
    ''' </summary>
    '''
    ''' <param name="options">
    ''' The options that control how the splash screen is displayed.
    ''' </param>
    '''
    ''' <param name="image">
    ''' The image to display. This instance takes ownership of it.
    ''' </param>
    '''
    ''' <param name="usesPerPixelAlpha">
    ''' A value indicating whether the image must be displayed with per-pixel alpha blending.
    ''' </param>
    '''
    ''' <param name="transparencyKey">
    ''' The transparency key color, or <see cref="Color.Empty"/> when <paramref name="usesPerPixelAlpha"/> is <see langword="True"/>.
    ''' </param>
    '''
    ''' <exception cref="ArgumentNullException">
    ''' Thrown when <paramref name="options"/> or <paramref name="image"/> is <see langword="Nothing"/>.
    ''' </exception>
    Friend Sub New(options As SplatOptions, image As Image, usesPerPixelAlpha As Boolean, transparencyKey As Color)

        If options Is Nothing Then
            Throw New ArgumentNullException(NameOf(options))
        End If

        If image Is Nothing Then
            Throw New ArgumentNullException(NameOf(image))
        End If

        Me.Options = options
        Me.Image = image
        Me.UsesPerPixelAlpha = usesPerPixelAlpha
        Me.TransparencyKey = transparencyKey

    End Sub

#End Region

#Region " IDisposable Implementation "

    ''' <summary>
    ''' Releases the image owned by this instance.
    ''' </summary>
    Public Sub Dispose() Implements IDisposable.Dispose
        Me.Image.Dispose()
    End Sub

#End Region

End Class

#End Region
