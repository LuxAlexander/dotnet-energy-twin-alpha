# SimulationMaster.Tests

**Zweck:** Tests der bestätigten Master-Steuerung über MQTT (`src/SimulationMaster`) ohne Broker und ohne Prozessstart.
**Verantwortlich:** Alexander Lux (Web-/Master-Bedienung)
**Basis aus LB02:** `MasterClock` und Zeitverträge aus [SimCore.Tests](../SimCore.Tests/README.md).

## Ausführen

```text
dotnet test tests/SimulationMaster.Tests
```

Die Tests sprechen keinen Broker an. Sie rufen die interne Befehlsverarbeitung direkt auf; Sitzung, Sequenz,
Ablaufzeit und der Duplikatcache werden mit der echten Uhr geprüft.

## Tests

| Klasse | Prüft |
|---|---|
| `MasterMqttServiceTests` | Doppelt zugestellter Neustart löst genau einen Lauf aus · gleiche Befehls-ID mit anderem Inhalt wird abgelehnt · doppelter Tempobefehl wirkt einmal · Pause/Fortsetzen · veraltete Sequenz und falsche Sitzung abgelehnt · abgelaufene und überlange Gültigkeit abgelehnt · unbekannte Aktion und ungültiges Tempo abgelehnt, Zustand unverändert |

Ergebnis am 2026-11-10: 11 von 11 bestanden.

## Grenzen

Kein MQTT-Transport, kein Broker-Ausfall und kein Browser-Klicktest. Diese Prüfungen bleiben dem Gesamtprobelauf
vorbehalten und stehen im [Journal](../../docs/development-journal/alexander-lux.md).
