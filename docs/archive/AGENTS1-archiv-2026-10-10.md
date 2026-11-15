> **OBSOLET (2026-10-26):** Dieses Dokument ist der Planungsstand vom 2026-10-10. Sein Inhalt ist
> vollständig in AGENTS.md (Repository-Hauptordner) überführt und fortgeschrieben. Es dient nur
> noch als historische Quelle und ist **keine Anweisung**; maßgeblich ist ausschließlich AGENTS.md.

# Archiv: LB02-Planungsdokument (dotnet-energy-sim-lb02, 2026-10-10)

**Last updated:** 2026-10-10  
**Phase:** Design & planning (no code yet)  
**Owner:** Alexander Lux (Team from LB01)

This file is mandatory context for ANY agent working on this project. Read it fully before touching
code. It captures what we know, what we've decided, and what we haven't yet agreed on.

---

## 1. Project Identity

| Field | Value |
|---|---|
| Assignment | .NET-Programmierung 2026, LB02 ("Vom einzelnen Simulator zum verteilten Teamsystem") |
| Team size | 3 (same team as LB01) |
| Target framework | .NET 10 (`net10.0`) |
| Repo | TBD — likely `dotnet-energy-sim-lb02` on its-git.fh-salzburg.ac.at or GitHub |
| Submission | Git tag `lb02-2026` on Moodle |

### Core constraints (from PDF — never violated):
- **No internet at runtime** — weather is deterministic, computed locally, no API calls.
- **No secrets in repo** — use `.env.example`, never commit `.env`.
- **async/await everywhere** — no `Thread.Sleep`, no `.Wait()`/`.Result`, `CancellationToken` on long ops, `Async` suffix on async methods.
- **Synchronous I/O from EasyModbusTCP.NETCore is a library boundary** — do NOT wrap in `Task.Run` just to fake async.
- **0 errors / 0 warnings at build** (same rule as LB01).
- Every project gets its own `README.md`; a central `README.md` describes the whole system.
- Development journals must be continued (`docs/development-journal/<name>.md`).

---

## 2. LB01 Codebase — What We're Building On

The existing project `dotnet-energy-sim-lb01` (at `C:\Users\Alex\Documents\GitHub\AI_Test/dotnet-energy-sim-lb01/`) is a single console app. Key components that are relevant for LB02:

### Current structure
```
src/EnergySimulator/
├── Program.cs                    # CLI parsing (--timescale, --start, --peak, etc.)
├── Simulation/
│   ├── SimulationClock.cs        # DateTime-based virtual clock, Timescale, unlimited runtime
│   ├── SimulationEngine.cs       # Tick loop (~80ms), keyboard input, auto-weather, CSV, render
│   ├── SimulationConfig.cs       # All config params
│   ├── SimulationMode.cs         # Auto / Manual / Paused
│   └── Season.cs                 # Month → Season, irradiation factors, sunrise/sunset, temp, snow
├── Devices/
│   ├── IDevice.cs                # Interface: Name, IsEnabled, PowerKw, StepContinuous, ResetDailyEnergy
│   ├── PvSystem.cs               # PV device: power from irradiation, daily/total energy
│   ├── PvFaultModel.cs           # Temperature, grid voltage, soiling, aging, fault states (70°C overheat, 200-250V grid)
│   ├── IrradiationCalculator.cs  # Half-sine day curve × weather factor × season factor
│   ├── WeatherCondition.cs       # Sunny, PartlyCloudy, Overcast, Storm, Snow, Night
│   ├── FaultType.cs              # None, Inverter, Overheat, GridVoltage
│   └── InverterStatus.cs         # UNUSED — enum Ok/Fault
├── Output/
│   ├── ConsoleDisplay.cs         # Diff-based console rendering (76-char wide boxes, ANSI colors)
│   └── BarChart.cs               # Bar splitting utility
└── Data/
    └── CsvExporter.cs            # CSV export with semicolon delimiter
```

