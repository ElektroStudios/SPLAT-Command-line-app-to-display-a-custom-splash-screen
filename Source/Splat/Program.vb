#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Windows.Forms

#End Region

#Region " Program "

''' <summary>
''' Contains the application entry point.
''' </summary>
Friend Module Program

    ''' <summary>
    ''' The main entry point of the application.
    ''' </summary>
    '''
    ''' <param name="args">
    ''' The command-line arguments.
    ''' </param>
    '''
    ''' <returns>
    ''' The application exit code. See <see cref="SplatExitCode"/>.
    ''' </returns>
    <STAThread>
    Public Function Main(args As String()) As Integer

        ' Attach to the console of the parent process (if any) to be able to print the help and the errors.
        ' When there is no parent console (e.g. launched from Explorer or an installer), the output is simply discarded.
        If NativeMethods.AttachConsole(NativeMethods.ATTACH_PARENT_PROCESS) OrElse
           Marshal.GetLastWin32Error() <> NativeMethods.ERROR_ACCESS_DENIED Then

            Return SplatLauncher.Run(args, Console.Out, AddressOf Program.ShowSplash)
        End If

        ' The parent console exists but cannot be accessed ("Access is denied"),
        ' e.g. because this process runs at a lower integrity level than the console.
        ' In that case, the help and the error messages are displayed in a message box instead.
        Using output As New StringWriter()
            Dim exitCode As SplatExitCode = SplatLauncher.Run(args, output, AddressOf Program.ShowSplash)
            Program.ShowOutputMessageBox(output.ToString(), exitCode)
            Return exitCode
        End Using

    End Function

    ''' <summary>
    ''' Displays the splash screen and waits until it is closed.
    ''' </summary>
    '''
    ''' <param name="request">
    ''' The splash screen request.
    ''' </param>
    '''
    ''' <returns>
    ''' <see cref="SplatExitCode.Success"/>.
    ''' </returns>
    Private Function ShowSplash(request As SplashRequest) As SplatExitCode

        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(False)

        Using form As New Splat(request)
            Application.Run(form)
        End Using

        Return SplatExitCode.Success

    End Function

    ''' <summary>
    ''' Displays the application output (help or error messages) in a message box, when there is any output.
    ''' </summary>
    '''
    ''' <param name="output">
    ''' The application output.
    ''' </param>
    '''
    ''' <param name="exitCode">
    ''' The application exit code, used to choose the message box icon.
    ''' </param>
    Private Sub ShowOutputMessageBox(output As String, exitCode As SplatExitCode)

        Dim text As String = output.Trim()
        If text.Length = 0 Then
            Return ' The splash screen was displayed successfully; nothing to report.
        End If

        Application.EnableVisualStyles()
        Dim icon As MessageBoxIcon = If(exitCode = SplatExitCode.Success, MessageBoxIcon.Information, MessageBoxIcon.Error)
        MessageBox.Show(text, "Splat", MessageBoxButtons.OK, icon)

    End Sub

End Module

#End Region
