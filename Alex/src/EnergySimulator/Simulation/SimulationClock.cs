namespace EnergySimulator.Simulation;

public sealed class SimulationClock
{
    public DateTime Start { get; }
    public DateTime Now { get; private set; }
    public int Timescale { get; set; }

    private DateTime _currentDay;

    public SimulationClock(int timescale, DateTime? startDate)
    {
        Timescale = timescale;
        Start = startDate ?? DateTime.Now;
        Now = Start;
        _currentDay = Now.Date;
    }

    public double HourOfDay => Now.Hour + Now.Minute / 60.0 + Now.Second / 3600.0;

    public double DayProgress => Now.TimeOfDay.TotalHours / 24.0;

    // Zeitsprung: echte verstrichene Zeit mal Zeitraffer (z. B. x60: 1 s
    // Echtzeit = 1 Minute Simulation).
    public bool Advance(double elapsedRealMilliseconds)
    {
        Now += TimeSpan.FromMilliseconds(elapsedRealMilliseconds * Timescale);

        if (Now.Date == _currentDay)
            return false;

        _currentDay = Now.Date;
        return true;
    }
}