Imports System.Windows.Forms
Imports System.Drawing
Imports System.Drawing.Imaging
Imports Splat.NativeMethods

' SEE : http://blogs.msdn.com/mswanson/archive/2005/07/07/436618.aspx
' Mike Swanson's Blog for original c#

Public Class AlphaBlendedForm
    Inherits Form

    Public Shared PNG_Size_X As Int32 = Nothing
    Public Shared PNG_Size_Y As Int32 = Nothing
    Public Shared PNG_Location_X As Int32 = Nothing
    Public Shared PNG_Location_Y As Int32 = Nothing

    Public Sub New(ByVal bitmap As Bitmap)

        Me.TopMost = Splat.TopMost
        Me.ShowInTaskbar = False
        Me.Size = bitmap.Size
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.SelectBitmap(bitmap)
        Me.Show()
        Me.MaximizeBox = False
        Me.MinimizeBox = False

    End Sub

    Public Sub SelectBitmap(ByVal bitmap As Bitmap)

        ' Does this bitmap contain an alpha channel?
        If bitmap.PixelFormat <> PixelFormat.Format32bppArgb Then
            Throw New ApplicationException("The bitmap must be 32bpp with alpha-channel.")
        End If

        ' Get device contexts
        Dim screenDc As IntPtr = GetDC(IntPtr.Zero)
        Dim memDc As IntPtr = CreateCompatibleDC(screenDc)
        Dim hBitmap As IntPtr = IntPtr.Zero
        Dim hOldBitmap As IntPtr = IntPtr.Zero

        Try
            Dim newsize
            Dim newLocation
            ' Get handle to the new bitmap and select it into the current device context
            hBitmap = bitmap.GetHbitmap(Color.FromArgb(0))
            hOldBitmap = SelectObject(memDc, hBitmap)

            ' Set parameters for layered window update
            ' Size window to match bitmap
            If PNG_Size_X = Nothing Then
                newsize = New NativeMethods.Size(bitmap.Width, bitmap.Height)
            Else
                newsize = New NativeMethods.Size(PNG_Size_X, PNG_Size_Y)
            End If

            Dim sourceLocation As New NativeMethods.Point(0, 0)

            If PNG_Location_X = Nothing Then
                newLocation = New NativeMethods.Point(Me.Left, Me.Top)
            Else
                newLocation = New NativeMethods.Point(PNG_Location_X, PNG_Location_Y)
            End If

            Dim blend As New BLENDFUNCTION
            With blend
                .BlendOp = AC_SRC_OVER          ' Only works with a 32bpp bitmap
                .BlendFlags = 0                 ' Always 0
                .SourceConstantAlpha = 255      ' Set to 255 for per-pixel alpha values
                .AlphaFormat = AC_SRC_ALPHA     ' Only works when the bitmap contains an alpha channel
            End With
            ' Update the window

            UpdateLayeredWindow(Handle, screenDc, newLocation, newsize, _
            memDc, sourceLocation, 0, blend, ULW_ALPHA)

        Finally

            ' Release device context
            ReleaseDC(IntPtr.Zero, screenDc)
            If hBitmap <> IntPtr.Zero Then

                SelectObject(memDc, hOldBitmap)
                DeleteObject(hBitmap)           ' Remove bitmap resources
            End If
            DeleteDC(memDc)

        End Try

    End Sub

    Protected Overrides ReadOnly Property CreateParams() As System.Windows.Forms.CreateParams
        Get
            ' Add the layered extended style (WS_EX_LAYERED) to this window
            Dim cParams As CreateParams = MyBase.CreateParams
            cParams.ExStyle = cParams.ExStyle Or WS_EX_LAYERED
            Return cParams
        End Get
    End Property

    Protected Overrides Sub WndProc(ByRef m As System.Windows.Forms.Message)
        If m.Msg = WM_NCHITTEST Then

            ' Tell Windows that the user is on the title bar (caption)
            If Splat.ImageIsClickable Then
                m.Result = New IntPtr(HTCAPTION)
            End If

        ElseIf m.Msg = WM_LBUTTONDOWN Then
            If Splat.ImageIsClickable Then
                Me.Close()
                Return
            End If
        End If

        MyBase.WndProc(m)

    End Sub

End Class