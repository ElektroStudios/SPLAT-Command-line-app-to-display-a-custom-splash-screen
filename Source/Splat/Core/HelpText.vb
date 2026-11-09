#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Reflection

#End Region

#Region " HelpText "

''' <summary>
''' Provides the help text of the application.
''' </summary>
Friend Module HelpText

#Region " Constants "

    ''' <summary>
    ''' The URL of the GitHub repository of the application.
    ''' </summary>
    Friend Const RepositoryUrl As String = "https://github.com/ElektroStudios/SPLAT-Command-line-app-to-display-a-custom-splash-screen"

#End Region

#Region " Properties "

    ''' <summary>
    ''' Gets the help text of the application, including the current version and the repository URL.
    ''' </summary>
    Friend ReadOnly Property Value As String
        Get
            Dim version As Version = Assembly.GetExecutingAssembly().GetName().Version
            Return HelpText.Template.
                Replace("{VERSION}", $"v{version.Major}.{version.Minor}").
                Replace("{URL}", HelpText.RepositoryUrl)
        End Get
    End Property

    ''' <summary>
    ''' Gets the help text template, with the "{VERSION}" and "{URL}" placeholders.
    ''' </summary>
    Private ReadOnly Property Template As String =
        <a><![CDATA[
  _______       __       __   
 |   _   .-----|  .---.-|  |_ 
 |   |___|  _  |  |  _  |   _|
 |____   |   __|__|___._|____|
 |:  |   |__|    Splash Screen   {VERSION}
 |::.. . |   By ElektroStudios                       
 `-------'                
 {URL}
                              
[+] Syntax:

    Splat.exe [IMAGE FILE] [SWITCHES]

[+] Switches:

    /Duration   | The splash screen duration.    (Default: 5000 ms)
    /FXDuration | The visual effects duration.   (Default: 1500 ms)
    /FadeIN     | Enable FadeIN effect.          (Not supported for PNG/ICO images)
    /FadeOUT    | Enable FadeOUT effect.         (Not supported for PNG/ICO images)
    /Resize     | Set a new size for the image.
    /Location   | Set the position of the image on the current screen.
    /Clickable  | Enables doing click on the image to close it.
    /OnTop      | Set the image on top of other windows.
    /?          | Shows this help.

[+] Switches values Syntax:

    /Duration   (ms)
    /FXDuration (ms)
    /Resize     (widthXheight)
    /Location   (X,Y)

[+] Exit codes:

    0 | Success.
    1 | Missing, unknown or invalid argument.
    2 | The image file does not exist.
    3 | The image file is not a valid or supported image.

[+] Examples:

    Splat.exe "C:\Image.png"
    (Shows a image at the center of the screen.)
   
    Splat.exe "C:\Image.png" /Resize 200x400 /Location 100,300
    (Shows a resized image to 200x400 px at 100,300 coordinates.)
   
    Splat.exe "C:\Image.jpg" /Duration 6000 /FadeIN /FadeOUT /FXDuration 2000
    (Shows a image with fade effects for 6000 ms plus 2000 ms for each effect)
]]></a>.Value

#End Region

End Module

#End Region
