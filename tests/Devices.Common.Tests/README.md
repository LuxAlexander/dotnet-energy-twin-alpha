# Devices.Common.Tests

**Zweck:** Tests der gemeinsamen Gerätebausteine in `src/Devices.Common`.
**Verantwortlich:** _offen_ (wie `Devices.Common`). Am 2026-10-25 von Susanne Lastowicka erstellt.
**Basis aus LB01:** –

## Ausführen

```
dotnet test tests/Devices.Common.Tests
```

## Tests

| Datei | Prüft |
|---|---|
| `StepSchedulerTests` | Erste Zeit ohne Schritt · nur volle Schritte fällig, Modell hinkt höchstens einen Schritt nach · gleiche Schritte unabhängig davon, wie oft abgefragt wird · neuer Lauf springt auf die neue (auch frühere) Zeit · Uhr kurz hinter dem Modell ⇒ warten statt zurückspringen · zu viele fällige Schritte werden begrenzt und gemeldet · ungültige Schrittweite |
| `TimeSyncOptionsTests` | Standard 127.0.0.1:5100 · `--master` mit und ohne Port · ungültige Adressen · standalone mit `--start` und `--speed` (Punkt/Komma) · ungültige Werte · fremde Optionen werden nicht verbraucht |
| `DeviceTimeSourceTests` | Standalone läuft sofort mit lokaler Uhr · Pause/Tempo standalone · mit Master werden lokale Pause/Tempo abgelehnt · mit echtem Master über TCP: Gerät ist synchron, Abweichung innerhalb 10 ms · Tempo |

Ergebnis am 2026-10-25: 24 von 24 bestanden.

## Bekannte Probleme
- `DeviceTimeSourceTests.WithMaster_FollowsMasterClock` braucht einen freien Port auf localhost.
