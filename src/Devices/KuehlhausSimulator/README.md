# Kühlhaus-Simulator (Industrieverbraucher)

Konsolenanwendung in C# (.NET 10), die ein **Kühlhaus** als eigenständiges Feldgerät simuliert: ein Kühlraum mit einem
Kompressor, einer Tür und einer festen Solltemperatur. Das Gerät ist das **vierte Feldgerät** in LB02 (neben PV, Batterie
und DC-Ladesäule) und nimmt am verteilten System teil:

- **virtuelle Zeit** vom Simulations-Master (`127.0.0.1:5100`) oder standalone von einer lokalen Uhr,
- **Außentemperatur** vom Wettersimulator (HTTP, `127.0.0.1:8080`), sonst aus einem lokalen Tagesprofil,
- **Modbus-TCP-Server** mit gemeinsamem Registerkopf 0–19 und Geräteregistern 20–31 (Standardport **1505**),
- **CSV-Verlauf** je Lauf, Anzeige und Tasten wie bei den anderen Geräten.

**Verantwortlich:** Dominik Zach (LB01-Modell); LB02-Umbau (Zeit, Modbus, Wetter, Ein-/Ausgabe, Tests, Doku) am 2026-10-26,
mit Dominik abzustimmen.

**Basis aus LB01:** `Kuehlhaus`, `KuehlhausConfig`, Testmenü in `Program.cs` (durch die LB02-Bedienung ersetzt).

---

## Voraussetzungen

- **.NET 10 SDK**, Teil der Solution `EnergyTwin.sln`.
- Windows-Konsole (Windows Terminal, Eingabeaufforderung oder VS-Debugging-Konsole), mindestens 80 Zeichen breit und
  ~20 Zeilen hoch. Die Konsole muss **interaktiv** sein; bei umgeleiteter Ausgabe erscheint statt der Anzeige alle 5 s
  eine Datenzeile und eine Statuszeile.
- Modbus-Port **1505** frei (1502 nutzen PV und Charger, 1503 die Batterie).

## Starten

Alle Befehle aus dem Repository-Hauptordner:

```bash
# 1. Wettersimulator (HTTP 8080) – optional, sonst lokales Tagesprofil
dotnet run --project src/WeatherSimulator

# 2. Simulations-Master (TCP 5100, startet pausiert)
dotnet run --project src/SimulationMaster -- --speed 600

# 3. Kühlhaus (Modbus 127.0.0.1:1505), dann im Master mit Leertaste starten
dotnet run --project src/Devices/KuehlhausSimulator -- --id KH-01

# 4. Modbus-Diagnose-Client
dotnet run --project src/ModbusDiagnosticClient -- --port 1505
```

Ohne Master läuft das Gerät nur mit `--standalone` (lokale Uhr), z. B. für einen 24-h-Lauf:

```bash
dotnet run --project src/Devices/KuehlhausSimulator -- --standalone --speed 6000 --duration 24 --weather off --seed 42
```

Programmausgaben:

```bash
dotnet run --project src/Devices/KuehlhausSimulator -- --help            # Optionen
dotnet run --project src/Devices/KuehlhausSimulator -- --register-plan   # Registerplan als Markdown
```

### Kommandozeilenoptionen

| Option | Bedeutung | Standard |
|---|---|---|
| `--id <text>` | Geräte-ID (auch CSV-Dateiname und Geräte-Nummer im Registerkopf) | `KH-01` |
| `--setpoint <°C>` | Solltemperatur, gültig −30 … 0 °C | −20 |
| `--cooling <kW>` | thermische Kälteleistung des Kompressors | 6 |
| `--step <min>` | feste Modellschrittweite in Minuten | 5 |
| `--seed <zahl>` | Zufalls-Seed für Türöffnungen (wiederholbare Läufe) | zufällig |
| `--output <ordner>` | Zielordner der CSV-Datei | `docs/data` im Repo |
| `--weather <url\|off>` | Wettersimulator; `off` = lokales Tagesprofil | mit Master `http://127.0.0.1:8080`, standalone `off` |
| `--location <kennung>` | Standort für die Wetterabfrage | `salzburg` |
| `--master <host:port>` | Simulations-Master | `127.0.0.1:5100` |
| `--standalone` | ohne Master, lokale Uhr | aus |
| `--start <zeit>` | nur standalone, `yyyy-MM-dd HH:mm` | heute 00:00 |
| `--speed <x>` | nur standalone, Tempo 1 … 100 000 | 600 |
| `--duration <h>` | nur standalone, Dauer; ohne Angabe endlos | endlos |
| `--modbus-port <port>` | Modbus-TCP-Port, 1024–65535 | 1505 |
| `--modbus-bind <ip>` | Adresse, auf der der Server lauscht | `127.0.0.1` |
| `--unit-id <n>` | Modbus-Unit-ID | 1 |
| `--no-modbus` | keinen Modbus-Server starten | aus |
| `--register-plan` | Registerplan ausgeben und beenden | – |

