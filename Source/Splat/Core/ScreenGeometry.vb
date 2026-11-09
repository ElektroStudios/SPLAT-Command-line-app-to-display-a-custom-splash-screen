#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Drawing

#End Region

#Region " ScreenGeometry "

''' <summary>
''' Provides screen geometry calculations.
''' </summary>
Friend Module ScreenGeometry

    ''' <summary>
    ''' Gets the location at which a window of the specified size is centered within the specified screen bounds.
    ''' <para></para>
    ''' When the window is larger than the screen, the resulting coordinates can be lower than the bounds origin,
    ''' so the window is still centered.
    ''' </summary>
    '''
    ''' <param name="screenBounds">
    ''' The screen bounds.
    ''' </param>
    '''
    ''' <param name="windowSize">
    ''' The window size.
    ''' </param>
    '''
    ''' <returns>
    ''' The top-left location of the centered window.
    ''' </returns>
    Friend Function GetCenteredLocation(screenBounds As Rectangle, windowSize As Size) As Point

        Dim x As Integer = screenBounds.X + ((screenBounds.Width - windowSize.Width) \ 2)
        Dim y As Integer = screenBounds.Y + ((screenBounds.Height - windowSize.Height) \ 2)
        Return New Point(x, y)

    End Function

End Module

#End Region
