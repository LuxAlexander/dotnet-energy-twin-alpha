---
source_file: "src/SimCore.TimeSync/MessageChannel.cs"
type: "code"
community: "Sync-Nachrichten"
location: "L11"
tags:
  - graphify/code
  - graphify/EXTRACTED
  - community/Sync-Nachrichten
---

# MessageChannel

## Connections
- [[ClientConnection_1]] - `references` [EXTRACTED]
- [[IAsyncDisposable]] - `implements` [EXTRACTED]
- [[MessageChannel.DisposeAsync()]] - `method` [EXTRACTED]
- [[MessageChannel.ReceiveAsync()]] - `method` [EXTRACTED]
- [[MessageChannel.SendAsync()]] - `method` [EXTRACTED]
- [[MessageChannel.cs]] - `contains` [EXTRACTED]
- [[RemoteEndPoint]] - `defines` [EXTRACTED]
- [[SemaphoreSlim]] - `references` [EXTRACTED]
- [[StreamReader]] - `references` [EXTRACTED]
- [[StreamWriter]] - `references` [EXTRACTED]
- [[TcpClient]] - `references` [EXTRACTED]
- [[TimeSyncClient.PingLoopAsync()]] - `references` [EXTRACTED]
- [[TimeSyncClient.ReceiveLoopAsync()]] - `references` [EXTRACTED]
- [[TimeSyncClient.RunConnectionAsync()]] - `calls` [INFERRED]
- [[TimeSyncClient.SendByeAsync()]] - `references` [EXTRACTED]
- [[TimeSyncClient.SendPingAsync()]] - `references` [EXTRACTED]
- [[TimeSyncServer.HandleClientAsync()]] - `calls` [INFERRED]
- [[TimeSyncServer.ReceiveHelloAsync()]] - `references` [EXTRACTED]
- [[TimeSyncServer.Register()]] - `references` [EXTRACTED]
- [[UTF8Encoding]] - `references` [EXTRACTED]

#graphify/code #graphify/EXTRACTED #community/Sync-Nachrichten