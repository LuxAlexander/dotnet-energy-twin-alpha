# Funktionsnachweis 2026-11-08: Selbsttest des Skills `probelauf` (Kühlhaus, Umfang A)

**Durchgeführt von:** Dominik Zach mit opencode (Modell Big Pickle) – Ablauf skriptgesteuert, Protokolle unverändert.
**Bezug:** Prüfung des Skills [`probelauf`](../../../skills/probelauf/README.md), Umfang **A – Gerät**; gleichzeitig
Nachweis der Punkte A1–A4 und A7 der Prüfliste [`probelauf/references/abnahmekriterien.md`](../../../skills/probelauf/references/abnahmekriterien.md).
**Kein** LB02- oder LB03-Nachweis – was dieser Lauf nicht zeigen kann, steht unter „Nicht geprüft“.

## Aufbau

| Programm | Start | Protokoll |
|---|---|---|
| Kühlhaus KH-01 (standalone) | `dotnet run --project src/Devices/KuehlhausSimulator -- --id KH-01 --standalone --speed 600 --duration 8 --weather off --seed 42 --modbus-port 1505` | `kuehlhaus.txt` |
| Diagnose-Client, lesen | `dotnet run --project src/ModbusDiagnosticClient -- --port 1505 --once` | `diagnose-01-read.txt` |
| Diagnose-Client, schreiben | `dotnet run --project src/ModbusDiagnosticClient -- --port 1505 --write "Solltemperatur=-15"` | `diagnose-02-write.txt` |
| Diagnose-Client, erneut lesen | wie oben mit `--once` | `diagnose-03-read.txt` |
| Ablauf mit Zeitmarken | – | `ablauf.txt` |

**Besonderheiten des Laufs:** ohne Master (Standalone), Wetter mit `--weather off`, 8 Modellstunden bei Tempo 600
(48 s Echtzeit), Ausgabe umgeleitet in Dateien (kein Terminal). Der Lauf prüft den **Ablauf des Skills**; die
Gerätefunktion selbst ist im Geräte-README und in `tests/FieldDevices.Tests` abgedeckt.

## Ergebnis je Prüfpunkt

| # | Prüfung | Beobachtung | Ergebnis | Beleg |
|---|---|---|---|---|
| A1 | Gerät startet, Zeitbezug | Register 12 = 2 (Standalone), Register 13 = 1 (läuft), Tempo 600 | ✅ | `diagnose-01-read.txt:16-18` |
| A2 | Registerkopf vollständig | Adressen 0–19 gelesen, Typ 5 = Industrieverbraucher, Instanz 1 | ✅ | `diagnose-01-read.txt:5-22` |
| A3 | Geräteregister | 20–31 vollständig: Soll −20 °C, Raum −19,77 °C, außen 7,5 °C, Tür zu, Kompressor aus | ✅ | `diagnose-01-read.txt:23-31` |
| A4 | Gültiger Sollwert | „ANGENOMMEN: Solltemperatur −20 °C → −15 °C. Gerät zeigt jetzt −15 °C.“; zweite Lesung: Register 20–21 = −15 °C, Register 19 = 20; Gerät meldet „Modbus-Schreibzugriff angenommen“ | ✅ | `diagnose-02-write.txt:4`, `diagnose-03-read.txt:22-25`, `kuehlhaus.txt:3-4` |
| A5 | Ungültiger Wert (`force`) | nicht ausgeführt | ⚠️ nicht geprüft | – |
| A6 | Nur-Lese-Register (`force 5` o. ä.) | nicht ausgeführt | ⚠️ nicht geprüft | – |
| A7 | CSV | `docs/data/KH-01_2026-11-08_155520.csv` geschrieben: Semikolon, Dezimalkomma, Spalte `Lauf`, Kopfzeile wie im Geräte-README | ✅ | `kuehlhaus.txt:28` (Pfad), Datei selbst ist über `*.csv` ignoriert und nach dem Test gelöscht |
| A8 | Wetter | mit `--weather off` nicht Teil dieses Laufs | ⚠️ nicht geprüft | – |

## Beobachtungen und Einschränkungen

- **Zeitmarken passen zum Aufbau:** Gerät 15:55:19 gestartet, Port 1505 nach 1,2 s erreichbar, Lesen 15:55:20,
  Schreiben 15:55:21, Gegenlesen 15:55:22, Gerät nach 8 Modellstunden um 15:56:08 beendet (`ablauf.txt`).
- **Modellzeit wandert im Protokoll:** erste Lesung 00:10, Schreiben ~00:15, Gegenlesung 00:25 Modellzeit – bei
  Tempo 600 sind das je Abfrage einige Sekunden Echtzeit. Der Diagnose-Client ist deshalb kein Zeitnormal.
- **Umleitung ist kein Terminal:** Gerät und Client rendern bei umgeleiteter Ausgabe nur alle 5 s eine Zeile; die
  Statuszeile wird nach Spaltenbreite abgeschnitten (`kuehlhaus.txt:22` endet mit „Meldung: Simulatio“). Das ist
  erwartet und kein Fehler (Skill `probelauf`, Fehlerbild 6).
- **Kodierung der Protokolle:** PowerShell-Redirection mit `*>` schreibt UTF-16 und ersetzt Umlaute durch `?`;
  mit `Start-Process -RedirectStandardOutput` bleiben die UTF-8-Bytes des Programms unverändert. Alle Dateien hier
  sind UTF-8 ohne BOM. Der Fall ist als Fehlerbild 17 im Skill vermerkt.
- **`ExitCode` leer:** `Start-Process -PassThru` lieferte in dieser Umgebung nach `WaitForExit` keinen Exit-Code.
  Für den Nachweis unerheblich (Ausgaben und Portverhalten sind belegt), im Skill als Einschränkung genannt.
- **Türverhalten:** Ab 06:40 Modellzeit öffnete das Gerät die Tür (Zufallsereignis aus dem Seed), der Kompressor
  kühlte gegen, Energie 2,53 kWh. Plausibel, nicht Teil der Prüfpunkte.
- Der Portmatrix-Check vor und nach dem Lauf zeigte alle Ports frei; es blieben keine Prozesse zurück.

## Nicht geprüft

- **Umfang B (LB02-Demo):** Zeitgleichheit mit dem Master, Toleranz, Pause, Fortsetzen, Tempowechsel, Neustart,
  Wetterausfall, Wetter-Reproduzierbarkeit, geordnetes Beenden per `Q`.
- **Umfang C (LB03-Kette):** RabbitMQ, Gateway, Viewer, Sollwert über MQTT, Geräte- und Broker-Ausfall,
  falsche Zugangsdaten.
- **Echte Bedienung:** Anzeige und Tasten in einem echten Terminalfenster, Strg+C, Screenshots.
- **Ungültige Schreibwerte** (`force`, Register 18/19) und **Wetter** (`--weather` mit Wettersimulator).

Maßgeblich für die vollständigen Kriterien: [`probelauf/references/abnahmekriterien.md`](../../../skills/probelauf/references/abnahmekriterien.md).
