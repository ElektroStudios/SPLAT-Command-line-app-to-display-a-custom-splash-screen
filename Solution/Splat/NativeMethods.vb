Imports System.Runtime.InteropServices
Friend Class NativeMethods

    Public Const WS_EX_LAYERED As Integer = &H80000
    Public Const HTCAPTION As Integer = &H2
    Public Const WM_NCHITTEST As Integer = &H84
    Public Const ULW_ALPHA As Integer = &H2
    Public Const AC_SRC_OVER As Byte = &H0
    Public Const AC_SRC_ALPHA As Byte = &H1

    <DllImport("user32.dll", ExactSpelling:=True, SetLastError:=True)> _
    Public Shared Function UpdateLayeredWindow(ByVal hwnd As IntPtr, ByVal hdcDst As IntPtr, _
    ByRef pptDst As Point, ByRef psize As Size, ByVal hdcSrc As IntPtr, ByRef pprSrc As Point, _
    ByVal crKey As Int32, ByRef pblend As BLENDFUNCTION, ByVal dwFlags As Int32) As Boolean
    End Function

    <DllImport("gdi32.dll", ExactSpelling:=True, SetLastError:=True)> _
    Public Shared Function CreateCompatibleDC(ByVal hDC As IntPtr) As IntPtr
    End Function

    <DllImport("user32.dll", ExactSpelling:=True, SetLastError:=True)> _
    Public Shared Function GetDC(ByVal hWnd As IntPtr) As IntPtr
    End Function

    <DllImport("user32.dll", ExactSpelling:=True)> _
    Public Shared Function ReleaseDC(ByVal hWnd As IntPtr, ByVal hDC As IntPtr) As Integer
    End Function

    <DllImport("gdi32.dll", ExactSpelling:=True, SetLastError:=True)> _
    Public Shared Function DeleteDC(ByVal hdc As IntPtr) As Boolean
    End Function

    <DllImport("gdi32.dll", ExactSpelling:=True)> _
    Public Shared Function SelectObject(ByVal hDC As IntPtr, ByVal hObject As IntPtr) As IntPtr
    End Function

    <DllImport("gdi32.dll", ExactSpelling:=True, SetLastError:=True)> _
    Public Shared Function DeleteObject(ByVal hObject As IntPtr) As Boolean
    End Function

    <StructLayout(LayoutKind.Sequential)> _
    Public Structure Point
        Public x As Int32
        Public y As Int32
        Sub New(ByVal x As Int32, ByVal y As Int32)
            Me.x = x
            Me.y = y
        End Sub
    End Structure

    <StructLayout(LayoutKind.Sequential)> _
    Public Structure Size
        Public cx As Int32
        Public cy As Int32
        Sub New(ByVal cx As Int32, ByVal cy As Int32)
            Me.cx = cx
            Me.cy = cy
        End Sub
    End Structure

    <StructLayout(LayoutKind.Sequential, Pack:=1)> _
    Public Structure ARGB
        Public Blue As Byte
        Public Green As Byte
        Public Red As Byte
        Public Alpha As Byte
    End Structure

    <StructLayout(LayoutKind.Sequential, Pack:=1)> _
    Public Structure BLENDFUNCTION
        Public BlendOp As Byte
        Public BlendFlags As Byte
        Public SourceConstantAlpha As Byte
        Public AlphaFormat As Byte
    End Structure

End Class