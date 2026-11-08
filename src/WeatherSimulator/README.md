# WeatherSimulator

**Zweck:** Deterministisches Wetter für (Ort, virtuelle Zeit) als eigenständiges Programm; ohne Internet zur Laufzeit.
Liefert Strahlung, Sonnenhöhe, Bedeckung, Temperatur, Niederschlag, Wind und einen Wetterzustand als `WeatherData` (JSON) über HTTP.
**Verantwortlich:** Alexander Lux

**LB03 umgesetzt (2026-11-08):** zusätzlicher MQTT-Ausgang zur Sichtbarkeit im Viewer, ausdrücklich
aktivierbar mit `--mqtt`; Geräte bleiben bei HTTP. Fünf Standorte inklusive Tamanrasset/Neumayer III,
Master-Zeitanbindung und gezielte manuelle Übersteuerung. Vertrag:
[`docs/Wetter-Nachrichtenvertrag-LB03.md`](../../docs/Wetter-Nachrichtenvertrag-LB03.md).

**Basis aus LB01:** `Season`, `IrradiationCalculator`, `PvFaultModel.AmbientTemperature`/`DailyOffsetC` (nur als Vorbild;
die Wetterlogik ist neu und deterministisch).

## Start

```bash
# Standard: Port 8080
dotnet run --project src/WeatherSimulator

# Anderer Port
dotnet run --project src/WeatherSimulator -- --port 8090

# LB03: zusätzlich MQTT; RabbitMQ und Master vorher starten
dotnet run --project src/WeatherSimulator -- --mqtt

# Andere Adressen (Passwort ausschließlich aus Umgebung/docker/.env)
dotnet run --project src/WeatherSimulator -- --mqtt --master 127.0.0.1:5100 --broker 127.0.0.1:1883
```

Beim Start erscheint eine Anzeige mit Modus, letzter Abfrage und Zähler. Mit umgeleiteter Ausgabe wird alle 5 s eine Statuszeile geschrieben.

### Tasten

| Taste | Funktion |
|---|---|
| `L` | ausgewählten Standort zyklisch wechseln; Standard Salzburg |
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

Die manuelle Übersteuerung gilt **nur für den ausgewählten Standort**, bis sie dort mit `X` aufgehoben wird.
Nicht gesetzte Größen kommen weiter aus dem Modell. Mit aktiver Master-Anbindung (`--mqtt`) werden beim neuen Master-Lauf oder einer neuen Master-Sitzung
alle manuellen Vorgaben zurückgesetzt. Pause und bloße Verbindungsunterbrechungen erhalten die Vorgaben.
Temperatur: −100 bis 70 °C; Wind: 0 bis 100 m/s, nur endliche Werte.
Die Anzeige „Letzte Abfrage“ beschreibt die letzte HTTP-Antwort, nicht eine unabhängig veröffentlichte MQTT-Nachricht.

## Schnittstelle

HTTP-Server auf `http://127.0.0.1:<port>` (Standard 8080).

| Anfrage | Antwort |
|---|---|
| `GET /weather?location=<kennung>&simTime=yyyy-MM-ddTHH:mm:ss&run=<lauf>` | `WeatherData` als JSON |
| `GET /locations` | unterstützte Standorte (Kennung, Name, Koordinaten) |
| `GET /` | Kurzbeschreibung und Endpunkt |

- `location` ist optional, Standard `salzburg`; unbekannte Kennung ⇒ **404**.
- `simTime` ist UTC; fehlend/ungültig ⇒ **400**. Sonnenhöhe, Niederschlag und Wind haben zwei Nachkommastellen,
  die übrigen numerischen Wettergrößen eine (siehe `WeatherModel.Get`).
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

- **Basis der österreichischen Standorte:** WeatherSynth 0.3.0 erzeugt für (Standort, Kalenderjahr, Seed) stündliche Werte: Globalstrahlung,
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
- **Zustand:** `Snow` bei Niederschlag und ≤ 1.5 °C, sonst `Storm` ab 90 % Bedeckung und 14 m/s;
  danach `Night` ohne Sonne, `Overcast` ab 80 %, `PartlyCloudy` ab 30 %, sonst `Sunny`. Schnee und Sturm
  bleiben dadurch auch in Dunkelheit sichtbar.
- **Ortszeit:** interne Rechnung UTC; Anzeige zeigt zusätzlich `Europe/Vienna`.

## Standorte

