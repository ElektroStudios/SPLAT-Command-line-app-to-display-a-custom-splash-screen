#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Collections.Generic
Imports System.Drawing
Imports System.Globalization

#End Region

#Region " CommandLineParser "

''' <summary>
''' Parses the command-line arguments of the application into a <see cref="SplatOptions"/> instance.
''' </summary>
Friend Module CommandLineParser

#Region " Constants "

    ''' <summary>
    ''' The name of the switch that sets the splash screen duration.
    ''' </summary>
    Friend Const SwitchDuration As String = "/Duration"

    ''' <summary>
    ''' The name of the switch that sets the visual effects duration.
    ''' </summary>
    Friend Const SwitchEffectsDuration As String = "/FXDuration"

    ''' <summary>
    ''' The name of the switch that enables the fade-in effect.
    ''' </summary>
    Friend Const SwitchFadeIn As String = "/FadeIN"

    ''' <summary>
    ''' The name of the switch that enables the fade-out effect.
    ''' </summary>
    Friend Const SwitchFadeOut As String = "/FadeOUT"

    ''' <summary>
    ''' The name of the switch that sets a new size for the image.
    ''' </summary>
    Friend Const SwitchResize As String = "/Resize"

    ''' <summary>
    ''' The name of the switch that sets the on-screen location of the image.
    ''' </summary>
    Friend Const SwitchLocation As String = "/Location"

    ''' <summary>
    ''' The name of the switch that allows to close the splash screen by clicking on the image.
    ''' </summary>
    Friend Const SwitchClickable As String = "/Clickable"

    ''' <summary>
    ''' The name of the switch that keeps the splash screen on top of other windows.
    ''' </summary>
    Friend Const SwitchOnTop As String = "/OnTop"

#End Region

#Region " Fields "

    ''' <summary>
    ''' The switches that display the help. Comparison is case-insensitive.
    ''' </summary>
    Private ReadOnly HelpSwitches As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase) From {
        "/?", "-?", "/h", "-h", "/help", "-help", "--help"
    }

#End Region

