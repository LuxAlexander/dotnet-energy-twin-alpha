# Modbus.Contracts

**Zweck:** Vertrag der Modbus-Register aller Geräte: gemeinsamer Registerkopf (Adressen 0–19), Bereich für gerätespezifische
Register (20–49), Datentypen und Codierung, `RegisterPlan` mit Prüfung und Markdown-Ausgabe. Keine Netzwerkabhängigkeit.
**Verantwortlich:** Susanne Lastowicka (Entwurf 2026-10-25, im Team zu bestätigen – der Kopf ist die Schnittstelle zu den Geräten der anderen)
**Basis aus LB01:** –

## Bausteine

| Klasse | Aufgabe |
|---|---|
| `CommonRegisters` | Adressen und Definitionen des Kopfs 0–19, Bereichsgrenzen (`SpecificStart` 20, `SpecificEnd` 50) |
| `RegisterDefinition` | Ein Eintrag: Adresse, Name, Typ (`UInt16`, `UInt32`, `Float32`), Zugriff, Einheit, Skalierung, Schreibbereich, `Labels` (Klartext zu Zahlenwerten); `Validate(wert)` |
| `ChargerRegisters`, `PvRegisters`, `BatteryRegisters` | Adressen und `RegisterPlan` der Geräte (Säule 20–37, PV 20–38, Batterie 20–43); gemeinsam von Gerät und Diagnose-Client benutzt |
| `RegisterPlans` | `For(DeviceType)` liefert den Plan zum Gerätetyp (Charger, PV, Batterie); neue Geräte (Wind) dort eintragen |
| `RegisterPlan` | Kopf + gerätespezifische Register; prüft Überschneidungen, `Find(adresse)`, `ToMarkdown()` für die Geräte-README |
| `RegisterCodec` | Wert ↔ Register: Float32/UInt32 Big-Endian, höherwertiges Wort zuerst; Modellzeit als Sekunden seit 1970 |
| `DeviceType`, `OperatingState`, `TimeStatus`, `WriteError` | Enums der Kopfregister |

## Gemeinsamer Registerkopf (Holding Registers, 0-basierte Protokolladressen)

| Adresse | Name | Typ | Zugriff |
|---|---|---|---|
| 0 | Gerätetyp (1 PV, 2 Batterie, 3 Charger, 4 Wind) | UInt16 | lesen |
| 1 | Geräte-Nummer (Zahl am Ende der Geräte-ID) | UInt16 | lesen |
| 2 | Betriebszustand (0 Aus, 1 Bereit, 2 Betrieb, 3 Störung) | UInt16 | lesen |
| 3 | Fehlercode (0 = kein Fehler, sonst gerätespezifisch) | UInt16 | lesen |
| 4–5 | Wirkleistung kW (positiv = Aufnahme, negativ = Abgabe) | Float32 | lesen |
| 6–7 | Kumulierte Energie kWh | Float32 | lesen |
| 8–9 | Modellzeit, Sekunden seit 1970-01-01 00:00 (ohne Zeitzone) | UInt32 | lesen |
| 10 | Lauf (ändert sich bei Neustart im Master) | UInt16 | lesen |
| 11 | Sequenznummer (wächst mit neuer Modellzeit) | UInt16 | lesen |
| 12 | Zeitstatus (0 keine Zeit, 1 synchron, 2 standalone, 3 Master verloren) | UInt16 | lesen |
| 13 | Simulation läuft (1/0) | UInt16 | lesen |
| 14–15 | Tempo | Float32 | lesen |
| 16 | Ein/Aus | UInt16 | lesen/schreiben (0, 1) |
| 17 | Fehler zurücksetzen (Schreiben von 1) | UInt16 | lesen/schreiben |
| 18 | Letzter Schreibfehler (0 ok, 1 nicht beschreibbar, 2 ungültiger Wert, 3 unvollständig) | UInt16 | lesen |
| 19 | Adresse des letzten Schreibzugriffs | UInt16 | lesen |

Mindestanforderungen der Aufgabenstellung (Betriebszustand, Fehlercode, Wirkleistung, Energie, Zeit/Sequenz, Ein/Aus, Sollwert,
Reset) sind damit abgedeckt; der Sollwert liegt im gerätespezifischen Bereich ab 20. Jedes Gerät belegt dort mindestens drei Werte.
Abweichung von der ersten Planung (Kopf 0–9, spezifisch 10–29): 0–9 reichten für Pflichtinhalt plus Zeit und Steuerung nicht.

## Konfiguration
Keine. Host, Port und Unit-ID legt jedes Gerät fest (`Devices.Common.Modbus.ModbusOptions`).

## Bedienung / Start
Bibliothek ohne eigenes Programm.

## Schnittstellen
Benutzt von `Devices.Common` (Server) und vom `ModbusDiagnosticClient` (Dekodieren, Plan über `RegisterPlans.For`). Neues Gerät: `new RegisterPlan(DeviceType.X, "Name", spezifischeRegister)`.

## Tests
`tests/Devices.Common.Tests` (`RegisterCodecTests`, `ModbusDeviceServerTests`), `tests/ModbusDiagnosticClient.Tests`.

## Bekannte Probleme
- Der Kopf muss mit den Geräten von Alexander und Dominik abgestimmt werden, bevor deren Registerpläne entstehen.
- Das Vorzeichen der Wirkleistung (positiv = Aufnahme) ist für Erzeuger (PV, Wind) gewöhnungsbedürftig: dort negative Werte.
- Skalierung (`Scale`) ist vorgesehen, aktuell nutzen alle Register Rohwert = technischer Wert.
