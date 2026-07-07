Public Class Splat

#Region " Info "

    ' Splat
    ' A SplashScreen Tool
    ' Version 1.8
    ' 14/06/2025

    ' [+] Changes v1.8:
    ' - Added support for GIF images.
    ' - Fixed "/Clickable" logic for PNG/ICO images.

    ' [+] Changes v1.7:
    ' - Updated to .NET Framework 4.8.
    ' - Syntax help messages were improved a bit.

    ' [+] Changes v1.1:
    ' - Bug corrected: Image is not correctly centered after a resize operation.
    ' - Bug corrected: Form transparency affect PNG/ICO images. (Images looses pixel colors) 
    ' - Added full compatibility with ICO/PNG transparents and with shadows.
    ' - Added "/Ontop" and "/Clickable" switches.
    ' - Added a icon to the application.
    ' - Code optimizations to load faster.
    ' - Mistypsed word corrections in Help section.

    ' [+] ToDo:
    ' - Add FadeIN/FadeOUT compatibility por PNG/ICO images.
    ' - Add more FX effects.

#End Region

#Region " Vars & Delegate "

    Dim Arguments As List(Of String) = Set_CommandLine_Arguments()

    Dim img As Image
    Public imgbmp
    Dim ImageIsTransparent As Boolean = False
    Public Shared ImageIsClickable As Boolean = False
    Public Enable_FadeIn As Boolean = False
    Public Enable_FadeOut As Boolean = False
    Dim Duration As Int64 = 5000
    Dim Effects_Duration As Int64 = 1500
    Dim Desktop_RES As System.Windows.Forms.Screen = System.Windows.Forms.Screen.PrimaryScreen

    Delegate Sub Fade_Delegate(ByVal [Form] As Form, ByVal [Action] As FadeEffect, ByVal [TimeMs] As Double, ByVal [ToOpacity] As Double)

    Declare Function AttachConsole Lib "kernel32.dll" (ByVal dwProcessId As Int32) As Boolean
    ' Declare Function FreeConsole Lib "kernel32.dll" () As Boolean

#End Region

#Region " Debug CommandLine Arguments "

    Public Function Set_CommandLine_Arguments() As List(Of String)
#If DEBUG Then
        ' Debug Commandline arguments for testing:
        If Debugger.IsAttached Then
            Dim DebugArguments = "C:\Users\Administrador\Desktop\images.jpg /fadein /duration 5000"
            Return DebugArguments.Split(" ").ToList
        Else
            ' Nomal Commandline arguments
            Return My.Application.CommandLineArgs.ToList
        End If
#Else
        ' Nomal Commandline arguments
        Return My.Application.CommandLineArgs.ToList
#End If
    End Function

#End Region

#Region " Form "

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Try
            AttachConsole(-1) ' Attach the console mode
        Catch 'ex As Exception
            ' MsgBox(ex.Message)
        End Try

        Me.Opacity = 1
        Me.SetStyle(ControlStyles.ResizeRedraw, True)
        Me.SetStyle(ControlStyles.AllPaintingInWmPaint, True)
        Me.SetStyle(ControlStyles.Opaque, True)
        Me.SetStyle(ControlStyles.OptimizedDoubleBuffer, True)
        Me.SetStyle(ControlStyles.UserPaint, True)
        Me.Size = New Point(0, 0)

        Pass_Arguments()

        If Not ImageIsTransparent Then
            For Each col In System.[Enum].GetValues(GetType(KnownColor))
                If Not Image_Has_Color(img, Color.FromKnownColor(col)) Then
                    If Not Color.FromKnownColor(col).ToString.ToLower.Contains("transparent") Then
                        Me.BackColor = Color.FromKnownColor(col)
                        Me.TransparencyKey = Color.FromKnownColor(col)
                        Exit For
                    End If
                End If
            Next
            If Enable_FadeIn Then
                Me.Opacity = 0
                Me.Show()
                Fade(Me, FadeEffect.FadeIN, Effects_Duration, 1)
            Else
                Me.Opacity = 1
            End If
        Else
            Dim f As New AlphaBlendedForm(imgbmp)
            'f.Opacity = 0
            f.Show()
            'Fade(f, FadeEffect.FadeIN, Effects_Duration, 1)
        End If

        Timer_CloseForm.Enabled = True

    End Sub

    ' PictureBox Click Event
    Private Sub PictureBox_Img_Click(sender As Object, e As EventArgs)
        Application.Exit()
    End Sub

    ' Timer to close the application
    Private Sub Timer_Close_Tick(sender As Object, e As EventArgs) Handles Timer_CloseForm.Tick
        If Enable_FadeOut Then Fade(Me, FadeEffect.FadeOut, Effects_Duration, 0)
        End ' End because if form loading event don't finished to load it throws an exception.
    End Sub


