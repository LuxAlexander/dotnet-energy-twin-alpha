# Fehlerbilder beim Probelauf

Symptom → Ursache → Abhilfe. Alle Fälle sind im Projekt schon einmal aufgetreten (Quellen: `AGENTS.md` §0.6, Geräte-READMEs,
`docs/devices/KuehlhausSimulator/umsetzung-lb02.md`, `docs/nachweise/2026-11-08-gateway-mit-master/README.md`).

| # | Symptom | Ursache | Abhilfe |
|---|---|---|---|
| 1 | Programm endet sofort mit `IOException`, „Port belegt“ | `ModbusDeviceServer.Start()` prüft den Port vorab; die Bibliothek würde sonst den ganzen Prozess beenden | anderen `--modbus-port` wählen; `pruefe-ports.ps1`; in der Demo Charger 1504 |
| 2 | Client meldet „angenommen“, Gerät zeigt noch den alten Wert | EasyModbus antwortet **vor** der Anwendung; der Befehl wirkt erst im nächsten Schleifendurchlauf | 200 ms warten (Client tut das); `--once` erneut; selten und ungeklärt (1 von 411 am 2026-11-08) |
| 3 | Belegter Port, Programm startet nicht – aber es „läuft“ etwas | `start-all.ps1` überspringt belegte Ports | vorher `pruefe-ports.ps1`; alte Prozesse identifizieren |
| 4 | Zeit läuft nicht, Register 13 = 0 | Master startet **pausiert** | Leertaste im Master oder `--autostart` |
| 5 | Gerät springt auf falsche Zeit / „Master verloren“ | alter Master oder Skript-Master hält 5100 | alle Programme beenden, Port prüfen, neu starten |
| 6 | Umgeleitete Ausgabe zeigt nur alle 5 s eine Zeile | so gewollt (Render-Intervall) | kein Hänger; für echte Anzeige Terminal nutzen |
| 7 | Absturz beim ersten Frame, Exit-Code `-1073741819` | `Console.SetCursorPosition` ohne echtes Terminal | Terminalfenster nutzen; Fallback der Renderer prüfen |
| 8 | `Start-Process` zerlegt `"2026-01-15 06:00"` | PowerShell 5.1 trennt Argumente an Leerzeichen | einen String mit inneren Anführungszeichen übergeben |
| 9 | `<` wird als Operator verstanden | PowerShell kennt keine Eingabeumleitung mit `<` | `-RedirectStandardInput <datei>` |
| 10 | `dotnet test EnergyTwin.sln` scheitert, einzeln grün | Testprojekte laufen parallel und binden echte TCP-Ports | `dotnet test tests/<Projekt>` einzeln |
| 11 | Modellzeit weicht ~1 Minute ab | `switch`-Ausdruck mit gemischten Typen rechnet in `float` und verliert Sekunden | `RegisterCodec.Decode` verwenden, nicht selbst dekodieren |
| 12 | CSV-Werte erscheinen in Excel als Datum | Punkt als Dezimaltrennzeichen | `Shared.Data.CsvFormat` (de-AT); Batterie-CSV ist noch offen (`AGENTS.md` §0.4) |
| 13 | Umlaute/Zeilenenden in Dateien kaputt | `Get-Content`/`Set-Content` zerstören UTF-8 ohne BOM; LB01-Dateien teils BOM/CRLF | Editor-Werkzeug oder Python mit `newline=''` |
| 14 | Gateway: „BROKER NICHT ERREICHBAR“ | Broker aus, startet gerade, oder falsche Zugangsdaten | `docker compose ps`; Zugangsdaten prüfen; Meldung „Anmeldung abgelehnt“ vs. „Verbindung geschlossen“ beachten |
| 15 | Viewer sieht Status eines ausgefallenen Geräts nicht | RabbitMQ liefert retained nur bei **exaktem** Topic, nicht bei `#`-Wildcards | exaktes Topic abonnieren; `--status-repeat` (Standard 30 s) abwarten |
| 16 | Nach `taskkill` ist der Port noch belegt | Prozess noch nicht beendet / `TIME_WAIT` | kurz warten, erneut prüfen; nicht sofort neu starten |
| 17 | Protokolle enthalten `?` statt Umlauten, Datei ist UTF-16 | PowerShell-Redirection (`*> datei`, `Out-File`) liest die Ausgabe mit der Konsolencodepage und schreibt neu | `Start-Process -RedirectStandardOutput <datei>`, sonst die Rohbytes umleiten; Ergebnis als UTF-8 prüfen |

## Weitere Eigenheiten, die man kennen sollte

- EasyModbus gibt beim Start je Gerät ein Bibliotheks-Banner aus; das lässt sich nicht abschalten und gehört nicht in die Bewertung.
- `DeviceConnection` verbindet nach einem Fehler beim nächsten Zugriff selbst neu; ein Ausfall ist deshalb kein Programmabbruch.
- Das Gateway liest Geräte **nacheinander**; ein nicht erreichbares Gerät verzögert die Runde um bis zu `--timeout`.
- Status-Nachrichten werden wiederholt, weil RabbitMQ retained nur bei exaktem Topic liefert.
- Das Passwort steht nie in der Prozessliste und nie im Protokoll – Zugangsdaten nur über Umgebung oder `docker/.env`.
