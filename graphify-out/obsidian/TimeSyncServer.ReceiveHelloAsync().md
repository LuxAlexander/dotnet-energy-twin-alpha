---
source_file: "src/SimCore.TimeSync/TimeSyncServer.cs"
type: "code"
community: "Sync-Nachrichten"
location: "L196"
tags:
  - graphify/code
  - graphify/EXTRACTED
  - community/Sync-Nachrichten
---

# TimeSyncServer.ReceiveHelloAsync()

## Connections
- [[ByeMessage]] - `calls` [INFERRED]
- [[CancellationToken]] - `references` [EXTRACTED]
- [[HelloMessage]] - `references` [EXTRACTED]
- [[MessageChannel]] - `references` [EXTRACTED]
- [[MessageChannel.ReceiveAsync()]] - `calls` [INFERRED]
- [[MessageChannel.SendAsync()]] - `calls` [INFERRED]
- [[Task]] - `references` [EXTRACTED]
- [[TimeSyncServer]] - `method` [EXTRACTED]
- [[TimeSyncServer.HandleClientAsync()]] - `calls` [EXTRACTED]
- [[TimeSyncServer.Report()]] - `calls` [EXTRACTED]

#graphify/code #graphify/EXTRACTED #community/Sync-Nachrichten