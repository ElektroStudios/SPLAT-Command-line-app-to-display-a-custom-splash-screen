#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Collections.Generic

#End Region

#Region " FadeStep "

''' <summary>
''' Represents a single step of a fade effect.
''' </summary>
Friend Structure FadeStep

    ''' <summary>
    ''' The opacity to apply at this step, in the range 0.0 to 1.0.
    ''' </summary>
    Friend ReadOnly Opacity As Double

    ''' <summary>
    ''' The elapsed time since the start of the effect, in milliseconds, at which this step must be applied.
    ''' </summary>
    Friend ReadOnly DueTimeMilliseconds As Integer

    ''' <summary>
    ''' Initializes a new instance of the <see cref="FadeStep"/> structure.
    ''' </summary>
    '''
    ''' <param name="opacity">
    ''' The opacity to apply at this step.
    ''' </param>
    '''
    ''' <param name="dueTimeMilliseconds">
    ''' The elapsed time since the start of the effect at which this step must be applied.
    ''' </param>
    Friend Sub New(opacity As Double, dueTimeMilliseconds As Integer)
        Me.Opacity = opacity
        Me.DueTimeMilliseconds = dueTimeMilliseconds
    End Sub

End Structure

#End Region

#Region " FadeScheduler "

''' <summary>
''' Calculates the steps of a linear fade effect.
''' </summary>
Friend Module FadeScheduler

    ''' <summary>
    ''' The default number of steps of a fade effect (an opacity change of 1% per step).
    ''' </summary>
    Friend Const DefaultStepCount As Integer = 100

    ''' <summary>
    ''' Creates the schedule of a linear fade effect.
    ''' <para></para>
    ''' The steps are evenly distributed over time; the last step always has exactly
    ''' the target opacity and a due time equal to the effect duration.
    ''' </summary>
    '''
    ''' <param name="fromOpacity">
    ''' The initial opacity, in the range 0.0 to 1.0.
    ''' </param>
    '''
    ''' <param name="toOpacity">
    ''' The target opacity, in the range 0.0 to 1.0.
    ''' </param>
    '''
    ''' <param name="durationMilliseconds">
    ''' The duration of the effect, in milliseconds.
    ''' </param>
    '''
    ''' <param name="stepCount">
    ''' The number of steps.
    ''' </param>
    '''
    ''' <returns>
    ''' The fade steps, in chronological order.
    ''' </returns>
    '''
    ''' <exception cref="ArgumentOutOfRangeException">
    ''' Thrown when an opacity is not in the range 0.0 to 1.0, when <paramref name="durationMilliseconds"/>
    ''' is negative, or when <paramref name="stepCount"/> is lower than 1.
    ''' </exception>
    Friend Function CreateSchedule(fromOpacity As Double, toOpacity As Double,
                                   durationMilliseconds As Integer, stepCount As Integer) As IReadOnlyList(Of FadeStep)

        FadeScheduler.ValidateOpacity(fromOpacity, NameOf(fromOpacity))
        FadeScheduler.ValidateOpacity(toOpacity, NameOf(toOpacity))

        If durationMilliseconds < 0 Then
            Throw New ArgumentOutOfRangeException(NameOf(durationMilliseconds), durationMilliseconds, "Value cannot be negative.")
        End If

        If stepCount < 1 Then
            Throw New ArgumentOutOfRangeException(NameOf(stepCount), stepCount, "Value must be greater than zero.")
        End If

        Dim steps As New List(Of FadeStep)(stepCount)
        For i As Integer = 1 To stepCount
            Dim opacity As Double = If(i = stepCount,
                                       toOpacity,
                                       fromOpacity + ((toOpacity - fromOpacity) * i / stepCount))
            Dim dueTime As Integer = CInt(CLng(durationMilliseconds) * i \ stepCount)
            steps.Add(New FadeStep(opacity, dueTime))
        Next

        Return steps.AsReadOnly()

    End Function

    ''' <summary>
    ''' Validates that the specified opacity is in the range 0.0 to 1.0.
    ''' </summary>
    '''
    ''' <param name="opacity">
    ''' The opacity to validate.
    ''' </param>
    '''
    ''' <param name="paramName">
    ''' The name of the parameter being validated.
    ''' </param>
    '''
    ''' <exception cref="ArgumentOutOfRangeException">
    ''' Thrown when <paramref name="opacity"/> is not in the range 0.0 to 1.0.
    ''' </exception>
    Private Sub ValidateOpacity(opacity As Double, paramName As String)

        If Double.IsNaN(opacity) OrElse opacity < 0.0R OrElse opacity > 1.0R Then
            Throw New ArgumentOutOfRangeException(paramName, opacity, "Opacity must be in the range 0.0 to 1.0.")
        End If

    End Sub

End Module

#End Region
