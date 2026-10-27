# WeatherSimulator

**Zweck:** Deterministisches Wetter für (Ort, virtuelle Zeit) als eigenständiges Programm; ohne Internet zur Laufzeit.
Liefert Strahlung, Sonnenhöhe, Bedeckung, Temperatur, Niederschlag, Wind und einen Wetterzustand als `WeatherData` (JSON) über HTTP.
**Verantwortlich:** Alexander Lux
**Basis aus LB01:** `Season`, `IrradiationCalculator`, `PvFaultModel.AmbientTemperature`/`DailyOffsetC` (nur als Vorbild;
die Wetterlogik ist neu und deterministisch).

## Start

```bash
# Standard: Port 8080
dotnet run --project src/WeatherSimulator

# Anderer Port
dotnet run --project src/WeatherSimulator -- --port 8090
```

Beim Start erscheint eine Anzeige mit Modus, letzter Abfrage und Zähler. Mit umgeleiteter Ausgabe wird alle 5 s eine Statuszeile geschrieben.

### Tasten

| Taste | Funktion |
|---|---|
| `1` | Manuell: sonnig |
| `2` | Manuell: bewölkt |
| `3` | Manuell: dicht bewölkt |
| `4` | Manuell: Gewitter |
| `S` | Manuell: Schnee |
| `R` | Manuell: Regen |
| `T` | Temperatur eingeben (°C) |
| `W` | Wind eingeben (m/s) |
| `X` | zurück auf Automatik (deterministisches Modell) |
| `Q` | Beenden |

Die manuelle Übersteuerung gilt für **alle** Standorte, bis sie mit `X` aufgehoben wird. Nicht gesetzte Größen
kommen weiter aus dem Modell. Die Bedienung ist für Demonstrationen gedacht, nicht für den Dauerbetrieb.

## Schnittstelle

HTTP-Server auf `http://127.0.0.1:<port>` (Standard 8080).

| Anfrage | Antwort |
|---|---|
| `GET /weather?location=<kennung>&simTime=yyyy-MM-ddTHH:mm:ss&run=<lauf>` | `WeatherData` als JSON |
| `GET /locations` | unterstützte Standorte (Kennung, Name, Koordinaten) |
| `GET /` | Kurzbeschreibung und Endpunkt |

- `location` ist optional, Standard `salzburg`; unbekannte Kennung ⇒ **404**.
- `simTime` ist UTC; fehlend/ungültig ⇒ **400**. Antwortfelder werden auf eine Nachkommastelle gerundet.
- `run` ist die Lauf-Nummer des Masters (Standard 0); sie fließt in den Seed ein, damit ein neuer Lauf neues Wetter ergibt.
- Die Antwort ist innerhalb einer Stunde konstant (Minuten/Sekunden ändern nichts).

Beispielantwort:

```json
{
  "location": "salzburg",
  "simTimeUtc": "2026-06-21T12:00:00Z",
  "runId": 1,
  "sunHeightDegrees": 64.71,
  "cloudCoverPercent": 22.4,
  "temperatureC": 24.3,
  "precipitationMmPerHour": 0.0,
  "windSpeedMs": 3.1,
  "irradiationWm2": 812.7,
  "condition": "PartlyCloudy"
}
```

Zustände: `Sunny`, `PartlyCloudy`, `Overcast`, `Storm`, `Snow`, `Night`.

## Modell

- **Basis:** WeatherSynth 0.3.0 erzeugt für (Standort, Kalenderjahr, Seed) stündliche Werte: Globalstrahlung,
  Klarhimmel-Strahlung und Wind (10 m, logarithmisches Profil). Offline-Bibliothek, DWD-Daten (CC BY 4.0).
- **Seed:** `runId · 7919 + FNV-1a(Standort|Jahr) + Jahr`; kein `string.GetHashCode`, damit der Wert über
  Neustarts und Prozesse hinweg gleich bleibt. Gleicher (Ort, Zeit, Lauf) ⇒ byte-identische Antwort.
- **Strahlung:** Globalstrahlung der Stunde; bei Sonnenhöhe ≤ 0 auf 0 gesetzt. Sonnenhöhe aus Deklination,
  Stundenwinkel und Standortkoordinaten (UTC).
- **Bedeckung:** `(1 − Globalstrahlung / Klarhimmelstrahlung) · 100`, nachts vom letzten Tageslicht übernommen.
- **Temperatur:** Jahresmittel aus Höhenlage (`10 °C − 0.0065 · (Höhe − 200 m)`), Jahresgang, Tagesgang
  (Maximum ~15 Uhr, durch Bewölkung gedämpft) und ein pro Tag und Standort geseedeter Offset von ±3 K.
