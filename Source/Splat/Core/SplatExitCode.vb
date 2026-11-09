#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " SplatExitCode "

''' <summary>
''' Specifies the exit codes returned by the application.
''' </summary>
Friend Enum SplatExitCode As Integer

    ''' <summary>
    ''' The splash screen (or the help) was displayed successfully.
    ''' </summary>
    Success = 0

    ''' <summary>
    ''' A command-line argument is missing, unknown or has an invalid value.
    ''' </summary>
    InvalidArguments = 1

    ''' <summary>
    ''' The specified image file does not exist.
    ''' </summary>
    FileNotFound = 2

    ''' <summary>
    ''' The specified image file is not a valid or supported image, or it could not be processed.
    ''' </summary>
    InvalidImage = 3

End Enum

#End Region
