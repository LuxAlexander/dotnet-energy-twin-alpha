---
source_file: "src/SimCore.TimeSync/MessageChannel.cs"
type: "code"
community: "Sync-Nachrichten"
location: "L52"
tags:
  - graphify/code
  - graphify/EXTRACTED
  - community/Sync-Nachrichten
---

# MessageChannel.ReceiveAsync()

## Connections
- [[CancellationToken]] - `references` [EXTRACTED]
- [[MessageChannel]] - `method` [EXTRACTED]
- [[SyncMessage]] - `references` [EXTRACTED]
- [[SyncProtocol.Deserialize()]] - `calls` [EXTRACTED]
- [[Task]] - `references` [EXTRACTED]
- [[TimeSyncClient.ReceiveLoopAsync()]] - `calls` [INFERRED]
- [[TimeSyncServer.HandleClientAsync()]] - `calls` [INFERRED]
- [[TimeSyncServer.ReceiveHelloAsync()]] - `calls` [INFERRED]

#graphify/code #graphify/EXTRACTED #community/Sync-Nachrichten