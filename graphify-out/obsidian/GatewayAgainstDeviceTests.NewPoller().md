---
source_file: "tests/ModbusRabbitGateway.Tests/GatewayAgainstDeviceTests.cs"
type: "code"
community: "DevicePoller & Gerätetests"
location: "L42"
tags:
  - graphify/code
  - graphify/EXTRACTED
  - community/DevicePoller__Gerätetests
---

# GatewayAgainstDeviceTests.NewPoller()

## Connections
- [[DeviceEndpoint]] - `calls` [INFERRED]
- [[DevicePoller]] - `references` [EXTRACTED]
- [[GatewayAgainstDeviceTests]] - `method` [EXTRACTED]
- [[GatewayAgainstDeviceTests.DeviceLost_GivesOfflineStatusWithoutMeasurements_ThenRecovers()]] - `calls` [EXTRACTED]
- [[GatewayAgainstDeviceTests.Poll_ReadsDeviceOverModbus_AndBuildsMessage()]] - `calls` [EXTRACTED]
- [[GatewayAgainstDeviceTests.Service_Once_PublishesGatewayStatusDeviceStatusAndMeasurements()]] - `calls` [EXTRACTED]

#graphify/code #graphify/EXTRACTED #community/DevicePoller__Gerätetests