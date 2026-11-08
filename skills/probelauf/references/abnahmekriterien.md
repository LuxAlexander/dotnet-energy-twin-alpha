# Abnahmekriterien für den Probelauf

Zwei Quellen: `AGENTS.md` §2 (LB02, „Muss vorführbar sein“) und Aufgabe 03 / `docs/SRS-LB03.md` (Nachweise 1–7).
Jeder Punkt braucht **Beobachtung** und **Beleg**; ohne Beleg gilt er als „nicht nachgewiesen“.

## A – Umfang „Gerät“ (ein Gerät, optional Wetter)

| # | Prüfung | Erwartung | Wie prüfen | Beleg |
|---|---|---|---|---|
| A1 | Gerät startet, Zeitbezug | Register 12 = 1 (synchron) bzw. 2 (Standalone) | `--once` | `<gerät>.txt`, `diagnose.txt` |
| A2 | Registerkopf vollständig | Adressen 0–19 lesbar, Typ und Instanz passen zur Geräte-ID | `read`, `info` | `diagnose.txt` |
| A3 | Geräteregister | Tabelle aus dem Geräte-README, plausible Werte | `read` | `diagnose.txt` |
| A4 | Gültiger Sollwert | „ANGENOMMEN … Gerät zeigt jetzt …“ | `--write "<Ziel>=<Wert>"` | `diagnose.txt` |
| A5 | Ungültiger Wert | Register 18 = 2, Register 19 = Adresse, alter Wert bleibt | `force` (umgeht die Client-Prüfung) | `diagnose.txt` |
| A6 | Nur-Lese-Register | Register 18 = 1 | `force <adresse> <wert>` | `diagnose.txt` |
| A7 | CSV | eine Zeile je Modellschritt, de-AT (Komma), Spalte `Lauf` | `docs/data/` ansehen | CSV-Pfad im Lauf |
| A8 | Wetter (falls aktiv) | gleicher Ort/Zeit ⇒ gleiches Wetter; Ausfall sichtbar | Wetter stoppen | `<gerät>.txt`, `wetter.txt` |

## B – Umfang „LB02-Demo“ (Wetter, Master, vier Geräte, Client)

| # | Prüfung | Erwartung | Wie prüfen | Beleg |
|---|---|---|---|---|
| Z1 | Alle Programme getrennt gestartet | sieben Prozesse, jeder mit eigenem Port | Portprüfung | `ablauf.txt` |
| Z2 | Zeitgleichheit | Master-Liste zeigt jedes Gerät mit Δ und ✓; Gerät „synchron mit Master“; Register 12 = 1 | Master-Anzeige, `--once` | `master.txt`, `diagnose.txt` |
| Z3 | Toleranz | Abweichung ≤ 10 ms · Tempo | Master-Spalte Δ und Unsicherheit | `master.txt` |
| Z4 | Tempowechsel im Betrieb | `+`/`-` ändert Register 14–15 bei **allen**; Modellzeit läuft stetig, kein Sprung | Master bedienen, `watch` | `master.txt`, `diagnose.txt` |
| Z5 | Globale Pause | Register 13 = 0, Modellzeit (8–9) steht, Modbus bleibt lesbar, letzter Stand | Pause, dann `--once` | `diagnose.txt` |
| Z6 | Fortsetzen | Modellzeit läuft ab demselben Wert weiter, Δ wieder ✓ | `ablauf.txt` | `master.txt` |
| Z7 | Neustart | Register 10 (Lauf) neu, Modellzeit springt, Zustand bleibt (z. B. Energiezähler, Raumtemperatur) | `N`/`1`–`4`, `--once` vor/nach | `diagnose.txt` |
| Z8 | Wetterausfall | nach 3 Fehlversuchen sichtbare Meldung, letztes Wetter bleibt | Wettersimulator beenden | `<gerät>.txt` |
| Z9 | Wetter-Reproduzierbarkeit | gleicher (Ort, Stunde, Lauf) ⇒ gleiche Antwort | zwei HTTP-Abfragen | `wetter.txt` |
| Z10 | Sollwert sichtbar | Client schreibt, Gerät zeigt den Wert | `write` + Anzeige | `diagnose.txt`, Gerät |
| Z11 | Beenden | `Q`/Strg+C, Master sendet `bye`, keine Restprozesse | Ports danach prüfen | `master.txt` |