#End Region

#Region " Arguments "

    Private Sub Pass_Arguments()

        ' Image file (First argument)
        If Not Arguments.Count = 0 Then
            If IO.File.Exists(Arguments.Item(0)) Then
                img = Image.FromFile(Arguments.Item(0))

                If Arguments.Item(0).ToLower.EndsWith(".png") Or Arguments.Item(0).ToLower.EndsWith(".ico") Then
                    ImageIsTransparent = True
                    imgbmp = New Bitmap(img)
                Else
                    PictureBox_Img.Image = img
                    Me.Size = New Point(img.Size.Width, img.Size.Height)
                    Me.Location = New Point((Desktop_RES.Bounds.Width - Me.Width) / 2, (Desktop_RES.Bounds.Height - Me.Height) / 2)
                End If

            Else
                Console.WriteLine("File " & ControlChars.Quote & Arguments.Item(0).ToString & ControlChars.Quote & " don't exist.")
                Application.Exit()
            End If
        Else
            Help()
        End If

        ' Rest of arguments
        For I As Integer = 1 To Arguments.Count - 1
            Application.DoEvents()

            ' Help
            If Arguments.Item(I) = "/?" Or Arguments.Item(I).ToLower = "-h" Or Arguments.Item(I).ToLower = "-help" Then
                Help()
            End If

            ' Duration
            If Arguments.Item(I).ToLower = "/duration" Then
                Try
                    Timer_CloseForm.Interval = Arguments.Item(I + 1)
                Catch ' ex As Exception
                    ' MsgBox(ex.Message)
                    Catch_Argument_Error("/Duration", Arguments.Item(I + 1).ToString)
                End Try
            End If

            ' Resize
            If Arguments(I).ToLower = "/resize" Then
                Try
                    Dim newWidth As Integer = Convert.ToInt32(Arguments.Item(I + 1).ToLower.Split("x").First)
                    Dim newHeight As Integer = Convert.ToInt32(Arguments.Item(I + 1).ToLower.Split("x").Last)

                    If ImageIsTransparent Then
                        imgbmp = Resize_Image(img, newWidth, newHeight)
                    Else

                        PictureBox_Img.SizeMode = PictureBoxSizeMode.Zoom
                        Me.Size = New Point(newWidth, newHeight)
                        Me.Location = New Point((Desktop_RES.Bounds.Width - Me.Width) / 2, (Desktop_RES.Bounds.Height - Me.Height) / 2)
                    End If
                Catch ' ex As Exception
                    ' MsgBox(ex.Message)
                    Catch_Argument_Error("/Resize", Arguments.Item(I + 1).ToString)
                End Try
            End If

            ' Location
            If Arguments(I).ToLower = "/location" Then
                Try
                    If ImageIsTransparent Then
                        AlphaBlendedForm.PNG_Location_X = Arguments.Item(I + 1).ToLower.Split(",").First
                        AlphaBlendedForm.PNG_Location_Y = Arguments.Item(I + 1).ToLower.Split(",").Last
                    Else
                        Me.Location = New Point(Arguments.Item(I + 1).Split(",").First, Arguments.Item(I + 1).Split(",").Last)
                    End If
                Catch ' ex As Exception
                    ' MsgBox(ex.Message)
                    Catch_Argument_Error("/Location", Arguments.Item(I + 1).ToString)
                End Try
            End If

            ' Fade IN
            If Arguments(I).ToLower = "/fadein" Then
                Enable_FadeIn = True
            End If

            ' Fade Out
            If Arguments(I).ToLower = "/fadeout" Then
                Enable_FadeOut = True
            End If

            ' Fade IN/Out Duration
            If Arguments.Item(I).ToLower = "/fxduration" Then
                Try
                    Effects_Duration = Arguments.Item(I + 1)
                Catch ' ex As Exception
                    ' MsgBox(ex.Message)
                    Catch_Argument_Error("/FXDuration", Arguments.Item(I + 1).ToString)
                End Try
            End If

            ' Clickable
            If Arguments(I).ToLower = "/clickable" Then
                ImageIsClickable = True
                PictureBox_Img.Cursor = Cursors.Hand
                AddHandler PictureBox_Img.Click, AddressOf PictureBox_Img_Click
            End If

            ' OnTop
            If Arguments(I).ToLower = "/ontop" Then
                Me.TopMost = True
            End If

        Next

    End Sub

    Private Sub Catch_Argument_Error(ByVal Switch As String, ByVal Value As String)
        Console.WriteLine("Wrong value specified for " & Switch & " " & ControlChars.Quote & Value & ControlChars.Quote)
        Application.Exit()
    End Sub

