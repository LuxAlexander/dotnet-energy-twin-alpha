---
type: community
cohesion: 0.07
members: 55
---

# MQTT-Publisher & Zugangsdaten

**Cohesion:** 0.07 - loosely connected
**Members:** 55 nodes

## Members
- [[Broker-Ausfall einmal gemeldet, Backoff 1–30 s, Messwerte verworfen, Status nachgeholt, Last Will]] - document - src/ModbusRabbitGateway/README.md
- [[BrokerSettings]] - code - src/ModbusRabbitGateway/MqttPublisher.cs
- [[BrokerUnavailableException]] - code - src/ModbusRabbitGateway/Publishing.cs
- [[Bug MQTTnet 5 meldet abgelehnte Anmeldung per Ergebniscode statt Exception (behoben)]] - document - docs/development-journal/susanne-lastowicka.md
- [[ConcurrentQueue]] - code
- [[Description]] - code - src/ModbusRabbitGateway/MqttPublisher.cs
- [[Dictionary]] - code
- [[Endpoint_3]] - code - src/ModbusRabbitGateway/MqttPublisher.cs
- [[Func]] - code
- [[Gateway-Konfiguration (--device, --interval, --broker, --gateway-id ...)]] - document - docs/Nachrichtenvertrag-LB03.md
- [[GatewayId_1]] - code - src/ModbusRabbitGateway/MqttPublisher.cs
- [[IAsyncLifetime]] - code
- [[IMqttClient]] - code
- [[IsConnected_2]] - code - src/ModbusRabbitGateway/MqttPublisher.cs
- [[MQTT-Broker im Testprozess (MQTTnet.Server, kein Docker)]] - document - tests/ModbusRabbitGateway.Tests/README.md
- [[MQTTnet 5.0.1 (+ MQTTnet.Server als Test-Broker)]] - document - AGENTS.md
- [[ManualTime]] - code
- [[ModbusRabbitGateway.Tests (52 Tests)]] - document - AGENTS.md
- [[MqttClientOptions]] - code
- [[MqttConnectingFailedException]] - code
- [[MqttCredentials]] - code - src/ModbusRabbitGateway/MqttCredentials.cs
- [[MqttCredentials.ReadEnvFile()]] - code - src/ModbusRabbitGateway/MqttCredentials.cs
- [[MqttCredentials.Resolve()]] - code - src/ModbusRabbitGateway/MqttCredentials.cs
- [[MqttCredentials.ToString()]] - code - src/ModbusRabbitGateway/MqttCredentials.cs
- [[MqttCredentials.cs]] - code - src/ModbusRabbitGateway/MqttCredentials.cs
- [[MqttCredentialsTests]] - code - tests/ModbusRabbitGateway.Tests/MqttCredentialsTests.cs
- [[MqttCredentialsTests.Dispose()]] - code - tests/ModbusRabbitGateway.Tests/MqttCredentialsTests.cs
- [[MqttCredentialsTests.Env()]] - code - tests/ModbusRabbitGateway.Tests/MqttCredentialsTests.cs
- [[MqttCredentialsTests.EnvFile_IsUsedWhenNoEnvironmentVariables()]] - code - tests/ModbusRabbitGateway.Tests/MqttCredentialsTests.cs
- [[MqttCredentialsTests.EnvironmentVariables_WinOverFile_AndOptionWinsForUser()]] - code - tests/ModbusRabbitGateway.Tests/MqttCredentialsTests.cs
- [[MqttCredentialsTests.Nothing_GivesAnonymous()]] - code - tests/ModbusRabbitGateway.Tests/MqttCredentialsTests.cs
- [[MqttCredentialsTests.ToString_NeverShowsPassword()]] - code - tests/ModbusRabbitGateway.Tests/MqttCredentialsTests.cs
- [[MqttPublisher]] - code - src/ModbusRabbitGateway/MqttPublisher.cs
- [[MqttPublisher.BuildOptions()]] - code - src/ModbusRabbitGateway/MqttPublisher.cs
- [[MqttPublisher.DisposeAsync()]] - code - src/ModbusRabbitGateway/MqttPublisher.cs
- [[MqttPublisher.EnsureConnectedAsync()]] - code - src/ModbusRabbitGateway/MqttPublisher.cs
- [[MqttPublisher.IsAuthenticationProblem()]] - code - src/ModbusRabbitGateway/MqttPublisher.cs
- [[MqttPublisher.PublishAsync()]] - code - src/ModbusRabbitGateway/MqttPublisher.cs
- [[MqttPublisher.ScheduleRetry()]] - code - src/ModbusRabbitGateway/MqttPublisher.cs
- [[MqttPublisher.SendAsync()]] - code - src/ModbusRabbitGateway/MqttPublisher.cs
- [[MqttPublisher.cs]] - code - src/ModbusRabbitGateway/MqttPublisher.cs
- [[MqttPublisherTests]] - code - tests/ModbusRabbitGateway.Tests/MqttPublisherTests.cs
- [[MqttPublisherTests.BrokerDown_FailsFast_WaitsForBackoff_ThenReconnectsAndRepublishesStatus()]] - code - tests/ModbusRabbitGateway.Tests/MqttPublisherTests.cs
- [[MqttPublisherTests.CountOf()]] - code - tests/ModbusRabbitGateway.Tests/MqttPublisherTests.cs
- [[MqttPublisherTests.DisposeAsync()]] - code - tests/ModbusRabbitGateway.Tests/MqttPublisherTests.cs
- [[MqttPublisherTests.FreePort()]] - code - tests/ModbusRabbitGateway.Tests/MqttPublisherTests.cs
- [[MqttPublisherTests.InitializeAsync()]] - code - tests/ModbusRabbitGateway.Tests/MqttPublisherTests.cs
- [[MqttPublisherTests.NewPublisher()]] - code - tests/ModbusRabbitGateway.Tests/MqttPublisherTests.cs
- [[MqttPublisherTests.Publish_SendsWithQos1_RetainOnlyForStatus()]] - code - tests/ModbusRabbitGateway.Tests/MqttPublisherTests.cs
- [[MqttPublisherTests.StartServerAsync()]] - code - tests/ModbusRabbitGateway.Tests/MqttPublisherTests.cs
- [[MqttPublisherTests.WaitForAsync()]] - code - tests/ModbusRabbitGateway.Tests/MqttPublisherTests.cs
- [[MqttPublisherTests.WrongPassword_IsBrokerUnavailable_WithReadableMessage()]] - code - tests/ModbusRabbitGateway.Tests/MqttPublisherTests.cs
- [[MqttQualityOfServiceLevel]] - code
- [[MqttServer]] - code
- [[Zugangsdaten-Reihenfolge --mqtt-user → Umgebungsvariablen → docker.env; Passwort nie als Option]] - document - src/ModbusRabbitGateway/README.md