| Kennung für `--location` und HTTP | Ort | Modellbasis |
|---|---|---|
| `salzburg` (Standard) | Salzburg | WeatherSynth und österreichisches Temperaturmodell |
| `wien` | Wien | gleiche Modellbasis, eigene Geometrie/Seeds |
| `innsbruck` | Innsbruck | gleiche Modellbasis, eigene Geometrie/Seeds |
| `tamanrasset` | Tamanrasset, Sahara | regionales Klimaprofil |
| `neumayer` | Forschungsstation Neumayer III, Antarktis | regionales Klimaprofil |

Die vollständigen Koordinaten liefert `GET /locations`; Hauptquelle ist `Simulation/LocationCatalog.cs`.

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

Die österreichischen Standorte behalten diese vereinfachte Modellbasis. WeatherSynth.Default wurde mit Solarwerten
aus Bochum und Windwerten aus Essen-Bredeney gefittet und wird deshalb nicht als Klimabasis auf Wüste/Antarktis übertragen.

### Klimaprofile für Sahara und Antarktis

Die neuen Orte verwenden `ClimateProfile` und dasselbe deterministische Modell für HTTP und MQTT. Keine Laufzeit-Downloads.

| Standort | Referenzgeometrie | Temperaturbasis | Niederschlag |
|---|---|---|---|
| Tamanrasset | 22,80° N, 5,43° E, 1362 m | monatliche JMA-Normalwerte | monatliche JMA-Normalwerte (Summe 56,2 mm/Jahr) |
| Neumayer III | ca. 70,667° S, 8,267° W, nominal 40 m | gerundete saisonale Näherung an BAS-Monatsbeobachtungen | synthetische Annahme 370 mm Wasseräquivalent/Jahr |