### Key takeaways from LB01 for LB02:
- **Weather is currently calculated IN the PV simulator** (`PvSystem` has `Weather` property, `SimulationEngine.UpdateAutoWeather()` generates random weather each run). This must move to the **WeatherSimulator** in LB02.
- **`SimulationClock`** is the virtual time — this becomes distributed. The Master owns it; devices receive updates.
- **Weather model**: deterministic per-day (via `DayRandom` seeded from date), with random transitions during the day. The `DailyOffsetC` pattern (seeded from date) is a good model for reproducible daily weather.
- **Season model**: `SeasonInfo` provides sunrise/sunset hours, ambient temperature, irradiation factor, snow probability. This stays as-is — it's deterministic, season-based, no internet needed.
- **Fault model constants**: `OverheatTripC = 70.0`, `OverheatResetC = 55.0`, `GridMinVolt = 200.0`, `GridMaxVolt = 250.0`, `MaxSoilingPercent = 25.0`, `MaxAgingPercent = 15.0`.
- **Console display** renders a 76-char-wide box. The `InnerWidth = 70`, `BorderWidth = 72` convention must be preserved (HANDOFF.md §4.1).
- **`InverterStatus.cs` is dead code** — not used anywhere in LB01. Flag for removal or reuse.
- **No solution file at repo root** — the `.sln` is at `dotnet-energy-sim-lb01.sln` (not in `src/`). Need to verify.
- **No NuGet packages currently** — LB02 adds `EasyModbusTCP.NETCore`.

### Build & run (from HANDOFF.md):
```powershell
dotnet build .\dotnet-energy-sim-lb01.sln -v minimal
dotnet run --project src/EnergySimulator -- -s 3600
```

---

## 3. LB02 Requirements — Summary

### Mandatory components (4+ programs):
1. **Simulations-Master** — controls virtual time for the whole system (restart, pause, continue, speed change, status display)
2. **Wettersimulator** — independent program; serves weather for (location, virtual-time); deterministic/reproducible
3. **Feldgeräte-Simulatoren** — at least 3 (PV, Battery, Dual-Port Charger) + 1 more (Wind OR industrial consumer)
4. **Modbus-Diagnose-Client** — reads common register head + device-specific values, writes a setpoint, shows errors

### Mandatory behaviors (must be demonstrable):
1. All simulators started as separate programs → virtual times agree within tolerance
2. Speed change during operation without desync
3. Global pause → Modbus servers stay accessible, registers show last state
4. Global resume → no time jump
5. Master-triggered restart with new virtual time → all adopt
6. Same (location, time) → same weather for all requesting devices
7. Diagnostic client reads common header + device registers
8. Diagnostic client writes a setpoint → change visible in simulator
9. Controlled shutdown of all programs

### Mandatory error handling:
- Simulator/Master temporarily unreachable → visible error, no silent drift
- WeatherSim gives invalid/no response → visible error
- Modbus client connection failure/disconnect → handled
- Invalid/unauthorized write value → rejected with error
- User-initiated program termination → controlled shutdown

### Mandatory tests:
- Simulation-Master time control (at least pause/resume/speed/restart)
- Weather reproducibility (same inputs → same output)

### Mandatory documentation:
- Central `README.md` (system overview, architecture, start order, all modules)
- Per-project `README.md` (purpose, owner, config, usage, interfaces, tests, known issues)
- Per-device **register plan** (host, port, unit-ID, device-ID, register address, type, data type, scaling, permissions, valid range, meaning)
- `AGENTS.md` (this file — dev conventions, build, conventions)
- `.env.example` or sample config
- `docs/development-journal/<name>.md` (per team member)
- Ownership assignment documented in central `README.md`

---

## 4. Architecture Decisions (from Grilling Round 1)

### Q1 — Repository structure: **NEW REPO** `dotnet-energy-sim-lb02`
- Copy/adapt LB01 source files; no git history carry-over needed.
- Folder: `C:\Users\Alex\Documents\GitHub\AI_Test/dotnet-energy-sim-lb02/` (created).
- **NOT DECIDED**: Whether to import LB01 git history or start fresh. For now: start fresh.

### Q2 — Shared assemblies: **4 libraries** (no circular deps)
```
SimCore.Contracts   → Interfaces + DTOs (ITimeProvider, IWeatherService, WeatherData, TimeState, DeviceConfig, etc.)
  ↑ (no deps)
SimCore.Time        → VirtualTime, TimeControlCmd, TimeState extensions
  ↑ deps: SimCore.Contracts
Modbus.Contracts    → Register addresses, RegisterMap, DeviceId, RegisterPlan
  ↑ deps: SimCore.Contracts
Devices.*           → Device implementations (PvSimulator, WindSimulator, etc.)
  ↑ deps: all above + EasyModbusTCP.NETCore
```
- Contracts has ZERO external dependencies (pure interfaces + DTOs).
- All shared types use `net10.0` class libraries.