- **Niederschlag:** Wahrscheinlichkeit wächst mit der Bedeckung; Menge aus einem stundenweise geseedeten Wert
  (0.2–3.2 mm/h, verdoppelt bei dichter Bewölkung und Wind ≥ 10 m/s).
- **Zustand:** `Night` ohne Sonne; `Snow` bei Niederschlag und ≤ 1.5 °C; `Storm` ab 90 % Bedeckung und 14 m/s;
  `Overcast` ab 80 %, `PartlyCloudy` ab 30 %, sonst `Sunny`.
- **Ortszeit:** interne Rechnung UTC; Anzeige zeigt zusätzlich `Europe/Vienna`.

Standorte: `salzburg` (Standard), `wien`, `innsbruck`.

### Formeln und Quellen

Belegt oder nahe an etablierten Standards:

| Formel | Quelle |
|---|---|
| Sonnenhöhe aus Deklination und Stundenwinkel | Standard-Astronomie (Cooper 1969; Spencer 1971; Duffie & Beckman, „Solar Engineering of Thermal Processes“); identisch mit dem NOAA-Solarrechner |
| Höhen-Abnahme der Temperatur `−0,0065 K/m` | Internationale Standardatmosphäre (ICAO/ISA, Doc 7488) – gemittelte Troposphärenrate |
| Strahlung und Wind (1 h) | NuGet `WeatherSynth` 0.3.0 – DWD-Daten (CC BY 4.0) |
| logarithmisches Windprofil | Monin-Obukhov-Bodenlogarithmengesetz; Standard in der Windenergie (IEC 61400-12-1) |
| Sturm-Schwelle 14 m/s | Beaufortstufe 7/8 (13,9–17,1 m/s) |
| Schnee bei ≤ 1,5 °C | Rain-Snow-Übergang liegt bei nasser Kugel-Temperatur ≈ 0–2 °C (z. B. Jennings et al. 2018) |
| „dicht bewölkt“ ab 80 %, Bedeckungsgrad aus Strahlungsverhältnis | 8/8 Okta = 87,5 %; Zusammenhang Bewölkung ↔ Globalstrahlung z. B. Kasten & Czeplak 1980 |

Eigene einfache Heuristik (deterministisch, offline – entsprechend der Aufgabenstellung, **nicht** aus Literatur kalibriert):

| Formel | Anmerkung |
|---|---|
| Jahresmittel `10 °C − 0.0065 · (Höhe − 200 m)` | Referenzwert 10 °C ist eine Annahme; trifft zufällig gut auf die Klimanormale von Salzburg (≈ 9,5 °C, 1991–2020, DWD/AustroControl) |
| Jahresgang `−9,5·cos(2π·(Tag−15)/365)` | Amplitude −9,5 K ist gefittet (Jan ≈ 0,5 / Jul ≈ 19,5 °C ≈ Salzburg); keine direkte Kalibrierung |
| Tages-Offset ±3 K aus dem Seed | vereinfachend weißes Rauschen je Tag; reale Wetterlagen wandern über Tage |
| Niederschlagswahrscheinlichkeit `Bedeckung → 0,25` | erfundene Skala, Wüste/Tropen unterschieden das nicht |
| Bewölkungsmäßige Dämpfung des Tagesgangs (Faktor 0,3) | Richtung belegt (Wolken dämpfen die Amplitude), Faktor nicht kalibriert |
| Nachts Bewölkung 40 % | Konvention |

Die drei österreichischen Standorte liefern damit plausibel nah an den Klimanormalen liegende Werte;
für Standorte mit anderem Klima (Wüste, Tropen, Pol) ist das Modell nicht kalibriert – ein Ausbau mit
einer Klimatabelle pro Standort ist im Team besprochen, aber noch nicht umgesetzt.

## Tests

```bash
dotnet test tests/Weather.Tests
```

11 Tests: gleiche Anfrage ⇒ gleiches Ergebnis, Konstanz innerhalb der Stunde, Unterschiede zwischen Standorten und Läufen,
Nacht ohne Strahlung, Sommerhöher als Winter, manuelle Übersteuerung inkl. Aufheben, Plausibilitätsbereiche über das Jahr.

## Bekannte Probleme

- WeatherSynth ist ein zusätzliches NuGet-Paket (AGPL-3.0; SolarCalculator LGPL; DWD-Daten CC BY 4.0) und wird beim
  ersten Build geladen. Die Laufzeit braucht kein Internet.
- Ortszeit-Anzeige fest auf `Europe/Vienna`; die Schnittstelle selbst ist reine UTC.
- Es gibt keine Persistenz: ein Neustart setzt die manuelle Übersteuerung zurück.
- Die Anzeige ist nur mit umgeleiteter Ausgabe geprüft; ein echtes Terminalfenster ist noch zu testen.
