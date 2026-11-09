#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.IO

#End Region

#Region " ImageFileHelper "

''' <summary>
''' Provides helper methods to classify image files.
''' </summary>
Friend Module ImageFileHelper

    ''' <summary>
    ''' Determines whether the specified image file must be displayed with per-pixel alpha blending
    ''' (layered window), based on its file extension.
    ''' <para></para>
    ''' PNG and ICO images are displayed with per-pixel alpha blending,
    ''' which preserves their semi-transparent pixels and shadows.
    ''' The rest of image formats are displayed in a regular window using a transparency key color.
    ''' </summary>
    '''
    ''' <param name="filePath">
    ''' The image file path.
    ''' </param>
    '''
    ''' <returns>
    ''' <see langword="True"/> if the image must be displayed with per-pixel alpha blending;
    ''' otherwise, <see langword="False"/>.
    ''' </returns>
    '''
    ''' <exception cref="ArgumentNullException">
    ''' Thrown when <paramref name="filePath"/> is <see langword="Nothing"/>.
    ''' </exception>
    Friend Function UsesPerPixelAlpha(filePath As String) As Boolean

        If filePath Is Nothing Then
            Throw New ArgumentNullException(NameOf(filePath))
        End If

        Dim extension As String = Path.GetExtension(filePath.Trim())
        Return String.Equals(extension, ".png", StringComparison.OrdinalIgnoreCase) OrElse
               String.Equals(extension, ".ico", StringComparison.OrdinalIgnoreCase)

    End Function

End Module

#End Region
