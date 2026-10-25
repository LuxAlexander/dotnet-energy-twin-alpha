# ModbusDiagnosticClient.Tests

**Zweck:** Tests des Diagnose-Clients (`src/ModbusDiagnosticClient`) gegen einen echten Modbus-Server aus `Devices.Common`.
**Verantwortlich:** Susanne Lastowicka (wie der Client)
**Basis aus LB01:** –

## Ausführen

```
dotnet test tests/ModbusDiagnosticClient.Tests
```

## Tests

| Klasse | Prüft |
|---|---|
| `DiagnosticClientTests` | Lesen und Dekodieren von Kopf und Ladesäulen-Registern mit Rohwerten · gültiger Sollwert wird angenommen und kommt als Befehl im Gerät an · ungültige Werte (zu klein/groß, keine Zahl) und nur lesbare Register vom Client abgelehnt, nichts gesendet · mit `force` lehnt das Gerät ungültige, nur lesbare und unbelegte Adressen ab, Fehlerregister und alter Wert werden gemeldet · mehrdeutige/unbekannte Ziele · Ein/Aus (Klartext) und Reset · Verbindungsabbruch ⇒ Verbindungsfehler, nach Neustart des Geräts verbindet der nächste Zugriff neu · Fehler als Text statt Absturz · `watch` läuft bei Ausfall weiter · `quit` |
| `ClientOptionsTests` | Standardwerte, alle Optionen, mehrere `--write`, ungültige Werte, fehlender Wert |
| `RegisterFormatterTests` | Rohwert hex, Klartext/Einheit, Modellzeit sekundengenau (kein float-Rundungsfehler), Modellzeit 0 |

Ergebnis am 2026-10-25: 28 von 28 bestanden.

## Bekannte Probleme
- Die Tests brauchen freie Ports auf localhost (sie suchen sich einen freien Port) und dauern wegen der Verbindungs-Zeitlimits einige Sekunden.
