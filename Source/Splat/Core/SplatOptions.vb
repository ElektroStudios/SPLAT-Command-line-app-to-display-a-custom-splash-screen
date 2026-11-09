#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Drawing

#End Region

#Region " SplatOptions "

''' <summary>
''' Represents the options that control how a splash screen is displayed,
''' as specified through the command-line arguments.
''' </summary>
Friend NotInheritable Class SplatOptions

#Region " Constants "

    ''' <summary>
    ''' The default splash screen duration, in milliseconds.
    ''' </summary>
    Friend Const DefaultDurationMilliseconds As Integer = 5000

    ''' <summary>
    ''' The default duration of the visual effects (fade-in / fade-out), in milliseconds.
    ''' </summary>
    Friend Const DefaultEffectsDurationMilliseconds As Integer = 1500

#End Region

#Region " Properties "

    ''' <summary>
    ''' Gets or sets the path of the image file to display.
    ''' <para></para>
    ''' This value is <see langword="Nothing"/> when <see cref="SplatOptions.ShowHelp"/> is <see langword="True"/>.
    ''' </summary>
    Friend Property ImageFilePath As String

    ''' <summary>
    ''' Gets or sets a value indicating whether the help must be displayed instead of the splash screen.
    ''' </summary>
    Friend Property ShowHelp As Boolean

    ''' <summary>
    ''' Gets or sets the splash screen duration, in milliseconds.
    ''' </summary>
    Friend Property DurationMilliseconds As Integer = SplatOptions.DefaultDurationMilliseconds

    ''' <summary>
    ''' Gets or sets the duration of the visual effects (fade-in / fade-out), in milliseconds.
    ''' </summary>
    Friend Property EffectsDurationMilliseconds As Integer = SplatOptions.DefaultEffectsDurationMilliseconds

    ''' <summary>
    ''' Gets or sets a value indicating whether the fade-in effect is enabled.
    ''' </summary>
    Friend Property FadeIn As Boolean

    ''' <summary>
    ''' Gets or sets a value indicating whether the fade-out effect is enabled.
    ''' </summary>
    Friend Property FadeOut As Boolean

    ''' <summary>
    ''' Gets or sets the custom size of the image,
    ''' or <see langword="Nothing"/> to use the original image size.
    ''' </summary>
    Friend Property Size As Size?

    ''' <summary>
    ''' Gets or sets the custom on-screen location of the image,
    ''' or <see langword="Nothing"/> to center the image on the primary screen.
    ''' </summary>
    Friend Property Location As Point?

    ''' <summary>
    ''' Gets or sets a value indicating whether clicking on the image closes the splash screen.
    ''' </summary>
    Friend Property Clickable As Boolean

    ''' <summary>
    ''' Gets or sets a value indicating whether the splash screen stays on top of other windows.
    ''' </summary>
    Friend Property OnTop As Boolean

#End Region

End Class

#End Region