Zahlen dürfen Punkt oder Komma als Dezimaltrenner haben. `--duration`, `--start` und `--speed` sind **nur** mit
`--standalone` erlaubt (mit Master bestimmt der Master Zeit und Tempo); ungültige Werte beenden das Programm mit
Fehlermeldung und Hilfe (Exit-Code 1).

## Bedienung

Die Anzeige zeigt Kopf (Gerät, Modus, Zeitquelle, Modbus, Wetter), Simulationszeit und Lauf, Außentemperatur und ihre
Quelle, Anlage (Ein/Aus), Soll- und Raumtemperatur mit Balken bis zum Alarm, Kompressor, Kälte- und elektrische Leistung,
Energie, Laufzeit, Türzustand und die letzte Meldung. Sie wird flackerfrei ab Position (0,0) überschrieben.

### Tasten

| Taste | Funktion |
|---|---|
| `T` | Tür öffnen/schließen (schaltet in den Handbetrieb, damit die Automatik sie nicht wieder schließt) |
| `S` | Solltemperatur eingeben (−30 … 0 °C; Esc bricht ab) |
| `F` | Kompressorausfall auslösen (Kühlung steht sofort) |
| `R` | Störung quittieren (Übertemperatur erst unter Alarm − Hysterese) |
| `O` | Anlage ein/aus (wie Register 16) |
| `A` | Automatik (Tür nach Tageszeit) / Handbetrieb |
| `P` | Pause/Fortsetzen – **nur standalone**, sonst steuert der Master |
| `<` / `>` | Tempo ÷2 / ×2 – **nur standalone** |
| `Q` | Beenden (Alternativen: Strg+C, Ende der `--duration`) |

Nach dem Ende der Dauer wird die Zusammenfassung angezeigt; `Q` beendet das Programm. Strg+C beendet jederzeit geordnet
(Abmeldung beim Master, CSV schließen, Modbus-Port freigeben).

## Modell

### Wärmebilanz

```
C · dT/dt = k · (T_aus − T) + Q_Tür − Q_Kühl
```

| Größe | Bedeutung | Standard |
|---|---|---|
| `C` | Wärmekapazität von Raum, Ware und Bauwerk | 1,0 kWh/K |
| `k` | Wärmedurchgang der Hülle | 0,05 kW/K |
| `Q_Tür` | zusätzlicher Wärmeeintrag bei offener Tür | 1,5 kW |
| `Q_Kühl` | thermische Kälteleistung des Kompressors | 6,0 kW |

Der Schritt wird **exakt** gelöst (keine Euler-Näherung), deshalb bleibt das Modell auch bei großen Schrittweiten stabil:

```
T_neu = T_∞ + (T_alt − T_∞) · e^(−Δt/τ)
T_∞   = T_aus + (Q_Tür − Q_Kühl) / k
τ     = C / k  = 20 h   (Standardwerte)
```

Ohne Kühlung und mit geschlossener Tür erwärmt sich der Raum also mit 20 h Zeitkonstante in Richtung Außentemperatur;
mit offener Tür auf ein Gleichgewicht von `T_aus + 30 K` (bei 20 °C außen: 50 °C), mit laufendem Kompressor auf
`T_aus − 120 K`.

### Kompressor und Hysterese

Der Kompressor schaltet **ein** bei `Soll + Hysterese` (Standard +2 K) und **aus** bei `Soll − Hysterese` (2 K).
Bei ausgeschalteter Anlage oder Kompressorausfall bleibt er aus. Die Hysterese wird vor dem Temperaturschritt geprüft.

### Elektrische Leistung und COP

```
P_elektrisch = Q_Kühl / COP(T_aus)
COP(T_aus)   = max(1,0; COP₂₀ − 0,05 · (T_aus − 20 °C))
```

Mit `COP₂₀ = 2,0` nimmt der Kompressor bei 20 °C außen und 6 kW Kälte also 3 kW elektrisch auf. Die Energie im
Registerkopf und in der CSV ist die **aufgenommene elektrische** Energie (positiv = Verbraucher).

