# Batteriespeicher-Simulator

Konsolenanwendung in C# (.NET), die einen **Batteriespeicher** als eigenständiges, simuliertes Gerät abbildet. Der Speicher wird mit Leistungsvorgaben geladen und entladen, der Ladezustand (SOC) wird über die Simulationszeit berechnet. Im Gesamtprojekt wird er später mit der PV-Anlage und der Ladestation zusammengeführt. Dieses Programm simuliert das Gerät einzeln.

## Voraussetzungen

- **.NET 10 SDK** (<https://dotnet.microsoft.com/download>). Prüfen mit `dotnet --version`.
- Windows-Konsole (Visual-Studio-Debugging-Konsole, Windows Terminal oder Eingabeaufforderung), mindestens 80 Zeichen breit und 15 Zeilen hoch.
- Die Konsole muss **interaktiv** sein (Tastatureingaben). Die Eingabe darf nicht umgeleitet werden.

## Starten

Im Projektordner:

```
dotnet run -- <Argumente>
```

Beispiel:

```
dotnet run -- --id BAT-01 --capacity 10 --start 06:00
```

In Visual Studio: *Projekt → Eigenschaften → Debuggen → Befehlszeilenargumente*, dort die Argumente eintragen (ohne `dotnet run --`).

Ohne Argumente läuft die Simulation mit den Standardwerten (siehe Tabelle) über 24 Simulationsstunden, das sind bei Standardgeschwindigkeit etwa 2,5 Minuten Echtzeit.

### Kommandozeilenargumente

| Argument | Bedeutung | Standard |
|---|---|---|
| `--id` | Geräte-ID (auch im CSV-Dateinamen) | BAT-01 |
| `--capacity` | Kapazität in kWh | 20 |
| `--max-charge` | max. Ladeleistung in kW | 5 |
| `--max-discharge` | max. Entladeleistung in kW | 5 |
| `--soc-min` | SOC-Untergrenze in % | 10 |
| `--soc-max` | SOC-Obergrenze in % | 100 |
| `--soc-start` | Start-SOC in % | 50 |
| `--speed` | Simulationssekunden pro Realsekunde | 600 |
| `--step` | Schrittweite in Simulationssekunden | 60 |
| `--duration` | Simulationsdauer in Stunden, `0` = endlos | 24 |
| `--start` | Startuhrzeit (z. B. `06:00`) | 00:00 |

Hinweise:

- Dezimalzahlen werden in den Argumenten mit **Punkt** geschrieben (`--capacity 7.5`).
- Ungültige Eingaben beenden das Programm mit einer Fehlermeldung. Geprüft werden: `--speed` und `--step` größer als 0, `--capacity` größer als 0, `--soc-min` kleiner als `--soc-max`.

### Beispielparameter

| Zweck | Aufruf |
|---|---|
| Standardlauf | `dotnet run` |
| Schneller Demolauf (24 h in ca. 24 s) | `dotnet run -- --speed 3600` |
| Kleine Batterie, ganzer Zyklus ab Morgen | `dotnet run -- --capacity 10 --start 06:00 --soc-start 20` |
| Leistungsbegrenzung sichtbar machen | `dotnet run -- --max-charge 2 --max-discharge 2` |
| Feinere Zeitauflösung | `dotnet run -- --step 10 --speed 600` |
| Dauerbetrieb (Tagesprofil wiederholt sich) | `dotnet run -- --duration 0` |
| Zweites Gerät parallel (zweites Terminal) | `dotnet run -- --id BAT-02 --capacity 10` |

## Bedienung

Während der Simulation zeigt ein festes Dashboard Simulationszeit, Tempo, Modus, Gerätezustand, Soll- und Ist-Leistung, SOC (Zahl und Balken), Temperatur, aktive Störung sowie Energie- und Zykluszähler.

### Tasten im laufenden Betrieb

| Taste | Funktion |
|---|---|
| Leertaste | Pause, öffnet das Pausenmenü |
| M | Umschalten Auto / Manuell (beim Wechsel auf Manuell wird der Sollwert 0) |
| Pfeil hoch / runter | Sollwert um 0,5 kW erhöhen / senken (nur im Manuell-Modus) |
| 0 | Sollwert auf 0 kW (nur im Manuell-Modus dauerhaft, im Auto-Modus überschreibt das Profil ihn wieder) |
| E | Gerät ein- bzw. ausschalten |
| N | Not-Aus auslösen |
| H | Überhitzung auslösen |
| R | Störung zurücksetzen |
| Q | Beenden |

Zum Beenden immer **Q** drücken (oder die Simulationsdauer ablaufen lassen). Wird der Debugger hart gestoppt, kann das Ende der CSV-Datei fehlen.

### Pausenmenü

Die Leertaste hält die gesamte Simulation an (Uhr, SOC, Temperatur, Zähler). Im Menü sind änderbar:

1. Sollwert (wechselt dabei in den Manuell-Modus)
2. Maximale Ladeleistung
3. Maximale Entladeleistung
4. SOC-Untergrenze
5. SOC-Obergrenze
6. Simulationsgeschwindigkeit
7. Schrittweite
8. Fortsetzen

Im Menü werden Dezimalzahlen mit der **Schreibweise des Betriebssystems** eingegeben, bei deutschen Windows-Einstellungen also mit **Komma** (`3,5`). Ungültige Werte werden abgelehnt und der alte Wert bleibt bestehen. Die Zeit, die man im Menü verbringt, wird nicht nachgeholt.

## Modellbeschreibung

### Grundprinzip

Vorzeichen: **positive Leistung = Laden, negative Leistung = Entladen.** Das Gerät bekommt einen **Sollwert**, entweder vom Tagesprofil (Auto), per Tasten oder über das Pausenmenü (Manuell). Es liefert die **Ist-Leistung**, also das, was es unter den aktuellen Bedingungen darf. Unzulässige Sollwerte werden nicht abgelehnt, sondern still begrenzt, so wie es ein Batteriemanagementsystem tut. Soll und Ist werden getrennt angezeigt und protokolliert.

### Begrenzung der Leistung

```
erlaubt_laden    = max_laden    × Taper(SOC) × Derating(T)
erlaubt_entladen = max_entladen × Derating(T)

Ist = Soll, begrenzt auf  −erlaubt_entladen … +erlaubt_laden
```

Zusätzlich gilt:

- Bei SOC an der Obergrenze ist Laden gesperrt, bei SOC an der Untergrenze ist Entladen gesperrt.
- Innerhalb eines Schritts wird nur so viel Energie umgesetzt, wie bis zur Grenze noch hineinpasst (bzw. entnommen werden kann). Der SOC überschreitet die Grenzen deshalb auch bei großen Schrittweiten nicht.
- Bei Aus, Not-Aus oder Überhitzungsabschaltung ist die Ist-Leistung 0.

### Ladezustand (SOC)

```
SOC(neu) = SOC + Ist-Leistung × Δt[h] / Kapazität[kWh] × 100
```

### Leistungsreduzierung bei hohem SOC (Taper)

Nur beim Laden: Bis 80 % SOC volle Leistung, danach sinkt die erlaubte Ladeleistung linear auf 10 % der Maximalleistung bei der SOC-Obergrenze. Die Ladekurve läuft dadurch flach aus.

### Temperatur und Derating

Die Temperatur ändert sich pro Simulationsminute um

```
ΔT = ( 0,06 × |Ist-Leistung|  −  0,02 × (T − 20 °C)  +  Störungswärme ) × Δt[min]
```

Bei dauerhaft 5 kW pendelt sich die Temperatur bei etwa 35 °C ein.

| Temperatur | Verhalten |
|---|---|
| unter 45 °C | volle Leistung |
| 45 bis 60 °C | Leistung sinkt linear von 100 % auf 0 % (Derating, gilt für Laden und Entladen) |
| ab 60 °C | Abschaltung, Zustand `Fault` |
| nach Reset bis 50 °C | Zustand `Cooldown`, gesperrt |
| 50 °C oder darunter | wieder freigegeben |

### Gerätezustände

`Off`, `Standby`, `Charging`, `Discharging`, `Fault`, `Cooldown`. Laden und Entladen ergeben sich aus der Ist-Leistung (Totzone ±0,01 kW).

### Störungen

| Störung | Auslösen | Wirkung | Zurücksetzen |
|---|---|---|---|
| Not-Aus | Taste N | Ist-Leistung sofort 0, Sollwert wird 0, Zustand `Fault` | Taste R, danach `Standby` |
| Überhitzung | Taste H | zusätzliche Wärmequelle (2 °C pro Minute), Derating ab 45 °C, Abschaltung bei 60 °C | Taste R schaltet die Wärmequelle ab, bis 50 °C `Cooldown`, danach frei |

Im Auto-Modus bleibt das Profil während einer Störung aktiv, die Ist-Leistung ist aber 0. Nach dem Reset läuft es an der aktuellen Simulationszeit weiter, in der CSV entsteht dadurch eine sichtbare Lücke. Eine Überhitzung allein durch Last erreicht wegen des Deratings praktisch nie 60 °C. Die manuelle Störung ist der vorgesehene Weg, sie auszulösen.

### Zähler

Geladene und entladene Energie in kWh sowie äquivalente Vollzyklen: `(geladen + entladen) / (2 × Kapazität)`.

### Auto-Modus: Tagesprofil

Das Profil liefert den Sollwert abhängig von der Tageszeit. Es hängt an der Simulationsuhr und wiederholt sich im Dauerbetrieb täglich.

| Zeit | Sollwert |
|---|---|
| 00:00 bis 07:00 | 0 kW |
| 07:00 bis 17:00 | Laden, Sinuskurve mit Maximum um 12:00 |
| 17:00 bis 18:00 | 0 kW |
| 18:00 bis 23:00 | Entladen, Grundlast mit Spitze um 19:30 |
| 23:00 bis 24:00 | 0 kW |

Auf den Sollwert wird ein Zufallsfaktor von ±10 % gelegt, der alle 5 Simulationsminuten neu gewürfelt wird. Das Profil beschreibt nur den Wunsch, die Begrenzungen des Geräts greifen weiterhin. Bei den Standardwerten ist die Batterie deshalb am Vormittag voll und am Abend vor Ende des Profils leer.

### Simulationsuhr

Die Hauptschleife läuft alle etwa 100 ms in Echtzeit. Aus der vergangenen Realzeit und der eingestellten Geschwindigkeit wird Simulationszeit gutgeschrieben, und für jede volle Schrittweite wird ein Rechenschritt ausgeführt. Geschwindigkeit und Schrittweite sind damit unabhängig voneinander einstellbar. Bei Speed 600 und Schritt 60 s entspricht eine Realsekunde zehn Rechenschritten (zehn Simulationsminuten). Während der Pause wird keine Zeit gutgeschrieben.

## CSV-Verlauf

Pro Gerät entsteht eine Datei `battery_<ID>_<Datum>.csv` im Arbeitsverzeichnis des Programms (bei Visual Studio im Ordner `bin\Debug\net…`). Es wird **bei jedem Simulationsschritt** eine Zeile geschrieben, mit dem Zustand am Ende des Schritts. Trennzeichen ist das Semikolon, Dezimalzeichen der Punkt.

| Spalte | Bedeutung |
|---|---|
| `SimTime` | Simulationszeit (Tag.Stunde:Minute:Sekunde, z. B. `1.06:15:00`) |
| `DeviceId` | Geräte-ID |
| `Mode` | Gerätezustand (Off, Standby, Charging, Discharging, Fault, Cooldown) |
| `Control` | Auto oder Manual |
| `SetpointKW` / `ActualKW` | Soll- und Ist-Leistung |
| `SOC` | Ladezustand in % |
| `TempC` | Temperatur in °C |
| `Fault` | aktive Störung (None, EmergencyStop, Overheat) |
| `MaxChargeKW` / `MaxDischargeKW` | aktuelle Leistungsgrenzen |
| `SocMin` / `SocMax` | aktuelle SOC-Grenzen |

Die CSV darf während des Laufs nicht in Excel geöffnet sein. Zwei Instanzen mit **derselben ID** am selben Tag blockieren sich gegenseitig, daher pro Instanz eine eigene `--id` verwenden.

## Aufbau

| Klasse | Aufgabe |
|---|---|
| `Batterie` | Physik des Geräts: Begrenzung, SOC, Temperatur, Zustände, Störungen, Zähler |
| `BatterieConfig` | Einstellungen (ID, Kapazität, Leistungen, SOC-Grenzen) |
| `DailyProfile` | Tagesprofil mit Rauschen für den Auto-Modus |
| `Simulator` | Simulationsuhr, Hauptschleife, Tasten, Pausenmenü |
| `CsvLogger` | schreibt den Verlauf |
| `ConsoleDashboard` | feste Statusanzeige |
| `Program` | liest die Argumente und startet die Simulation |

`Batterie` kennt weder Konsole noch Uhr noch Profil und nimmt Leistungsvorgaben ausschließlich über `SetPower(kW)` entgegen. Dadurch kann später ein externer Controller (z. B. bei der Kopplung mit PV-Anlage und Ladestation) den Sollwert vorgeben.

## Bekannte Einschränkungen

- Wirkungsgrad, Selbstentladung, Spannung/Strom und Alterung (SOH) sind nicht modelliert.
- Die Kopplung mit PV-Anlage und Ladestation ist noch nicht umgesetzt. Das Gerät läuft eigenständig.
- Die Eingaben im Pausenmenü werden nur grob geprüft, Dezimalzahlen müssen in der Schreibweise des Betriebssystems eingegeben werden.
- Ein Ereignisprotokoll im Dashboard und eine Zusammenfassung am Programmende gibt es nicht.
![alt text](bild.png)