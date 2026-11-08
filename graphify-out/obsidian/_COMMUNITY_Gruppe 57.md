---
type: community
cohesion: 0.38
members: 13
---

# Gruppe 57

**Cohesion:** 0.38 - loosely connected
**Members:** 13 nodes

## Members
- [[Kuehlhaus.ResetFault()]] - code - src/Devices/KuehlhausSimulator/Kuehlhaus.cs
- [[Kuehlhaus.TurnOff()]] - code - src/Devices/KuehlhausSimulator/Kuehlhaus.cs
- [[Kuehlhaus.Update()]] - code - src/Devices/KuehlhausSimulator/Kuehlhaus.cs
- [[KuehlhausModelTests]] - code - tests/FieldDevices.Tests/ConsumerTests.cs
- [[KuehlhausModelTests.CompressorFailure_IsAlwaysClearedByReset()]] - code - tests/FieldDevices.Tests/ConsumerTests.cs
- [[KuehlhausModelTests.Compressor_SwitchesWithHysteresisAroundSetpoint()]] - code - tests/FieldDevices.Tests/ConsumerTests.cs
- [[KuehlhausModelTests.CreateRoom()]] - code - tests/FieldDevices.Tests/ConsumerTests.cs
- [[KuehlhausModelTests.ElectricPowerAndEnergy_FollowCoolingPowerAndCop()]] - code - tests/FieldDevices.Tests/ConsumerTests.cs
- [[KuehlhausModelTests.LargeStep_StaysStableAndDoesNotOvershoot()]] - code - tests/FieldDevices.Tests/ConsumerTests.cs
- [[KuehlhausModelTests.OverTemperature_LatchesAndNeedsCoolDownBeforeReset()]] - code - tests/FieldDevices.Tests/ConsumerTests.cs
- [[KuehlhausModelTests.Setpoint_OutsideMinus30To0_IsRejected()]] - code - tests/FieldDevices.Tests/ConsumerTests.cs
- [[KuehlhausModelTests.TurnOff_StopsCompressorAndReportsOffState()]] - code - tests/FieldDevices.Tests/ConsumerTests.cs
- [[KuehlhausModelTests.Update_UsesExactExponentialSolution()]] - code - tests/FieldDevices.Tests/ConsumerTests.cs

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/Gruppe_57
SORT file.name ASC
```

## Connections to other communities
- 8 edges to [[_COMMUNITY_Kühlhaus-Modell]]
- 8 edges to [[_COMMUNITY_Master-Zeit Tests]]
- 5 edges to [[_COMMUNITY_Batterie-Modbus & Update]]
- 2 edges to [[_COMMUNITY_Batterie-Simulator Loop]]
- 2 edges to [[_COMMUNITY_Gruppe 58]]
- 1 edge to [[_COMMUNITY_Gruppe 59]]
- 1 edge to [[_COMMUNITY_Bibliotheks-Namespaces]]
- 1 edge to [[_COMMUNITY_StepScheduler]]

## Top bridge nodes
- [[Kuehlhaus.Update()]] - degree 15, connects to 4 communities
- [[KuehlhausModelTests.CreateRoom()]] - degree 12, connects to 3 communities
- [[Kuehlhaus.TurnOff()]] - degree 8, connects to 2 communities
- [[KuehlhausModelTests.OverTemperature_LatchesAndNeedsCoolDownBeforeReset()]] - degree 7, connects to 2 communities
- [[KuehlhausModelTests.CompressorFailure_IsAlwaysClearedByReset()]] - degree 6, connects to 2 communities