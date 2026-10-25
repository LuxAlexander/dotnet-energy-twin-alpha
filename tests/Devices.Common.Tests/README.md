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
| `ModbusDeviceServerTests` | Echter Server + Client über TCP: Kopf lesbar (Typ, Instanz, Zustand, Leistung, Energie, Modellzeit, Lauf, Zeitstatus) · Sequenznummer wächst nur mit neuer Modellzeit/Lauf · gültiger Sollwert wird angenommen und als Befehl gereicht · ungültige Werte (zu klein/groß, negativ, NaN, ∞) abgelehnt, alter Wert bleibt, Fehlerregister gesetzt · Schreiben auf nur lesbare/unbelegte Adresse abgelehnt · halber Float abgelehnt · Zugriff über lesbar+schreibbar komplett abgelehnt · Ein/Aus und Reset als Befehle, ungültige Werte dafür abgelehnt · Register in der Pause lesbar mit letztem Stand · belegter Port ⇒ `IOException` |
| `RegisterCodecTests` | Float32/UInt32 Big-Endian, höheres Wort zuerst · Modellzeit hin und zurück · Registerplan lehnt Überschneidungen und falsche Bereiche ab · Bereichsprüfung (Typ, Grenzen, NaN) |

Ergebnis am 2026-10-25: 44 von 44 bestanden.

## Bekannte Probleme
- `DeviceTimeSourceTests.WithMaster_FollowsMasterClock` und die Modbus-Tests brauchen freie Ports auf localhost (die Modbus-Tests suchen sich einen freien Port).
