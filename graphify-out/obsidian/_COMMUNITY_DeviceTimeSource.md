---
type: community
cohesion: 0.11
members: 27
---

# DeviceTimeSource

**Cohesion:** 0.11 - loosely connected
**Members:** 27 nodes

## Members
- [[Clock]] - code - src/Devices.Common/DeviceTimeSource.cs
- [[DeviceTimeSource]] - code - src/Devices.Common/DeviceTimeSource.cs
- [[DeviceTimeSource.ChangeSpeed()]] - code - src/Devices.Common/DeviceTimeSource.cs
- [[DeviceTimeSource.Create()]] - code - src/Devices.Common/DeviceTimeSource.cs
- [[DeviceTimeSource.FormatSpan()]] - code - src/Devices.Common/DeviceTimeSource.cs
- [[DeviceTimeSource.SetSpeed()]] - code - src/Devices.Common/DeviceTimeSource.cs
- [[DeviceTimeSource.Speed()]] - code - src/Devices.Common/DeviceTimeSource.cs
- [[DeviceTimeSource.TogglePause()]] - code - src/Devices.Common/DeviceTimeSource.cs
- [[DeviceTimeSourceTests]] - code - tests/Devices.Common.Tests/DeviceTimeSourceTests.cs
- [[DeviceTimeSourceTests.Standalone_PauseAndSpeed_ChangeLocalClock()]] - code - tests/Devices.Common.Tests/DeviceTimeSourceTests.cs
- [[DeviceTimeSourceTests.Standalone_RunsImmediately_WithLocalClock()]] - code - tests/Devices.Common.Tests/DeviceTimeSourceTests.cs
- [[DeviceTimeSourceTests.WithMaster_LocalControlsAreRefused()]] - code - tests/Devices.Common.Tests/DeviceTimeSourceTests.cs
- [[HasTime]] - code - src/Devices.Common/DeviceTimeSource.cs
- [[IsRunning]] - code - src/Devices.Common/DeviceTimeSource.cs
- [[IsStandalone]] - code - src/Devices.Common/DeviceTimeSource.cs
- [[ManualTimeProvider]] - code - tests/Devices.Common.Tests/DeviceTimeSourceTests.cs
- [[ManualTimeProvider.Advance()]] - code - tests/Devices.Common.Tests/DeviceTimeSourceTests.cs
- [[ManualTimeProvider.GetTimestamp()]] - code - tests/Devices.Common.Tests/DeviceTimeSourceTests.cs
- [[ManualTimeProvider.GetUtcNow()]] - code - tests/Devices.Common.Tests/DeviceTimeSourceTests.cs
- [[MonotonicUtcClock]] - code - src/SimCore.Time/MonotonicUtcClock.cs
- [[MonotonicUtcClock.cs]] - code - src/SimCore.Time/MonotonicUtcClock.cs
- [[Now_2]] - code - src/SimCore.Time/MonotonicUtcClock.cs
- [[Options]] - code - src/Devices.Common/DeviceTimeSource.cs
- [[Status]] - code - src/Devices.Common/DeviceTimeSource.cs
- [[StatusText]] - code - src/Devices.Common/DeviceTimeSource.cs
- [[TimeProvider]] - code
- [[TimestampFrequency]] - code - tests/Devices.Common.Tests/DeviceTimeSourceTests.cs

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/DeviceTimeSource
SORT file.name ASC
```

## Connections to other communities
- 8 edges to [[_COMMUNITY_Batterie-Modbus & Update]]
- 6 edges to [[_COMMUNITY_Master-Zeit Tests]]
- 5 edges to [[_COMMUNITY_Batterie-Simulator Loop]]
- 5 edges to [[_COMMUNITY_Gruppe 55]]
- 5 edges to [[_COMMUNITY_MasterClock]]
- 4 edges to [[_COMMUNITY_SyncedClock]]
- 4 edges to [[_COMMUNITY_PV-Engine & Fehler]]
- 3 edges to [[_COMMUNITY_Zeit-Bibliotheken & Master]]
- 2 edges to [[_COMMUNITY_Gruppe 44]]
- 2 edges to [[_COMMUNITY_Gruppe 63]]
- 2 edges to [[_COMMUNITY_Sync-Nachrichten]]
- 2 edges to [[_COMMUNITY_Gruppe 41]]
- 2 edges to [[_COMMUNITY_WeatherClient & Anzeige]]
- 2 edges to [[_COMMUNITY_RegisterCodec & Befehle]]
- 2 edges to [[_COMMUNITY_TimeSyncServer]]
- 2 edges to [[_COMMUNITY_Gruppe 81]]
- 2 edges to [[_COMMUNITY_Gruppe 51]]
- 2 edges to [[_COMMUNITY_TimeState & TimeLine]]
- 1 edge to [[_COMMUNITY_CsvLogger]]
- 1 edge to [[_COMMUNITY_Diagnose-Client Verbindung]]
- 1 edge to [[_COMMUNITY_WeatherFeed]]
- 1 edge to [[_COMMUNITY_Gruppe 84]]

## Top bridge nodes
- [[DeviceTimeSource]] - degree 36, connects to 15 communities
- [[DeviceTimeSource.Create()]] - degree 15, connects to 9 communities
- [[DeviceTimeSource.TogglePause()]] - degree 11, connects to 7 communities
- [[DeviceTimeSource.ChangeSpeed()]] - degree 8, connects to 3 communities
- [[DeviceTimeSourceTests]] - degree 7, connects to 3 communities