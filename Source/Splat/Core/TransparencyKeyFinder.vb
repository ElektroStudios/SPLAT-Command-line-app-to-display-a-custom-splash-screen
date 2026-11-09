#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Collections.Generic
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.Runtime.InteropServices

#End Region

#Region " TransparencyKeyFinder "

''' <summary>
''' Finds a color that is not used by an image, to be used as the transparency key of a window.
''' </summary>
Friend Module TransparencyKeyFinder

#Region " Public Methods "

    ''' <summary>
    ''' Finds an opaque color that is not used by any visible (non fully-transparent) pixel of the specified image.
    ''' <para></para>
    ''' The <see cref="KnownColor"/> values are evaluated first, in their enumeration order;
    ''' if all of them are used by the image, the first unused RGB value is returned.
    ''' </summary>
    '''
    ''' <param name="image">
    ''' The source image.
    ''' </param>
    '''
    ''' <returns>
    ''' An opaque <see cref="Color"/> that is not used by the image.
    ''' </returns>
    '''
    ''' <exception cref="ArgumentNullException">
    ''' Thrown when <paramref name="image"/> is <see langword="Nothing"/>.
    ''' </exception>
    '''
    ''' <exception cref="InvalidOperationException">
    ''' Thrown when the image uses every possible RGB color.
    ''' </exception>
    Friend Function FindUnusedColor(image As Image) As Color

        If image Is Nothing Then
            Throw New ArgumentNullException(NameOf(image))
        End If

        Dim usedColors As HashSet(Of Integer) = TransparencyKeyFinder.GetVisibleRgbValues(image)

        For Each knownColor As KnownColor In [Enum].GetValues(GetType(KnownColor))
            Dim color As Color = Color.FromKnownColor(knownColor)
            If color.A <> Byte.MaxValue Then
                Continue For ' Skip "Transparent"; a transparency key must be opaque.
            End If
            If Not usedColors.Contains(color.ToArgb() And &HFFFFFF) Then
                Return color
            End If
        Next

        For rgb As Integer = 0 To &HFFFFFF
            If Not usedColors.Contains(rgb) Then
                Return Color.FromArgb(Byte.MaxValue, Color.FromArgb(rgb))
            End If
        Next

        Throw New InvalidOperationException("The image uses every possible RGB color; no transparency key color is available.")

    End Function

    ''' <summary>
    ''' Gets the distinct RGB values (alpha channel discarded) of the visible (non fully-transparent) pixels
    ''' of the specified image.
    ''' </summary>
    '''
    ''' <param name="image">
    ''' The source image.
    ''' </param>
    '''
    ''' <returns>
    ''' The distinct 24-bit RGB values, in <c>&amp;HRRGGBB</c> format.
    ''' </returns>
    '''
    ''' <exception cref="ArgumentNullException">
    ''' Thrown when <paramref name="image"/> is <see langword="Nothing"/>.
    ''' </exception>
    Friend Function GetVisibleRgbValues(image As Image) As HashSet(Of Integer)

        If image Is Nothing Then
            Throw New ArgumentNullException(NameOf(image))
        End If

        Dim result As New HashSet(Of Integer)()

        ' Bitmap(Image) constructor draws the image using its pixel size (ignoring its DPI)
        ' into a new bitmap with Format32bppArgb pixel format.
        Using bitmap As New Bitmap(image)

            Dim bounds As New Rectangle(0, 0, bitmap.Width, bitmap.Height)
            Dim data As BitmapData = bitmap.LockBits(bounds, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb)
            Try
                Dim row As Integer() = New Integer(bitmap.Width - 1) {}
                For y As Integer = 0 To bitmap.Height - 1
                    Marshal.Copy(IntPtr.Add(data.Scan0, y * data.Stride), row, 0, row.Length)
                    For Each pixel As Integer In row
                        If (pixel And &HFF000000) <> 0 Then
                            result.Add(pixel And &HFFFFFF)
                        End If
                    Next
                Next
            Finally
                bitmap.UnlockBits(data)
            End Try

        End Using

        Return result

    End Function

#End Region

End Module

#End Region
