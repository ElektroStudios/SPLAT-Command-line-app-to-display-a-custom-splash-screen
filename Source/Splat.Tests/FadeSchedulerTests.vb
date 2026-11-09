#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Collections.Generic

Imports Microsoft.VisualStudio.TestTools.UnitTesting

#End Region

#Region " FadeSchedulerTests "

''' <summary>
''' Unit tests for the <see cref="FadeScheduler"/> module.
''' </summary>
<TestClass>
Public NotInheritable Class FadeSchedulerTests

    <TestMethod>
    Public Sub CreateSchedule_HasRequestedStepCount()

        Dim schedule As IReadOnlyList(Of FadeStep) = FadeScheduler.CreateSchedule(0.0R, 1.0R, 1500, FadeScheduler.DefaultStepCount)

        Assert.AreEqual(FadeScheduler.DefaultStepCount, schedule.Count)

    End Sub

    <TestMethod>
    Public Sub CreateSchedule_FadeIn_LastStepIsExactlyTargetAndDuration()

        Dim schedule As IReadOnlyList(Of FadeStep) = FadeScheduler.CreateSchedule(0.0R, 1.0R, 1500, 100)

        Assert.AreEqual(1.0R, schedule(schedule.Count - 1).Opacity)
        Assert.AreEqual(1500, schedule(schedule.Count - 1).DueTimeMilliseconds)

    End Sub

    <TestMethod>
    Public Sub CreateSchedule_FadeOut_LastStepIsExactlyZero()

        Dim schedule As IReadOnlyList(Of FadeStep) = FadeScheduler.CreateSchedule(1.0R, 0.0R, 1500, 100)

        Assert.AreEqual(0.0R, schedule(schedule.Count - 1).Opacity)

    End Sub

    <TestMethod>
    Public Sub CreateSchedule_FadeIn_OpacityIsStrictlyIncreasingAndInRange()

        Dim schedule As IReadOnlyList(Of FadeStep) = FadeScheduler.CreateSchedule(0.0R, 1.0R, 1000, 100)

        Dim previous As Double = 0.0R
        For Each currentStep As FadeStep In schedule
            Assert.IsGreaterThan(previous, currentStep.Opacity)
            Assert.IsLessThanOrEqualTo(1.0R, currentStep.Opacity)
            previous = currentStep.Opacity
        Next

    End Sub

    <TestMethod>
    Public Sub CreateSchedule_FadeOut_OpacityIsStrictlyDecreasingAndInRange()

        Dim schedule As IReadOnlyList(Of FadeStep) = FadeScheduler.CreateSchedule(1.0R, 0.0R, 1000, 100)

        Dim previous As Double = 1.0R
        For Each currentStep As FadeStep In schedule
            Assert.IsLessThan(previous, currentStep.Opacity)
            Assert.IsGreaterThanOrEqualTo(0.0R, currentStep.Opacity)
            previous = currentStep.Opacity
        Next

    End Sub

    <TestMethod>
    Public Sub CreateSchedule_DueTimes_AreEvenlyDistributedAndNonDecreasing()

        Dim schedule As IReadOnlyList(Of FadeStep) = FadeScheduler.CreateSchedule(0.0R, 1.0R, 1500, 100)

        Assert.AreEqual(15, schedule(0).DueTimeMilliseconds)
        Assert.AreEqual(750, schedule(49).DueTimeMilliseconds)
        For i As Integer = 1 To schedule.Count - 1
            Assert.IsGreaterThanOrEqualTo(schedule(i - 1).DueTimeMilliseconds, schedule(i).DueTimeMilliseconds)
        Next

    End Sub

    <TestMethod>
    Public Sub CreateSchedule_StepOpacity_IsLinear()

        Dim schedule As IReadOnlyList(Of FadeStep) = FadeScheduler.CreateSchedule(0.0R, 1.0R, 1000, 4)

        Assert.AreEqual(0.25R, schedule(0).Opacity, 0.000001R)
        Assert.AreEqual(0.5R, schedule(1).Opacity, 0.000001R)
        Assert.AreEqual(0.75R, schedule(2).Opacity, 0.000001R)
        Assert.AreEqual(1.0R, schedule(3).Opacity)

    End Sub

    <TestMethod>
    Public Sub CreateSchedule_ZeroDuration_AllStepsAreDueImmediately()

        Dim schedule As IReadOnlyList(Of FadeStep) = FadeScheduler.CreateSchedule(0.0R, 1.0R, 0, 100)

        For Each currentStep As FadeStep In schedule
            Assert.AreEqual(0, currentStep.DueTimeMilliseconds)
        Next

    End Sub

    <TestMethod>
    Public Sub CreateSchedule_MaximumDuration_DoesNotOverflow()

        Dim schedule As IReadOnlyList(Of FadeStep) = FadeScheduler.CreateSchedule(0.0R, 1.0R, Integer.MaxValue, 100)

        Assert.AreEqual(Integer.MaxValue, schedule(schedule.Count - 1).DueTimeMilliseconds)

    End Sub

    <TestMethod>
    Public Sub CreateSchedule_SameOpacity_KeepsOpacity()

        Dim schedule As IReadOnlyList(Of FadeStep) = FadeScheduler.CreateSchedule(0.5R, 0.5R, 100, 10)

        For Each currentStep As FadeStep In schedule
            Assert.AreEqual(0.5R, currentStep.Opacity)
        Next

    End Sub

    <TestMethod>
    Public Sub CreateSchedule_SingleStep_JumpsToTarget()

        Dim schedule As IReadOnlyList(Of FadeStep) = FadeScheduler.CreateSchedule(0.0R, 1.0R, 500, 1)

        Assert.AreEqual(1, schedule.Count)
        Assert.AreEqual(1.0R, schedule(0).Opacity)
        Assert.AreEqual(500, schedule(0).DueTimeMilliseconds)

    End Sub

    <TestMethod>
    <DataRow(-0.01R, 1.0R)>
    <DataRow(1.01R, 1.0R)>
    <DataRow(Double.NaN, 1.0R)>
    <DataRow(0.0R, -0.01R)>
    <DataRow(0.0R, 1.01R)>
    <DataRow(0.0R, Double.NaN)>
    Public Sub CreateSchedule_OpacityOutOfRange_Throws(fromOpacity As Double, toOpacity As Double)

        Assert.ThrowsExactly(Of ArgumentOutOfRangeException)(Sub() FadeScheduler.CreateSchedule(fromOpacity, toOpacity, 100, 10))

    End Sub

    <TestMethod>
    Public Sub CreateSchedule_NegativeDuration_Throws()

        Assert.ThrowsExactly(Of ArgumentOutOfRangeException)(Sub() FadeScheduler.CreateSchedule(0.0R, 1.0R, -1, 10))

    End Sub

    <TestMethod>
    <DataRow(0)>
    <DataRow(-1)>
    Public Sub CreateSchedule_InvalidStepCount_Throws(stepCount As Integer)

        Assert.ThrowsExactly(Of ArgumentOutOfRangeException)(Sub() FadeScheduler.CreateSchedule(0.0R, 1.0R, 100, stepCount))

    End Sub

End Class

#End Region
