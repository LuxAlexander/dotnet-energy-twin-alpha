using DualChargerSim.Simulation;
using EnergyTwin.Web.Presentation;

namespace EnergyTwin.Web.Management;

/// <summary>Ladesäulen-Container der Web-App (Image <c>energytwin/dc:lb04</c>, Steuerport 5193).</summary>
public sealed class ChargerContainers(ILogger<ChargerContainers> logger) : EquipmentContainers(logger,
    new(ModuleKind.Charger, "Ladesäulen", "dc", "DualPortCharger", SimulationEngine.ControlKind,
        SimulationConfig.ContainerControlPort, "ENERGYTWIN_CHARGER_CONTAINER_CONTROL"));
