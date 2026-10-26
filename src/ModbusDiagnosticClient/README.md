# ModbusDiagnosticClient

**Zweck:** Konsolenprogramm zum Testen und Debuggen der Feldgeräte über Modbus TCP: liest den gemeinsamen Registerkopf und die
gerätespezifischen Register (Rohwert und gedeuteter Wert), schreibt Sollwerte und prüft beim Gerät nach, ob sie angenommen wurden,
und zeigt Verbindungs-, Protokoll- und Wertefehler verständlich an. Vorbereitung für LB03.
**Verantwortlich:** Susanne Lastowicka
**Basis aus LB01:** –
**Abhängigkeiten:** `Modbus.Contracts` (Registerkopf, Registerpläne, Codec), EasyModbusTCP.NETCore.

## Konfiguration

| Option | Standard | Bedeutung |
|---|---|---|
| `--host <name\|ip>` | `127.0.0.1` | Host des Geräts |
| `--port <port>` | `1502` | Modbus-TCP-Port des Geräts (z. B. Charger DC-02: 1503) |
| `--unit-id <1-247>` | `1` | Modbus-Unit-ID |
| `--timeout <ms>` | `3000` | Zeitlimit für Verbinden und Antwort (100–60000) |
| `--once` | aus | Register einmal lesen und beenden |
| `--write <ziel>=<wert>` | | Wert schreiben und beenden; mehrfach möglich |
| `--help` | | Hilfe |

Es gibt keine Zugangsdaten und keine Umgebungsvariablen. Exit-Code: 0 ok · 1 Verbindungs-/Protokollfehler · 2 Schreibzugriff vom Gerät abgelehnt · 64 falscher Aufruf oder ungültiger Wert.

## Bedienung / Start

```
dotnet run --project src/ModbusDiagnosticClient -- --port 1502                 # Eingabeaufforderung
dotnet run --project src/ModbusDiagnosticClient -- --port 1502 --once          # einmal lesen
dotnet run --project src/ModbusDiagnosticClient -- --port 1502 --write "Leistungsgrenze=200"
```

Eingabeaufforderung (`modbus>`), Befehle:

| Befehl | Wirkung |
|---|---|
| `read` | Kopf (0–19) und gerätespezifische Register lesen und dekodieren; Spalten Adresse, Name, Rohwert (hex), Wert |
| `header` | nur den gemeinsamen Registerkopf |
| `watch [anzahl] [s]` | Verlauf, eine Zeile je Abfrage (Modellzeit, Sequenz, Lauf, Zustand, Leistung, Energie, Zeitstatus, Tempo); Strg+C beendet nur `watch`; bei Verbindungsabbruch wird weiter versucht |
| `write <ziel> <wert>` | Sollwert schreiben. Ziel = Adresse oder (Teil des) Registernamens, Wert = Zahl (Punkt oder Komma) oder Klartext (`ein`, `aus`). Wird vorher gegen Registerplan und Bereich geprüft |
| `force <ziel> <wert>` | wie `write`, aber **ohne Prüfung im Client**, auch auf nur lesbare oder unbelegte Adressen – zeigt, wie das Gerät Ungültiges ablehnt |
| `on`, `off`, `reset` | Kurzbefehle für Ein/Aus (Adresse 16) und Fehler zurücksetzen (Adresse 17) |
| `raw <adresse> <n>` | n Register roh lesen (dezimal, hex, binär) |
| `info` | Verbindung, Gerätetyp und Registerplan als Tabelle |
| `connect <host[:port]> [unit]` | mit einem anderen Gerät verbinden |
| `help`, `quit` | Hilfe, Beenden (auch Strg+C) |

Der Registerplan wird aus Register 0 (Gerätetyp) gewählt (`RegisterPlans.For`). Hinterlegt sind Ladesäule, PV-Anlage, Batteriespeicher
und das Kühlhaus (Industrieverbraucher); für andere Gerätetypen (Wind) zeigt der Client den Kopf dekodiert und die übrigen Register mit `raw`.

