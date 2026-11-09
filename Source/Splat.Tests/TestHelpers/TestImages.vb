#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Drawing
Imports System.Drawing.Imaging

#End Region

#Region " TestImages "

Namespace TestHelpers

    ''' <summary>
    ''' Creates in-memory and on-disk images for testing purposes.
    ''' </summary>
    Friend Module TestImages

        ''' <summary>
        ''' Creates a 32-bpp ARGB bitmap filled with the specified color.
        ''' </summary>
        '''
        ''' <param name="width">
        ''' The bitmap width.
        ''' </param>
        '''
        ''' <param name="height">
        ''' The bitmap height.
        ''' </param>
        '''
        ''' <param name="fillColor">
        ''' The fill color.
        ''' </param>
        '''
        ''' <returns>
        ''' The resulting <see cref="Bitmap"/>. The caller is responsible for disposing it.
        ''' </returns>
        Friend Function CreateFilled(width As Integer, height As Integer, fillColor As Color) As Bitmap

            Dim bitmap As New Bitmap(width, height, PixelFormat.Format32bppArgb)
            For y As Integer = 0 To height - 1
                For x As Integer = 0 To width - 1
                    bitmap.SetPixel(x, y, fillColor)
                Next
            Next
            Return bitmap

        End Function

        ''' <summary>
        ''' Creates a filled bitmap and saves it to the specified file, using the specified format.
        ''' </summary>
        '''
        ''' <param name="filePath">
        ''' The destination file path.
        ''' </param>
        '''
        ''' <param name="format">
        ''' The image file format.
        ''' </param>
        '''
        ''' <param name="width">
        ''' The image width.
        ''' </param>
        '''
        ''' <param name="height">
        ''' The image height.
        ''' </param>
        '''
        ''' <param name="fillColor">
        ''' The fill color.
        ''' </param>
        Friend Sub SaveFilled(filePath As String, format As ImageFormat, width As Integer, height As Integer, fillColor As Color)

            Using bitmap As Bitmap = TestImages.CreateFilled(width, height, fillColor)
                bitmap.Save(filePath, format)
            End Using

        End Sub

    End Module

End Namespace

#End Region
