---
source_file: "src/ModbusRabbitGateway/DevicePoller.cs"
type: "code"
community: "DevicePoller & Gerätetests"
location: "L45"
tags:
  - graphify/code
  - graphify/EXTRACTED
  - community/DevicePoller__Gerätetests
---

# DevicePoller.Poll()

## Connections
- [[DateTimeOffset]] - `references` [EXTRACTED]
- [[DeviceConnection.ReadRegisters()]] - `calls` [INFERRED]
- [[DevicePoller]] - `method` [EXTRACTED]
- [[DevicePoller.Status()]] - `calls` [EXTRACTED]
- [[GatewayAgainstDeviceTests.DeviceLost_GivesOfflineStatusWithoutMeasurements_ThenRecovers()]] - `calls` [INFERRED]
- [[GatewayAgainstDeviceTests.Poll_ReadsDeviceOverModbus_AndBuildsMessage()]] - `calls` [INFERRED]
- [[GatewayAgainstDeviceTests.Status_IsRepeatedAfterInterval_WithOriginalSince_ForWildcardSubscribers()]] - `calls` [INFERRED]
- [[MessageBuilder.Build()]] - `calls` [EXTRACTED]
- [[PollOutcome]] - `references` [EXTRACTED]

#graphify/code #graphify/EXTRACTED #community/DevicePoller__Gerätetests