### Zustände und Störungen

| Zustand | Bedeutung |
|---|---|
| `Off` | Anlage aus |
| `Idle` | Anlage ein, Kompressor aus (Temperatur im Hystereseband) |
| `Cooling` | Kompressor läuft |
| `Fault` | Störung aktiv |

| Störung | Auslösen | Wirkung | Quittieren |
|---|---|---|---|
| Kompressorausfall (Fehlercode **1**) | Taste `F` | Kühlung sofort aus, Raum erwärmt sich | `R` oder Register 17 = 1, **immer** möglich |
| Übertemperatur (Fehlercode **2**) | Raumtemperatur > Alarm (−10 °C) | wird gespeichert (Alarm-Latch), Kühlung läuft weiter | `R` erst, wenn der Raum unter Alarm − Hysterese (−12 °C) ist |

Der Kompressorausfall wird immer quittiert, damit die Kühlung wieder anlaufen kann. Der Übertemperatur-Alarm lässt sich
im warmen Raum nicht quittieren – sonst würde er sich selbst aussperren, weil ohne Kompressor nichts kühlt. Wird die
Anlage ausgeschaltet, ist die elektrische Leistung sofort 0.

### Automatik

Die Automatik öffnet die Tür nach Tageszeit (Lieferverkehr, Schichtwechsel). Die Rate ist eine Wahrscheinlichkeit je
Stunde, umgerechnet auf die Schrittweite; die Tür bleibt danach 1–10 Minuten offen:

| Zeit | Öffnungen je Stunde |
|---|---|
| 00:00–06:00 | 0,1 |
| 06:00–09:00 | 1,0 |
| 09:00–12:00 | 1,5 |
| 12:00–18:00 | 0,8 |
| 18:00–24:00 | 0,4 |

Alles nutzt das übergebene `Random`, mit `--seed` sind Läufe wiederholbar. `T` schaltet auf Handbetrieb, damit die
Automatik eine von Hand geöffnete Tür nicht beim nächsten Schritt wieder schließt.

### Außentemperatur

Mit Master und ohne `--weather off` fragt das Gerät **je Modellstunde** den Wettersimulator
(`GET /weather?location=…&simTime=…&run=…`) und übernimmt `TemperatureC` als Außentemperatur. Fehler werden sichtbar
gemeldet (ab 3 Fehlversuchen in Folge als Meldung), das letzte gültige Wetter bleibt aktiv, neue Versuche alle 10 s.
Ohne Wettersimulator (standalone oder `--weather off`) gilt das lokale Tagesprofil:

```
T_lokal(t) = 14 °C + 9 K · sin(2π · (Stunde + Minute/60 − 9) / 24)
```

Minimum 5 °C um 3 Uhr, Maximum 23 °C um 15 Uhr. Die CSV-Spalte `Aussenquelle` zeigt, woher der Wert kam
(`Wettersimulator` bzw. `lokal`).

## CSV-Verlauf

`docs/data/<ID>_<yyyy-MM-dd_HHmmss>.csv` (überschreibbar mit `--output`), UTF-8 mit BOM, Semikolon als Trenner,
Dezimalkomma (de-AT). Beim Start, bei jedem Neustart im Master und nach jedem Modellschritt eine Zeile:

| Spalte | Bedeutung |
|---|---|
| `Zeit` | virtuelle Zeit am Ende des Schritts |
| `Lauf` | Lauf-Nummer (ändert sich bei Neustart im Master) |
| `Modus` | `AUTO` oder `MANUELL` |
| `Anlage_Ein` | 1 = Anlage ein |
| `Soll_C` | Solltemperatur |
| `Raum_C` | Raumtemperatur |
| `Aussen_C` | Außentemperatur des Schritts |
| `Aussenquelle` | `Wettersimulator`, `lokal` oder `lokal (kein Wetter)` |
| `Kompressor` | 1 = läuft |
| `Kaelte_kW` | thermische Kälteleistung |
| `Leistung_kW` | elektrische Leistung |
| `Energie_kWh` | aufgenommene elektrische Energie seit Programmstart |
| `Laufzeit_h` | Kompressorlaufzeit |
| `Tuer` | 1 = offen |
| `Zustand` | `Off`, `Idle`, `Cooling`, `Fault` |
| `Fehler` | `None`, `CompressorFailure`, `OverTemperature` |

## Registerplan

