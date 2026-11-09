# SPLAT Change Log 📋

## v1.9 *(current)* 🆕
#### 🌟 Improvements:
    • Added a unit test project (Splat.Tests).
    • Added exit codes: 0 = Success, 1 = Invalid arguments, 2 = File not found, 3 = Invalid image.
    • Unknown arguments and malformed /Resize and /Location values are now reported as errors.
    • Improved the image resize quality for PNG/ICO images, and removed the semi-transparent artifact at the image edges.
    • The transparency key color for JPG/BMP/GIF images is now computed in a single pass (faster startup with large images).
    • Fade effects no longer consume 100% of a CPU core.
    • When the console of the parent process cannot be accessed (AttachConsole fails with "Access is denied", e.g. due to a lower integrity level), the help and error messages are displayed in a message box.
    • /FadeOUT no longer adds a pointless delay for PNG/ICO images (fade effects are not supported for them).
#### 🐛 Bug Fixes:
    • "/?" did not display the help when used as the first (or only) argument.
    • Displaying the help or an argument error was followed by an error message box.
    • A valued switch without value at the end of the command-line (e.g. /Duration) crashed the application.
    • An invalid or corrupt image file crashed the application.
    • "/Location 0,Y" was ignored for PNG/ICO images (the image was centered instead).
    • /Location was ignored for JPG/BMP/GIF images when /Resize was specified after it.
    • /OnTop had no effect for PNG/ICO images.
    • /Clickable did not close PNG/ICO images (the click started a window drag operation instead).
    • The fade-out effect was interrupted when /FXDuration was greater than /Duration.

## v1.8 🔄
#### 🌟 Improvements:
    • Added support for GIF images.
    • Fixed "/Clickable" logic for PNG/ICO images.

## v1.7 🔄
#### 🌟 Improvements:
    • Updated to .NET Framework 4.8.
    • Syntax help messages were improved a bit.

## v1.1 to v1-6 🔄
*Change log information is not available.*

## v1.0 (2013) 🔄
Initial Release.