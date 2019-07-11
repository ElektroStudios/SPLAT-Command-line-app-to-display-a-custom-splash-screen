# SPLAT

**SPLAT** is a command-line interface tool to generate a splash-screen, that is, displays an image for a short period of time before starting a specific process.

The main idea is to use **SPLAT** as part of any **WinRar** or similar self-extracting files, Batch-script codes, etc.

The program is aware of png files with transparency.

# **Donations**

##### Through Paypal:
If you like my work and want to support it, then please consider to deposit a donation through **Paypal** by clicking on the next button:

[![Donataion Account](Images/Paypal.png)](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=E4RQEV6YF5NZY)

[![Donataion Amount](https://img.shields.io/badge/Current%20donations-0%24-red.svg)](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=E4RQEV6YF5NZY)

You are free to specify whatever amount of money you wish. That money will be sent to my **Paypal** account.

##### Through Envato:
If you are a .NET programmer, then maybe you would like to consider the purchase of 
'**DevCase for .NET Framework**', a powerful set of APIs for .NET developers, created by me. 

You can click the next button to go to the product specifications and the purchase page:

[![DevCase for .NET Framework](Images/DevCase%20Banner.png)](https://codecanyon.net/item/elektrokit-class-library-for-net/19260282)

Note that any source-code within the namespace 'DevCase' included in this **GitHub** repository, was freely extracted and distributed from the commercial library '**DevCase for .NET Framework**'.

<u>**Thanks in advance for your consideration!**</u> :thumbsup:

# Screenshots

![](Preview/SPLAT%2001.png)

# Demo video

[![IMAGE ALT TEXT HERE](https://img.youtube.com/vi/yn4c9w50Z9U/0.jpg)](https://www.youtube.com/watch?v=yn4c9w50Z9U) 
# Usage

#### Syntax

    Splat.exe [IMAGEFILE] [SWITCHES]
    
#### Switches

    /Duration   | The Splash duration.    (Default: 5000)
    /FXDuration | The effects duration.   (Default: 1500)
    /FadeIN     | Enables FadeIN Effect.
    /FadeOUT    | Enables FadeOUT Effect.
    /Resize     | Resizes the image.
    /Location   | Relocates the image.
    /Clickable  | Enables click on image to close.
    /OnTop      | Set the image on top of other windows.
    /?          | Shows this help.
    
#### Switches syntax

    /Duration   (ms)
    /FXDuration (ms)
    /Resize     (WidthXHeight)
    /Location   (X,Y)

### Real world examples

    Splat.exe "C:\Image.png"
    (Shows a image at the center of the screen for 3000 ms.)

    Splat.exe "C:\Image.png" /Resize 400x400 /Location 100,300
    (Shows a resized image to 200x400 px at 100,300 coordenates for 3000 ms.)

    Splat.exe "C:\Image.png" /Duration 6000 /FadeIN /FadeOUT /FXDuration 2000
    (Shows a image at the center of the screen with fade effects for 6000 ms.
    , plus 2000 ms for each effect then is a total of 10.000 ms.)
    