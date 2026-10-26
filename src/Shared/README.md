# Shared – gemeinsame Bausteine

Kleine, geräteunabhängige Hilfen, die mehrere Feldgeräte verwenden können. Das Projekt hat keine
Abhängigkeiten und wird von den Geräten per `ProjectReference` eingebunden.

**Verantwortlich:** Alexander Lux  
**Team:** Alpha

## Inhalt

| Namespace | Datei | Zweck |
|---|---|---|
| `Shared.Data` | `Data/CsvFileWriter.cs` | Asynchroner CSV-Writer: Datei mit Zeitstempel anlegen, Kopfzeile schreiben, Zeilen anhängen, Felder maskieren |
| `Shared.Output` | `Output/BarChart.cs` | Balken in gefüllten und leeren Teil aufteilen (`Split`) |
| `Shared.Weather` | `Weather/Season.cs` | Jahreszeit aus dem Monat, deutsche Bezeichnung, Tageslänge, Einstrahlungsfaktor, Basistemperatur, Schneewahrscheinlichkeit |
| `Shared.Weather` | `Weather/WeatherCondition.cs` | Lokale Wetterzustände und deutsche Bezeichnung |
| `Shared.Weather` | `Weather/IrradiationCalculator.cs` | Idealisierte Einstrahlung aus Uhrzeit, Wetter und Jahreszeit |

Das lokale Wettermodell ist nur für Standalone-Läufe gedacht. Mit Simulations-Master beziehen die
Geräte das Wetter über `Devices.Common.Weather` vom Wettersimulator.

## Verwendung

```csharp
using Shared.Data;
using Shared.Output;
using Shared.Weather;

var chart = BarChart.Split(42, 100, 20);
var season = SeasonInfo.FromMonth(DateTime.Now.Month);

await using var csv = await CsvFileWriter.CreateAsync(
    "PV-01", "docs/data", "Datum;Zeit;Leistung_kW", cancellationToken);
await csv.WriteRowAsync(["2026-10-26", "12:00", "4,2"], cancellationToken);
```

Die Geräte bleiben für ihre Spalten und ihre Anzeige selbst verantwortlich; hier liegen nur die
wiederverwendbaren Grundlagen.
