---
type: community
cohesion: 0.20
members: 15
---

# DeviceTimeSource (2)

**Cohesion:** 0.20 - loosely connected
**Members:** 15 nodes

## Members
- [[DeviceTimeSource.ChangeSpeed()]] - code - src/Devices.Common/DeviceTimeSource.cs
- [[DeviceTimeSource.Create()]] - code - src/Devices.Common/DeviceTimeSource.cs
- [[DeviceTimeSource.TogglePause()]] - code - src/Devices.Common/DeviceTimeSource.cs
- [[DeviceTimeSourceTests]] - code - tests/Devices.Common.Tests/DeviceTimeSourceTests.cs
- [[DeviceTimeSourceTests.Standalone_PauseAndSpeed_ChangeLocalClock()]] - code - tests/Devices.Common.Tests/DeviceTimeSourceTests.cs
- [[DeviceTimeSourceTests.Standalone_RunsImmediately_WithLocalClock()]] - code - tests/Devices.Common.Tests/DeviceTimeSourceTests.cs
- [[DeviceTimeSourceTests.WithMaster_LocalControlsAreRefused()]] - code - tests/Devices.Common.Tests/DeviceTimeSourceTests.cs
- [[ManualTimeProvider]] - code - tests/Devices.Common.Tests/DeviceTimeSourceTests.cs
- [[ManualTimeProvider.Advance()]] - code - tests/Devices.Common.Tests/DeviceTimeSourceTests.cs
- [[ManualTimeProvider.GetTimestamp()]] - code - tests/Devices.Common.Tests/DeviceTimeSourceTests.cs
- [[ManualTimeProvider.GetUtcNow()]] - code - tests/Devices.Common.Tests/DeviceTimeSourceTests.cs
- [[MonotonicUtcClock]] - code - src/SimCore.Time/MonotonicUtcClock.cs
- [[Now_1]] - code - src/SimCore.Time/MonotonicUtcClock.cs
- [[TimeProvider]] - code
- [[TimestampFrequency]] - code - tests/Devices.Common.Tests/DeviceTimeSourceTests.cs

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/DeviceTimeSource_2
SORT file.name ASC
```

## Connections to other communities
- 5 edges to [[_COMMUNITY_Gruppe 23]]
- 4 edges to [[_COMMUNITY_MasterClock]]
- 4 edges to [[_COMMUNITY_DeviceTimeSource]]
- 4 edges to [[_COMMUNITY_Gruppe 44]]
- 3 edges to [[_COMMUNITY_TimeState & TimeLine]]
- 3 edges to [[_COMMUNITY_WeatherClient & Anzeige]]
- 2 edges to [[_COMMUNITY_SyncedClock]]
- 2 edges to [[_COMMUNITY_Zeit-Bibliotheken & Master]]
- 2 edges to [[_COMMUNITY_Sync-Nachrichten]]
- 2 edges to [[_COMMUNITY_PV-Engine & Fehler]]
- 2 edges to [[_COMMUNITY_Batterie-Modell]]
- 2 edges to [[_COMMUNITY_Batterie-Simulator Loop (3)]]
- 1 edge to [[_COMMUNITY_Wettermodell & Tests (2)]]
- 1 edge to [[_COMMUNITY_Gruppe 56]]
- 1 edge to [[_COMMUNITY_Gruppe 21]]
- 1 edge to [[_COMMUNITY_Sync-Nachrichten (2)]]
- 1 edge to [[_COMMUNITY_Gruppe 119]]
- 1 edge to [[_COMMUNITY_Batterie-Modbus & Update (4)]]
- 1 edge to [[_COMMUNITY_RegisterCodec & Befehle (2)]]
- 1 edge to [[_COMMUNITY_Gruppe 65]]
- 1 edge to [[_COMMUNITY_GatewayService]]
- 1 edge to [[_COMMUNITY_MQTT-Publisher & Zugangsdaten]]

## Top bridge nodes
- [[DeviceTimeSource.Create()]] - degree 15, connects to 10 communities
- [[DeviceTimeSource.TogglePause()]] - degree 11, connects to 9 communities
- [[DeviceTimeSource.ChangeSpeed()]] - degree 8, connects to 5 communities
- [[TimeProvider]] - degree 7, connects to 4 communities
- [[MonotonicUtcClock]] - degree 6, connects to 4 communities