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
| `MqttPublisherTests` | gegen einen MQTT-Broker im Testprozess (`MQTTnet.Server`, kein Docker nötig): QoS 1, Retain nur für Status · falsches Passwort ⇒ verständlicher Fehler ohne Passwort · Broker-Ausfall ⇒ sofortiger Fehler, Wartezeit vor dem Neuversuch, nach Wiederkehr Status nachgeholt (auch aus der Ausfallzeit), Messwert aus der Ausfallzeit verworfen · `GatewayService` liest bei Broker-Ausfall weiter und meldet Ausfall/Wiederkehr je einmal |
| `MqttCredentialsTests` | Reihenfolge Option → Umgebung → `.env`, `.env`-Format mit Kommentar und Anführungszeichen, anonym, Passwort nie in `ToString`, `--mqtt-password` abgelehnt, `--broker`/`--console`/`--env-file` |
| `GatewayAgainstDeviceTests` | gegen echten `ModbusDeviceServer` (Ladesäule): Lesen und Dekodieren · Gerät weg ⇒ `offline` ohne Messwerte, Status nur bei Änderung, Gerät zurück ⇒ wieder `online` · Status-Wiederholung nach dem Intervall mit unverändertem `sinceUtc` · `GatewayService --once`: Reihenfolge und Retain-Flag der Topics, Gateway `online`/`offline` |

**Schlüssel ändern:** Schlägt `Keys_MatchTheRecordedCatalog` an, hat sich ein Registername geändert. Wenn das gewollt
ist: `dotnet run --project src/ModbusRabbitGateway -- --keys > tests/ModbusRabbitGateway.Tests/measurement-keys.txt`
und die Änderung im Team bekanntgeben (spätere Zeitreihen ändern ihren Namen).

Ergebnis am 2026-11-08: 52 von 52 bestanden (zweimal wiederholt). Gefundene und behobene Fehler: (1) beim Geräteausfall
kam eine zweite `offline`-Statusnachricht, weil der Fehlertext wechselte (bei `offline` zählt jetzt nur der Zustand);
(2) MQTTnet 5 wirft bei abgelehnter Anmeldung keine Exception, sondern liefert einen Ergebniscode – das Gateway hätte
falsche Zugangsdaten nur als unklaren Sendefehler gemeldet (Ergebniscode wird jetzt ausgewertet).

## Bekannte Probleme
- Die Tests mit echtem Server brauchen freie Ports auf localhost und dauern wegen der Verbindungs-Zeitlimits einige Sekunden.