#Region " Public Methods "

    ''' <summary>
    ''' Parses the specified command-line arguments.
    ''' <para></para>
    ''' The first argument must be the image file path; the rest of arguments are switches.
    ''' <para></para>
    ''' Switch names are case-insensitive. When a switch is specified more than once, the last value wins.
    ''' </summary>
    '''
    ''' <param name="arguments">
    ''' The command-line arguments, without the executable path.
    ''' </param>
    '''
    ''' <returns>
    ''' The resulting <see cref="SplatOptions"/>.
    ''' <para></para>
    ''' When <paramref name="arguments"/> is empty or contains a help switch,
    ''' the returned options have <see cref="SplatOptions.ShowHelp"/> set to <see langword="True"/>.
    ''' </returns>
    '''
    ''' <exception cref="ArgumentNullException">
    ''' Thrown when <paramref name="arguments"/> or any of its items is <see langword="Nothing"/>.
    ''' </exception>
    '''
    ''' <exception cref="CommandLineArgumentException">
    ''' Thrown when an argument is missing, unknown or has an invalid value.
    ''' </exception>
    Friend Function Parse(arguments As IReadOnlyList(Of String)) As SplatOptions

        If arguments Is Nothing Then
            Throw New ArgumentNullException(NameOf(arguments))
        End If

        For Each argument As String In arguments
            If argument Is Nothing Then
                Throw New ArgumentNullException(NameOf(arguments), "The arguments collection cannot contain null items.")
            End If
        Next

        Dim options As New SplatOptions()

        ' Help is displayed when no arguments are specified, or when a help switch is found at any position.
        If arguments.Count = 0 OrElse CommandLineParser.ContainsHelpSwitch(arguments) Then
            options.ShowHelp = True
            Return options
        End If

        Dim imageFilePath As String = arguments(0).Trim()
        If imageFilePath.Length = 0 Then
            Throw New CommandLineArgumentException("The image file path cannot be empty.")
        End If
        options.ImageFilePath = imageFilePath

        Dim index As Integer = 1
        While index < arguments.Count

            Dim argument As String = arguments(index)

            Select Case True

                Case CommandLineParser.IsSwitch(argument, SwitchDuration)
                    Dim value As String = CommandLineParser.GetSwitchValue(arguments, index, SwitchDuration)
                    options.DurationMilliseconds = CommandLineParser.ParseMilliseconds(SwitchDuration, value, minimum:=1)
                    index += 1

                Case CommandLineParser.IsSwitch(argument, SwitchEffectsDuration)
                    Dim value As String = CommandLineParser.GetSwitchValue(arguments, index, SwitchEffectsDuration)
                    options.EffectsDurationMilliseconds = CommandLineParser.ParseMilliseconds(SwitchEffectsDuration, value, minimum:=0)
                    index += 1

                Case CommandLineParser.IsSwitch(argument, SwitchResize)
                    Dim value As String = CommandLineParser.GetSwitchValue(arguments, index, SwitchResize)
                    options.Size = CommandLineParser.ParseSize(SwitchResize, value)
                    index += 1

                Case CommandLineParser.IsSwitch(argument, SwitchLocation)
                    Dim value As String = CommandLineParser.GetSwitchValue(arguments, index, SwitchLocation)
                    options.Location = CommandLineParser.ParseLocation(SwitchLocation, value)
                    index += 1

                Case CommandLineParser.IsSwitch(argument, SwitchFadeIn)
                    options.FadeIn = True

                Case CommandLineParser.IsSwitch(argument, SwitchFadeOut)
                    options.FadeOut = True

                Case CommandLineParser.IsSwitch(argument, SwitchClickable)
                    options.Clickable = True

                Case CommandLineParser.IsSwitch(argument, SwitchOnTop)
                    options.OnTop = True

                Case Else
                    Throw New CommandLineArgumentException($"Unknown argument: ""{argument}"".")

            End Select

            index += 1
        End While

        Return options

    End Function

    ''' <summary>
    ''' Determines whether the specified argument is a help switch (e.g. "/?").
    ''' </summary>
    '''
    ''' <param name="argument">
    ''' The argument to check.
    ''' </param>
    '''
    ''' <returns>
    ''' <see langword="True"/> if <paramref name="argument"/> is a help switch; otherwise, <see langword="False"/>.
    ''' </returns>
    '''
    ''' <exception cref="ArgumentNullException">
    ''' Thrown when <paramref name="argument"/> is <see langword="Nothing"/>.
    ''' </exception>
    Friend Function IsHelpSwitch(argument As String) As Boolean

        If argument Is Nothing Then
            Throw New ArgumentNullException(NameOf(argument))
        End If

        Return CommandLineParser.HelpSwitches.Contains(argument.Trim())

    End Function

    ''' <summary>
    ''' Parses a duration value in milliseconds.
    ''' <para></para>
    ''' Only plain decimal digits are accepted (no sign, no thousands separators, no whitespace).
    ''' </summary>
    '''
    ''' <param name="switchName">
    ''' The name of the switch that owns the value (used in the error message).
    ''' </param>
    '''
    ''' <param name="value">
    ''' The value to parse.
    ''' </param>
    '''
    ''' <param name="minimum">
    ''' The minimum accepted value.
    ''' </param>
    '''
    ''' <returns>
    ''' The parsed value.
    ''' </returns>
    '''
    ''' <exception cref="ArgumentNullException">
    ''' Thrown when <paramref name="switchName"/> or <paramref name="value"/> is <see langword="Nothing"/>.
    ''' </exception>
    '''
    ''' <exception cref="CommandLineArgumentException">
    ''' Thrown when <paramref name="value"/> is not a valid integer, or it is lower than <paramref name="minimum"/>.
    ''' </exception>
    Friend Function ParseMilliseconds(switchName As String, value As String, minimum As Integer) As Integer

        CommandLineParser.ValidateParseArguments(switchName, value)

        Dim result As Integer
        If Not Integer.TryParse(value.Trim(), NumberStyles.None, CultureInfo.InvariantCulture, result) OrElse
           result < minimum Then

            Throw CommandLineParser.CreateWrongValueException(switchName, value,
                $"Expected an integer number of milliseconds greater than or equal to {minimum}.")
        End If

        Return result

    End Function

    ''' <summary>
    ''' Parses a size value with format "WIDTHxHEIGHT" (e.g. "200x400"). The "x" separator is case-insensitive.
    ''' <para></para>
    ''' Both width and height must be positive integers.
    ''' </summary>
    '''
    ''' <param name="switchName">
    ''' The name of the switch that owns the value (used in the error message).
    ''' </param>
    '''
    ''' <param name="value">
    ''' The value to parse.
    ''' </param>
    '''
    ''' <returns>
    ''' The parsed <see cref="Size"/>.
    ''' </returns>
    '''
    ''' <exception cref="ArgumentNullException">
    ''' Thrown when <paramref name="switchName"/> or <paramref name="value"/> is <see langword="Nothing"/>.
    ''' </exception>
    '''
    ''' <exception cref="CommandLineArgumentException">
    ''' Thrown when <paramref name="value"/> does not have a valid format.
    ''' </exception>
    Friend Function ParseSize(switchName As String, value As String) As Size

        CommandLineParser.ValidateParseArguments(switchName, value)

        Dim parts As String() = value.Trim().Split({"x"c, "X"c})
        Dim width As Integer
        Dim height As Integer

        If parts.Length <> 2 OrElse
           Not Integer.TryParse(parts(0), NumberStyles.None, CultureInfo.InvariantCulture, width) OrElse
           Not Integer.TryParse(parts(1), NumberStyles.None, CultureInfo.InvariantCulture, height) OrElse
           width <= 0 OrElse height <= 0 Then

            Throw CommandLineParser.CreateWrongValueException(switchName, value,
                "Expected format: WIDTHxHEIGHT, with positive integers (e.g. 200x400).")
        End If

        Return New Size(width, height)

    End Function

    ''' <summary>
    ''' Parses a location value with format "X,Y" (e.g. "100,300").
    ''' <para></para>
    ''' Negative coordinates are accepted, since they are valid in multi-monitor setups.
    ''' </summary>
    '''
    ''' <param name="switchName">
    ''' The name of the switch that owns the value (used in the error message).
    ''' </param>
    '''
    ''' <param name="value">
    ''' The value to parse.
    ''' </param>
    '''
    ''' <returns>
    ''' The parsed <see cref="Point"/>.
    ''' </returns>
    '''
    ''' <exception cref="ArgumentNullException">
    ''' Thrown when <paramref name="switchName"/> or <paramref name="value"/> is <see langword="Nothing"/>.
    ''' </exception>
    '''
    ''' <exception cref="CommandLineArgumentException">
    ''' Thrown when <paramref name="value"/> does not have a valid format.
    ''' </exception>
    Friend Function ParseLocation(switchName As String, value As String) As Point

        CommandLineParser.ValidateParseArguments(switchName, value)

        Dim parts As String() = value.Trim().Split(","c)
        Dim x As Integer
        Dim y As Integer

        If parts.Length <> 2 OrElse
           Not Integer.TryParse(parts(0), NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, x) OrElse
           Not Integer.TryParse(parts(1), NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, y) Then

            Throw CommandLineParser.CreateWrongValueException(switchName, value,
                "Expected format: X,Y, with integers (e.g. 100,300).")
        End If

        Return New Point(x, y)

    End Function

