#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.IO

#End Region

#Region " TemporaryDirectory "

Namespace TestHelpers

    ''' <summary>
    ''' A uniquely named temporary directory that is deleted (with all its contents) when disposed.
    ''' </summary>
    Friend NotInheritable Class TemporaryDirectory : Implements IDisposable

        ''' <summary>
        ''' Gets the full path of the temporary directory.
        ''' </summary>
        Friend ReadOnly Property FullPath As String

        ''' <summary>
        ''' Initializes a new instance of the <see cref="TemporaryDirectory"/> class,
        ''' creating a new uniquely named directory in the user's temporary folder.
        ''' </summary>
        Friend Sub New()
            Me.FullPath = Path.Combine(Path.GetTempPath(), $"Splat.Tests_{Guid.NewGuid():N}")
            Directory.CreateDirectory(Me.FullPath)
        End Sub

        ''' <summary>
        ''' Gets the full path of a file with the specified name inside the temporary directory.
        ''' The file is not created.
        ''' </summary>
        '''
        ''' <param name="fileName">
        ''' The file name.
        ''' </param>
        '''
        ''' <returns>
        ''' The full file path.
        ''' </returns>
        Friend Function GetFilePath(fileName As String) As String
            Return Path.Combine(Me.FullPath, fileName)
        End Function

        ''' <summary>
        ''' Deletes the temporary directory and all its contents.
        ''' </summary>
        Public Sub Dispose() Implements IDisposable.Dispose
            If Directory.Exists(Me.FullPath) Then
                Directory.Delete(Me.FullPath, recursive:=True)
            End If
        End Sub

    End Class

End Namespace

#End Region
