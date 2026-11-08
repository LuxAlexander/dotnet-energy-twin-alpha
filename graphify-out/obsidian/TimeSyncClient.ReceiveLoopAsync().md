---
source_file: "src/SimCore.TimeSync/TimeSyncClient.cs"
type: "code"
community: "Sync-Nachrichten (2)"
location: "L118"
tags:
  - graphify/code
  - graphify/EXTRACTED
  - community/Sync-Nachrichten_2
---

# TimeSyncClient.ReceiveLoopAsync()

## Connections
- [[CancellationToken]] - `references` [EXTRACTED]
- [[MessageChannel]] - `references` [EXTRACTED]
- [[MessageChannel.ReceiveAsync()]] - `calls` [INFERRED]
- [[SyncedClock.AddSyncSample()]] - `calls` [INFERRED]
- [[Task]] - `references` [EXTRACTED]
- [[TimeSyncClient]] - `method` [EXTRACTED]
- [[TimeSyncClient.ApplyStates()]] - `calls` [EXTRACTED]
- [[TimeSyncClient.Report()]] - `calls` [EXTRACTED]
- [[TimeSyncClient.RunConnectionAsync()]] - `calls` [EXTRACTED]

#graphify/code #graphify/EXTRACTED #community/Sync-Nachrichten_2