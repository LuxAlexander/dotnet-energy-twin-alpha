---
type: community
cohesion: 0.20
members: 18
---

# TimeState & TimeLine

**Cohesion:** 0.20 - loosely connected
**Members:** 18 nodes

## Members
- [[DateTimeOffset]] - code
- [[Guid]] - code
- [[Latest_1]] - code - src/SimCore.Time/TimeLine.cs
- [[ManualTime_1]] - code - tests/ModbusRabbitGateway.Tests/MqttPublisherTests.cs
- [[ManualTime.Advance()]] - code - tests/ModbusRabbitGateway.Tests/MqttPublisherTests.cs
- [[ManualTime.GetUtcNow()]] - code - tests/ModbusRabbitGateway.Tests/MqttPublisherTests.cs
- [[MasterClock.Change()]] - code - src/SimCore.Time/MasterClock.cs
- [[States]] - code - src/SimCore.Time/TimeLine.cs
- [[SyncedClock.EvaluationTime()]] - code - src/SimCore.Time/SyncedClock.cs
- [[SyncedClock.StateNow()]] - code - src/SimCore.Time/SyncedClock.cs
- [[TimeLine]] - code - src/SimCore.Time/TimeLine.cs
- [[TimeLine.Apply()]] - code - src/SimCore.Time/TimeLine.cs
- [[TimeLine.Prune()]] - code - src/SimCore.Time/TimeLine.cs
- [[TimeLine.SimTimeAt()]] - code - src/SimCore.Time/TimeLine.cs
- [[TimeLine.StateAt()]] - code - src/SimCore.Time/TimeLine.cs
- [[TimeState]] - code - src/SimCore.Contracts/TimeState.cs
- [[TimeState.SimTimeAt()]] - code - src/SimCore.Contracts/TimeState.cs
- [[TimeSyncServer.OnStateChanged()]] - code - src/SimCore.TimeSync/TimeSyncServer.cs

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/TimeState__TimeLine
SORT file.name ASC
```

## Connections to other communities
- 13 edges to [[_COMMUNITY_MasterClock]]
- 8 edges to [[_COMMUNITY_Sync-Nachrichten]]
- 7 edges to [[_COMMUNITY_Gruppe 23]]
- 5 edges to [[_COMMUNITY_SyncedClock]]
- 5 edges to [[_COMMUNITY_MQTT-Publisher & Zugangsdaten]]
- 3 edges to [[_COMMUNITY_Gruppe 65]]
- 3 edges to [[_COMMUNITY_DeviceTimeSource (2)]]
- 3 edges to [[_COMMUNITY_Wettermodell & Tests (2)]]
- 3 edges to [[_COMMUNITY_MessageBuilder-Tests]]
- 2 edges to [[_COMMUNITY_Zeit-Bibliotheken & Master]]
- 2 edges to [[_COMMUNITY_Gruppe 71]]
- 2 edges to [[_COMMUNITY_Gruppe 119]]
- 2 edges to [[_COMMUNITY_DevicePoller & Gerätetests]]
- 2 edges to [[_COMMUNITY_Nachrichtentypen (Contract)]]
- 1 edge to [[_COMMUNITY_Gruppe 88]]
- 1 edge to [[_COMMUNITY_Gruppe 52]]
- 1 edge to [[_COMMUNITY_Shared Konsolenausgabe]]
- 1 edge to [[_COMMUNITY_Sync-Nachrichten (2)]]
- 1 edge to [[_COMMUNITY_GatewayService]]
- 1 edge to [[_COMMUNITY_MessageBuilder]]

## Top bridge nodes
- [[DateTimeOffset]] - degree 38, connects to 15 communities
- [[TimeState]] - degree 22, connects to 6 communities
- [[TimeLine]] - degree 12, connects to 5 communities
- [[MasterClock.Change()]] - degree 10, connects to 3 communities
- [[ManualTime_1]] - degree 5, connects to 2 communities