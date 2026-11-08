# Ports, Adressen und Startbefehle

Stand 2026-11-08. Quelle: `README.md` (zentral), `start-all.ps1`, Projekt-READMEs, `docker/docker-compose.yml`.

## Portmatrix

| Port | Dienst | Standard | Bindet | Prüfung |
|---|---|---|---|---|
| 8080 | Wetter (HTTP) | `http://127.0.0.1:8080` | `WeatherSimulator` | `Invoke-RestMethod http://127.0.0.1:8080/locations` |
| 5100 | Zeitverteilung (TCP, JSON-Zeilen) | `127.0.0.1:5100` | `SimulationMaster` | TCP-Verbindung, z. B. `Test-NetConnection 127.0.0.1 -Port 5100` |
| 1502 | Modbus | PV **und** Charger (Charger nur, wenn allein) | `PvSimulator` / `DualPortCharger` | `ModbusDiagnosticClient --port 1502 --once` |
| 1503 | Modbus | Batterie | `BatterySimulator` | `--port 1503 --once` |
| 1504 | Modbus | Charger **in der Demo mit allen Geräten** | `DualPortCharger` | `--port 1504 --once` |
| 1505 | Modbus | Kühlhaus | `KuehlhausSimulator` | `--port 1505 --once` |
| 1883 | MQTT (RabbitMQ-Plugin) | `127.0.0.1:1883` | Docker-Container `rabbitmq` | Gateway-Ausgabe / Viewer |
| 15672 | RabbitMQ-Management-UI | `http://127.0.0.1:15672` | Docker | Browser |
| 5672 | AMQP | – | Docker | **nicht genutzt** (entschieden: MQTT) |

**Kollisionsregel:** PV und Charger haben beide 1502 als Standard. In der Demo bekommt der Charger deshalb **1504**. Zwei Instanzen desselben Geräts brauchen zwei Ports (`--modbus-port`).

## Startbefehle (Repository-Root)

```powershell
# Wetter und Master
dotnet run --project src/WeatherSimulator
dotnet run --project src/SimulationMaster -- --speed 600          # startet pausiert
dotnet run --project src/SimulationMaster -- --speed 600 --autostart

# Feldgeräte
dotnet run --project src/Devices/PvSimulator -- --id PV-01
dotnet run --project src/Devices/BatterySimulator -- --id BAT-01 --modbus-port 1503
dotnet run --project src/Devices/DualPortCharger -- --id DC-01 --modbus-port 1504
dotnet run --project src/Devices/KuehlhausSimulator -- --id KH-01

# Diagnose-Client (Port je Gerät)
dotnet run --project src/ModbusDiagnosticClient -- --port 1504
dotnet run --project src/ModbusDiagnosticClient -- --port 1504 --once
dotnet run --project src/ModbusDiagnosticClient -- --port 1504 --write "Leistungsgrenze=200"

# LB03
docker compose -f docker/docker-compose.yml up -d
docker compose -f docker/docker-compose.yml ps
dotnet run --project src/ModbusRabbitGateway -- --device DC-01=127.0.0.1:1504
dotnet run --project src/ModbusRabbitGateway -- --console --device DC-01=127.0.0.1:1504 --once --json

# ohne Prozessstart prüfen
dotnet run --project src/Devices/KuehlhausSimulator -- --register-plan
dotnet run --project src/ModbusRabbitGateway -- --keys
```

## Standardwerte, die im Protokoll stehen sollten

| Wert | Standard | Wo einstellen |
|---|---|---|
| Master-Tempo | 60 (Master), Geräte-Standard je Gerät (Charger 120) | `--speed` |
| Master-Startzeit | 15.04. des laufenden Jahres, 06:00 | `--start "2026-01-15 06:00"` |
| Toleranz | 10 ms real | `--tolerance` |
| Vorlaufzeit | 200 ms | `--lead` |
| Gateway-Intervall | 1 s | `--interval` |
| Status-Wiederholung | 30 s | `--status-repeat` |
| Unit-ID | 1 | `--unit-id` |
| CSV-Ausgabe | `docs/data/` | `-o` (PV), `--output` |

## Prozess prüfen statt raten

`scripts/pruefe-ports.ps1` prüft alle Ports read-only und zeigt `frei`/`belegt`. Vor **und** nach dem Lauf ausführen.

Ein belegter Port ist nicht automatisch ein Fehler – aber es muss klar sein, **welcher** Prozess ihn hält. `start-all.ps1` startet ein Programm bei belegtem Port **nicht** neu; ohne Vorprüfung testet man dann unbemerkt einen alten Prozess.
