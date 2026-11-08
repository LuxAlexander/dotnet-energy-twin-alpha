---
type: community
cohesion: 0.11
members: 38
---

# Batterie-Simulator Loop

**Cohesion:** 0.11 - loosely connected
**Members:** 38 nodes

## Members
- [[AsyncConsole.TryReadKey()]] - code - src/Devices.Common/AsyncConsole.cs
- [[CancellationToken]] - code
- [[ConsoleRenderer.InitializeAsync()]] - code - src/Devices/DualPortCharger/IO/ConsoleRenderer.cs
- [[CsvLogger]] - code - src/Devices/BatterySimulator/CsvLogger.cs
- [[CsvLogger_1]] - code - src/Devices/DualPortCharger/IO/CsvLogger.cs
- [[CsvLogger.CreateAsync()_1]] - code - src/Devices/BatterySimulator/CsvLogger.cs
- [[CsvLogger.CreateAsync()]] - code - src/Devices/DualPortCharger/IO/CsvLogger.cs
- [[CsvLogger.DisposeAsync()_1]] - code - src/Devices/DualPortCharger/IO/CsvLogger.cs
- [[CsvLogger.FlushAsync()]] - code - src/Devices/BatterySimulator/CsvLogger.cs
- [[CsvLogger.FlushAsync()_1]] - code - src/Devices/DualPortCharger/IO/CsvLogger.cs
- [[CsvLogger.LogAsync()]] - code - src/Devices/BatterySimulator/CsvLogger.cs
- [[CsvLogger.Number()]] - code - src/Devices/DualPortCharger/IO/CsvLogger.cs
- [[CsvLogger.WriteAsync()]] - code - src/Devices/DualPortCharger/IO/CsvLogger.cs
- [[DeviceTimeSource.RunAsync()]] - code - src/Devices.Common/DeviceTimeSource.cs
- [[FakeWeatherService.GetAsync()]] - code - tests/Devices.Common.Tests/WeatherFeedTests.cs
- [[FilePath]] - code - src/Devices/BatterySimulator/CsvLogger.cs
- [[FilePath_1]] - code - src/Devices/DualPortCharger/IO/CsvLogger.cs
- [[IWeatherService.GetAsync()]] - code - src/SimCore.Contracts/IWeatherService.cs
- [[InputHandler.HandlePendingKeysAsync()]] - code - src/Devices/DualPortCharger/IO/InputHandler.cs
- [[InputHandler.WaitForQuitAsync()]] - code - src/Devices/DualPortCharger/IO/InputHandler.cs
- [[InputHandler.WaitForQuitAsync()_1]] - code - src/Devices/KuehlhausSimulator/IO/InputHandler.cs
- [[PvEngineTests.Pv_RunsStandalone_WritesCsvAndStopsOnCancel()]] - code - tests/FieldDevices.Tests/FieldDeviceTests.cs
- [[PvSystem.ApplyExternalWeather()]] - code - src/Devices/PvSimulator/Devices/PvSystem.cs
- [[SimulationEngine.AdvanceAsync()_1]] - code - src/Devices/DualPortCharger/Simulation/SimulationEngine.cs
- [[SimulationEngine.AdvanceAsync()]] - code - src/Devices/PvSimulator/Simulation/SimulationEngine.cs
- [[SimulationEngine.HandleKeys()]] - code - src/Devices/PvSimulator/Simulation/SimulationEngine.cs
- [[SimulationEngine.RunAsync()]] - code - src/Devices/DualPortCharger/Simulation/SimulationEngine.cs
- [[SimulationEngine.RunAsync()_2]] - code - src/Devices/PvSimulator/Simulation/SimulationEngine.cs
- [[SimulationEngine.UpdateWeatherAsync()]] - code - src/Devices/PvSimulator/Simulation/SimulationEngine.cs
- [[Simulator.AdvanceAsync()]] - code - src/Devices/BatterySimulator/Simulator.cs
- [[Simulator.HandleKeys()]] - code - src/Devices/BatterySimulator/Simulator.cs
- [[Simulator.RunAsync()]] - code - src/Devices/BatterySimulator/Simulator.cs
- [[StreamWriter]] - code
- [[WeatherClient.GetAsync()]] - code - src/Devices.Common/Weather/WeatherClient.cs
- [[WeatherData]] - code - src/SimCore.Contracts/WeatherData.cs
- [[WeatherFeed.Dispose()]] - code - src/Devices.Common/Weather/WeatherFeed.cs
- [[WeatherFeed.UpdateAsync()]] - code - src/Devices.Common/Weather/WeatherFeed.cs
- [[WeatherUnavailableException]] - code - src/Devices.Common/Weather/WeatherClient.cs

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/Batterie-Simulator_Loop
SORT file.name ASC
```

## Connections to other communities
- 33 edges to [[_COMMUNITY_TimeSyncServer]]
- 22 edges to [[_COMMUNITY_Batterie-Simulator Loop (2)]]
- 12 edges to [[_COMMUNITY_WeatherClient & Anzeige]]
- 11 edges to [[_COMMUNITY_PV-Engine & Fehler]]
- 9 edges to [[_COMMUNITY_Gruppe 21]]
- 9 edges to [[_COMMUNITY_Sync-Nachrichten]]
- 8 edges to [[_COMMUNITY_WeatherFeed]]
- 8 edges to [[_COMMUNITY_Wettermodell & Tests (2)]]
- 7 edges to [[_COMMUNITY_Batterie-Simulator Loop (3)]]
- 6 edges to [[_COMMUNITY_Sync-Nachrichten (2)]]
- 5 edges to [[_COMMUNITY_Batterie-Modell]]
- 5 edges to [[_COMMUNITY_Batterie-Modbus & Update (4)]]
- 5 edges to [[_COMMUNITY_GatewayService]]
- 4 edges to [[_COMMUNITY_Wettermodell & Tests (4)]]
- 4 edges to [[_COMMUNITY_Diagnose-Client Verbindung]]
- 3 edges to [[_COMMUNITY_Gruppe 24]]
- 3 edges to [[_COMMUNITY_DualCharger]]
- 3 edges to [[_COMMUNITY_Gruppe 69]]
- 3 edges to [[_COMMUNITY_Batterie-Modbus & Update (3)]]
- 3 edges to [[_COMMUNITY_Batterie-Modbus & Update (2)]]
- 3 edges to [[_COMMUNITY_StepScheduler]]
- 3 edges to [[_COMMUNITY_MQTT-Publisher & Zugangsdaten]]
- 2 edges to [[_COMMUNITY_PvSystem]]
- 2 edges to [[_COMMUNITY_PV-Namespaces]]
- 2 edges to [[_COMMUNITY_Gruppe 56]]
- 2 edges to [[_COMMUNITY_Batterie-Modbus & Update]]
- 2 edges to [[_COMMUNITY_Zeitsync-Client & Konsole]]
- 1 edge to [[_COMMUNITY_DeviceConnection (Modbus.Client)]]
- 1 edge to [[_COMMUNITY_JSON-Serialisierung]]
- 1 edge to [[_COMMUNITY_Gruppe 84]]
- 1 edge to [[_COMMUNITY_Wettermodell & Tests (3)]]
- 1 edge to [[_COMMUNITY_DeviceTimeSource]]
- 1 edge to [[_COMMUNITY_Gruppe 19]]
- 1 edge to [[_COMMUNITY_Charger-Namespaces]]
- 1 edge to [[_COMMUNITY_Modbus-Tests]]
- 1 edge to [[_COMMUNITY_Gruppe 38]]
- 1 edge to [[_COMMUNITY_Gruppe 91]]
- 1 edge to [[_COMMUNITY_Gruppe 65]]
- 1 edge to [[_COMMUNITY_Gruppe 37]]
- 1 edge to [[_COMMUNITY_Gruppe 23]]
- 1 edge to [[_COMMUNITY_ArgsParser (2)]]

## Top bridge nodes
- [[CancellationToken]] - degree 77, connects to 11 communities
- [[WeatherData]] - degree 15, connects to 8 communities
- [[SimulationEngine.RunAsync()]] - degree 17, connects to 7 communities
- [[SimulationEngine.AdvanceAsync()]] - degree 13, connects to 7 communities
- [[Simulator.AdvanceAsync()]] - degree 12, connects to 6 communities