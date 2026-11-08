---
source_file: "src/SimCore.TimeSync/MessageChannel.cs"
type: "code"
community: "Sync-Nachrichten (2)"
location: "L35"
tags:
  - graphify/code
  - graphify/EXTRACTED
  - community/Sync-Nachrichten_2
---

# MessageChannel.SendAsync()

## Connections
- [[CancellationToken]] - `references` [EXTRACTED]
- [[MessageChannel]] - `method` [EXTRACTED]
- [[SyncMessage]] - `references` [EXTRACTED]
- [[SyncProtocol.Serialize()]] - `calls` [EXTRACTED]
- [[Task]] - `references` [EXTRACTED]
- [[TimeSyncClient.RunConnectionAsync()]] - `calls` [INFERRED]
- [[TimeSyncClient.SendByeAsync()]] - `calls` [INFERRED]
- [[TimeSyncClient.SendPingAsync()]] - `calls` [INFERRED]
- [[TimeSyncServer.HandleClientAsync()]] - `calls` [INFERRED]
- [[TimeSyncServer.ReceiveHelloAsync()]] - `calls` [INFERRED]

#graphify/code #graphify/EXTRACTED #community/Sync-Nachrichten_2