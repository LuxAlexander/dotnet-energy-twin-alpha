# Skill `probelauf`

| Feld | Inhalt |
|---|---|
| **Verantwortliche Person** | Dominik Zach |
| **Zweck** | Einen reproduzierbaren Funktionsnachweis (Probelauf/Demo) des EnergyTwin-Systems durchführen: Ports und Build prüfen, Wetter/Master/Geräte/Diagnose-Client/Gateway in der richtigen Reihenfolge starten, die Abnahmekriterien Punkt für Punkt beobachten und das Ergebnis mit Protokollen unter `docs/nachweise/` ablegen. Entspricht dem LB02-Punkt „Muss vorführbar sein“ und den LB03-Nachweisen 1–7. |
| **Aufruf** | `@probelauf LB03 Gateway` · `@probelauf LB02 Demo komplett` · `@probelauf nur Ladesäule` · ohne Thema: Umfang erfragen. Kein Slash-Command; der Text nach `@probelauf` ist das Thema. |
| **Benötigter Kontext** | `AGENTS.md` (§0.6, §2, §6), zentrale `README.md`, die READMEs der beteiligten Programme, `docs/Nachrichtenvertrag-LB03.md` (nur Umfang C), `docker/docker-compose.yml`, `docs/nachweise/` als Format-Vorbild, die Dateien in diesem Ordner |
| **Arbeitsschritte** | 1. Ziel, Aufbau und Grenze des Laufs notieren · 2. Vorprüfung: Build, Ports, Broker · 3. Nachweisordner anlegen · 4. Programme starten (Reihenfolge und Ports aus `references/ports.md`) · 5. Kriterien beobachten und Zeitmarken mitschreiben · 6. geordnet beenden, Ports erneut prüfen · 7. Ergebnistabelle, Beobachtungen und „Nicht geprüft“ in die Nachweis-`README.md` |
| **Grenzen** | Kein Ersatz für Unit-Tests, keine Codeänderung während des Laufs, kein Commit/Push/Tag. Interaktive Anzeige, Tasten und Strg+C kann der Agent nicht abnehmen – die bleiben bei der Userin. Echte Fenster (`Start-Process` ohne `-NoNewWindow`) und das Beenden fremder Prozesse werden vorher angefragt. Keine Secrets in Protokollen (`.env` nie lesen oder ausgeben). |

## Nutzen und Wirkung

Zur Abgabe gehört neben dem Code ein **vorführbarer** Lauf: Zeitgleichheit, Pause, Tempowechsel, Neustart,
Wetterausfall, Modbus-Schreiben, Geräte- und Broker-Ausfall, geordnetes Beenden. Solche Läufe gehen selten beim ersten
Mal glatt, und die teuersten Fehler sind stille: ein alter Master, der noch Port 5100 hält, ein `start-all.ps1`, das
einen belegten Port überspringt, oder „angenommen“ im Client, obwohl das Gerät den Wert erst im nächsten Durchlauf
sieht. Der Skill macht die Prüfreihenfolge, die typischen Fallen und das Belegformat explizit, damit ein Lauf
wiederholbar ist und ein Nachweis entsteht, der auch einer Prüfung standhält – inklusive der Punkte, die **nicht**
geprüft wurden.

Drei Umfänge, damit der Aufwand zum Ziel passt:

| Umfang | Beteiligte Programme | Belegt |
|---|---|---|
| **A – Gerät** | Wetter (optional), ein Gerät, Diagnose-Client | Registerplan, Sollwert, Wetter, CSV |
| **B – LB02-Demo** | Wetter, Master, vier Geräte, Diagnose-Client | Zeit, Pause, Tempo, Neustart, Wetterausfall |
| **C – LB03-Kette** | wie B + RabbitMQ + Gateway + Viewer | Nachweise 1–7 aus Aufgabe 03 |

## Beispielaufruf mit Ergebnis (2026-11-08)

Aufruf: `@probelauf nur Kühlhaus, ohne Master` – Umfang **A**, Kühlhaus KH-01 standalone, Tempo 600, 8 Modellstunden,
`--weather off`, Seed 42, Modbus 1505. Vollständiger Nachweis mit allen Protokollen:
[`docs/nachweise/2026-11-08-probelauf-skill/`](../../docs/nachweise/2026-11-08-probelauf-skill/README.md). Auszug:

| # | Prüfung | Beobachtung | Ergebnis |
|---|---|---|---|
| A1 | Zeitbezug | Register 12 = 2 (Standalone), 13 = 1 (läuft), Tempo 600 | ✅ |
| A2 | Registerkopf 0–19 | Typ 5 = Industrieverbraucher, Instanz 1, Modellzeit 2026-11-08 00:10 | ✅ |
| A3 | Geräteregister 20–31 | Soll −20 °C, Raum −19,77 °C, außen 7,5 °C, Tür zu, Kompressor aus | ✅ |
| A4 | Sollwert schreiben | „ANGENOMMEN: Solltemperatur −20 °C → −15 °C“; Gegenlesung: Register 20–21 = −15 °C, Register 19 = 20 | ✅ |
| A5/A6 | ungültig / nur lesbar (`force`) | nicht ausgeführt | ⚠️ nicht geprüft |
| A7 | CSV | `KH-01_2026-11-08_155520.csv`, Semikolon, Dezimalkomma, Spalte `Lauf` | ✅ |
| A8 | Wetter | mit `--weather off` nicht Teil des Laufs | ⚠️ nicht geprüft |

