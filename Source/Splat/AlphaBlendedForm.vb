#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.ComponentModel
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.Windows.Forms

#End Region

#Region " AlphaBlendedForm "

' SEE : http://blogs.msdn.com/mswanson/archive/2005/07/07/436618.aspx
' Mike Swanson's Blog for original c#

''' <summary>
''' A borderless layered window that displays a 32-bpp bitmap with per-pixel alpha blending,
''' preserving its semi-transparent pixels and shadows.
''' </summary>
Friend Class AlphaBlendedForm : Inherits Form

#Region " Fields "

    ''' <summary>
    ''' A value indicating whether clicking on the image closes the application.
    ''' </summary>
    Private ReadOnly clickable As Boolean

#End Region

#Region " Constructors "

    ''' <summary>
    ''' Initializes a new instance of the <see cref="AlphaBlendedForm"/> class.
    ''' </summary>
    '''
    ''' <param name="bitmap">
    ''' The bitmap to display. It must have <see cref="PixelFormat.Format32bppArgb"/> pixel format.
    ''' </param>
    '''
    ''' <param name="location">
    ''' The on-screen location of the window.
    ''' </param>
    '''
    ''' <param name="topMost">
    ''' A value indicating whether the window stays on top of other windows.
    ''' </param>
    '''
    ''' <param name="clickable">
    ''' A value indicating whether clicking on the image closes the application.
    ''' </param>
    '''
    ''' <exception cref="ArgumentNullException">
    ''' Thrown when <paramref name="bitmap"/> is <see langword="Nothing"/>.
    ''' </exception>
    '''
    ''' <exception cref="ArgumentException">
    ''' Thrown when <paramref name="bitmap"/> does not have <see cref="PixelFormat.Format32bppArgb"/> pixel format.
    ''' </exception>
    Friend Sub New(bitmap As Bitmap, location As Point, topMost As Boolean, clickable As Boolean)

        If bitmap Is Nothing Then
            Throw New ArgumentNullException(NameOf(bitmap))
        End If

        If bitmap.PixelFormat <> PixelFormat.Format32bppArgb Then
            Throw New ArgumentException("The bitmap must be 32bpp with alpha-channel.", NameOf(bitmap))
        End If

        Me.clickable = clickable

        Me.FormBorderStyle = FormBorderStyle.None
        Me.ShowInTaskbar = False
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.TopMost = topMost
        Me.StartPosition = FormStartPosition.Manual
        Me.Bounds = New Rectangle(location, bitmap.Size)

        Me.SelectBitmap(bitmap, location)

    End Sub

#End Region

#Region " Private Methods "

    ''' <summary>
    ''' Updates the layered window with the specified bitmap, at the specified location.
    ''' </summary>
    '''
    ''' <param name="bitmap">
    ''' The 32-bpp bitmap to display.
    ''' </param>
    '''
    ''' <param name="location">
    ''' The on-screen location of the window.
    ''' </param>
    '''
    ''' <exception cref="Win32Exception">
    ''' Thrown when the layered window cannot be updated.
    ''' </exception>
    Private Sub SelectBitmap(bitmap As Bitmap, location As Point)

        ' Get device contexts
        Dim screenDc As IntPtr = NativeMethods.GetDC(IntPtr.Zero)
        Dim memDc As IntPtr = NativeMethods.CreateCompatibleDC(screenDc)
        Dim hBitmap As IntPtr = IntPtr.Zero
        Dim hOldBitmap As IntPtr = IntPtr.Zero

        Try
            ' Get handle to the new bitmap and select it into the current device context
            hBitmap = bitmap.GetHbitmap(Color.FromArgb(0))
            hOldBitmap = NativeMethods.SelectObject(memDc, hBitmap)

            ' Set parameters for layered window update
            Dim newSize As New NativeMethods.Size(bitmap.Width, bitmap.Height)
            Dim newLocation As New NativeMethods.Point(location.X, location.Y)
            Dim sourceLocation As New NativeMethods.Point(0, 0)

            Dim blend As New NativeMethods.BLENDFUNCTION With {
                .BlendOp = NativeMethods.AC_SRC_OVER,         ' Only works with a 32bpp bitmap
                .BlendFlags = 0,                              ' Always 0
                .SourceConstantAlpha = Byte.MaxValue,         ' Set to 255 for per-pixel alpha values
                .AlphaFormat = NativeMethods.AC_SRC_ALPHA     ' Only works when the bitmap contains an alpha channel
            }

            ' Update the window
            If Not NativeMethods.UpdateLayeredWindow(Me.Handle, screenDc, newLocation, newSize,
                                                     memDc, sourceLocation, 0, blend, NativeMethods.ULW_ALPHA) Then
                Throw New Win32Exception()
            End If

        Finally
            ' Release device context
            NativeMethods.ReleaseDC(IntPtr.Zero, screenDc)
            If hBitmap <> IntPtr.Zero Then
                NativeMethods.SelectObject(memDc, hOldBitmap)
                NativeMethods.DeleteObject(hBitmap) ' Remove bitmap resources
            End If
            NativeMethods.DeleteDC(memDc)

        End Try

    End Sub

#End Region

#Region " Overridden Members "

    ''' <summary>
    ''' Gets the required creation parameters when the control handle is created.
    ''' <para></para>
    ''' Adds the layered extended style (WS_EX_LAYERED) to this window.
    ''' </summary>
    Protected Overrides ReadOnly Property CreateParams As CreateParams
        Get
            Dim cParams As CreateParams = MyBase.CreateParams
            cParams.ExStyle = cParams.ExStyle Or NativeMethods.WS_EX_LAYERED
            Return cParams
        End Get
    End Property

    ''' <summary>
    ''' Processes Windows messages.
    ''' </summary>
    '''
    ''' <param name="m">
    ''' The Windows <see cref="Message"/> to process.
    ''' </param>
    Protected Overrides Sub WndProc(ByRef m As Message)

        ' Note: WM_NCHITTEST must not be answered with HTCAPTION, otherwise Windows sends WM_NCLBUTTONDOWN
        ' (starting a window drag operation) instead of WM_LBUTTONDOWN, and the click is never detected.
        If m.Msg = NativeMethods.WM_LBUTTONDOWN AndAlso Me.clickable Then
            Application.Exit()
            Return
        End If

        MyBase.WndProc(m)

    End Sub

#End Region

End Class

#End Region
