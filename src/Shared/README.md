# Shared – gemeinsame Bausteine

Kleine, geräteunabhängige Hilfen, die mehrere Feldgeräte verwenden können. Das Projekt hat keine
Abhängigkeiten und wird von den Geräten per `ProjectReference` eingebunden.

**Verantwortlich:** Alexander Lux  
**Team:** Alpha

## Inhalt

| Namespace | Datei | Zweck |
|---|---|---|
| `Shared.Data` | `Data/CsvFileWriter.cs` | Asynchroner CSV-Writer: Datei mit Zeitstempel anlegen, Kopfzeile schreiben, Zeilen anhängen, Felder maskieren |
| `Shared.Data` | `Data/CsvFormat.cs` | Einheitliches CSV-Zahlenformat: de-AT (Dezimalkomma, Excel liest Punkt-Werte sonst als Datum), InvariantCulture für ISO-Datum/Uhrzeit |
| `Shared.Data` | `Data/CsvSchema.cs` | Gemeinsame Grundspalten der Geräte-CSVs: `Gerät;Datum;Zeit;Lauf`, darunter die gerätespezifischen Spalten |
| `Shared.Output` | `Output/BarChart.cs` | Balken in gefüllten und leeren Teil aufteilen (`Split`) |
| `Shared.Output` | `Output/ConsoleText.cs` | Zeilen auf feste Breite schneiden/füllen (`Fit`) – mehrfach in Geräte-Renderern dupliziert |
| `Shared.Weather` | `Weather/Season.cs` | Jahreszeit aus dem Monat, deutsche Bezeichnung, Tageslänge, Einstrahlungsfaktor, Basistemperatur, Schneewahrscheinlichkeit |
| `Shared.Weather` | `Weather/WeatherCondition.cs` | Lokale Wetterzustände und deutsche Bezeichnung |
| `Shared.Weather` | `Weather/IrradiationCalculator.cs` | Idealisierte Einstrahlung aus Uhrzeit, Wetter und Jahreszeit |

Das lokale Wettermodell dient als Ersatz bei ausgeschaltetem oder noch nicht verfügbarem HTTP-Wetterbezug.
Mit Master ist der Wetterdienst standardmäßig aktiv, standalone nur nach ausdrücklicher Angabe von `--weather`.
Der gemeinsame Zugriff liegt in [Devices.Common](../Devices.Common/README.md).

Die Helfer `CsvFormat`, `CsvSchema` und `ConsoleText.Fit` stehen den Feldgeräten bereit;
die PV nutzt sie bereits. Das Kühlhaus verwendet außerdem `CsvFileWriter`; die Batterie schreibt weiterhin mit
eigenem Logger und Dezimalpunkt. Die Verfügbarkeit eines Helfers bedeutet keine Umstellung aller Geräte.

## Verwendung

```csharp
using Shared.Data;
using Shared.Output;
using Shared.Weather;

var chart = BarChart.Split(42, 100, 20);
var season = SeasonInfo.FromMonth(DateTime.Now.Month);

await using var csv = await CsvFileWriter.CreateAsync(
    "PV-01", "docs/data", $"{CsvSchema.BaseHeader};Leistung_kW", cancellationToken);
await csv.WriteRowAsync(["PV-Anlage", "2026-10-26", "12:00", "1", "4,2"], cancellationToken);
```

Die Geräte bleiben für ihre Spalten und ihre Anzeige selbst verantwortlich; hier liegen nur die
wiederverwendbaren Grundlagen.

