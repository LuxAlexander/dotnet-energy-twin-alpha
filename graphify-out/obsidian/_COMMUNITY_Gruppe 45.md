---
type: community
cohesion: 0.22
members: 15
---

# Gruppe 45

**Cohesion:** 0.22 - loosely connected
**Members:** 15 nodes

## Members
- [[ArgsParser_2]] - code - src/Devices/KuehlhausSimulator/IO/ArgsParser.cs
- [[ArgsParser.IsHelpRequested()_2]] - code - src/Devices/KuehlhausSimulator/IO/ArgsParser.cs
- [[ArgsParser.NextValue()_2]] - code - src/Devices/KuehlhausSimulator/IO/ArgsParser.cs
- [[ArgsParser.Parse()_2]] - code - src/Devices/KuehlhausSimulator/IO/ArgsParser.cs
- [[ArgsParser.ParseNumber()_1]] - code - src/Devices/KuehlhausSimulator/IO/ArgsParser.cs
- [[ArgsParser.ParsePositive()_1]] - code - src/Devices/KuehlhausSimulator/IO/ArgsParser.cs
- [[KuehlhausArgsTests]] - code - tests/FieldDevices.Tests/ConsumerTests.cs
- [[KuehlhausArgsTests.DurationWithoutStandalone_IsRejected()]] - code - tests/FieldDevices.Tests/ConsumerTests.cs
- [[KuehlhausArgsTests.StandaloneDefaults_MatchTheDeviceDocumentation()]] - code - tests/FieldDevices.Tests/ConsumerTests.cs
- [[KuehlhausArgsTests.UnknownOption_IsRejected()]] - code - tests/FieldDevices.Tests/ConsumerTests.cs
- [[KuehlhausArgsTests.Values_AreParsedWithDotAndComma()]] - code - tests/FieldDevices.Tests/ConsumerTests.cs
- [[KuehlhausArgsTests.WeatherOff_DisablesTheWeatherSimulator()]] - code - tests/FieldDevices.Tests/ConsumerTests.cs
- [[KuehlhausArgsTests.WithMaster_WeatherSimulatorIsDefault()]] - code - tests/FieldDevices.Tests/ConsumerTests.cs
- [[KuehlhausEngineTests]] - code - tests/FieldDevices.Tests/ConsumerTests.cs
- [[KuehlhausEngineTests.Renderer_BuildsInteractiveLayout()]] - code - tests/FieldDevices.Tests/ConsumerTests.cs

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/Gruppe_45
SORT file.name ASC
```

## Connections to other communities
- 7 edges to [[_COMMUNITY_Master-Zeit Tests]]
- 4 edges to [[_COMMUNITY_TimeSyncOptions Tests]]
- 3 edges to [[_COMMUNITY_Bibliotheks-Namespaces]]
- 3 edges to [[_COMMUNITY_Batterie-Simulator Loop]]
- 2 edges to [[_COMMUNITY_CsvLogger]]
- 1 edge to [[_COMMUNITY_Gruppe 64]]
- 1 edge to [[_COMMUNITY_Gruppe 55]]
- 1 edge to [[_COMMUNITY_Gruppe 52]]
- 1 edge to [[_COMMUNITY_Gruppe 46]]
- 1 edge to [[_COMMUNITY_WeatherClient & Anzeige]]

## Top bridge nodes
- [[ArgsParser.Parse()_2]] - degree 18, connects to 7 communities
- [[KuehlhausEngineTests.Renderer_BuildsInteractiveLayout()]] - degree 5, connects to 3 communities
- [[KuehlhausArgsTests]] - degree 8, connects to 2 communities
- [[KuehlhausArgsTests.DurationWithoutStandalone_IsRejected()]] - degree 4, connects to 2 communities
- [[KuehlhausArgsTests.UnknownOption_IsRejected()]] - degree 4, connects to 2 communities