#End Region

#Region " Private Methods "

    ''' <summary>
    ''' Determines whether any of the specified arguments is a help switch.
    ''' </summary>
    '''
    ''' <param name="arguments">
    ''' The arguments to check.
    ''' </param>
    '''
    ''' <returns>
    ''' <see langword="True"/> if a help switch is found; otherwise, <see langword="False"/>.
    ''' </returns>
    Private Function ContainsHelpSwitch(arguments As IReadOnlyList(Of String)) As Boolean

        For Each argument As String In arguments
            If CommandLineParser.IsHelpSwitch(argument) Then
                Return True
            End If
        Next
        Return False

    End Function

    ''' <summary>
    ''' Determines whether the specified argument matches the specified switch name (case-insensitive).
    ''' </summary>
    '''
    ''' <param name="argument">
    ''' The argument to check.
    ''' </param>
    '''
    ''' <param name="switchName">
    ''' The switch name.
    ''' </param>
    '''
    ''' <returns>
    ''' <see langword="True"/> if the argument matches the switch; otherwise, <see langword="False"/>.
    ''' </returns>
    Private Function IsSwitch(argument As String, switchName As String) As Boolean

        Return String.Equals(argument.Trim(), switchName, StringComparison.OrdinalIgnoreCase)

    End Function

    ''' <summary>
    ''' Gets the value that follows the switch at the specified index.
    ''' </summary>
    '''
    ''' <param name="arguments">
    ''' The command-line arguments.
    ''' </param>
    '''
    ''' <param name="switchIndex">
    ''' The index of the switch in <paramref name="arguments"/>.
    ''' </param>
    '''
    ''' <param name="switchName">
    ''' The name of the switch (used in the error message).
    ''' </param>
    '''
    ''' <returns>
    ''' The switch value.
    ''' </returns>
    '''
    ''' <exception cref="CommandLineArgumentException">
    ''' Thrown when the switch is the last argument, so it has no value.
    ''' </exception>
    Private Function GetSwitchValue(arguments As IReadOnlyList(Of String), switchIndex As Integer, switchName As String) As String

        If switchIndex + 1 >= arguments.Count Then
            Throw New CommandLineArgumentException($"Missing value for {switchName}.")
        End If

        Return arguments(switchIndex + 1)

    End Function

    ''' <summary>
    ''' Validates the common arguments of the value parsing methods.
    ''' </summary>
    '''
    ''' <param name="switchName">
    ''' The name of the switch.
    ''' </param>
    '''
    ''' <param name="value">
    ''' The value to parse.
    ''' </param>
    '''
    ''' <exception cref="ArgumentNullException">
    ''' Thrown when <paramref name="switchName"/> or <paramref name="value"/> is <see langword="Nothing"/>.
    ''' </exception>
    Private Sub ValidateParseArguments(switchName As String, value As String)

        If switchName Is Nothing Then
            Throw New ArgumentNullException(NameOf(switchName))
        End If

        If value Is Nothing Then
            Throw New ArgumentNullException(NameOf(value))
        End If

    End Sub

    ''' <summary>
    ''' Creates a <see cref="CommandLineArgumentException"/> for a wrong switch value.
    ''' </summary>
    '''
    ''' <param name="switchName">
    ''' The name of the switch.
    ''' </param>
    '''
    ''' <param name="value">
    ''' The wrong value.
    ''' </param>
    '''
    ''' <param name="hint">
    ''' A hint describing the expected value.
    ''' </param>
    '''
    ''' <returns>
    ''' The resulting <see cref="CommandLineArgumentException"/>.
    ''' </returns>
    Private Function CreateWrongValueException(switchName As String, value As String, hint As String) As CommandLineArgumentException

        Return New CommandLineArgumentException($"Wrong value specified for {switchName} ""{value}"". {hint}")

    End Function

#End Region

End Module

#End Region
