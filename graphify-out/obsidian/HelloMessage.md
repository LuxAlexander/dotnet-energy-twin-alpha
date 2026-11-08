---
source_file: "src/SimCore.Contracts/SyncMessages.cs"
type: "code"
community: "Sync-Nachrichten"
location: "L22"
tags:
  - graphify/code
  - graphify/EXTRACTED
  - community/Sync-Nachrichten
---

# HelloMessage

## Connections
- [[ClientConnection]] - `references` [EXTRACTED]
- [[SyncMessage]] - `inherits` [EXTRACTED]
- [[SyncMessages.cs]] - `contains` [EXTRACTED]
- [[SyncProtocolTests.Messages()]] - `calls` [INFERRED]
- [[TimeSyncClient.RunConnectionAsync()]] - `calls` [INFERRED]
- [[TimeSyncIntegrationTests.Garbage_DisconnectsOnlyThatClient()]] - `calls` [INFERRED]
- [[TimeSyncServer.ReceiveHelloAsync()]] - `references` [EXTRACTED]
- [[TimeSyncServer.Register()]] - `references` [EXTRACTED]

#graphify/code #graphify/EXTRACTED #community/Sync-Nachrichten