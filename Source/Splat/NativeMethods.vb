#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Runtime.InteropServices


#End Region

#Region " NativeMethods "

''' <summary>
''' Platform Invocation (P/Invoke) declarations.
''' </summary>
Friend NotInheritable Class NativeMethods

    Private Sub New()
    End Sub

    Public Const ATTACH_PARENT_PROCESS As Integer = -1
    Public Const ERROR_ACCESS_DENIED As Integer = 5
    Public Const WS_EX_LAYERED As Integer = &H80000
    Public Const WM_LBUTTONDOWN As Integer = &H201
    Public Const ULW_ALPHA As Integer = &H2
    Public Const AC_SRC_OVER As Byte = &H0
    Public Const AC_SRC_ALPHA As Byte = &H1

    <DllImport("kernel32.dll", ExactSpelling:=True, SetLastError:=True)>
    Public Shared Function AttachConsole(processId As Integer) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    <DllImport("user32.dll", ExactSpelling:=True, SetLastError:=True)>
    Public Shared Function UpdateLayeredWindow(hwnd As IntPtr, hdcDst As IntPtr,
                                               ByRef pptDst As Point, ByRef psize As Size,
                                               hdcSrc As IntPtr, ByRef pprSrc As Point,
                                               crKey As Integer, ByRef pblend As BLENDFUNCTION,
                                               dwFlags As Integer) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    <DllImport("gdi32.dll", ExactSpelling:=True, SetLastError:=True)>
    Public Shared Function CreateCompatibleDC(hDC As IntPtr) As IntPtr
    End Function

    <DllImport("user32.dll", ExactSpelling:=True, SetLastError:=True)>
    Public Shared Function GetDC(hWnd As IntPtr) As IntPtr
    End Function

    <DllImport("user32.dll", ExactSpelling:=True)>
    Public Shared Function ReleaseDC(hWnd As IntPtr, hDC As IntPtr) As Integer
    End Function

    <DllImport("gdi32.dll", ExactSpelling:=True, SetLastError:=True)>
    Public Shared Function DeleteDC(hdc As IntPtr) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    <DllImport("gdi32.dll", ExactSpelling:=True)>
    Public Shared Function SelectObject(hDC As IntPtr, hObject As IntPtr) As IntPtr
    End Function

    <DllImport("gdi32.dll", ExactSpelling:=True, SetLastError:=True)>
    Public Shared Function DeleteObject(hObject As IntPtr) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    <StructLayout(LayoutKind.Sequential)>
    Public Structure Point
        Public x As Integer
        Public y As Integer
        Public Sub New(x As Integer, y As Integer)
            Me.x = x
            Me.y = y
        End Sub
    End Structure

    <StructLayout(LayoutKind.Sequential)>
    Public Structure Size
        Public cx As Integer
        Public cy As Integer
        Public Sub New(cx As Integer, cy As Integer)
            Me.cx = cx
            Me.cy = cy
        End Sub
    End Structure

    <StructLayout(LayoutKind.Sequential, Pack:=1)>
    Public Structure BLENDFUNCTION
        Public BlendOp As Byte
        Public BlendFlags As Byte
        Public SourceConstantAlpha As Byte
        Public AlphaFormat As Byte
    End Structure

End Class

#End Region
