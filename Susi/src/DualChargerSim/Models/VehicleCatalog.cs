namespace DualChargerSim.Models;

/// <summary>
/// Technische Daten eines Fahrzeugmodells (ohne SOC, der gehört zum einzelnen Fahrzeug).
/// </summary>
public record VehicleModel(string Name, double BatteryCapacityKWh, double MaxChargePowerKW, bool HasDamagedInlet = false);

/// <summary>
/// Feste Liste realer Fahrzeugmodelle; liefert zufällige Fahrzeuge für den Auto-Modus.
/// </summary>
public static class VehicleCatalog
{
    /// <summary>
    /// Gerundete Realwerte. Modelle über 150 kW zeigen, dass die Säule begrenzt.
    /// </summary>
    public static IReadOnlyList<VehicleModel> Models { get; } =
    [
        new("Renault Zoe",           52,  50),
        new("Fiat 500e",             42,  85),
        new("VW ID.3",               58, 120),
        new("Tesla Model 3",         60, 170),
        new("Hyundai Ioniq 5",       77, 220),
        new("Mercedes eActros 600", 621, 400), // LKW: mehr als das ganze Budget der Säule
        new("Kia EV6 (defekt)",      77, 240, HasDamagedInlet: true), // überhitzt ab ca. 140 kW
    ];

    /// <summary>Erzeugt ein Fahrzeug eines bestimmten Modells (manuelles Anstecken).</summary>
    public static Vehicle Create(VehicleModel model, double socPercent, double targetSocPercent) =>
        new(model.Name, model.BatteryCapacityKWh, model.MaxChargePowerKW, socPercent, targetSocPercent, model.HasDamagedInlet);

    /// <summary>
    /// Zufälliges Fahrzeug für den Auto-Modus: Ankunfts-SOC 10–50 %, Ziel-SOC 80–100 %.
    /// </summary>
    public static Vehicle CreateRandom(Random random)
    {
        var model = Models[random.Next(Models.Count)];
        double soc = 10 + random.NextDouble() * 40;
        double target = 80 + random.NextDouble() * 20;
        return Create(model, soc, target);
    }
}
