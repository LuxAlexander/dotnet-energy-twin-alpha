---
source_file: "src/SimCore.TimeSync/TimeSyncServer.cs"
type: "code"
community: "Sync-Nachrichten"
location: "L146"
tags:
  - graphify/code
  - graphify/EXTRACTED
  - community/Sync-Nachrichten
---

# TimeSyncServer.HandleClientAsync()

## Connections
- [[CancellationToken]] - `references` [EXTRACTED]
- [[ClientConnection.MarkClosed()]] - `calls` [INFERRED]
- [[ClientConnection.Report()]] - `calls` [INFERRED]
- [[ClientConnection.Touch()]] - `calls` [INFERRED]
- [[MessageChannel]] - `calls` [INFERRED]
- [[MessageChannel.DisposeAsync()]] - `calls` [INFERRED]
- [[MessageChannel.ReceiveAsync()]] - `calls` [INFERRED]
- [[MessageChannel.SendAsync()]] - `calls` [INFERRED]
- [[PongMessage]] - `calls` [INFERRED]
- [[StatesMessage]] - `calls` [INFERRED]
- [[Task]] - `references` [EXTRACTED]
- [[TcpClient]] - `references` [EXTRACTED]
- [[TimeSyncServer]] - `method` [EXTRACTED]
- [[TimeSyncServer.AcceptLoopAsync()]] - `calls` [EXTRACTED]
- [[TimeSyncServer.IsConnectionError()]] - `calls` [EXTRACTED]
- [[TimeSyncServer.ReceiveHelloAsync()]] - `calls` [EXTRACTED]
- [[TimeSyncServer.Register()]] - `calls` [EXTRACTED]
- [[TimeSyncServer.Report()]] - `calls` [EXTRACTED]

#graphify/code #graphify/EXTRACTED #community/Sync-Nachrichten