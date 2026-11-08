# Funktionsnachweis 2026-11-08: Ladesäule → Modbus → Gateway → RabbitMQ → Viewer, mit Master

**Durchgeführt von:** Susanne Lastowicka mit Claude Opus 5.5 (Ablauf skriptgesteuert, Protokolle unverändert).
**Bezug:** Aufgabe 03, „Nachweis der Funktion“ Punkte 1–7; Akzeptanzkriterien in [`docs/SRS-LB03.md`](../../SRS-LB03.md).

## Aufbau

| Programm | Start | Protokoll |
|---|---|---|
| RabbitMQ 4.4.0 (Docker, `docker/docker-compose.yml`, MQTT-Plugin) | vorher von Hand gestartet | – |
| Wettersimulator | `dotnet run --project src/WeatherSimulator` | `wetter.txt` |
| Master (**Skript**, siehe unten) | Tempo 60 (1 Modellminute je Sekunde), Start 2026-01-15 06:00 | `master.txt` |
| Ladesäule DC-01 (mit Master, Modbus 1504) | `dotnet run --project src/Devices/DualPortCharger -- --id DC-01 --modbus-port 1504` | `ladesaeule.txt` |
| Gateway | `dotnet run --project src/ModbusRabbitGateway -- --device DC-01=127.0.0.1:1504` | `gateway.txt` |
| Viewer (Test-Empfänger, MQTTnet, Abo `energytwin/#`, eine Zeile je Nachricht) | – | `viewer.txt` |
| Diagnose-Client | `--port 1504 --write "Leistungsgrenze=…"` | `diagnose.txt` |
| Ablauf mit Zeitmarken | – | `ablauf.txt` |

**Skript-Master:** Der echte `SimulationMaster` wird über Tasten bedient und braucht ein Terminal; in einem
automatisierten Lauf geht das nicht. Der Skript-Master verwendet dieselben Klassen (`MasterClock`, `TimeSyncServer`)
und pausiert/setzt fort nach festem Zeitplan. Die Tastenbedienung des echten Masters ist separat live/per Screenshot zu zeigen.
Zugangsdaten zum Broker wurden nur als Umgebungsvariablen für den Lauf gesetzt; in den Protokollen steht kein Passwort.

## Ergebnis je Nachweis

| # | Nachweis (Aufgabe 03) | Ergebnis | Beleg |
|---|---|---|---|
| 1 | RabbitMQ aus der versionierten Compose-Konfiguration | ✅ Container `rabbitmq` lief (RabbitMQ 4.4.0) | `ablauf.txt` (letzte Zeile) |
| 2 | TimeSim, WeatherSim und Gerät gestartet | ✅ Ladesäule „synchron mit Master“, nur DC-01 (PV, Batterie, Kühlhaus nicht Teil dieses Laufs) | `ladesaeule.txt`, `master.txt` |
| 3 | Gateway und Viewer, Nachrichten erkennbar | ✅ Messwerte jede Sekunde mit `simTime`, `seq`, `publishSeq`, Qualität | `viewer.txt` ab 15:16:01 |
| 4 | Sollwert ändern: Simulator, Modbus, Nachricht | ✅ 200 kW angenommen („Gerät zeigt jetzt 200 kW“), ab Nachricht `nr=5` `leistungsgrenze=200`; 999 kW vom Client abgelehnt | `diagnose.txt`, `viewer.txt` 15:16:05 |
| 5 | Pause/Fortsetzen, richtige virtuelle Zeit; Veröffentlichung in der Pause | ✅ Pause: `simTime` bleibt `06:11:22`, weiter veröffentlicht mit `good/paused` (nr 13–22); Fortsetzen ohne Sprung mit `06:16:22` | `viewer.txt` 15:16:13–15:16:23, `master.txt` |
| 6a | Geräteausfall und Wiederanlauf | ✅ `DC-01 OFFLINE` (keine Messwerte), nach Neustart `ONLINE`, wieder synchron mit dem Master | `gateway.txt`, `viewer.txt` 15:16:28–33 |
| 6b | Broker-Ausfall und Wiederanlauf | ✅ einmal `BROKER NICHT ERREICHBAR`, Gerät weiter gelesen, 10 Nachrichten verworfen, `BROKER WIEDER VERBUNDEN`, Status nachgeholt | `gateway.txt` Zeilen 51–62 |
| 7 | Kontrolliertes Beenden | ⚠️ teilweise: Master beendet geordnet (`bye`), danach meldet das Gateway korrekt `bad/masterLost`. Gateway, Ladesäule und Wetter wurden am Ende per `taskkill` beendet (Strg+C/`Q` brauchen ein Terminal); das geordnete Beenden des Gateways ist mit `--once` und in den Tests belegt | `master.txt`, `gateway.txt` (Ende) |

## Beobachtungen und Einschränkungen (ehrlich festgehalten)

- **Neustart der Ladesäule setzt die Leistungsgrenze zurück** (150 kW, Standardwert): Der Sollwert lebt nur im laufenden
  Prozess. Gewolltes Verhalten, aber in der Vorführung erwähnen.
- **999 kW** wurde schon im Diagnose-Client abgelehnt (vor dem Senden). Die Ablehnung *im Gerät* (Register 18 = 2)
  zeigt `--write` mit `force`; das wurde hier nicht vorgeführt.
- **Einmal ging ein Sollwert verloren:** In einem früheren Durchlauf am selben Tag meldete der Diagnose-Client
  „angenommen – Gerät zeigt noch 150 kW“, die Leistungsgrenze blieb bei 150. In anschließend 410 gezielten Versuchen
  (mit/ohne Master, Gateway, Wetter, direkt nach dem Start) trat das nicht mehr auf; die Ursache ist nicht geklärt
  (Verdacht: gleichzeitige Zugriffe im Modbus-Server der Bibliothek). Der Fehler war für die Bedienung sichtbar, nicht still.
- **Zwei frühere Durchläufe sind ungültig** und nicht in diesem Ordner: einer, weil der Skript-Master wegen deaktivierter
  JSON-Reflexion in .NET-Dateiprogrammen abstürzte (Gateway meldete korrekt `bad/noTime`), einer, weil ein alter
  Skript-Master Port 5100 belegte.
