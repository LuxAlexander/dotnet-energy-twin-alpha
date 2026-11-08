---
type: community
cohesion: 0.18
members: 16
---

# Messwert-Schlüssel

**Cohesion:** 0.18 - loosely connected
**Members:** 16 nodes

## Members
- [[DeviceType_2]] - code
- [[Festgehaltene Schlüsselliste measurement-keys.txt]] - document - tests/ModbusRabbitGateway.Tests/README.md
- [[GatewayOptions.IsHelpRequested()]] - code - src/ModbusRabbitGateway/GatewayOptions.cs
- [[IEnumerable]] - code
- [[KeyCatalog]] - code - src/ModbusRabbitGateway/MeasurementKeys.cs
- [[KeyCatalog.For()]] - code - src/ModbusRabbitGateway/MeasurementKeys.cs
- [[MeasurementKeys]] - code - src/ModbusRabbitGateway/MeasurementKeys.cs
- [[MeasurementKeys.From()]] - code - src/ModbusRabbitGateway/MeasurementKeys.cs
- [[MeasurementKeys.cs]] - code - src/ModbusRabbitGateway/MeasurementKeys.cs
- [[MeasurementKeysTests]] - code - tests/ModbusRabbitGateway.Tests/MeasurementKeysTests.cs
- [[MeasurementKeysTests.Keys_AreUniqueAndAsciiSafe_InEveryPlan()]] - code - tests/ModbusRabbitGateway.Tests/MeasurementKeysTests.cs
- [[MeasurementKeysTests.Keys_MatchTheRecordedCatalog()]] - code - tests/ModbusRabbitGateway.Tests/MeasurementKeysTests.cs
- [[MeasurementKeysTests.cs]] - code - tests/ModbusRabbitGateway.Tests/MeasurementKeysTests.cs
- [[MessageBuilder.MeasuredRegisters()]] - code - src/ModbusRabbitGateway/MessageBuilder.cs
- [[RegisterDefinition_1]] - code
- [[RegisterPlan_2]] - code

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/Messwert-Schlüssel
SORT file.name ASC
```

## Connections to other communities
- 3 edges to [[_COMMUNITY_MessageBuilder]]
- 2 edges to [[_COMMUNITY_Gruppe 23]]
- 2 edges to [[_COMMUNITY_Gruppe 19]]
- 2 edges to [[_COMMUNITY_DeviceEndpoint]]
- 2 edges to [[_COMMUNITY_Gateway-Publisher]]
- 2 edges to [[_COMMUNITY_Gruppe 48]]
- 1 edge to [[_COMMUNITY_Gruppe 52]]
- 1 edge to [[_COMMUNITY_DeviceConnection (Modbus.Client)]]
- 1 edge to [[_COMMUNITY_Nachrichtentypen (Contract)]]
- 1 edge to [[_COMMUNITY_Gateway-Optionen]]
- 1 edge to [[_COMMUNITY_Nachrichtenvertrag & Messwerte]]
- 1 edge to [[_COMMUNITY_JSON-Serialisierung]]
- 1 edge to [[_COMMUNITY_DevicePoller & Gerätetests]]
- 1 edge to [[_COMMUNITY_MessageBuilder-Tests]]

## Top bridge nodes
- [[MeasurementKeysTests]] - degree 7, connects to 2 communities
- [[MeasurementKeys.cs]] - degree 5, connects to 2 communities
- [[MeasurementKeys]] - degree 5, connects to 2 communities
- [[MeasurementKeys.From()]] - degree 5, connects to 2 communities
- [[DeviceType_2]] - degree 4, connects to 2 communities