Holding Registers (Funktionscode 03 lesen, 06/16 schreiben). Standard: Host `127.0.0.1`, Port `1505`, Unit-ID `1` –
alle drei sind konfigurierbar. Adressen sind Protokolladressen (0-basiert, Referenzadresse = 40001 + Adresse).
Pro Register Big-Endian; mehrteilige Werte (`UInt32`, `Float32`) belegen zwei Register, das höherwertige Wort steht
zuerst (Float32 = IEEE 754, Byte-Folge ABCD). Mehrteilige Werte müssen mit **einem** Schreibzugriff (FC16) auf beide
Register geschrieben werden.

| Adresse | Ref. | Name | Typ | Regs | Einheit | Zugriff | Schreibbereich | Bedeutung / Fehlerverhalten |
|---|---|---|---|---|---|---|---|---|
| 0 | 40001 | Gerätetyp | UInt16 | 1 |  | lesen | – | 1 PV, 2 Batterie, 3 Charger, 4 Wind, 5 Industrieverbraucher |
| 1 | 40002 | Geräte-Nummer | UInt16 | 1 |  | lesen | – | Zahl am Ende der Geräte-ID (DC-02 → 2), 0 wenn keine |
| 2 | 40003 | Betriebszustand | UInt16 | 1 |  | lesen | – | 0 Aus, 1 Bereit/Leerlauf, 2 Betrieb, 3 Störung |
| 3 | 40004 | Fehlercode | UInt16 | 1 |  | lesen | – | 0 = kein Fehler, 1 = Kompressorausfall, 2 = Übertemperatur |
| 4–5 | 40005 | Wirkleistung | Float32 | 2 | kW | lesen | – | Elektrische Leistungsaufnahme; positiv = Aufnahme (Verbraucher) |
| 6–7 | 40007 | Kumulierte Energie | Float32 | 2 | kWh | lesen | – | Aufgenommene elektrische Energie seit Programmstart |
| 8–9 | 40009 | Modellzeit | UInt32 | 2 | s | lesen | – | Virtuelle Zeit des dargestellten Zustands, Sekunden seit 1970-01-01 00:00 (ohne Zeitzone); 0 = noch keine Zeit |
| 10 | 40011 | Lauf | UInt16 | 1 |  | lesen | – | Lauf-Nummer; ändert sich bei jedem Neustart im Master (Modellzeit springt dann) |
| 11 | 40012 | Sequenznummer | UInt16 | 1 |  | lesen | – | Zählt bei jeder neuen Modellzeit hoch (Überlauf bei 65535); gleiche Nummer = gleicher Zustand |
| 12 | 40013 | Zeitstatus | UInt16 | 1 |  | lesen | – | 0 keine Zeit, 1 synchron mit Master, 2 Standalone, 3 Master verloren (Zeit steht) |
| 13 | 40014 | Simulation läuft | UInt16 | 1 |  | lesen | – | 1 = virtuelle Zeit läuft, 0 = pausiert oder keine Zeit; bei Pause zeigen die Register den letzten Zustand |
| 14–15 | 40015 | Tempo | Float32 | 2 | x | lesen | – | Simulationsgeschwindigkeit gegenüber Echtzeit |
| 16 | 40017 | Ein/Aus | UInt16 | 1 |  | lesen/schreiben | 0 … 1 | Lesen: 1 = Gerät ein. Schreiben: 0 aus (Leistung sofort 0), 1 ein |
| 17 | 40018 | Fehler zurücksetzen | UInt16 | 1 |  | lesen/schreiben | 1 … 1 | Schreiben von 1 quittiert Störungen, sofern das Gerät es zulässt; liest immer 0 |
| 18 | 40019 | Letzter Schreibfehler | UInt16 | 1 |  | lesen | – | 0 = letzter Schreibzugriff angenommen, 1 nicht beschreibbar, 2 ungültiger Wert, 3 Wert unvollständig geschrieben |
| 19 | 40020 | Adresse des letzten Schreibzugriffs | UInt16 | 1 |  | lesen | – | Erste Adresse des letzten Schreibzugriffs (bei Fehler: die fehlerhafte Adresse) |
| 20–21 | 40021 | Solltemperatur | Float32 | 2 | °C | lesen/schreiben | −30 … 0 | Solltemperatur des Kühlraums (Sollwert). Außerhalb −30 … 0 °C wird der Schreibzugriff abgelehnt (Letzter Schreibfehler = 2), der alte Wert bleibt |
| 22 | 40023 | Tür | UInt16 | 1 |  | lesen/schreiben | 0 … 1 | 0 = geschlossen, 1 = offen; eine offene Tür erhöht den Wärmeeintrag |
| 23–24 | 40024 | Raumtemperatur | Float32 | 2 | °C | lesen | – | Temperatur im Kühlraum |
| 25–26 | 40026 | Außentemperatur | Float32 | 2 | °C | lesen | – | Außentemperatur des letzten Schritts (vom Wettersimulator oder aus dem lokalen Tagesprofil) |
| 27 | 40028 | Kompressor | UInt16 | 1 |  | lesen | – | 1 = läuft, 0 = aus (Hysterese um die Solltemperatur) |
| 28–29 | 40029 | Kälteleistung | Float32 | 2 | kW | lesen | – | Thermische Leistung des Kompressors; die elektrische Leistung steht im Kopf (Kälteleistung / COP) |
| 30–31 | 40031 | Laufzeit | Float32 | 2 | h | lesen | – | Kompressorlaufzeit seit Programmstart |

