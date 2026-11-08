---
source_file: "src/SimulationMaster/Program.cs"
type: "code"
community: "Sync-Nachrichten"
location: "L9"
tags:
  - graphify/code
  - graphify/INFERRED
  - community/Sync-Nachrichten
---

# Program.Main()

## Connections
- [[MasterClock]] - `calls` [INFERRED]
- [[MasterClock.Resume()]] - `calls` [INFERRED]
- [[MasterConsole]] - `calls` [INFERRED]
- [[MasterConsole.AddMessage()]] - `calls` [INFERRED]
- [[MasterConsole.RunAsync()]] - `calls` [INFERRED]
- [[MasterConsole.ShutdownAsync()]] - `calls` [INFERRED]
- [[MasterOptions.Parse()]] - `calls` [EXTRACTED]
- [[Program]] - `method` [EXTRACTED]
- [[Task]] - `references` [EXTRACTED]
- [[TimeSyncServer]] - `calls` [INFERRED]
- [[TimeSyncServer.RunAsync()]] - `calls` [INFERRED]
- [[TimeSyncServer.Start()]] - `calls` [INFERRED]

#graphify/code #graphify/INFERRED #community/Sync-Nachrichten