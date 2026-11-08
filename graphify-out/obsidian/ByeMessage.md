---
source_file: "src/SimCore.Contracts/SyncMessages.cs"
type: "code"
community: "Sync-Nachrichten"
location: "L43"
tags:
  - graphify/code
  - graphify/INFERRED
  - community/Sync-Nachrichten
---

# ByeMessage

## Connections
- [[SyncMessage]] - `inherits` [EXTRACTED]
- [[SyncMessages.cs]] - `contains` [EXTRACTED]
- [[SyncProtocolTests.Messages()]] - `calls` [INFERRED]
- [[TimeSyncClient.SendByeAsync()]] - `calls` [INFERRED]
- [[TimeSyncIntegrationTests.InvalidClient_IsRejected_WithVisibleMessage()]] - `references` [EXTRACTED]
- [[TimeSyncServer.ReceiveHelloAsync()]] - `calls` [INFERRED]
- [[TimeSyncServer.RunAsync()]] - `calls` [INFERRED]

#graphify/code #graphify/INFERRED #community/Sync-Nachrichten