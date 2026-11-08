---
source_file: "src/ModbusDiagnosticClient/DeviceConnection.cs"
type: "code"
community: "Diagnose-Client Verbindung"
location: "L21"
tags:
  - graphify/code
  - graphify/EXTRACTED
  - community/Diagnose-Client_Verbindung
---

# DiagnosticException

## Connections
- [[DeviceConnection.Connect()]] - `calls` [EXTRACTED]
- [[DeviceConnection.Translate()]] - `calls` [EXTRACTED]
- [[DeviceConnection.cs]] - `contains` [EXTRACTED]
- [[DiagnosticClientTests.ConnectionLoss_IsReportedAsConnectionError_AndReconnectsWhenDeviceIsBack()]] - `references` [EXTRACTED]
- [[DiagnosticClientTests.Write_AmbiguousOrUnknownTarget_ThrowsValueError()]] - `references` [EXTRACTED]
- [[DiagnosticClientTests.Write_InvalidValue_IsRefusedByClientBeforeSending()]] - `references` [EXTRACTED]
- [[DiagnosticClientTests.Write_ReadOnlyWithoutForce_IsRefusedByClient()]] - `references` [EXTRACTED]
- [[DiagnosticErrorKind]] - `references` [EXTRACTED]
- [[DiagnosticSession.ConnectAsync()]] - `calls` [INFERRED]
- [[DiagnosticSession.ExecuteAsync()]] - `calls` [INFERRED]
- [[DiagnosticSession.ParseValue()]] - `calls` [INFERRED]
- [[DiagnosticSession.PrintRawAsync()]] - `calls` [INFERRED]
- [[DiagnosticSession.ResolveTarget()]] - `calls` [INFERRED]
- [[DiagnosticSession.WatchAsync()]] - `calls` [INFERRED]
- [[DiagnosticSession.WriteAsync()]] - `calls` [INFERRED]
- [[Exception]] - `inherits` [EXTRACTED]
- [[Kind]] - `defines` [EXTRACTED]

#graphify/code #graphify/EXTRACTED #community/Diagnose-Client_Verbindung