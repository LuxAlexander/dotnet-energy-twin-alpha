# FieldDevices.Tests

**Zweck:** Tests für PV-Anlage, Batteriespeicher und Kühlhaus: Modelle, Modbus-Anbindung, Optionen und Standalone-Läufe.
Die Ladesäulen-Anbindung wird über `Devices.Common.Tests` und Gateway-Tests mit abgedeckt.
**Verantwortlich:** Susanne Lastowicka
**Basis aus LB01:** –

## Ausführen

```
dotnet test tests/FieldDevices.Tests
```

## Tests

| Klasse | Prüft |
|---|---|
| `PvModbusTests` | Kopf mit negativer Leistung (Einspeisung) und PV-Registern · Leistungsbegrenzung gültig (wirkt im nächsten Schritt) und ungültig (abgelehnt, alter Wert) · Störung im Kopf, Reset und Ein/Aus per Modbus · Register in der Pause lesbar |
| `BatteryModbusTests` | Kopf mit positiver Ladeleistung und Batterie-Registern · Sollwert innerhalb der konfigurierten Leistung angenommen (Modus Manuell) · außerhalb/NaN abgelehnt · Modus Auto/Manuell, ungültiger Modus · Not-Aus im Kopf, Reset, Ein/Aus |
| `RegisterPlanRequirementTests` | jeder Gerätplan hat einen schreibbaren Sollwert mit Bereich und mindestens drei gerätespezifische Werte · Markdown nennt Byte-/Wortreihenfolge |
| `ArgsParserTests` | PV: Standard, Kürzel `-s`/`-d`, standalone-Pflicht, ungültige Werte · Batterie: Gerätedaten, Zeit, Modbus, Seed, ungültige Konfiguration, standalone-Pflicht |
| `PvEngineTests` | kompletter Standalone-Lauf mit Zeitraffer: CSV beginnt genau bei `--start`, Energie wächst, Abbruch beendet geordnet |
| `BatteryUnitTests` | `Update(TimeSpan)` = `Update(double)`, Tagesprofil mit Seed wiederholbar |
| Kühlhaus-Prüfklassen in `ConsumerTests.cs` | Modell, Modbus, Optionen, CSV/Engine, Anzeigeaufbau und Tür-Automatik; 28 zusätzliche Fälle, Einzelheiten im [Geräte-README](../../src/Devices/KuehlhausSimulator/README.md#tests) |


## Bekannte Probleme
- Die Modbus-Tests brauchen freie Ports auf localhost und warten auf Schreibzugriffe, die die Bibliothek erst nach der Antwort meldet (`WaitUntil`).
- Es gibt keinen Lauf des Batterie-Simulators bis zum Ende, weil er die CSV immer nach `docs/data` schreibt.