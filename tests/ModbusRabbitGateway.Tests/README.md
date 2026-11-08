# ModbusRabbitGateway.Tests

**Zweck:** Tests des Modbus–RabbitMQ-Gateways (`src/ModbusRabbitGateway`) nach dem Nachrichtenvertrag.
**Verantwortlich:** Susanne Lastowicka (wie das Gateway)
**Basis aus LB01:** –

## Ausführen

```
dotnet test tests/ModbusRabbitGateway.Tests
```

## Tests

| Klasse | Prüft |
|---|---|
| `MessageBuilderTests` | Umschlag aus dem Registerkopf (Geräte-ID, Typ, virtuelle Zeit, Epoch, Lauf, Sequenz, Nachrichtenzähler) · virtuelle Zeit und Abfragezeit getrennt · Messwerte mit Schlüssel, Einheit, Klartext, Rundung · Schreibfehler-Register 18/19 enthalten, Umschlag-Register nicht · Qualität für synchron/pausiert/standalone/Master verloren/keine Zeit · Pause veröffentlicht weiter mit gleicher Zeit · `stale` nach Modellzeit (bei Tempo 600 ja, bei Tempo 1 nicht) · Neustart (neuer Lauf) · falscher Typ, falsche Geräte-Nummer, unbekannter Typ ⇒ keine Nachricht · NaN ⇒ `null` und `uncertain` · JSON camelCase mit Umlauten und °C |
| `MeasurementKeysTests` | Schlüsselbildung (Umlaute, Satzzeichen) · Schlüssel je Plan eindeutig und ASCII · alle Schlüssel gleich der festgehaltenen Liste `measurement-keys.txt` |
| `GatewayOptionsTests` | Standardgeräte und -werte · `--device` mit Unit-ID, Komma-Zahlen · Typ und Nummer aus der Geräte-ID · ungültige Werte und doppelte IDs abgelehnt |
| `GatewayAgainstDeviceTests` | gegen echten `ModbusDeviceServer` (Ladesäule): Lesen und Dekodieren · Gerät weg ⇒ `offline` ohne Messwerte, Status nur bei Änderung, Gerät zurück ⇒ wieder `online` · `GatewayService --once`: Reihenfolge und Retain-Flag der Topics, Gateway `online`/`offline` |

**Schlüssel ändern:** Schlägt `Keys_MatchTheRecordedCatalog` an, hat sich ein Registername geändert. Wenn das gewollt
ist: `dotnet run --project src/ModbusRabbitGateway -- --keys > tests/ModbusRabbitGateway.Tests/measurement-keys.txt`
und die Änderung im Team bekanntgeben (spätere Zeitreihen ändern ihren Namen).

Ergebnis am 2026-11-08: 41 von 41 bestanden (dreimal wiederholt). Der erste Lauf fand einen Fehler im Gateway: Beim Geräteausfall kam eine zweite `offline`-Statusnachricht, weil der Fehlertext wechselte; behoben (bei `offline` zählt nur der Zustand).

## Bekannte Probleme
- Die Tests mit echtem Server brauchen freie Ports auf localhost und dauern wegen der Verbindungs-Zeitlimits einige Sekunden.