Zeitmarken aus `ablauf.txt`: Gerät 15:55:19 gestartet, Port 1505 nach 1,2 s erreichbar, Lesen 15:55:20,
Schreiben 15:55:21, Gegenlesen 15:55:22, Gerät nach 8 Modellstunden um 15:56:08 beendet, alle Ports danach frei.
Der Lauf prüft den **Ablauf**; Master, Broker und echte Tastenbedienung sind ausdrücklich als „nicht geprüft“ vermerkt.

## Ablage und Aktivierung

Die versionierte Fassung liegt in **`skills/probelauf/`**. `.opencode/` ist eine lokale, ignorierte Ablage.
Für OpenCode aus dem Repository-Root kopieren, sofern dort noch keine eigene Fassung liegt:

```powershell
New-Item -ItemType Directory -Force .opencode/skills
Copy-Item -Recurse skills/probelauf .opencode/skills/probelauf
```

Bei späteren Änderungen die lokale Kopie gezielt abgleichen. Der Skill-Einstieg und das Portskript enthalten noch
Aufrufe über `.opencode/skills/probelauf/`; diese setzen die lokale Installation voraus. Das Portskript lässt sich
aus der versionierten Ablage direkt mit `powershell -File skills/probelauf/scripts/pruefe-ports.ps1` aufrufen.

Aufruf im Prompt:

- `@probelauf <Thema>` – lädt diesen Skill und übergibt das Thema als Text (das Skill-Werkzeug selbst hat keine
  Parameter), z. B. `@probelauf LB03 Gateway`.
- Ohne Mention: den Auftrag einfach beschreiben; das Modell sieht die `description` und lädt den Skill selbst.

Einen Slash-Command `/probelauf` gibt es bewusst nicht (Commands liegen in `.opencode/commands/` und sind ein
eigenes Konzept). Die Ablage trennt die Agenten: Claude Code liest `.claude/skills/`, OpenCode liest
`.opencode/skills/` – und zusätzlich `.claude/skills/` und `.agents/skills/` als Kompatibilitätsquellen, wobei
`.opencode/skills/` die höhere Priorität hat.

## Dateien im Ordner

| Datei | Inhalt |
|---|---|
| `SKILL.md` | Ablauf, Regeln, Prüf-Tabelle, Grenzen (das eigentliche Skill) |
| `references/ports.md` | Portmatrix (8080, 5100, 1502–1505, 1883, 15672), Startbefehle, Standardwerte |
| `references/abnahmekriterien.md` | Prüfpunkte A1–A8, Z1–Z11 (LB02) und N1–N7 (LB03, inkl. Register 18/19 und Qualitätswerte) |
| `references/fehlerbilder.md` | 17 Fehlerbilder mit Ursache und Abhilfe |
| `templates/nachweis-README.md` | Vorlage für `docs/nachweise/<datum>-<thema>/README.md` |
| `scripts/pruefe-ports.ps1` | Portcheck, nur lesend, ändert nichts und beendet keine Prozesse |

## Quelle und Lizenz

Eigenentwicklung für dieses Projekt, nicht aus einer fremden Vorlage übernommen. Grundlage sind die Aufgabenstellung
(Roland Graf, 2026), `AGENTS.md` und die vorhandenen Nachweise im Repo. Keine fremde Lizenz zu beachten.

## Bekannte Grenzen

- **Interaktive Bedienung** (Leertaste, `+`/`-`, `T`, `N`, `1`–`4`, `Q`, Gerätetasten) braucht ein echtes Terminal und
  bleibt bei der Userin. Für Agentenläufe gibt es `--autostart`, `--duration` und einen Skript-Master (wie im Nachweis
  2026-11-08), aber die Tastenbedienung selbst ist damit **nicht** nachgewiesen.
- **Umgeleitete Ausgabe** ist kein Terminal: Geräte rendern nur alle 5 s eine Zeile, Statuszeilen werden abgeschnitten,
  Cursor-Funktionen greifen nicht. Für Anzeige-Nachweise braucht es zusätzlich einen echten Fensterlauf.
- **`ExitCode` nach `Start-Process -PassThru`** war im Selbsttest leer; der Skill stützt sich deshalb auf Ausgaben,
  Portverhalten und Zeitmarken statt auf Exit-Codes. Fehlt der Exit-Code, ist das kein Fehlschlag.
- **Protokolle müssen `.txt` sein** (`.gitignore` schließt `*.log` aus) und dürfen keine Zugangsdaten enthalten.
- Der Skill **ändert keinen Code**: Funde werden notiert, die Entscheidung über eine Korrektur fällt danach.
- Ein Lauf ohne Broker (Umfang A/B) sagt nichts über MQTT aus; ein Lauf ohne Gerät sagt nichts über dessen Register aus.
  „Nicht nachgewiesen“ ist ein gültiges Ergebnis und wird so berichtet.
