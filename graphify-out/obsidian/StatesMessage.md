---
source_file: "src/SimCore.Contracts/SyncMessages.cs"
type: "code"
community: "Sync-Nachrichten"
location: "L28"
tags:
  - graphify/code
  - graphify/EXTRACTED
  - community/Sync-Nachrichten
---

# StatesMessage

## Connections
- [[IReadOnlyList]] - `references` [EXTRACTED]
- [[SyncMessage]] - `inherits` [EXTRACTED]
- [[SyncMessages.cs]] - `contains` [EXTRACTED]
- [[SyncProtocolTests.Message_IsReadableJson()]] - `calls` [INFERRED]
- [[SyncProtocolTests.Messages()]] - `calls` [INFERRED]
- [[TimeState]] - `references` [EXTRACTED]
- [[TimeSyncClient.ApplyStates()]] - `references` [EXTRACTED]
- [[TimeSyncServer.BroadcastLoopAsync()]] - `calls` [INFERRED]
- [[TimeSyncServer.HandleClientAsync()]] - `calls` [INFERRED]
- [[TimeSyncServer.HeartbeatLoopAsync()]] - `calls` [INFERRED]

#graphify/code #graphify/EXTRACTED #community/Sync-Nachrichten