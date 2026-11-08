---
type: community
cohesion: 0.20
members: 22
---

# TimeSyncServer

**Cohesion:** 0.20 - loosely connected
**Members:** 22 nodes

## Members
- [[AsyncConsole]] - code - src/Devices.Common/AsyncConsole.cs
- [[AsyncConsole.ReadLineAsync()]] - code - src/Devices.Common/AsyncConsole.cs
- [[ConsolePublisher.PublishAsync()]] - code - src/ModbusRabbitGateway/Publishing.cs
- [[HasKeyboard_1]] - code - src/Devices.Common/AsyncConsole.cs
- [[IMessagePublisher.PublishAsync()]] - code - src/ModbusRabbitGateway/Publishing.cs
- [[RecordingPublisher.PublishAsync()]] - code - tests/ModbusRabbitGateway.Tests/GatewayAgainstDeviceTests.cs
- [[Simulator.ClearPromptAsync()]] - code - src/Devices/BatterySimulator/Simulator.cs
- [[Simulator.PromptAsync()]] - code - src/Devices/BatterySimulator/Simulator.cs
- [[Simulator.SettingsMenuAsync()]] - code - src/Devices/BatterySimulator/Simulator.cs
- [[Task]] - code
- [[TeePublisher.PublishAsync()]] - code - src/ModbusRabbitGateway/Publishing.cs
- [[TimeSyncClient.RunAsync()]] - code - src/SimCore.TimeSync/TimeSyncClient.cs
- [[TimeSyncIntegrationTests]] - code - tests/SimCore.Tests/TimeSyncIntegrationTests.cs
- [[TimeSyncIntegrationTests.DisposeAsync()]] - code - tests/SimCore.Tests/TimeSyncIntegrationTests.cs
- [[TimeSyncIntegrationTests.FreePort()]] - code - tests/SimCore.Tests/TimeSyncIntegrationTests.cs
- [[TimeSyncIntegrationTests.Garbage_DisconnectsOnlyThatClient()]] - code - tests/SimCore.Tests/TimeSyncIntegrationTests.cs
- [[TimeSyncIntegrationTests.InvalidClient_IsRejected_WithVisibleMessage()]] - code - tests/SimCore.Tests/TimeSyncIntegrationTests.cs
- [[TimeSyncIntegrationTests.MasterShutdown_IsReported_AndSimulatorStopsTime()]] - code - tests/SimCore.Tests/TimeSyncIntegrationTests.cs
- [[TimeSyncIntegrationTests.Server_ListsSimulators_WithReportedDeviation()]] - code - tests/SimCore.Tests/TimeSyncIntegrationTests.cs
- [[TimeSyncIntegrationTests.SimulatorStarted_BeforeMaster_ConnectsLater()]] - code - tests/SimCore.Tests/TimeSyncIntegrationTests.cs
- [[TimeSyncIntegrationTests.StartSimulator()]] - code - tests/SimCore.Tests/TimeSyncIntegrationTests.cs
- [[TimeSyncIntegrationTests.WaitUntilAsync()]] - code - tests/SimCore.Tests/TimeSyncIntegrationTests.cs

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/TimeSyncServer
SORT file.name ASC
```

## Connections to other communities
- 33 edges to [[_COMMUNITY_Batterie-Simulator Loop]]
- 24 edges to [[_COMMUNITY_Sync-Nachrichten]]
- 19 edges to [[_COMMUNITY_Diagnose-Client Verbindung]]
- 13 edges to [[_COMMUNITY_MQTT-Publisher & Zugangsdaten]]
- 13 edges to [[_COMMUNITY_Sync-Nachrichten (2)]]
- 12 edges to [[_COMMUNITY_Batterie-Simulator Loop (2)]]
- 11 edges to [[_COMMUNITY_WeatherClient & Anzeige]]
- 10 edges to [[_COMMUNITY_Gruppe 23]]
- 9 edges to [[_COMMUNITY_GatewayService]]
- 5 edges to [[_COMMUNITY_MasterClock]]
- 5 edges to [[_COMMUNITY_Batterie-Simulator Loop (3)]]
- 4 edges to [[_COMMUNITY_Gruppe 21]]
- 4 edges to [[_COMMUNITY_Gateway-Publisher]]
- 3 edges to [[_COMMUNITY_SyncedClock]]
- 3 edges to [[_COMMUNITY_WeatherFeed]]
- 2 edges to [[_COMMUNITY_Gruppe 65]]
- 2 edges to [[_COMMUNITY_Zeitsync-Client & Konsole]]
- 2 edges to [[_COMMUNITY_Batterie-Modell]]
- 2 edges to [[_COMMUNITY_Gruppe 69]]
- 1 edge to [[_COMMUNITY_Shared Konsolenausgabe]]
- 1 edge to [[_COMMUNITY_Wettermodell & Tests (2)]]
- 1 edge to [[_COMMUNITY_Zeit-Bibliotheken & Master]]
- 1 edge to [[_COMMUNITY_Gruppe 19]]
- 1 edge to [[_COMMUNITY_Gruppe 52]]
- 1 edge to [[_COMMUNITY_Gruppe 61]]
- 1 edge to [[_COMMUNITY_Gruppe 48]]
- 1 edge to [[_COMMUNITY_Gruppe 37]]

## Top bridge nodes
- [[Task]] - degree 134, connects to 17 communities
- [[TimeSyncIntegrationTests]] - degree 21, connects to 8 communities
- [[TimeSyncIntegrationTests.MasterShutdown_IsReported_AndSimulatorStopsTime()]] - degree 12, connects to 5 communities
- [[TimeSyncIntegrationTests.SimulatorStarted_BeforeMaster_ConnectsLater()]] - degree 11, connects to 5 communities
- [[IMessagePublisher.PublishAsync()]] - degree 9, connects to 4 communities