### Q3 — Time distribution: **TCP push model**
- **NOT FULLY DECIDED**: Transport (TCP vs HTTP vs gRPC). Current lean: TCP push.
  - Master = TCP server; devices = TCP clients (subscribe to time updates).
  - Master sends `TimeState` messages on state changes + heartbeat.
  - Drif tolerance: ≤ 250ms sim-time per update cycle (TBD).
  - Unreachable devices: marked offline, other devices continue, master reports.

### Q4 — Weather distribution: **Deterministic computation, request-response**
- Weather is a pure function of `(location, virtual-date-hour)`.
- No real-time randomness — seed from day-index + location (like LB01's `DailyOffsetC`).
- WeatherSim exposes async API (HTTP or TCP) returning `WeatherData { IrradiationWm2, SunHeight, CloudCover, TemperatureC, WindSpeedMs, Condition }`.
- **NOT DECIDED**: Transport (HTTP REST vs TCP vs gRPC). Current lean: HTTP (simplest, EasyModbus already uses TCP so no conflict).
- WeatherSim depends on the same `Season`/`IrradiationCalculator` logic from LB01 as the base, extended with wind speed.

### Q5 — Devices: **Wind turbine as 4th device**
- Mandatory: PV-Anlage, Batteriespeicher, Dual-Port-Charger (3-person team).
- 4th: Windkraftanlage (more dynamic, adds WindSpeed weather dependency).
- Ownership split (3 team members):
  - **Person A (Alexander)**: Simulation Master + `SimCore.Time` + `SimCore.Contracts`
  - **Person B**: Weather Simulator + shared `SimCore.Contracts` contracts
  - **Person C**: All 4 field devices + Modbus diagnostic client + `Modbus.Contracts`

### Q6 — Async architecture: **Task.Delay + CancellationToken**
- Tick loop becomes `async Task StartAsync(CancellationToken ct)` with `await Task.Delay(80, ct)`.
- Keyboard input: `Task.Run(() => Console.ReadKey(true))` as async task, signals master via cancellation/command.
- Network I/O (time subscription, weather queries): `async`/`await` with `HttpClient`/`TcpClient` async methods.
- **EasyModbusTCP.NETCore**: synchronous API is accepted as library boundary — no `Task.Run` wrapping.
- **BLOCKING calls forbidden**: `Thread.Sleep`, `HttpClient.Get().Result`, etc.

### Q7 — Modbus integration: **Each device hosts its own Server**
- Each device exe creates `EasyModbus.ModbusServer` on its configured port (start ≥ 1502).
- Register model: `HoldingRegisters` (ushort[]), common head = registers 0-9, device-specific = 10-29, writable setpoint at a designated address.
- Multi-word values (double): 2 registers, IEEE 754 float, big-endian word order.
- Server stays accessible during simulation pause (registers retain last values).
- **NOT FULLY DECIDED**: Exact register map per device — needs detailed design.

---

## 5. Open Questions — NEED TEAM DECISIONS

These are unresolved and must be agreed before/early in implementation:

| # | Question | Options | Lean |
|---|---|---|---|
| 1 | WeatherSim transport | HTTP REST, TCP, gRPC, WebSocket | HTTP (simplest, clear contract) |
| 2 | Time distribution transport | TCP push, HTTP polling, gRPC stream, WebSocket | TCP push (low latency, same domain) |
| 3 | Exact drift tolerance | 100ms, 250ms, 500ms? | 250ms sim-time |
| 4 | Device register plan addresses | Custom vs standard mapping? | Registers 0-9 common, 10-29 device-specific |
| 5 | How devices discover Master | Config-file IP:port vs DNS/name vs broadcast? | Config file (most reliable, no discovery complexity) |
| 6 | WeatherSim location model | Single global location vs per-device location? | Single global location (simpler, PDF says "derselbe Ort") |
| 7 | Battery model complexity | Simple SoC + charge/discharge or detailed chemistry? | Simple SoC model (charge/discharge efficiency, power limits) |
| 8 | Dual-Port Charger model | What does it charge from/to? | Charges battery from PV surplus (configurable priorities) |

---

## 6. Planned Project Structure

```
dotnet-energy-sim-lb02/
├── src/
│   ├── SimCore.Contracts/        # Interfaces + DTOs (no deps)
│   ├── SimCore.Time/             # Virtual time primitives
│   ├── Modbus.Contracts/         # Register addresses, RegisterMap
│   ├── SimulationMaster/         # TCP server, time engine (exe)
│   ├── WeatherSimulator/         # Deterministic weather (exe)
│   ├── Devices/
│   │   ├── PvSimulator/          # PV device (exe) — adapted from LB01
│   │   ├── WindSimulator/        # Wind turbine (exe) — new
│   │   ├── BatterySimulator/     # Battery storage (exe) — new
│   │   └── DualPortCharger/      # Charger (exe) — new
│   ├── ModbusDiagnosticClient/   # Diagnostic client (exe)
│   └── Shared/
│       └── DeviceConfig/         # Config loading (JSON + env vars)
├── tests/
│   ├── SimCore.Tests/            # Master time control tests
│   └── Weather.Tests/            # Weather reproducibility tests
├── data/                         # Weather data files (season tables, etc.)
├── docs/
│   ├── development-journal/
│   │   ├── alexander-lux.md      # CONTINUE journal from LB01
│   │   ├── person-b.md
│   │   └── person-c.md
│   └── register-plans/           # Per-device register documentation
├── dotnet-energy-sim-lb02.sln   # Root solution
├── AGENTS.md                     # This file
├── README.md                     # Central system README (TBD)
└── .env.example                  # Sample environment variables (TBD)
```

---

## 7. Key Decisions Log

| Date | Decision | Rationale |
|---|---|---|
| 2026-10-10 | Start new repo, don't migrate git history | LB02 is a separate team project; clean separation from LB01 submission |
| 2026-10-10 | 4 shared assemblies (Contracts, Time, Modbus, Devices-common) | Follows single-responsibility, avoids circular deps, matches PDF "sinnvoll geschnittene Assemblies" |
| 2026-10-10 | Wind turbine as 4th device | More dynamic than industrial consumer; adds WindSpeed weather dimension |
| 2026-10-10 | Async/await with Task.Delay + CancellationToken | PDF mandates TAP; existing Thread.Sleep must be replaced |

---

## 8. Gotchas & Pitfalls (from LB01 experience)

1. **Redirected console ≠ real terminal** — All screen features (cursor positioning, buffer height) need fallback paths. Tests in CI/pipe → WriteAll fallback. Always test both.
2. **`InverterStatus.cs` is dead code** — either wire it up or remove it.
3. **Frame width must be 76 chars exactly** — `InnerWidth = 70`, `BorderWidth = 72`, `Row()` adds 2 padding. Don't change without testing.
4. **Console crashes silently** — NullRef in first-frame diff rendering → ExitCode -1073741819. Test in real terminal window, not just redirect.
5. **CSV Flush interval** — flush every 5 rows (from LB01). Adjust if needed but keep incremental.
6. **Weather determinism** — use date-seeded `Random` for reproducible daily weather, not time-seeded.
7. **No `Thread.Sleep` in async loops** — use `await Task.Delay(ms, ct)` instead.
8. **EasyModbus is synchronous** — this is explicitly allowed as a "begründete Bibliotheksgrenze". Don't `Task.Run` it.
9. **Environment variables for dev-time opt-in features only** — e.g., `ENERGYSIM_RENDER_LOG` pattern is good.

---

## 9. Build & Verification Commands (to be filled in)

```powershell
# Build (TBD — will populate after solution created)
dotnet build .\dotnet-energy-sim-lb02.sln -v minimal

# Run master + all devices (TBD)
# ...

# Tests (TBD)
dotnet test .\tests/SimCore.Tests
dotnet test .\tests/Weather.Tests
```

---

## 10. Next Steps

1. **Team decisions needed** on open questions in §5 (especially #1 #2 #3 #5).
2. **Create the solution** with shared assemblies first.
3. **Extract LB01 `Season` + `IrradiationCalculator`** into `SimCore.Contracts` (or a shared `Devices.Common`).
4. **Build SimulationMaster** first — it's the spine of the system.
5. **Build WeatherSimulator** next — devices depend on it.
6. **Adapt PV simulator** from LB01 into the distributed model.
7. **Build Modbus diagnostic client** + new devices.
8. **Write tests** alongside each component.
9. **Write documentation** incrementally (per-project READMEs, register plans).

---

*This AGENTS.md is a living document. Update it with every major decision. Append a journal entry in `docs/development-journal/`.*