#End Region

#Region " Image Has Color? "

    Private Function Image_Has_Color(ByVal image As Image, ByVal color As Color) As Boolean

        Try
            Using Bitmap_Image = New Bitmap(image.Width, image.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb)

                Graphics.FromImage(Bitmap_Image).DrawImage(image, 0, 0)

                Dim Bitmap_Data = Bitmap_Image.LockBits(New Rectangle(0, 0, Bitmap_Image.Width, Bitmap_Image.Height), System.Drawing.Imaging.ImageLockMode.[ReadOnly], Bitmap_Image.PixelFormat)
                Dim Bitmap_Pointer As IntPtr = Bitmap_Data.Scan0

                Dim Pixel_Color As Int32
                Dim Result As Boolean = False

                For i = 0 To Bitmap_Data.Height * Bitmap_Data.Width - 1

                    Pixel_Color = System.Runtime.InteropServices.Marshal.ReadInt32(Bitmap_Pointer, i * 4)

                    If (Pixel_Color And &HFF000000) <> 0 AndAlso (Pixel_Color And &HFFFFFF) = (color.ToArgb() And &HFFFFFF) Then
                        Result = True
                        Exit For
                    End If

                Next

                Bitmap_Image.UnlockBits(Bitmap_Data)
                Return Result

            End Using

        Catch ex As Exception
            MsgBox(ex.Message)
            Return Nothing
        End Try

    End Function

#End Region

#Region " Resize Image "

    Private Function Resize_Image(ByVal img As Image, ByVal Width As Int32, ByVal Height As Int32) As Bitmap
        Dim Bitmap_Source As New Bitmap(img)
        Dim Bitmap_Dest As New Bitmap(CInt(Width), CInt(Height), Imaging.PixelFormat.Format32bppArgb)
        Dim Graphic As Graphics = Graphics.FromImage(Bitmap_Dest)
        Graphic.DrawImage(Bitmap_Source, 0, 0, Bitmap_Dest.Width + 1, Bitmap_Dest.Height + 1)
        Return Bitmap_Dest
    End Function

#End Region

#Region " Fade IN/OUT "

    Public Enum FadeEffect
        FadeIN
        FadeOut
    End Enum

    Public Sub Fade(ByVal whd As Form, ByVal Action As String, ByVal ms As Double, ByVal ToOpacity As Double)
        ms = ms / 100
        Dim etime As DateTime
        If whd.InvokeRequired Then
            Dim d As New Fade_Delegate(AddressOf Fade)
            whd.Invoke(d, New Object() {whd, ms, ToOpacity})
        Else
            Try
                While 1 = 1
                    If Action = FadeEffect.FadeIN Then whd.Opacity += 0.01
                    If Action = FadeEffect.FadeOut Then whd.Opacity -= 0.01
                    If whd.Opacity.ToString.Substring(0, 3) = ToOpacity.ToString Then Exit While
                    etime = Now.AddMilliseconds(ms)
                    While Now < etime
                        System.Windows.Forms.Application.DoEvents()
                    End While
                End While
            Catch ex As Exception
            End Try
        End If
    End Sub

#End Region

#Region " Help "

    Private Sub Help()
        Dim Logo As String = <a><![CDATA[
  _______       __       __   
 |   _   .-----|  .---.-|  |_ 
 |   |___|  _  |  |  _  |   _|
 |____   |   __|__|___._|____|
 |:  |   |__|    Splash Screen                 
 |::.. . |   By ElektroStudios                       
 `-------'                
                              
[+] Syntax:

    Splat.exe [IMAGE FILE] [SWITCHES]

[+] Switches:

    /Duration   | The splash screen duration.    (Default: 5000 ms)
    /FXDuration | The visual effects duration.   (Default: 1500 ms)
    /FadeIN     | Enable FadeIN effect.
    /FadeOUT    | Enable FadeOUT effect.
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

[+] Examples:

    Splat.exe "C:\Image.png"
    (Shows a image at the center of the screen.)
   
    Splat.exe "C:\Image.png" /Resize 200x400 /Location 100,300
    (Shows a resized image to 200x400 px at 100,300 coordinates.)
   
    Splat.exe "C:\Image.png" /Duration 6000 /FadeIN /FadeOUT /FXDuration 2000
    (Shows a image with fade effects for 6000 ms plus 2000 ms for each effect)
]]></a>.Value

        Console.WriteLine(Logo)
        Application.Exit()
    End Sub

    Private Sub PictureBox_Img_Click_1(sender As Object, e As EventArgs) Handles PictureBox_Img.Click

    End Sub

#End Region

End Class