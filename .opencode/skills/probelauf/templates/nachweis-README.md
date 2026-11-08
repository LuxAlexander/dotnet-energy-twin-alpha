# Funktionsnachweis JJJJ-MM-TT: &lt;Thema&gt;

**Durchgeführt von:** &lt;Person&gt; mit &lt;KI/Modell&gt; (Ablauf &lt;skriptgesteuert / handgeführt&gt;, Protokolle unverändert).
**Bezug:** &lt;Aufgabe 02/03, Nachweise 1–7, Akzeptanzkriterien in `docs/SRS-LB03.md`&gt;.

## Aufbau

| Programm | Start | Protokoll |
|---|---|---|
| RabbitMQ (Docker, `docker/docker-compose.yml`) | `docker compose up -d` | – |
| Wettersimulator | `dotnet run --project src/WeatherSimulator` | `wetter.txt` |
| Master | `dotnet run --project src/SimulationMaster -- --speed 600` | `master.txt` |
| &lt;Gerät&gt; | `dotnet run --project src/Devices/… -- --id … --modbus-port …` | `&lt;geraet&gt;.txt` |
| Gateway | `dotnet run --project src/ModbusRabbitGateway -- --device …` | `gateway.txt` |
| Viewer | &lt;MQTT Explorer oder Test-Empfänger&gt; | `viewer.txt` |
| Diagnose-Client | `dotnet run --project src/ModbusDiagnosticClient -- --port …` | `diagnose.txt` |
| Ablauf mit Zeitmarken | – | `ablauf.txt` |

**Besonderheiten des Laufs:** &lt;z. B. Skript-Master statt Tastenbedienung, nur ein Gerät, Broker von Hand gestartet&gt;.
**Zugangsdaten:** nur über Umgebungsvariablen bzw. `docker/.env`; in den Protokollen steht kein Passwort.

## Ergebnis je Nachweis

| # | Nachweis | Ergebnis | Beleg |
|---|---|---|---|
| 1 | &lt;…&gt; | ✅ / ⚠️ / ❌ | `&lt;datei&gt;.txt` |
| 2 | &lt;…&gt; |  |  |

## Beobachtungen und Einschränkungen

- &lt;ehrlich, auch unerklärte Einmal-Effekte und zurückgesetzte Zustände&gt;

## Nicht geprüft

- &lt;z. B. echte Terminalanzeige, Tastenbedienung, Screenshots, Strg+C&gt;
