---
name: probelauf
description: Führt einen reproduzierbaren Funktionsnachweis (Probelauf, Demo) des EnergyTwin-Systems durch – Ports und Build prüfen, Wetter/Master/Geräte/Diagnose-Client/Gateway nach Portmatrix starten, Abnahmekriterien prüfen (Zeitgleichheit, Pause, Tempo, Neustart, Wetterausfall, Modbus-Schreiben, Geräte- und Broker-Ausfall, geordnetes Beenden), Protokolle sammeln und einen Nachweis unter docs/nachweise/ ablegen. Aufruf z. B. "@probelauf LB03 Gateway". Nicht für Unit-Tests oder Codeänderungen.
---

# probelauf – Funktionsnachweis reproduzierbar durchführen

**Verantwortlich:** Dominik Zach (Team Alpha). Details, Beispiel und Ablage: [`README.md`](README.md).

## Aufruf

`@probelauf <Thema>` – z. B. `@probelauf LB03 Gateway`, `@probelauf LB02 Demo komplett`, `@probelauf nur Ladesäule`.
Der Text dahinter ist das Thema; das Skill-Werkzeug selbst nimmt keine Parameter. Ohne Thema: **Umfang erfragen**, nicht raten.

Der Skill wird über seine ID `probelauf` geladen (Ordnername; versioniert in `skills/probelauf/`, lokal aktiv
unter `.opencode/skills/probelauf/`). Ein Slash-Command `/probelauf` gibt es bewusst nicht – `@probelauf` genügt,
und wenn der Auftrag passt, lädt das Modell den Skill auch ohne Mention selbst.

| Umfang | Beteiligte Programme | Belegt |
|---|---|---|
| **A – Gerät** | Wetter (optional), ein Gerät, Diagnose-Client | Registerplan, Sollwert, Wetter, CSV |
| **B – LB02-Demo** | Wetter, Master, vier Geräte, Diagnose-Client | Zeit, Pause, Tempo, Neustart, Wetterausfall |
| **C – LB03-Kette** | wie B + RabbitMQ + Gateway + Viewer | Nachweise 1–7 aus Aufgabe 03 |

## Zuerst lesen (Pflicht, in dieser Reihenfolge)

1. `AGENTS.md` §0.6 (Stolperfallen), §6 (Befehle), §2 (Abnahmekriterien).
2. `README.md` im Repo-Root (Startreihenfolge, Ports, Demo-Ablauf).
3. `docs/nachweise/2026-11-08-gateway-mit-master/README.md` – **Format-Vorbild** für den Nachweis.
4. In diesem Ordner: `references/ports.md`, `references/abnahmekriterien.md`, `references/fehlerbilder.md`.
5. READMEs der beteiligten Programme (Optionen, Tasten) – nicht aus dem Gedächtnis bedienen.

## Regeln vor dem Start

- **Nichts committen, pushen oder taggen** ohne ausdrückliche Anweisung (AGENTS §0.1).
- **Echte Fenster und fremde Prozesse:** `Start-Process` ohne `-NoNewWindow` sowie `Stop-Process`/`taskkill` auf fremde Prozesse **vorher** bei der Userin anfragen.
- **Tests** nur, wenn ausdrücklich gewünscht oder vor einem Commit. Ein Probelauf ersetzt keine Tests und umgekehrt.
- **Keine Secrets in Protokolle:** `.env` nie lesen oder ausgeben; Zugangsdaten nur über Umgebungsvariablen; Protokolle vor dem Ablegen prüfen.
- **Nicht schönfärben:** ⚠️ und ❌ sind zulässige Ergebnisse. Vorbild ist Punkt 7 im Nachweis 2026-11-08 („teilweise“).
- **Ein Lauf, eine Frage:** Was nicht geprüft wurde, wird als „nicht geprüft“ aufgeführt, nicht als bestanden.

## Ablauf

### 1. Ziel und Umfang festlegen

Vor dem Start drei Zeilen notieren:
- **Ziel:** welche Nachweise/Punkte (Nummern aus Aufgabe bzw. `docs/SRS-LB03.md`).
- **Aufbau:** Geräte, mit/ohne Broker, Tempo, Startzeit, Dauer.
- **Grenze:** was dieser Lauf **nicht** zeigen kann (z. B. Tastenbedienung).

### 2. Vorprüfung (nur lesend)

```powershell
dotnet build EnergyTwin.sln                     # 0 Fehler, 0 Warnungen
powershell -File skills/probelauf/scripts/pruefe-ports.ps1
docker compose -f docker/docker-compose.yml ps  # nur Umfang C
```

- Ist ein Port belegt, **muss** klar sein, welcher Prozess ihn hält. Ein alter Master auf 5100 oder ein alter Skript-Master macht den Lauf ungültig (so am 2026-11-08 passiert).
- `start-all.ps1` überspringt belegte Ports – dann wird unbemerkt der **alte** Prozess getestet. Vorher Ports prüfen.
- Broker-Zugangsdaten: `ENERGYTWIN_MQTT_USER`/`ENERGYTWIN_MQTT_PASSWORD` gesetzt oder `docker/.env` vorhanden (nie anzeigen).

### 3. Nachweisordner anlegen

`docs/nachweise/JJJJ-MM-TT-<thema>/` mit:
- `README.md` – Vorlage: `templates/nachweis-README.md`
- `ablauf.txt` – Zeitmarken (`HH:mm:ss.fff +Ns  Ereignis`)
- je Programm eine Datei: `master.txt`, `wetter.txt`, `pv.txt`, `batterie.txt`, `ladesaeule.txt`, `kuehlhaus.txt`, `diagnose.txt`, `gateway.txt`, `viewer.txt`

