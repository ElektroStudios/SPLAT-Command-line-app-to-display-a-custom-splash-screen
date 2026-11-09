#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " SplashImageException "

''' <summary>
''' The exception that is thrown when the splash screen image cannot be loaded or processed.
''' <para></para>
''' The <see cref="Exception.Message"/> of this exception is meant to be displayed to the end-user.
''' </summary>
<Serializable>
Friend NotInheritable Class SplashImageException : Inherits Exception

    ''' <summary>
    ''' Initializes a new instance of the <see cref="SplashImageException"/> class.
    ''' </summary>
    '''
    ''' <param name="message">
    ''' The user-friendly message that describes the error.
    ''' </param>
    '''
    ''' <param name="innerException">
    ''' The exception that is the cause of the current exception.
    ''' </param>
    Friend Sub New(message As String, innerException As Exception)
        MyBase.New(message, innerException)
    End Sub

End Class

#End Region