Besonderheiten des Kühlhauses:

- **Gerätetyp 5** = Industrieverbraucher (Wind bleibt 4).
- **Leistung positiv** = Aufnahme. Die Energie im Kopf ist die elektrische Energie; die Kälteleistung steht getrennt in
  Register 28–29.
- **Solltemperatur (20–21):** Grenzen sind sowohl im Registerplan als auch im Modell geprüft. Ein Wert außerhalb
  −30 … 0 °C oder `NaN` wird abgelehnt: Register 18 = 2, Register 19 = 20, der alte Wert bleibt (die Bibliothek
  bestätigt den Schreibzugriff trotzdem – siehe „Bekannte Probleme“).
- **Tür (22):** wirkt sofort auf die Wärmeleitung; der Automatikbetrieb kann sie danach wieder schließen. Im Handbetrieb
  bleibt sie, bis sie erneut geschrieben oder `T` gedrückt wird.
- **Ein/Aus (16)** wirkt wie `O`: Aus setzt die elektrische Leistung sofort auf 0.
- **Fehler zurücksetzen (17)** wirkt wie `R`; der Übertemperatur-Alarm wird nur unter −12 °C quittiert.
- **Master getrennt:** Register bleiben lesbar, Zeitstatus = 3, Modellzeit und alle Werte stehen (letzter Zustand).
- Fehlerverhalten der Bibliothek: siehe `src/Devices/DualPortCharger/README.md`, Abschnitt „Fehlerverhalten bei
  Schreibzugriffen“.

Prüfen mit dem Diagnose-Client:

```bash
dotnet run --project src/ModbusDiagnosticClient -- --port 1505 --once
dotnet run --project src/ModbusDiagnosticClient -- --port 1505 --write "Solltemperatur=-15"
```

## Schnittstellen

| Schnittstelle | Richtung | Transport | Standard |
|---|---|---|---|
| Simulations-Master (Zeit, Pause, Tempo, Neustart) | benötigt | TCP, JSON-Zeilen | `127.0.0.1:5100` |
| Wettersimulator (Außentemperatur je Modellstunde) | optional | HTTP GET `/weather` | `http://127.0.0.1:8080` |
| Modbus-Server (Register lesen/schreiben) | angeboten | Modbus TCP, Holding Registers | `127.0.0.1:1505` |
| CSV-Verlauf | angeboten | Datei | `docs/data/` |

## Aufbau

```
src/Devices/KuehlhausSimulator/
├── KuehlhausSimulator.csproj   # Einstellungen aus src/Directory.Build.props
├── Program.cs                  # Einstieg: Hilfe, --register-plan, Strg+C, Zusammenfassung
├── Kuehlhaus.cs                # Modell (exakte Exponentiallösung, Hysterese, COP, Störungen)
├── KuehlhausConfig.cs          # Modellparameter (Einheit im Namen)
├── Enums.cs                    # DeviceState, FaultType
├── Simulation/
│   ├── SimulationConfig.cs     # Optionen (Soll, Kälte, Schritt, Dauer, Seed, Zeit, Modbus, Wetter)
│   ├── SimulationEngine.cs     # Hauptschleife: Zeit, Schritte, Wetter, CSV, Modbus, Tasten, Anzeige
│   ├── KuehlhausModbus.cs      # Register veröffentlichen, Schreibzugriffe anwenden
│   ├── WeatherClient.cs        # HTTP-Abfrage je Modellstunde
│   └── AutoModeController.cs   # Türöffnungen nach Tageszeit
└── IO/
    ├── ArgsParser.cs           # Kommandozeile und Hilfe
    ├── CsvLogger.cs            # CSV je Lauf (async, de-AT, BOM)
    ├── ConsoleRenderer.cs      # flackerfreie Anzeige, Fallback bei umgeleiteter Ausgabe
    └── InputHandler.cs         # Tasten T/S/F/R/O/A/P/</>/Q
```