| Gerät | Standardport (Vorschlag) | Sollwert zum Testen |
|---|---|---|
| PV-Anlage | 1502 | `write Leistungsbegrenzung 50` |
| Batteriespeicher | 1503 | `write Leistungssollwert -2,5` |
| Ladesäule | 1502 (in der Demo mit allen Geräten 1504) | `write Leistungsgrenze 200` |
| Kühlhaus | 1505 | `write Solltemperatur -15` |

### Schreiben und Prüfen
Nach jedem Schreibzugriff wartet der Client 200 ms (das Gerät setzt Befehle im nächsten Schleifendurchlauf um) und liest dann Register 18/19
(Letzter Schreibfehler, Adresse) und das beschriebene Register zurück. Ausgabe:

```
ANGENOMMEN: Leistungsgrenze der Säule 150 kW → 200 kW. Gerät zeigt jetzt 200 kW.
ABGELEHNT vom Gerät: Leistungsgrenze der Säule = 999 kW → Fehlerregister 18 = 2 (ungültiger Wert), Adresse 20. Register zeigt weiter 200 kW.
```
Grund: Die Modbus-Bibliothek der Geräte meldet einen Schreibfehler nicht als Modbus-Ausnahme, sondern der Server macht den Zugriff rückgängig und
setzt Register 18/19 (siehe Charger-README). Register 18/19 gelten für den **letzten** Schreibzugriff **irgendeines** Clients.

### Fehlerbehandlung
- **Verbindungsfehler** (Gerät nicht gestartet, Port falsch, Verbindung abgebrochen, keine Antwort im Zeitlimit): Meldung `VERBINDUNGSFEHLER: …`; der Client bleibt
  offen und verbindet beim nächsten Befehl neu. Im Einmal-Modus Exit-Code 1.
- **Protokollfehler** (Adresse nicht vorhanden, Funktionscode nicht unterstützt, falsche Anzahl): `PROTOKOLLFEHLER: …`.
- **Wertefehler** (unbekanntes oder mehrdeutiges Register, keine Zahl, außerhalb des Bereichs, nur lesbar): `WERTEFEHLER: …`, es wird nichts gesendet.
- **Beenden:** Strg+C beendet zuerst nur den laufenden Befehl (z. B. `watch`), dann das Programm; die Verbindung wird geschlossen.

### Vorführung der Abnahmepunkte 8 und 9
```
dotnet run --project src/ModbusDiagnosticClient -- --port 1502 --once                 # Kopf und Ladesäulen-Register lesen
dotnet run --project src/ModbusDiagnosticClient -- --port 1502 --write "Leistungsgrenze=100"   # Auswirkung: Limit in der Säule
dotnet run --project src/ModbusDiagnosticClient -- --port 1502 --once                 # Wert zurückgelesen, in der Säulenkonsole sichtbar
```

## Schnittstellen
- **Benötigt:** ein Gerät mit Modbus-TCP-Server (Registerplan in dessen README, Kopf in `src/Modbus.Contracts/README.md`).
- **Angeboten:** Konsole; Klassen `DeviceConnection` (Verbindung, Fehlerübersetzung), `DiagnosticSession` (Befehle), `RegisterFormatter`, `ClientOptions`.
- Die Bibliothek EasyModbusTCP.NETCore ist synchron (Bibliotheksgrenze laut Aufgabe, nicht in `Task.Run` verpackt); Eingabe, Wartezeiten und Ausgabe sind async.

## Tests
`dotnet test tests/ModbusDiagnosticClient.Tests` (28 Tests, grün am 2026-10-25): echter `ModbusDeviceServer` mit Ladesäulen-Plan; Lesen und Dekodieren,
gültiger Sollwert, Ablehnung im Client, Ablehnung im Gerät mit `force`, Verbindungsabbruch und Wiederverbindung, `watch` bei Ausfall, Optionen, Formatierung.

## Bekannte Probleme
- Für das Windgerät gibt es noch keinen Registerplan (`RegisterPlans.For`).
- Die Bibliothek gibt beim Start `EasyModbus Client Library Version …` aus; das lässt sich nicht abschalten.
- Verbinden und Lesen blockieren bis zum Zeitlimit (Bibliotheksgrenze); während `watch` reagiert Strg+C daher erst nach dem laufenden Zugriff.
- Fehler des Geräts sind nur über die Register 18/19 erkennbar (siehe oben), nicht über eine Modbus-Ausnahme.
