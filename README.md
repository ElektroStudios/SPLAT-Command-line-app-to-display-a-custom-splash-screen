# SPLAT

**SPLAT** is a command-line interface tool to generate a splash-screen, that is, displays an image for a short period of time before starting a specific process.

The main idea is to use **SPLAT** as part of any **WinRar** or similar self-extracting files, Batch-script codes, etc.

The program is aware of png files with transparency.

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
    