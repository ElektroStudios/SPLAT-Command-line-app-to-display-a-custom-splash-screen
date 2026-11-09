#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " CommandLineArgumentException "

''' <summary>
''' The exception that is thrown when a command-line argument is missing, unknown or has an invalid value.
''' <para></para>
''' The <see cref="Exception.Message"/> of this exception is meant to be displayed to the end-user.
''' </summary>
<Serializable>
Friend NotInheritable Class CommandLineArgumentException : Inherits ArgumentException

    ''' <summary>
    ''' Initializes a new instance of the <see cref="CommandLineArgumentException"/> class.
    ''' </summary>
    '''
    ''' <param name="message">
    ''' The user-friendly message that describes the error.
    ''' </param>
    Friend Sub New(message As String)
        MyBase.New(message)
    End Sub

End Class

#End Region
