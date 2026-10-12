namespace EnergySimulator.Devices;

public interface IDevice
{
    string Name { get; }
    bool IsEnabled { get; set; }
    double PowerKw { get; }
    double DailyEnergyKwh { get; }
    double TotalEnergyKwh { get; }
    void StepContinuous(DateTime now, double elapsedSimHours);
    void ResetDailyEnergy();
}