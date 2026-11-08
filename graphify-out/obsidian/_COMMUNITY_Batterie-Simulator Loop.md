---
type: community
cohesion: 0.10
members: 55
---

# Batterie-Simulator Loop

**Cohesion:** 0.10 - loosely connected
**Members:** 55 nodes

## Members
- [[AsyncConsole]] - code - src/Devices.Common/AsyncConsole.cs
- [[AsyncConsole.ReadLineAsync()]] - code - src/Devices.Common/AsyncConsole.cs
- [[AsyncConsole.TryReadKey()]] - code - src/Devices.Common/AsyncConsole.cs
- [[CancellationToken]] - code
- [[ConsoleKeyInfo]] - code
- [[ConsoleRenderer.BelowStatus()]] - code - src/Devices/DualPortCharger/IO/ConsoleRenderer.cs
- [[ConsoleRenderer.InitializeAsync()]] - code - src/Devices/DualPortCharger/IO/ConsoleRenderer.cs
- [[ConsoleRenderer.InitializeAsync()_1]] - code - src/Devices/KuehlhausSimulator/IO/ConsoleRenderer.cs
- [[ConsoleRenderer.PromptAsync()]] - code - src/Devices/DualPortCharger/IO/ConsoleRenderer.cs
- [[ConsoleRenderer.ShutdownAsync()]] - code - src/Devices/DualPortCharger/IO/ConsoleRenderer.cs
- [[CsvExporter.CreateAsync()]] - code - src/Devices/PvSimulator/Data/CsvExporter.cs
- [[CsvExporter.FlushAsync()]] - code - src/Devices/PvSimulator/Data/CsvExporter.cs
- [[CsvFileWriter.CreateAsync()]] - code - src/Shared/Data/CsvFileWriter.cs
- [[CsvFileWriter.FlushAsync()]] - code - src/Shared/Data/CsvFileWriter.cs
- [[CsvFileWriter.WriteRowAsync()]] - code - src/Shared/Data/CsvFileWriter.cs
- [[CsvLogger_2]] - code - src/Devices/KuehlhausSimulator/IO/CsvLogger.cs
- [[CsvLogger.CreateAsync()]] - code - src/Devices/BatterySimulator/CsvLogger.cs
- [[CsvLogger.CreateAsync()_2]] - code - src/Devices/KuehlhausSimulator/IO/CsvLogger.cs
- [[CsvLogger.FlushAsync()_1]] - code - src/Devices/DualPortCharger/IO/CsvLogger.cs
- [[CsvLogger.FlushAsync()_2]] - code - src/Devices/KuehlhausSimulator/IO/CsvLogger.cs
- [[CsvLogger.Number()_1]] - code - src/Devices/KuehlhausSimulator/IO/CsvLogger.cs
- [[CsvLogger.WriteAsync()]] - code - src/Devices/DualPortCharger/IO/CsvLogger.cs
- [[CsvLogger.WriteAsync()_1]] - code - src/Devices/KuehlhausSimulator/IO/CsvLogger.cs
- [[DeviceTimeSource.RunAsync()]] - code - src/Devices.Common/DeviceTimeSource.cs
- [[FilePath_2]] - code - src/Devices/KuehlhausSimulator/IO/CsvLogger.cs
- [[HasKeyboard]] - code - src/Devices.Common/AsyncConsole.cs
- [[InputHandler]] - code - src/Devices/DualPortCharger/IO/InputHandler.cs
- [[InputHandler_1]] - code - src/Devices/KuehlhausSimulator/IO/InputHandler.cs
- [[InputHandler.AskPercentAsync()]] - code - src/Devices/DualPortCharger/IO/InputHandler.cs
- [[InputHandler.AskSetpointAsync()]] - code - src/Devices/KuehlhausSimulator/IO/InputHandler.cs
- [[InputHandler.ChangePowerLimit()]] - code - src/Devices/DualPortCharger/IO/InputHandler.cs
- [[InputHandler.ConnectVehicleAsync()]] - code - src/Devices/DualPortCharger/IO/InputHandler.cs
- [[InputHandler.HandleAsync()]] - code - src/Devices/DualPortCharger/IO/InputHandler.cs
- [[InputHandler.HandleAsync()_1]] - code - src/Devices/KuehlhausSimulator/IO/InputHandler.cs
- [[InputHandler.HandlePendingKeysAsync()]] - code - src/Devices/DualPortCharger/IO/InputHandler.cs
- [[InputHandler.HandlePendingKeysAsync()_1]] - code - src/Devices/KuehlhausSimulator/IO/InputHandler.cs
- [[InputHandler.WaitForQuitAsync()]] - code - src/Devices/DualPortCharger/IO/InputHandler.cs
- [[InputHandler.WaitForQuitAsync()_1]] - code - src/Devices/KuehlhausSimulator/IO/InputHandler.cs
- [[KuehlhausEngineTests.Kuehlhaus_RunsStandalone_WritesCsvAndStopsOnCancel()]] - code - tests/FieldDevices.Tests/ConsumerTests.cs
- [[PvEngineTests.Pv_RunsStandalone_WritesCsvAndStopsOnCancel()]] - code - tests/FieldDevices.Tests/FieldDeviceTests.cs
- [[SimulationEngine.AdvanceAsync()]] - code - src/Devices/DualPortCharger/Simulation/SimulationEngine.cs
- [[SimulationEngine.AdvanceAsync()_1]] - code - src/Devices/KuehlhausSimulator/Simulation/SimulationEngine.cs
- [[SimulationEngine.HandleKeys()]] - code - src/Devices/PvSimulator/Simulation/SimulationEngine.cs
- [[SimulationEngine.RunAsync()]] - code - src/Devices/DualPortCharger/Simulation/SimulationEngine.cs
- [[SimulationEngine.RunAsync()_1]] - code - src/Devices/KuehlhausSimulator/Simulation/SimulationEngine.cs
- [[SimulationEngine.RunAsync()_2]] - code - src/Devices/PvSimulator/Simulation/SimulationEngine.cs
- [[SimulationEngine.StartRunAsync()]] - code - src/Devices/KuehlhausSimulator/Simulation/SimulationEngine.cs
- [[SimulationEngine.UpdateWeatherAsync()]] - code - src/Devices/KuehlhausSimulator/Simulation/SimulationEngine.cs
- [[Simulator.ClearPromptAsync()]] - code - src/Devices/BatterySimulator/Simulator.cs
- [[Simulator.HandleKeys()]] - code - src/Devices/BatterySimulator/Simulator.cs
- [[Simulator.PromptAsync()]] - code - src/Devices/BatterySimulator/Simulator.cs
- [[Simulator.RunAsync()]] - code - src/Devices/BatterySimulator/Simulator.cs
- [[Simulator.SettingsMenuAsync()]] - code - src/Devices/BatterySimulator/Simulator.cs
- [[Task]] - code
- [[WeatherFeed.Dispose()]] - code - src/Devices.Common/Weather/WeatherFeed.cs

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/Batterie-Simulator_Loop
SORT file.name ASC
```

## Connections to other communities
- 29 edges to [[_COMMUNITY_WeatherFeed]]
- 26 edges to [[_COMMUNITY_CsvLogger]]
- 24 edges to [[_COMMUNITY_TimeSyncServer]]
- 24 edges to [[_COMMUNITY_Diagnose-Client Verbindung]]
- 21 edges to [[_COMMUNITY_Sync-Nachrichten]]
- 18 edges to [[_COMMUNITY_WeatherClient & Anzeige]]
- 14 edges to [[_COMMUNITY_Batterie-Modbus & Update]]
- 8 edges to [[_COMMUNITY_Gruppe 66]]
- 7 edges to [[_COMMUNITY_Gruppe 44]]
- 6 edges to [[_COMMUNITY_Zeitsync-Client & Konsole]]
- 5 edges to [[_COMMUNITY_Gruppe 51]]
- 5 edges to [[_COMMUNITY_DeviceTimeSource]]
- 5 edges to [[_COMMUNITY_Gruppe 81]]
- 5 edges to [[_COMMUNITY_PV-Engine & Fehler]]
- 4 edges to [[_COMMUNITY_Gruppe 70]]
- 4 edges to [[_COMMUNITY_DualCharger]]
- 4 edges to [[_COMMUNITY_Gruppe 41]]
- 3 edges to [[_COMMUNITY_Register-Dekodierung]]
- 3 edges to [[_COMMUNITY_Ladepunkt]]
- 3 edges to [[_COMMUNITY_Gruppe 45]]
- 3 edges to [[_COMMUNITY_Gruppe 58]]
- 2 edges to [[_COMMUNITY_StepScheduler]]
- 2 edges to [[_COMMUNITY_Fahrzeug]]
- 2 edges to [[_COMMUNITY_Gruppe 82]]
- 2 edges to [[_COMMUNITY_Gruppe 57]]
- 2 edges to [[_COMMUNITY_Master-Zeit Tests]]
- 1 edge to [[_COMMUNITY_Gruppe 40]]
- 1 edge to [[_COMMUNITY_Gruppe 43]]
- 1 edge to [[_COMMUNITY_Charger-Namespaces]]
- 1 edge to [[_COMMUNITY_Gruppe 73]]
- 1 edge to [[_COMMUNITY_Kühlhaus-Modell]]
- 1 edge to [[_COMMUNITY_Bibliotheks-Namespaces]]
- 1 edge to [[_COMMUNITY_ArgsParser]]
- 1 edge to [[_COMMUNITY_PV-Namespaces]]

## Top bridge nodes
- [[Task]] - degree 113, connects to 11 communities
- [[CancellationToken]] - degree 65, connects to 8 communities
- [[SimulationEngine.RunAsync()]] - degree 17, connects to 7 communities
- [[InputHandler]] - degree 11, connects to 5 communities
- [[SimulationEngine.AdvanceAsync()]] - degree 11, connects to 5 communities