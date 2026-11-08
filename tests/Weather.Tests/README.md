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
- `ManualOverride_AppliesOnlyToSelectedLocation`, `RainPreset_ProducesPrecipitation`, `Clear_RemovesManualOverride`
- `Values_AreInPlausibleRanges` (Stichproben über das Jahr)

Die Tests laufen gegen `WeatherModel` direkt, ohne HTTP-Server; sie brauchen kein Netzwerk.

Zusätzlich 7 Prüffälle in `RegionalWeatherTests`: Reproduzierbarkeit beider neuer Orte, Polartag/Polarnacht,
Schnee in Dunkelheit, standortbezogenes Zurücksetzen, ausdrückliche MQTT-Aktivierung und Ablehnung nicht endlicher Werte.
Stand 2026-11-08: neue Prüffälle gebaut, gemäß Projektregel noch kein neuer Unit-Testlauf. Die bisherige Angabe 11/11
bezieht sich auf den früheren Teststand. MQTT und HTTP wurden separat mit echtem RabbitMQ geprüft, siehe
[Funktionsnachweis](../../docs/nachweise/2026-11-08-wetter-mqtt/README.md).
