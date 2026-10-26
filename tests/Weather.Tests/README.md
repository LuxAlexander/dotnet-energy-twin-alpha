# Weather.Tests

**Zweck:** Pflicht-Tests für die Reproduzierbarkeit des Wetters: gleicher (Ort, Zeit, Lauf) ⇒ gleiches `WeatherData`.
Zusätzlich Plausibilität, Standortunterschiede, Nachtverhalten und die manuelle Übersteuerung.
**Verantwortlich:** Alexander Lux

## Tests

```bash
dotnet test tests/Weather.Tests
```

11 Tests in `WeatherModelTests`:

- `SameLocationTimeAndRun_ReturnsIdenticalWeather`, `MinuteWithinHour_DoesNotChangeWeather`
- `DifferentLocations_GiveDifferentWeather`, `DifferentRuns_GiveDifferentTemperature`
- `UnknownLocation_Throws`
- `Night_HasNoIrradiationAndNightCondition`, `SummerNoon_IsHigherThanWinterNoon`
- `ManualOverride_AppliesToAllLocations`, `RainPreset_ProducesPrecipitation`, `Clear_RemovesManualOverride`
- `Values_AreInPlausibleRanges` (Stichproben über das Jahr)

Die Tests laufen gegen `WeatherModel` direkt, ohne HTTP-Server; sie brauchen kein Netzwerk.