**Registerkopf (Adressen 0–19):** 0 Typ · 1 Instanz · 2 Betriebszustand · 3 Fehlercode · 4–5 Leistung · 6–7 Energie ·
8–9 Modellzeit · 10 Lauf · 11 Sequenz · 12 Zeitstatus (1 synchron, 2 standalone, 3 Master verloren) ·
13 Simulation läuft · 14–15 Tempo · 16 Ein/Aus · 17 Reset · 18 letzter Schreibfehler · 19 dessen Adresse.
Schreibfehler: 1 nicht beschreibbar, 2 ungültiger Wert, 3 unvollständig.

## C – Umfang „LB03-Kette“ (zusätzlich RabbitMQ, Gateway, Viewer)

| # | Nachweis (Aufgabe 03) | Erwartung | Wie prüfen | Beleg |
|---|---|---|---|---|
| N1 | Broker aus der Compose-Datei | Container `rabbitmq` läuft, MQTT-Port 1883 offen | `docker compose ps` | `ablauf.txt` |
| N2 | TimeSim, WeatherSim, Gerät | Gerät „synchron mit Master“ | `--once`, Master | `<gerät>.txt` |
| N3 | Gateway und Viewer | Messwerte mit `simTime`, `seq`, `publishSeq`, Qualität; Topics `energytwin/devices/<id>/measurements` | `--json`, Viewer | `gateway.txt`, `viewer.txt` |
| N4 | Sollwert-Kette | Diagnose schreibt → Gerät übernimmt → nächste Nachricht trägt den Wert | `--write`, Viewer | `diagnose.txt`, `viewer.txt` |
| N5 | Pause/Fortsetzen | `simTime` steht, es wird weiter veröffentlicht (Qualität „paused“); Fortsetzen ohne Sprung | Master pausieren | `viewer.txt`, `master.txt` |
| N6a | Geräteausfall | Status `offline`, keine Messwerte; nach Neustart `online` und wieder synchron | Gerät hart beenden/neu starten | `gateway.txt`, `viewer.txt` |
| N6b | Broker-Ausfall | einmalige Meldung, Gerät wird weiter gelesen, Nachrichten verworfen, nach Wiederanlauf Status nachgeholt | `docker stop/start rabbitmq` | `gateway.txt` |
| N6c | Falsche Zugangsdaten | sichtbare Meldung, **kein** Passwort in der Ausgabe | Lauf mit falschem Passwort | `gateway.txt` |
| N7 | Kontrolliertes Beenden | Master `bye` ⇒ Gateway meldet `bad/masterLost`; Gateway-Status `offline`; keine Restprozesse | `Q`/Strg+C | `gateway.txt`, Ports |

**Qualitätswerte** (Nachrichtenvertrag §3.1): u. a. `good`, `paused`, `stale` (Modellzeit ohne neue Sequenz länger als `--stale-after`), `bad/*`.
Ungültige oder veraltete Werte dürfen **nicht** als gültig erscheinen (Mindestanforderung M4).

## Immer am Ende

- [ ] Ports frei bzw. bekannter Prozess (nach dem Lauf)
- [ ] `git status --short` gesichtet (CSVs, keine Fremdänderungen)
- [ ] Nachweis-`README.md` vollständig: Aufbau, Ergebnistabelle, Beobachtungen, **Nicht geprüft**
- [ ] keine Zugangsdaten in den Protokollen
- [ ] Journal-Eintrag (eigene Person) nach Projektregel
