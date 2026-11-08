---
source_file: "src/ModbusDiagnosticClient/DiagnosticSession.cs"
type: "code"
community: "Diagnose-Client Verbindung"
location: "L82"
tags:
  - graphify/code
  - graphify/EXTRACTED
  - community/Diagnose-Client_Verbindung
---

# DiagnosticSession.ExecuteAsync()

## Connections
- [[CancellationToken]] - `references` [EXTRACTED]
- [[DiagnosticClientTests.ConnectionLoss_IsReportedAsConnectionError_AndReconnectsWhenDeviceIsBack()]] - `calls` [INFERRED]
- [[DiagnosticClientTests.Execute_ShowsErrorsAsTextInsteadOfThrowing()]] - `calls` [INFERRED]
- [[DiagnosticClientTests.OnOffReset_ByLabelAndShortcut_ReachTheDevice()]] - `calls` [INFERRED]
- [[DiagnosticClientTests.Quit_EndsTheSession()]] - `calls` [INFERRED]
- [[DiagnosticClientTests.Watch_KeepsGoingDuringOutage()]] - `calls` [INFERRED]
- [[DiagnosticSession]] - `method` [EXTRACTED]
- [[DiagnosticSession.ConnectAsync()]] - `calls` [EXTRACTED]
- [[DiagnosticSession.Prefix()]] - `calls` [EXTRACTED]
- [[DiagnosticSession.PrintAllAsync()]] - `calls` [EXTRACTED]
- [[DiagnosticSession.PrintHeaderAsync()]] - `calls` [EXTRACTED]
- [[DiagnosticSession.PrintInfoAsync()]] - `calls` [EXTRACTED]
- [[DiagnosticSession.PrintRawAsync()]] - `calls` [EXTRACTED]
- [[DiagnosticSession.WatchAsync()]] - `calls` [EXTRACTED]
- [[DiagnosticSession.WriteAsync()]] - `calls` [EXTRACTED]
- [[Task]] - `references` [EXTRACTED]

#graphify/code #graphify/EXTRACTED #community/Diagnose-Client_Verbindung