**Immer `.txt`, nie `.log`** (`.gitignore` schließt `*.log` aus). Keine Passwörter in den Dateien.

### 4. Starten

Reihenfolge und Ports: `references/ports.md`. Nach dem Build immer `--no-build`.

Für Umfang B ist `.\start-all.ps1` der schnellste Weg – **aber** es öffnet sieben Fenster (vorher fragen) und startet **kein** Gateway. Für umgeleitete Läufe die Programme einzeln starten:

```powershell
Start-Process dotnet -ArgumentList 'run --no-build --project src/WeatherSimulator' `
    -RedirectStandardOutput 'docs/nachweise/<ordner>/wetter.txt' -NoNewWindow
```

- Ausgabe umgeleitet ⇒ Master/Geräte rendern nur alle 5 s eine Zeile. Das ist **kein** Hänger.
- Argumente mit Leerzeichen (z. B. `--start "2026-01-15 06:00"`) brauchen innere Anführungszeichen, sonst zerlegt PowerShell sie.
- **Interaktive Bedienung** (Leertaste, `+`/`-`, `T`, `N`, `1`–`4`, `Q`, Gerätetasten) braucht ein echtes Terminal. Wenn die Userin nicht bedient: `--autostart` nutzen und nur die Punkte prüfen, die ohne Tastendruck gehen – oder mit Auftrag einen Skript-Master wie im Nachweis 2026-11-08 bauen (nicht im Repo). Die echte Tastenbedienung bleibt ein separater, offener Nachweis.

### 5. Prüfen

`references/abnahmekriterien.md` Punkt für Punkt. Je Punkt gilt:
- **Erwartung** aus der Tabelle,
- **Beobachtung** aus der laufenden Anzeige oder aus `--once`,
- **Beleg** = Datei + Zeitmarke/Zeile. Nichts aus dem Gedächtnis, nichts „müsste passen“.

Während des Laufs in `ablauf.txt` mitschreiben: Start, Tempowechsel, Pause, Neustart, Sollwert, Ausfälle, Ende.

### 6. Beenden und aufräumen

1. Geordnet: `Q` im Master (sendet `bye`), `Q` in Geräten und Gateway, Strg+C im Wettersimulator.
2. Ports erneut prüfen (`pruefe-ports.ps1`).
3. Hängende Prozesse nur nach Freigabe beenden: `taskkill //F //IM <Programm>.exe`.
4. `git status --short` ansehen: neue CSVs in `docs/data/` sind normal (`*.csv` ist ignoriert).

### 7. Auswerten und ablegen

- Ergebnistabelle je Nachweis mit ✅ / ⚠️ / ❌ und Beleg.
- Abschnitt **„Beobachtungen und Einschränkungen“** – ehrlich, inklusive unerklärter Einmal-Effekte.
- Abschnitt **„Nicht geprüft“** – z. B. echte Terminalanzeige, Tasten, Screenshots.
- Zahlen nur nennen, wenn belegt: Testzahlen aus den Testprojekten **nachrechnen**, nicht aus `AGENTS.md` abschreiben.
- Journal der eigenen Person ergänzen (`AGENTS.md` §0.1) – nur die eigene Datei, nur auf Auftrag bzw. nach Projektregel.

## Kurz-Prüftabelle

| # | Prüfung | Beobachtung | typischer Beleg |
|---|---|---|---|
| Z1 | Zeitgleichheit | Master-Liste Δ ✓; Gerät „synchron mit Master“; Register 12 = 1 | `master.txt`, Gerät |
| Z2 | Tempowechsel | Register 14–15 ändert sich bei allen; Modellzeit läuft stetig | `master.txt`, `--once` |
| Z3 | Pause | Register 13 = 0, Modellzeit steht, Modbus lesbar | `diagnose.txt` |
| Z4 | Fortsetzen | Modellzeit ab demselben Wert, kein Sprung | `ablauf.txt` |
| Z5 | Neustart | Register 10 (Lauf) neu, Modellzeit springt, Zustand bleibt | `--once` vor/nach |
| Z6 | Wetter | gleicher Ort/Zeit ⇒ gleiches Wetter; Ausfall ⇒ Meldung nach 3 Versuchen | `wetter.txt`, Gerät |
| Z7 | Schreiben | `write` angenommen und im Gerät sichtbar; `force` ⇒ 18 = 2, 19 = Adresse | `diagnose.txt` |
| Z8 | Beenden | `bye`, keine Restprozesse | `master.txt`, Ports |

LB03 (N1–N7): siehe `references/abnahmekriterien.md`, Abschnitt C.

## Grenzen

- Kein Ersatz für Unit-Tests; kein Code-Fix während des Laufs (Funde notieren, danach entscheiden).
- Interaktive Anzeige, Tasten und Eingaben kann der Agent nicht abnehmen – das bleibt bei der Userin.
- Ohne Broker (Umfang A/B) wird nichts über MQTT ausgesagt; ohne Gerät nichts über dessen Register.
- „Nicht nachgewiesen“ ist ein gültiges Ergebnis und wird so berichtet.

## Typische Fehler

Die 17 häufigsten Fälle mit Ursache und Abhilfe: `references/fehlerbilder.md`.
Die drei wichtigsten: belegter Port (alter Prozess), EasyModbus antwortet **vor** der Anwendung (200 ms), umgeleitete Ausgabe ist kein Terminal.
