namespace EnergyTwin.Web.Management;

/// <summary>Ein von der Web-App gestarteter und noch laufender Geräte-Container mit bestätigtem Steuerzugang.</summary>
public sealed record DeviceContainerRun(string ContainerId, int ControlPort, string ControlDirectory, InstanceConfig Startup);
