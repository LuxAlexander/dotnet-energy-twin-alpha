---
type: community
cohesion: 0.09
members: 28
---

# Gateway-Optionen

**Cohesion:** 0.09 - loosely connected
**Members:** 28 nodes

## Members
- [[BrokerHost]] - code - src/ModbusRabbitGateway/GatewayOptions.cs
- [[BrokerPort]] - code - src/ModbusRabbitGateway/GatewayOptions.cs
- [[ConsoleOnly]] - code - src/ModbusRabbitGateway/GatewayOptions.cs
- [[DefaultDevices]] - code - src/ModbusRabbitGateway/GatewayOptions.cs
- [[DeviceEndpoint.Parse()]] - code - src/ModbusRabbitGateway/DeviceEndpoint.cs
- [[Devices]] - code - src/ModbusRabbitGateway/GatewayOptions.cs
- [[EnvFile]] - code - src/ModbusRabbitGateway/GatewayOptions.cs
- [[GatewayId]] - code - src/ModbusRabbitGateway/GatewayOptions.cs
- [[GatewayOptions]] - code - src/ModbusRabbitGateway/GatewayOptions.cs
- [[GatewayOptions.Next()]] - code - src/ModbusRabbitGateway/GatewayOptions.cs
- [[GatewayOptions.Parse()]] - code - src/ModbusRabbitGateway/GatewayOptions.cs
- [[GatewayOptions.ParseNumber()]] - code - src/ModbusRabbitGateway/GatewayOptions.cs
- [[GatewayOptionsTests]] - code - tests/ModbusRabbitGateway.Tests/GatewayOptionsTests.cs
- [[GatewayOptionsTests.Defaults_AreTheFourDemoDevices()]] - code - tests/ModbusRabbitGateway.Tests/GatewayOptionsTests.cs
- [[GatewayOptionsTests.Device_ReplacesDefaults_WithHostPortAndUnit()]] - code - tests/ModbusRabbitGateway.Tests/GatewayOptionsTests.cs
- [[GatewayOptionsTests.DuplicateDeviceId_IsRejected()]] - code - tests/ModbusRabbitGateway.Tests/GatewayOptionsTests.cs
- [[GatewayOptionsTests.UnknownPrefix_SkipsTypeCheck()]] - code - tests/ModbusRabbitGateway.Tests/GatewayOptionsTests.cs
- [[Interval]] - code - src/ModbusRabbitGateway/GatewayOptions.cs
- [[ListKeys]] - code - src/ModbusRabbitGateway/GatewayOptions.cs
- [[MqttCredentialsTests.BrokerAndConsoleOptions_AreParsed()]] - code - tests/ModbusRabbitGateway.Tests/MqttCredentialsTests.cs
- [[MqttCredentialsTests.PasswordOption_IsRejected()]] - code - tests/ModbusRabbitGateway.Tests/MqttCredentialsTests.cs
- [[MqttUser]] - code - src/ModbusRabbitGateway/GatewayOptions.cs
- [[Once_1]] - code - src/ModbusRabbitGateway/GatewayOptions.cs
- [[Quality]] - code - src/ModbusRabbitGateway/GatewayOptions.cs
- [[ShowJson]] - code - src/ModbusRabbitGateway/GatewayOptions.cs
- [[StaleModelTime]] - code - src/ModbusRabbitGateway/GatewayOptions.cs
- [[StatusRepeat]] - code - src/ModbusRabbitGateway/GatewayOptions.cs
- [[TimeoutMs_2]] - code - src/ModbusRabbitGateway/GatewayOptions.cs

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/Gateway-Optionen
SORT file.name ASC
```

## Connections to other communities
- 6 edges to [[_COMMUNITY_Gruppe 23]]
- 5 edges to [[_COMMUNITY_Gruppe 48]]
- 4 edges to [[_COMMUNITY_DeviceEndpoint]]
- 3 edges to [[_COMMUNITY_Gruppe 52]]
- 3 edges to [[_COMMUNITY_MQTT-Publisher & Zugangsdaten]]
- 3 edges to [[_COMMUNITY_GatewayService]]
- 2 edges to [[_COMMUNITY_SRS Gateway-Anforderungen]]
- 1 edge to [[_COMMUNITY_Gruppe 65]]
- 1 edge to [[_COMMUNITY_Nachrichtentypen (Contract)]]
- 1 edge to [[_COMMUNITY_Messwert-Schlüssel]]
- 1 edge to [[_COMMUNITY_MessageBuilder]]
- 1 edge to [[_COMMUNITY_JSON-Serialisierung]]
- 1 edge to [[_COMMUNITY_DevicePoller & Gerätetests]]

## Top bridge nodes
- [[GatewayOptions]] - degree 31, connects to 9 communities
- [[GatewayOptions.Parse()]] - degree 13, connects to 3 communities
- [[GatewayOptionsTests]] - degree 8, connects to 3 communities
- [[MqttCredentialsTests.BrokerAndConsoleOptions_AreParsed()]] - degree 4, connects to 3 communities
- [[MqttCredentialsTests.PasswordOption_IsRejected()]] - degree 4, connects to 3 communities