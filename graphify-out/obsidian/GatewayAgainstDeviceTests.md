---
source_file: "tests/ModbusRabbitGateway.Tests/GatewayAgainstDeviceTests.cs"
type: "code"
community: "DevicePoller & Gerätetests"
location: "L11"
tags:
  - graphify/code
  - graphify/EXTRACTED
  - community/DevicePoller__Gerätetests
---

# GatewayAgainstDeviceTests

## Connections
- [[--status-repeat Status alle 30 s wiederholen, sinceUtc unverändert]] - `conceptually_related_to` [EXTRACTED]
- [[DateTime]] - `references` [EXTRACTED]
- [[DevicePoller]] - `references` [INFERRED]
- [[DeviceTimeSource_2]] - `references` [EXTRACTED]
- [[GatewayAgainstDeviceTests.DeviceLost_GivesOfflineStatusWithoutMeasurements_ThenRecovers()]] - `method` [EXTRACTED]
- [[GatewayAgainstDeviceTests.Dispose()]] - `method` [EXTRACTED]
- [[GatewayAgainstDeviceTests.FreePort()]] - `method` [EXTRACTED]
- [[GatewayAgainstDeviceTests.NewPoller()]] - `method` [EXTRACTED]
- [[GatewayAgainstDeviceTests.Poll_ReadsDeviceOverModbus_AndBuildsMessage()]] - `method` [EXTRACTED]
- [[GatewayAgainstDeviceTests.Service_Once_PublishesGatewayStatusDeviceStatusAndMeasurements()]] - `method` [EXTRACTED]
- [[GatewayAgainstDeviceTests.StartServer()]] - `method` [EXTRACTED]
- [[GatewayAgainstDeviceTests.Status_IsRepeatedAfterInterval_WithOriginalSince_ForWildcardSubscribers()]] - `method` [EXTRACTED]
- [[GatewayAgainstDeviceTests.cs]] - `contains` [EXTRACTED]
- [[IDisposable]] - `implements` [EXTRACTED]
- [[ModbusDeviceServer]] - `references` [EXTRACTED]
- [[ModbusDeviceServer_2]] - `references` [EXTRACTED]
- [[ModbusRabbitGateway.Tests (5252 am 2026-11-08)]] - `references` [EXTRACTED]
- [[RecordingPublisher]] - `contains` [EXTRACTED]

#graphify/code #graphify/EXTRACTED #community/DevicePoller__Gerätetests