## Live Query (requires Dataview plugin)

```dataview
TABLE source_file, type FROM #community/MQTT-Publisher__Zugangsdaten
SORT file.name ASC
```

## Connections to other communities
- 13 edges to [[_COMMUNITY_TimeSyncServer]]
- 7 edges to [[_COMMUNITY_Gruppe 23]]
- 5 edges to [[_COMMUNITY_TimeState & TimeLine]]
- 5 edges to [[_COMMUNITY_GatewayService]]
- 5 edges to [[_COMMUNITY_Gateway-Publisher]]
- 4 edges to [[_COMMUNITY_Nachrichtentypen (Contract)]]
- 3 edges to [[_COMMUNITY_Batterie-Simulator Loop]]
- 3 edges to [[_COMMUNITY_Gateway-Optionen]]
- 3 edges to [[_COMMUNITY_Nachrichtenvertrag & Messwerte]]
- 3 edges to [[_COMMUNITY_Status-Wiederholung & RabbitMQ]]
- 3 edges to [[_COMMUNITY_DevicePoller & Gerätetests]]
- 2 edges to [[_COMMUNITY_Batterie-Modbus & Update]]
- 2 edges to [[_COMMUNITY_Gruppe 65]]
- 2 edges to [[_COMMUNITY_Batterie-Simulator Loop (2)]]
- 2 edges to [[_COMMUNITY_Aufgabenabgleich (readTask)]]
- 1 edge to [[_COMMUNITY_Zeitsync-Client & Konsole]]
- 1 edge to [[_COMMUNITY_Sync-Nachrichten]]
- 1 edge to [[_COMMUNITY_Gruppe 21]]
- 1 edge to [[_COMMUNITY_Batterie-Modbus & Update (2)]]
- 1 edge to [[_COMMUNITY_WeatherFeed]]
- 1 edge to [[_COMMUNITY_Gruppe 44]]
- 1 edge to [[_COMMUNITY_Gruppe 89]]
- 1 edge to [[_COMMUNITY_Batterie-Modbus & Update (3)]]
- 1 edge to [[_COMMUNITY_DeviceTimeSource (2)]]
- 1 edge to [[_COMMUNITY_DeviceConnection (Modbus.Client)]]
- 1 edge to [[_COMMUNITY_DeviceEndpoint]]
- 1 edge to [[_COMMUNITY_Gruppe 19]]
- 1 edge to [[_COMMUNITY_MessageBuilder]]
- 1 edge to [[_COMMUNITY_MessageBuilder-Tests]]
- 1 edge to [[_COMMUNITY_RabbitMQ unter Docker]]
- 1 edge to [[_COMMUNITY_Aufgabe 03 & Nachweise]]
- 1 edge to [[_COMMUNITY_Diagnose-Client & Modbus.Client]]
- 1 edge to [[_COMMUNITY_Fehlerverhalten Gateway]]
- 1 edge to [[_COMMUNITY_JSON-Serialisierung]]
- 1 edge to [[_COMMUNITY_Modbus-Tests]]
- 1 edge to [[_COMMUNITY_LB03-Projektstand]]
- 1 edge to [[_COMMUNITY_Funktionsnachweis mit Master]]

## Top bridge nodes
- [[MqttPublisher]] - degree 31, connects to 12 communities
- [[Func]] - degree 10, connects to 7 communities
- [[MqttPublisherTests]] - degree 24, connects to 4 communities
- [[MqttCredentialsTests]] - degree 13, connects to 4 communities
- [[MqttPublisher.BuildOptions()]] - degree 7, connects to 4 communities