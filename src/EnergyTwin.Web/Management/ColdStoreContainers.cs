using EnergyTwin.Web.Presentation;
using KuehlhausSimulator.Simulation;

namespace EnergyTwin.Web.Management;

/// <summary>Kühlhaus-Container der Web-App (Image <c>energytwin/kh:lb04</c>, Steuerport 5192).</summary>
public sealed class ColdStoreContainers(ILogger<ColdStoreContainers> logger) : EquipmentContainers(logger,
    new(ModuleKind.ColdStore, "Kühlhaus", "kh", "KuehlhausSimulator", SimulationEngine.ControlKind,
        SimulationConfig.ContainerControlPort, "ENERGYTWIN_KUEHLHAUS_CONTAINER_CONTROL"));