`Kuehlhaus` kennt weder Konsole noch Uhr noch Netzwerk: die Außentemperatur kommt als Parameter, die Zeit als
Schrittweite. Ein-/Ausgabe liegt in `IO/`, die Verdrahtung in `Simulation/`.

## Tests

```bash
dotnet test tests/FieldDevices.Tests
```

Enthalten (`ConsumerTests.cs`, 28 Tests):

| Bereich | Geprüft |
|---|---|
| Modell | exakte Exponentiallösung, Stabilität bei sehr großen Schritten, Hysterese ein/aus, COP und Energie, Sollwertgrenzen, Aus-Zustand, Kompressorausfall, Übertemperatur-Latch |
| Modbus | Registerkopf (Typ 5, Geräte-Nummer, Zustand, Zeitstatus), Soll-/Raum-/Außentemperatur, Tür, Kompressor, Kälteleistung, Sollwert gültig/ungültig, Ein/Aus, Reset |
| Optionen | Standardwerte (Port 1505), Wetter-Standard mit/ohne Master, Punkt und Komma, ungültige Werte, `--duration` nur standalone |
| Engine | Standalone-Lauf schreibt CSV ab `--start`, Kopfzeile, `Lauf`-Spalte, lokale Außenquelle, Kompressor kühlt |
| Anzeige | interaktives Layout (`BuildLines`) enthält Kopf, Außentemperatur und Tastenhilfe |
| Automatik | Tagesprofil (Tag > Nacht), gleicher Seed ⇒ gleiche Türöffnungen |

Zusätzlich prüft `RegisterPlanRequirementTests` in `FieldDeviceTests.cs`, dass der Kühlhaus-Plan einen schreibbaren
Sollwert, mindestens drei lesbare Geräteregister und eine Registrierung in `RegisterPlans.For` hat.

Probeläufe am 2026-10-26 (umgeleitete Ausgabe):

- standalone 24 h: 289 CSV-Zeilen, 20,7 kWh, Tür in 25 Schritten offen;
- mit Master (x600): synchron, nach Master-Abbruch Zeitstatus = 3 und stehende Werte;
- Wetter vom Wettersimulator übernommen (Anzeige „Wetter“ und Spalte `Aussenquelle`);
- Diagnose-Client: Sollwert `-20 → -15 °C` angenommen, `force 5` abgelehnt (Register 18 = 2, Adresse 20).

## Bekannte Probleme und Einschränkungen

- **Interaktives Terminal noch ungetestet:** Anzeige, Tasten und Eingabe der Solltemperatur wurden nur mit umgeleiteter
  Ausgabe geprüft. Bei umgeleiteter Ausgabe erscheinen alle 5 s zwei Zeilen (Daten + Status), Tasten werden ignoriert.
- **Schreibfehler ohne Modbus-Ausnahme:** Die Bibliothek bestätigt Schreibzugriffe, bevor die Anwendung sie prüft.
  Abgelehnte Werte erkennt man an Register 18/19 und in der Konsole, nicht als Modbus-Exception.
- **Neustart im Master** setzt den Gerätezustand nicht zurück (Raumtemperatur, Energie, Störungen bleiben); nur die
  Modellzeit springt auf den neuen Lauf.
- **Wetter für die anderen Geräte** (Charger, Batterie) ist nicht angebunden; sie nutzen feste Umgebungstemperaturen.
  Das Kühlhaus fragt nur die Temperatur ab, nicht Strahlung oder Niederschlag.
- **Projektname und Ports sind entschieden (2026-10-26):** Projekt `KuehlhausSimulator`, Modbus-Standardport **1505**
  (1502 = PV/Charger, 1503 = Batterie, 1504 = Charger in der Demo), Wärmeeintrag bei offener Tür 1,5 kW.
- **Modellvereinfachungen:** ein Kompressor ohne Vereisung, keine Ware, kein Abtauen, kein Lastprofil außer der Tür;
  der COP hängt nur von der Außentemperatur ab.
