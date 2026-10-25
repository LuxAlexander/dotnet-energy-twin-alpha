namespace Devices.Common.Tests;

public class StepSchedulerTests
{
    private static readonly DateTime T0 = new(2026, 4, 15, 6, 0, 0);
    private static readonly TimeSpan Step = TimeSpan.FromMinutes(5);

    [Fact]
    public void FirstUpdate_StartsAtClockTime_WithoutSteps()
    {
        var scheduler = new StepScheduler(Step);

        var update = scheduler.Update(T0, runId: 1);

        Assert.Equal(StepUpdateKind.Started, update.Kind);
        Assert.Equal(0, update.StepCount);
        Assert.Equal(T0, scheduler.ModelTime);
    }

    [Fact]
    public void OnlyFullStepsAreDue_ModelLagsAtMostOneStep()
    {
        var scheduler = new StepScheduler(Step);
        scheduler.Update(T0, 1);

        var partial = scheduler.Update(T0.AddMinutes(4.9), 1);
        Assert.Equal(0, partial.StepCount);

        var update = scheduler.Update(T0.AddMinutes(17), 1);
        Assert.Equal(StepUpdateKind.Advance, update.Kind);
        Assert.Equal(3, update.StepCount);
        Assert.Equal(T0, update.FirstStepAt);
        Assert.Equal(T0.AddMinutes(15), update.ModelTime);       // 2 min Rest bleiben für den nächsten Schritt
    }

    [Fact]
    public void SameTimeline_GivesSameSteps_RegardlessOfPollingRate()
    {
        var fast = new StepScheduler(Step);
        var slow = new StepScheduler(Step);
        fast.Update(T0, 1);
        slow.Update(T0, 1);

        int fastSteps = 0;
        for (int minute = 1; minute <= 60; minute++)
            fastSteps += fast.Update(T0.AddMinutes(minute), 1).StepCount;
        int slowSteps = slow.Update(T0.AddMinutes(60), 1).StepCount;

        Assert.Equal(12, fastSteps);
        Assert.Equal(12, slowSteps);
        Assert.Equal(fast.ModelTime, slow.ModelTime);
    }

    [Fact]
    public void NewRun_RestartsAtNewTime_EvenIfEarlier()
    {
        var scheduler = new StepScheduler(Step);
        scheduler.Update(T0, 1);
        scheduler.Update(T0.AddHours(3), 1);
        var winter = new DateTime(2026, 1, 15, 6, 0, 0);

        var update = scheduler.Update(winter, runId: 2);

        Assert.Equal(StepUpdateKind.Restarted, update.Kind);
        Assert.Equal(0, update.StepCount);
        Assert.Equal(winter, scheduler.ModelTime);
        Assert.Equal(2, scheduler.RunId);
    }

    [Fact]
    public void ClockSlightlyBehindModel_WaitsInsteadOfGoingBack()
    {
        var scheduler = new StepScheduler(Step);
        scheduler.Update(T0, 1);
        scheduler.Update(T0.AddMinutes(10), 1);

        var update = scheduler.Update(T0.AddMinutes(9), 1);       // z. B. verspätet angekommene Pause

        Assert.Equal(0, update.StepCount);
        Assert.Equal(T0.AddMinutes(10), scheduler.ModelTime);
    }

    [Fact]
    public void TooManyDueSteps_AreLimited_AndReported()
    {
        var scheduler = new StepScheduler(Step, maxCatchUpSteps: 100);
        scheduler.Update(T0, 1);

        var update = scheduler.Update(T0 + Step * 250, 1);

        Assert.Equal(100, update.StepCount);
        Assert.Equal(150, update.SkippedSteps);
        Assert.Equal(T0 + Step * 150, update.FirstStepAt);
        Assert.Equal(T0 + Step * 250, update.ModelTime);
    }

    [Fact]
    public void InvalidStep_IsRejected()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new StepScheduler(TimeSpan.Zero));
    }
}
