---
type: community
cohesion: 0.06
members: 58
---

# Zeitsync-Client & Konsole

**Cohesion:** 0.06 - loosely connected
**Members:** 58 nodes

## Members
- [[AutoStart]] - code - src/SimulationMaster/MasterOptions.cs
- [[BindAddress_1]] - code - src/SimulationMaster/MasterOptions.cs
- [[ClientConnectionState]] - code - src/SimCore.TimeSync/TimeSyncClientInfo.cs
- [[ConcurrentQueue]] - code
- [[Connected_1]] - code - src/SimCore.TimeSync/TimeSyncClientInfo.cs
- [[Disconnected]] - code - src/SimCore.TimeSync/TimeSyncClientInfo.cs
- [[Handled]] - code - src/SimulationMaster/MasterConsole.cs
- [[HasKeyboard_1]] - code - src/SimulationMaster/MasterConsole.cs
- [[HeartbeatInterval_1]] - code - src/SimulationMaster/MasterOptions.cs
- [[IPAddress]] - code
- [[IsInteractive_3]] - code - src/SimulationMaster/MasterConsole.cs
- [[KeyResult_1]] - code
- [[KeyResult]] - code - src/SimulationMaster/MasterConsole.cs
- [[LeadTime_1]] - code - src/SimulationMaster/MasterOptions.cs
- [[MasterConsole]] - code - src/SimulationMaster/MasterConsole.cs
- [[MasterConsole.AddMessage()]] - code - src/SimulationMaster/MasterConsole.cs
- [[MasterConsole.BuildLines()]] - code - src/SimulationMaster/MasterConsole.cs
- [[MasterConsole.BuildStatusLine()]] - code - src/SimulationMaster/MasterConsole.cs
- [[MasterConsole.ChangeSpeed()]] - code - src/SimulationMaster/MasterConsole.cs
- [[MasterConsole.DrawAsync()]] - code - src/SimulationMaster/MasterConsole.cs
- [[MasterConsole.Fit()]] - code - src/SimulationMaster/MasterConsole.cs
- [[MasterConsole.FormatSigned()]] - code - src/SimulationMaster/MasterConsole.cs
- [[MasterConsole.FormatSpan()]] - code - src/SimulationMaster/MasterConsole.cs
- [[MasterConsole.HandleKey()]] - code - src/SimulationMaster/MasterConsole.cs
- [[MasterConsole.HandlePromptKey()]] - code - src/SimulationMaster/MasterConsole.cs
- [[MasterConsole.Initialize()]] - code - src/SimulationMaster/MasterConsole.cs
- [[MasterConsole.ModeText()]] - code - src/SimulationMaster/MasterConsole.cs
- [[MasterConsole.Restart()]] - code - src/SimulationMaster/MasterConsole.cs
- [[MasterConsole.RestartSeason()]] - code - src/SimulationMaster/MasterConsole.cs
- [[MasterConsole.RunAsync()]] - code - src/SimulationMaster/MasterConsole.cs
- [[MasterConsole.ShutdownAsync()]] - code - src/SimulationMaster/MasterConsole.cs
- [[MasterConsole.StartPrompt()]] - code - src/SimulationMaster/MasterConsole.cs
- [[MasterConsole.StateText()]] - code - src/SimulationMaster/MasterConsole.cs
- [[MasterConsole.SubmitPrompt()]] - code - src/SimulationMaster/MasterConsole.cs
- [[MasterConsole.TogglePause()]] - code - src/SimulationMaster/MasterConsole.cs
- [[MasterOptions]] - code - src/SimulationMaster/MasterOptions.cs
- [[MasterOptions.Parse()]] - code - src/SimulationMaster/MasterOptions.cs
- [[MasterOptions.ParseAddress()]] - code - src/SimulationMaster/MasterOptions.cs
- [[MasterOptions.ParseNumber()]] - code - src/SimulationMaster/MasterOptions.cs
- [[MasterOptions.ParseTime()]] - code - src/SimulationMaster/MasterOptions.cs
- [[MasterOptions.TryParseNumber()]] - code - src/SimulationMaster/MasterOptions.cs
- [[MasterOptions.TryParseTime()]] - code - src/SimulationMaster/MasterOptions.cs
- [[MasterOptions.Value()]] - code - src/SimulationMaster/MasterOptions.cs
- [[None_4]] - code - src/SimulationMaster/MasterConsole.cs
- [[NotResponding]] - code - src/SimCore.TimeSync/TimeSyncClientInfo.cs
- [[Port_3]] - code - src/SimulationMaster/MasterOptions.cs
- [[PromptKind]] - code
- [[PromptKind_1]] - code - src/SimulationMaster/MasterConsole.cs
- [[Quit]] - code - src/SimulationMaster/MasterConsole.cs
- [[Restart]] - code - src/SimulationMaster/MasterConsole.cs
- [[Speed_3]] - code - src/SimulationMaster/MasterConsole.cs
- [[Speed_4]] - code - src/SimulationMaster/MasterOptions.cs
- [[StartTime]] - code - src/SimulationMaster/MasterOptions.cs
- [[StringBuilder]] - code
- [[TimeSyncClientInfo]] - code - src/SimCore.TimeSync/TimeSyncClientInfo.cs
- [[TimeSyncClientInfo.cs]] - code - src/SimCore.TimeSync/TimeSyncClientInfo.cs
- [[Tolerance]] - code - src/SimulationMaster/MasterOptions.cs
- [[Width_2]] - code - src/SimulationMaster/MasterConsole.cs

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/Zeitsync-Client__Konsole
SORT file.name ASC
```

## Connections to other communities
- 7 edges to [[_COMMUNITY_TimeSyncServer]]
- 6 edges to [[_COMMUNITY_Batterie-Simulator Loop]]
- 5 edges to [[_COMMUNITY_WeatherFeed]]
- 5 edges to [[_COMMUNITY_Gruppe 51]]
- 3 edges to [[_COMMUNITY_Zeit-Bibliotheken & Master]]
- 3 edges to [[_COMMUNITY_Master-Zeit Tests]]
- 2 edges to [[_COMMUNITY_Batterie-Modbus & Update]]
- 2 edges to [[_COMMUNITY_TimeState & TimeLine]]
- 2 edges to [[_COMMUNITY_MasterClock]]
- 1 edge to [[_COMMUNITY_Gruppe 64]]
- 1 edge to [[_COMMUNITY_Gruppe 44]]
- 1 edge to [[_COMMUNITY_Gruppe 63]]
- 1 edge to [[_COMMUNITY_CsvLogger]]
- 1 edge to [[_COMMUNITY_Shared Konsolenausgabe]]

## Top bridge nodes
- [[MasterConsole]] - degree 35, connects to 5 communities
- [[TimeSyncClientInfo]] - degree 8, connects to 4 communities
- [[MasterOptions]] - degree 20, connects to 3 communities
- [[MasterConsole.RunAsync()]] - degree 8, connects to 3 communities
- [[MasterConsole.Restart()]] - degree 6, connects to 2 communities