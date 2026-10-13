using EnergySimulator.Simulation;

namespace EnergySimulator.Devices;

public sealed class PvFaultModel
{
    public const double OverheatTripC = 70.0;
    public const double OverheatResetC = 55.0;
    public const double GridMinVolt = 200.0;
    public const double GridMaxVolt = 250.0;
    public const double MaxSoilingPercent = 25.0;
    public const double MaxAgingPercent = 15.0;

    private const double CellTemperatureRisePerWm2 = 0.040;
    // Rund 0.04 K pro W/m²: bei STC (1000 W/m²) liegen die Zellen damit ca.
    // 40 K ueber der Aussentemperatur – typischer Wert fuer kristalline Module
    // (NOCT/NOCT-Verfahren), bewusst leicht vereinfacht.
    //https://www.selfmade-energy.com/noct-was-ist-das/
    private const double AmbientDailyAmplitudeC = 8.0;
    private const double MaxHeatWaveC = 12.0;
    private const double DailyOffsetSpreadC = 6.0;
    private const double SoilingPerDayPercent = 0.6;
    private const double AgingPerDayPercent = 0.04;

    private readonly Random _random;

    public PvFaultModel(Random random)
    {
        _random = random;
    }

    public bool InverterFault { get; private set; }
    public bool OverheatForced { get; private set; }
    public bool GridDisturbanceForced { get; private set; }

    public double ModuleTemperatureC { get; private set; } = 12.0;
    public double GridVoltageV { get; private set; } = 230.0;
    public double SoilingLossPercent { get; private set; }
    public double AgingLossPercent { get; private set; }
    public double AmbientTemperatureC { get; private set; } = 12.0;

    private DateTime _offsetDay = DateTime.MinValue;
    private double _dailyOffsetC;

    public double TotalLossPercent => Math.Min(90.0, SoilingLossPercent + AgingLossPercent);

    public FaultType ActiveFault
    {
        get
        {
            if (InverterFault)
                return FaultType.Inverter;

            if (ModuleTemperatureC >= OverheatTripC)
                return FaultType.Overheat;

            if (GridVoltageV < GridMinVolt || GridVoltageV > GridMaxVolt)
                return FaultType.GridVoltage;

            return FaultType.None;
        }
    }

    public bool HasFault => ActiveFault != FaultType.None;

    public void ToggleInverterFault() => InverterFault = !InverterFault;

    public void ToggleForceOverheat() => OverheatForced = !OverheatForced;

    public void ToggleGridDisturbance() => GridDisturbanceForced = !GridDisturbanceForced;

    public void CleanModules() => SoilingLossPercent = 0.0;

    public void Step(double irradiation, DateTime now, Season season, double hour, double elapsedSimHours)
    {
        StepTemperature(irradiation, now, season, hour, elapsedSimHours);
        StepGridVoltage(irradiation, hour, elapsedSimHours);
        StepDegradation(elapsedSimHours);
    }

    private void StepTemperature(double irradiation, DateTime now, Season season, double hour, double elapsedSimHours)
    {
        double ambient = AmbientTemperature(season, hour) + DailyOffsetC(now, season);
        AmbientTemperatureC = Math.Round(ambient, 1);

        if (OverheatForced)
        {
            ModuleTemperatureC = OverheatTripC + 4.0;
            return;
        }

        // Gleichgewichtstemperatur = Aussentemperatur + Aufheizung durch die
        // Einstrahlung. Die Modultemperatur naehert sich dem Gleichgewicht per
        // Exponentialfunktion (1 - e^-t/tau): unter Sonne schnell (tau=0.8 h),
        // nachts ohne Strahlung langsam (tau=2.5 h, Abkuehlung).
        double equilibrium = ambient + CellTemperatureRisePerWm2 * irradiation;

        double timeConstantHours = irradiation > 1.0 ? 0.8 : 2.5;
        double k = 1.0 - Math.Exp(-elapsedSimHours / timeConstantHours);

        ModuleTemperatureC += (equilibrium - ModuleTemperatureC) * k;
    }

    private double DailyOffsetC(DateTime now, Season season)
    {
        if (now.Date != _offsetDay)
        {
            _offsetDay = now.Date;

            // Deterministisch aus dem Datum, damit gleiche Tage immer gleich verlaufen
            var dayRandom = new Random(now.Date.Year * 1000 + now.Date.DayOfYear);
            double heatWave = season == Season.Summer ? dayRandom.NextDouble() * MaxHeatWaveC : 0.0;
            _dailyOffsetC = (dayRandom.NextDouble() - 0.5) * DailyOffsetSpreadC + heatWave;
        }

        return _dailyOffsetC;
    }

    private static double AmbientTemperature(Season season, double hour)
    {
        // Tagesgang der Lufttemperatur als Kosinuskurve: Minimum gegen 3:00,
        // Maximum gegen 15:00 – das Maximum liegt bewusst erst nach dem
        // Sonnenhoehepunkt (die Luft heizt sich nachmittags noch nach).
        // Die Jahreszeit ist in Season.BaseAmbientC() hinterlegt.
        double daily = AmbientDailyAmplitudeC * Math.Cos((hour - 15.0) / 24.0 * 2.0 * Math.PI);
        return season.BaseAmbientC() + daily;
    }

    private void StepGridVoltage(double irradiation, double hour, double elapsedSimHours)
    {
        double target;

        if (GridDisturbanceForced)
        {
            target = 178.0;
        }
        else
        {
            // Netzmodell: Grundspannung 230 V sinkt bei Haushaltslast (Abend),
            // steigt durch Einspeisung (Mittag), plus gleichverteiltes Rauschen.
            // Lastkoeffizient 24 V und Einspeisekoeffizient 9 V sind bewusst so
            // gewaehlt, dass die Spannung werktags in den 200-250-V-Bereich bleibt.
            double load = HouseholdLoadFactor(hour);
            double injection = irradiation / IrradiationCalculator.PeakIrradiationWm2;
            double noise = (_random.NextDouble() - 0.5) * 24.0;

            target = 230.0 - 24.0 * load + 9.0 * injection + noise;
        }

        // Traegheit/Glättung: ziemlich grobe Stabilisierung auf den Zielwert;
        // mit zunehmender Simulationsdauer (elapsedSimHours) wird k groesser,
        // bis die Abweichung nach ~10 Minuten fast vollstaendig abgebaut ist.
        double k = Math.Min(1.0, elapsedSimHours * 6.0);
        GridVoltageV += (target - GridVoltageV) * k;
    }

    private static double HouseholdLoadFactor(double hour)
    {
        // Haushaltslast als zwei Gauß-Kurven im Tagesverlauf: ein deutlicher
        // Abendpeak um 19:00 (hoher Wert ~1) und ein schwächerer Morgenpeak
        // um 7:30 (Bruchteil davon). Zwischendrin (berufstätige Abwesenheit)
        // fast keine Last.
        double morning = Math.Exp(-Math.Pow(hour - 7.5, 2) / 4.0);
        double evening = Math.Exp(-Math.Pow(hour - 19.0, 2) / 6.0);
        return Math.Min(1.0, morning * 0.85 + evening);
    }

    private void StepDegradation(double elapsedSimHours)
    {
        double days = elapsedSimHours / 24.0;

        SoilingLossPercent = Math.Min(MaxSoilingPercent, SoilingLossPercent + SoilingPerDayPercent * days);
        AgingLossPercent = Math.Min(MaxAgingPercent, AgingLossPercent + AgingPerDayPercent * days);
    }
}