#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Drawing.Imaging

#End Region

#Region " ImageResizer "

''' <summary>
''' Provides methods to create 32-bpp ARGB copies of images, optionally resized.
''' </summary>
Friend Module ImageResizer

    ''' <summary>
    ''' Creates a copy of the specified image, with <see cref="PixelFormat.Format32bppArgb"/> pixel format,
    ''' preserving its pixel size and its alpha channel.
    ''' </summary>
    '''
    ''' <param name="image">
    ''' The source image.
    ''' </param>
    '''
    ''' <returns>
    ''' The resulting <see cref="Bitmap"/>. The caller is responsible for disposing it.
    ''' </returns>
    '''
    ''' <exception cref="ArgumentNullException">
    ''' Thrown when <paramref name="image"/> is <see langword="Nothing"/>.
    ''' </exception>
    Friend Function ToArgb32(image As Image) As Bitmap

        If image Is Nothing Then
            Throw New ArgumentNullException(NameOf(image))
        End If

        ' Bitmap(Image) constructor draws the image using its pixel size (ignoring its DPI)
        ' into a new bitmap with Format32bppArgb pixel format.
        Return New Bitmap(image)

    End Function

    ''' <summary>
    ''' Creates a resized copy of the specified image, with <see cref="PixelFormat.Format32bppArgb"/> pixel format,
    ''' preserving its alpha channel.
    ''' <para></para>
    ''' The image is stretched to the exact target size (the aspect ratio is not preserved).
    ''' </summary>
    '''
    ''' <param name="image">
    ''' The source image.
    ''' </param>
    '''
    ''' <param name="size">
    ''' The target size, in pixels.
    ''' </param>
    '''
    ''' <returns>
    ''' The resulting <see cref="Bitmap"/>. The caller is responsible for disposing it.
    ''' </returns>
    '''
    ''' <exception cref="ArgumentNullException">
    ''' Thrown when <paramref name="image"/> is <see langword="Nothing"/>.
    ''' </exception>
    '''
    ''' <exception cref="ArgumentOutOfRangeException">
    ''' Thrown when the width or height of <paramref name="size"/> is not a positive number.
    ''' </exception>
    Friend Function Resize(image As Image, size As Size) As Bitmap

        If image Is Nothing Then
            Throw New ArgumentNullException(NameOf(image))
        End If

        If size.Width <= 0 OrElse size.Height <= 0 Then
            Throw New ArgumentOutOfRangeException(NameOf(size), size, "Width and height must be positive numbers.")
        End If

        Dim result As New Bitmap(size.Width, size.Height, PixelFormat.Format32bppArgb)
        Try
            Using graphics As Graphics = Graphics.FromImage(result),
                  attributes As New ImageAttributes()

                graphics.CompositingMode = CompositingMode.SourceCopy
                graphics.CompositingQuality = CompositingQuality.HighQuality
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality
                graphics.SmoothingMode = SmoothingMode.HighQuality

                ' Prevents the semi-transparent "ghost" border that GDI+ draws at the image edges when scaling.
                attributes.SetWrapMode(WrapMode.TileFlipXY)

                graphics.DrawImage(image, New Rectangle(0, 0, size.Width, size.Height),
                                   0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attributes)
            End Using

        Catch
            result.Dispose()
            Throw

        End Try

        Return result

    End Function

End Module

#End Region
