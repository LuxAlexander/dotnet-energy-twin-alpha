using SimCore.Time;

namespace SimCore.Tests;

public class ClockOffsetEstimatorTests
{
    private static readonly DateTimeOffset T0 = new(2026, 10, 25, 8, 0, 0, TimeSpan.Zero);

    [Fact]
    public void WithoutSample_OffsetIsZero()
    {
        var estimator = new ClockOffsetEstimator();

        Assert.False(estimator.HasSample);
        Assert.Equal(TimeSpan.Zero, estimator.Offset);
    }

    [Fact]
    public void UsesSampleWithSmallestRoundTrip()
    {
        var estimator = new ClockOffsetEstimator();

        // RTT 100 ms, durch Wartezeit verfälscht: Offset falsch geschätzt.
        estimator.AddSample(T0, T0.AddMilliseconds(90), T0.AddMilliseconds(100));
        // RTT 4 ms, Master genau 1 s voraus.
        estimator.AddSample(T0.AddSeconds(1), T0.AddSeconds(2).AddMilliseconds(2), T0.AddSeconds(1).AddMilliseconds(4));

        Assert.Equal(TimeSpan.FromSeconds(1), estimator.Offset);
        Assert.Equal(TimeSpan.FromMilliseconds(4), estimator.RoundTrip);
        Assert.Equal(TimeSpan.FromMilliseconds(2), estimator.Uncertainty);
    }

    [Fact]
    public void OldSamples_LeaveTheWindow()
    {
        var estimator = new ClockOffsetEstimator();
        estimator.AddSample(T0, T0, T0);                          // RTT 0, wird später verdrängt

        for (int i = 1; i <= ClockOffsetEstimator.WindowSize; i++)
        {
            var sent = T0.AddSeconds(i);
            estimator.AddSample(sent, sent.AddSeconds(3).AddMilliseconds(5), sent.AddMilliseconds(10));
        }

        Assert.Equal(TimeSpan.FromSeconds(3), estimator.Offset);
        Assert.Equal(TimeSpan.FromMilliseconds(10), estimator.RoundTrip);
    }

    [Fact]
    public void ImpossibleSample_IsRejected()
    {
        var estimator = new ClockOffsetEstimator();

        Assert.False(estimator.AddSample(T0, T0, T0.AddMilliseconds(-1)));
        Assert.False(estimator.HasSample);
    }
}
