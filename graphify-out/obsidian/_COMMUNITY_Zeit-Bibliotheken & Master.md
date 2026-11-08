---
type: community
cohesion: 0.16
members: 29
---

# Zeit-Bibliotheken & Master

**Cohesion:** 0.16 - loosely connected
**Members:** 29 nodes

## Members
- [[ClockOffsetEstimatorTests.cs]] - code - tests/SimCore.Tests/ClockOffsetEstimatorTests.cs
- [[DeviceTimeSource.cs]] - code - src/Devices.Common/DeviceTimeSource.cs
- [[DeviceTimeSourceTests.cs]] - code - tests/Devices.Common.Tests/DeviceTimeSourceTests.cs
- [[MasterClock.cs]] - code - src/SimCore.Time/MasterClock.cs
- [[MasterClockTests.cs]] - code - tests/SimCore.Tests/MasterClockTests.cs
- [[MasterConsole.cs]] - code - src/SimulationMaster/MasterConsole.cs
- [[MasterOptions.cs]] - code - src/SimulationMaster/MasterOptions.cs
- [[MessageChannel.cs]] - code - src/SimCore.TimeSync/MessageChannel.cs
- [[ModbusDeviceServer.cs]] - code - src/Devices.Common/Modbus/ModbusDeviceServer.cs
- [[SimCore.Contracts]] - code - src/SimCore.Contracts/ISimClock.cs
- [[SimCore.Tests]] - code - tests/SimCore.Tests/ClockOffsetEstimatorTests.cs
- [[SimCore.Time]] - code - src/SimCore.Time/ClockOffsetEstimator.cs
- [[SimCore.TimeSync]] - code - src/SimCore.TimeSync/MessageChannel.cs
- [[SimulationMaster]] - code - src/SimulationMaster/MasterConsole.cs
- [[SimulationMasterProgram.cs]] - code - src/SimulationMaster/Program.cs
- [[SyncProtocolTests.cs]] - code - tests/SimCore.Tests/SyncProtocolTests.cs
- [[SyncedClock.cs]] - code - src/SimCore.Time/SyncedClock.cs
- [[SyncedClockTests.cs]] - code - tests/SimCore.Tests/SyncedClockTests.cs
- [[TimeLine.cs]] - code - src/SimCore.Time/TimeLine.cs
- [[TimeSyncClient.cs]] - code - src/SimCore.TimeSync/TimeSyncClient.cs
- [[TimeSyncIntegrationTests.cs]] - code - tests/SimCore.Tests/TimeSyncIntegrationTests.cs
- [[TimeSyncOptions.cs]] - code - src/Devices.Common/TimeSyncOptions.cs
- [[TimeSyncServer.cs]] - code - src/SimCore.TimeSync/TimeSyncServer.cs
- [[WeatherModel.cs]] - code - src/WeatherSimulator/Simulation/WeatherModel.cs
- [[system_collections_concurrent]] - concept
- [[system_net]] - concept
- [[system_net_sockets]] - concept
- [[system_threading_channels]] - concept
- [[weathersynth_1]] - concept

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/Zeit-Bibliotheken__Master
SORT file.name ASC
```

## Connections to other communities
- 9 edges to [[_COMMUNITY_Modbus-Tests]]
- 7 edges to [[_COMMUNITY_Charger-Namespaces]]
- 4 edges to [[_COMMUNITY_Bibliotheks-Namespaces]]
- 4 edges to [[_COMMUNITY_Sync-Nachrichten]]
- 4 edges to [[_COMMUNITY_Wettermodell & Tests]]
- 3 edges to [[_COMMUNITY_DeviceTimeSource]]
- 3 edges to [[_COMMUNITY_Batterie-Modbus & Update]]
- 3 edges to [[_COMMUNITY_Gruppe 53]]
- 3 edges to [[_COMMUNITY_Gruppe 40]]
- 3 edges to [[_COMMUNITY_Zeitsync-Client & Konsole]]
- 3 edges to [[_COMMUNITY_TimeSyncServer]]
- 2 edges to [[_COMMUNITY_WeatherFeed]]
- 2 edges to [[_COMMUNITY_Gruppe 63]]
- 2 edges to [[_COMMUNITY_TimeState & TimeLine]]
- 2 edges to [[_COMMUNITY_Gruppe 54]]
- 2 edges to [[_COMMUNITY_MasterClock]]
- 2 edges to [[_COMMUNITY_SyncedClock]]
- 2 edges to [[_COMMUNITY_PV-Namespaces]]
- 1 edge to [[_COMMUNITY_RegisterCodec & Befehle]]
- 1 edge to [[_COMMUNITY_Gruppe 55]]
- 1 edge to [[_COMMUNITY_Gruppe 84]]
- 1 edge to [[_COMMUNITY_Master-Zeit Tests]]

## Top bridge nodes
- [[SimCore.Contracts]] - degree 29, connects to 7 communities
- [[ModbusDeviceServer.cs]] - degree 12, connects to 5 communities
- [[MasterConsole.cs]] - degree 8, connects to 3 communities
- [[system_net]] - degree 8, connects to 3 communities
- [[DeviceTimeSource.cs]] - degree 6, connects to 3 communities