Tamanrasset: [JMA ClimatView, Station 60680](https://www.data.jma.go.jp/tcc/tcc/products/climate/climatview/graph_mkhtml.php?d=0&e=0&k=0&m=7&n=60680&r=0&s=1&y=2026).
Verwendet werden die **Normal**-Spalten, keine Beobachtungen einzelner Jahre; Bezugsperiode wird auf der verwendeten
Ansicht nicht ausgewiesen. Temperatur Jan–Dez (°C): 12,8 / 15,3 / 19,2 / 23,8 / 27,7 / 30,0 / 30,1 / 29,1 / 27,9 / 23,8 / 18,2 / 14,4.
Niederschlag (mm): 1,1 / 0,9 / 5,2 / 0,1 / 1,5 / 5,7 / 7,5 / 18,5 / 6,5 / 8,2 / 0,4 / 0,6.

Neumayer: [BAS READER, Monatsbeobachtungen seit 1981](https://legacy.bas.ac.uk/met/READER/surface/Neumayer.All.temperature.html),
[AWI Stationsbeschreibung](https://www.awi.de/en/fleet-stations/stations/neumayer-station-iii.html).
Die verwendete Temperaturkurve ist eine **eigene gerundete Näherung**, kein berechnetes Klimanormalmittel:
−4,2 / −8,0 / −13,0 / −17,5 / −20,5 / −23,5 / −25,0 / −25,0 / −23,0 / −17,5 / −10,0 / −4,8 °C.
Der Referenzpunkt ist fest, obwohl die Station auf dem Schelfeis wandert; die nominale Höhe ist eine Modellannahme.
Die südliche Jahreszeitenlage folgt dieser Monatskurve.

**Formeln:** Zwischen Monatsmitten wird linear interpoliert. Temperatur = Monatskurve + geseedeter Tagesoffset
(Tamanrasset ±3 K, Neumayer ±6 K) + lokaler solarer Tagesgang (Amplitude 7 bzw. 1,5 K, durch Wolken gedämpft).
In Polarnacht entfällt der solare Tagesgang. Bewölkung = regionaler Mittelwert (20 bzw. 70 %) plus geseedete Tagesabweichung
±35 Prozentpunkte, auf 0–100 % begrenzt. Das sind vereinfachende Annahmen, keine angepassten Messverteilungen.

Wind wird als Rayleigh-Verteilung mit Mittel 4,0 m/s (Sahara, **Annahme**) bzw. 8,7 m/s (Neumayer,
[Beobachtungsstudie](https://tc.copernicus.org/articles/15/4745/2021/tc-15-4745-2021.html)) erzeugt.
Stündliche Windwerte sind unabhängig; reale zeitliche Persistenz und Katabatik werden nicht modelliert.
Niederschlag tritt mit Wahrscheinlichkeit `Monatsmenge / (Monatsstunden · 0,5 mm)` auf;
Mengen sind exponentiell mit Mittel 0,5 mm pro nasser Stunde. Damit entspricht die monatliche Menge im Erwartungswert
der Vorgabe, ein einzelner Lauf darf abweichen. Nasse Stunden erhalten mindestens 85 % Bedeckung.
Neumayers Niederschlagsverteilung und beide Wolken-/Tagesgangparameter sind ausdrücklich synthetisch.

Klarhimmelstrahlung: `1098 · cos(Z) · exp(−0,059/cos(Z))` nach
[Haurwitz, Referenzimplementierung pvlib](https://github.com/pvlib/pvlib-python/blob/main/pvlib/clearsky.py);
bei Sonnenhöhe ≤ 0 exakt 0. Wir verwenden die geometrische Sonnenhöhe ohne Refraktionskorrektur.
Wolkendämpfung `1 − 0,75 · (Bedeckung/100)^3,4` ist eine vereinfachte Modellannahme; lokale Aerosole, Staub,
Albedo und Mehrfachreflexion an Schnee werden nicht kalibriert. Keine wissenschaftlich validierte Polareinstrahlung.

MQTT führt zusätzlich `isDaylight`. Die oben beschriebene Zustandspriorität gilt auch an österreichischen Standorten;
deren numerische Wetterbasis bleibt sonst unverändert.

## MQTT-Betrieb und Viewer

| Option | Standard | Wirkung |
|---|---|---|
| `--port <n>` | 8080 | HTTP-Port, 1–65535 |
| `--mqtt` | aus | MQTT-Ausgang und Master-Anbindung aktivieren |
| `--master <host:port>` | `127.0.0.1:5100` | Zeitquelle des MQTT-Stroms; ohne `--mqtt` keine Verbindung |
| `--broker <host:port>` | `127.0.0.1:1883` | MQTT-Ziel |
| `--env-file <pfad>` | `<Repo>/docker/.env` | Datei für MQTT-Zugangsdaten |

`--mqtt` aktiviert den zusätzlichen Ausgang und die Master-Anbindung. `--env-file <pfad>` überschreibt den Standard
`docker/.env`. `ENERGYTWIN_MQTT_USER`/`ENERGYTWIN_MQTT_PASSWORD` haben Vorrang. Zugangsdatenauflösung gemeinsam
mit dem Gateway in `Shared.Config`; kein Passwort auf der Kommandozeile.

Einmal pro realer Sekunde neueste Wetterstände aller Orte. Pause: einmal `paused`, keine regelmäßigen Werte;
manuelle Änderungen senden den betroffenen Standort einmal. Fortsetzen: einmal `continued`. Master fehlt/verloren:
`noTime`/`masterLost`, keine neuen Wetterwerte. Neue RunId oder neue Master-Sitzung löschen manuelle Vorgaben.
Broker-Ausfall beeinträchtigt HTTP nicht; automatische Wiederverbindung, keine Nachlieferung alter Messwerte.

MQTT-Viewer: `127.0.0.1:1883`, Zugangsdaten aus `.env`; für gespeicherte Zustände die sechs konkreten Topics aus dem
[Wettervertrag](../../docs/Wetter-Nachrichtenvertrag-LB03.md) abonnieren. Der MQTT-Status muss bei der Gültigkeitsbewertung
gespeicherter Wetterwerte berücksichtigt werden. Es darf nur ein aktiver Wetter-Publisher diese Topics bedienen.

Gemeinsamer Start: `.\start-all.ps1 -Autostart -NoClient -WeatherMqtt` (Broker vorher starten).
Ein bereits laufender Wetterdienst wird vom Startskript übersprungen; für MQTT muss dieser mit `--mqtt` gestartet sein.

## Tests

```bash
dotnet test tests/Weather.Tests
```

11 bestehende Prüffälle (Übersteuerung jetzt standortbezogen) plus 7 neue regionale/Options-Prüffälle.
In diesem Arbeitsschritt nur gebaut, kein neuer Unit-Testlauf gemäß Projektregel.
Echter RabbitMQ-Funktionsnachweis mit Pause, manuellen Änderungen und Wiederanlauf:
[`docs/nachweise/2026-11-08-wetter-mqtt/`](../../docs/nachweise/2026-11-08-wetter-mqtt/README.md).

## Bekannte Probleme

- WeatherSynth ist ein zusätzliches NuGet-Paket (AGPL-3.0; SolarCalculator LGPL; DWD-Daten CC BY 4.0) und wird beim
  ersten Build geladen. Die Laufzeit braucht kein Internet.
- Ortszeit-Anzeige fest auf `Europe/Vienna`; die Schnittstelle selbst ist reine UTC.
- Es gibt keine Persistenz: ein Neustart setzt die manuelle Übersteuerung zurück.
- Die Anzeige ist nur mit umgeleiteter Ausgabe geprüft; ein echtes Terminalfenster ist noch zu testen.