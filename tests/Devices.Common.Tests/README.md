# Devices.Common.Tests

**Zweck:** Tests der gemeinsamen Gerätebausteine in `src/Devices.Common`.
**Verantwortlich:** Susanne Lastowicka für Zeit/Modbus/Dual-Charger, Alexander Lux für Wetter/PV-Anlage, Dominik für Batterie/Verbraucher.
Zuordnung entsprechend [Devices.Common](../../src/Devices.Common/README.md).
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

Die spätere Erweiterung `WeatherFeedTests` enthält Wetter-Cache, Wiederholungsversuche/Fehlermeldungen und
Wetteroptionen (Standard mit Master, standalone aus, `off`, Standortkennung).

Letzter dokumentierter Lauf am **2026-11-08: 51/51**, siehe
[Susannes Journal](../../docs/development-journal/susanne-lastowicka.md). Seitdem wurde unter anderem
`DeviceTimeSource.MasterSession` ergänzt; kein neuer Lauf im Rahmen dieser README-Pflege.

## Bekannte Probleme
- `DeviceTimeSourceTests.WithMaster_FollowsMasterClock` und die Modbus-Tests brauchen freie Ports auf localhost (die Modbus-Tests suchen sich einen freien Port).
