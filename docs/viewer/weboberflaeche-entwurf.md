# Lokale Weboberfläche – Entwurf

**Umsetzungsstand 2026-11-09:** Das [erste Grundgerüst](../../src/EnergyTwin.Web/README.md) ist als
eigenständiges Blazor-Projekt angelegt: Navigation, Modulansichten und markierte Beispieldaten.
Live-Anbindung, Befehlsverarbeitung, Instanzpersistenz und Prozessverwaltung bleiben offen.
Der folgende bestätigte Plan beschreibt weiterhin das Gesamtziel; die historischen Hinweise zum
visuellen Entwurf sind kein Funktionsnachweis dieser Implementierung.

**Stand:** 2026-11-09. **Status:** Produktentscheidungen im Gespräch bestätigt; technische Umsetzung und Detailansichten noch in Planung. Kein Implementierungs- oder Funktionsnachweis. Die bisherige MQTT-Explorer-Einrichtung bleibt ein separater, bestehender Viewer.

## Bestätigte Architektur und Bedienung

- Lokale Browseroberfläche für Windows und Linux, ohne Internet zur Laufzeit und mit möglichst geringem Zusatzaufwand.
- Zugriff ausschließlich auf demselben Rechner über localhost. Fernzugriff ist nicht vorgesehen.
- RabbitMQ ist der einzige Kommunikationsweg zwischen Webdienst und Modulen für Messwerte, Status und fachliche Bedienbefehle. Keine zusätzlichen direkten HTTP-, Modbus- oder Zeitserver-Verbindungen des Webdiensts zu Simulatoren. Browser und lokaler Webdienst kommunizieren weiterhin miteinander; bestehende Zeitverteilung, HTTP-Wetterversorgung und Modbus-Verbindungen innerhalb des Simulationssystems bleiben erhalten.
- Ein Betrieb der Anzeige und fachlichen Bedienung ohne Broker ist nicht vorgesehen. Bei Broker-Verlust werden letzte Werte als nicht aktuell bestätigt gekennzeichnet; Befehle dürfen nicht stillschweigend für spätere Ausführung gesammelt werden. Die konkrete Befehlszustellung und Prozessverwaltung sind noch zu entwerfen.
- Alle Module bleiben eigenständige Prozesse und ohne Oberfläche nutzbar; spätere Containerisierung berücksichtigen. Konsolenbedienung bleibt erhalten. Browser und Konsole sollen dieselben fachlichen Funktionen und Zustände verwenden.
- Zunächst ein Master, ein Wetterdienst und mehrere Geräteinstanzen ohne fest codierte Stückzahl; tatsächliche Kapazität hängt von Ressourcen und Schnittstellen ab.
- Webdienst und Startverwaltung laufen auf demselben Rechner wie die verwalteten Prozesse. Start, geordnetes Stoppen und Neustart gelten für selbst verwaltete Instanzen. Extern gestartete Module können eingebunden und fachlich bedient werden, werden aber nicht automatisch beendet.
- „Anlage starten“ startet benötigte Module in passender Reihenfolge und prüft Erreichbarkeit. Bereits laufende Module nicht doppelt starten. Master bleibt zunächst pausiert, Fortsetzen erfolgt ausdrücklich.
- Bei einem fehlgeschlagenen Modulstart bleiben erfolgreich gestartete Module aktiv; Fehler werden pro Modul angezeigt und der Master bleibt pausiert. Erneuter Startversuch oder bewusstes Fortfahren mit verfügbaren Geräten erfolgt ausdrücklich.
- Bestätigter Startweg: Die lokale Startverwaltung darf den vorhandenen RabbitMQ-Container starten, sofern Docker läuft. Erst nach Broker-Bereitschaft folgen die übrigen Module. Bei fehlendem Docker konkrete Fehlermeldung statt stiller Ausweichverbindung. Prozessverwaltung ist vom fachlichen Nachrichtenverkehr getrennt.
- Bestätigter Befehlsweg: Der Webdienst sendet Gerätebefehle über RabbitMQ an das Gateway; dieses prüft und schreibt Modbus-Sollwerte und meldet das Ergebnis zurück. Master und Wetter erhalten eigene MQTT-Bediennachrichten. Keine zusätzliche RabbitMQ-Anbindung der einzelnen Feldgeräte erforderlich.
- Browser schließen beendet keine Module. Erneutes Öffnen zeigt gespeicherte Instanzen und aktuelle Zustände. Ein Webdienst-Neustart soll laufende Module ebenfalls nicht automatisch beenden; verlässliche Wiedererkennung ist noch technisch zu entwerfen.
- Prozess läuft, Gerät erreichbar und Simulation läuft sind getrennte Zustände. Veraltete Messwerte und Verbindungsverlust müssen erkennbar sein.

## Bestätigter Bildschirmaufbau

- Ruhige Codex-ähnliche Gestaltung mit heller/dunkler Darstellung nach Systemeinstellung.
- Obere Leiste: virtuelle Zeit, Tempo, Pause/Fortsetzen und „Anlage starten“.
- Linke Navigation: Anlagenübersicht, Master, Wetter; aufklappbare Gerätegruppen für PV, Batterie, Ladesäulen und Kühlhäuser mit einzelnen Instanzen; Infrastruktur darunter.
- Mitte: ausgewählte Instanz, wenige große Kennzahlen, Verlauf und häufige Bedienaktionen. Reiter: Übersicht, Einstellungen, Register, Meldungen. Technische Konfiguration wie Ports unter Einstellungen.
- Unten: einklappbare Meldungen der ausgewählten Instanz.
- Messwerte und Diagramme erhalten passende Farben nach dem Vorbild der PV-Konsole; konkrete Zuordnung noch als Gestaltungsvorschlag zu prüfen. Farbe durch Beschriftung und Zustandskennzeichen ergänzen.

## Instanzen und Aktionen

- Gerätetyp wählen; Namen, eindeutige Kennung und freie Ports vorschlagen; Standort auswählen. Gespeicherte Konfigurationen auf Kennungs- und Portkonflikte prüfen. Ein vorab freier Port ist bis zum tatsächlichen Start nicht garantiert reserviert.
- Duplizieren übernimmt Konfiguration mit neu vergebener Kennung und neuen Ports. Speichern allein startet keinen Prozess.
- Pause/Fortsetzen direkt; Sollwertänderungen mit „Übernehmen“ und sichtbarer Geräterückmeldung.
- Neuer Simulationslauf, Prozessneustart und „Alle stoppen“ mit Bestätigung wegen möglichem Zustandsverlust.
- Geräteaufnahme am Gateway erfolgt im Hintergrund. Benutzer legen eine Instanz an und starten sie; technische Konfigurationsbefehle werden nicht als eigener Arbeitsschritt angezeigt. Fortschritt und Fehler bleiben verständlich sichtbar.
- Unerwartete Prozessabbrüche werden als Fehler gemeldet und bieten manuelles Neustarten; zunächst kein automatischer Neustart mit möglichem Zustandsverlust.
- Befehle je Ziel geordnet verarbeiten, tatsächlich übernommenen Zustand zurückmelden und veraltete Eingaben nicht unbemerkt überschreiben lassen. Die Absicherung paralleler Konsolenzugriffe muss innerhalb der Module erfolgen, nicht nur im Webdienst.
- Bestätigt: Ein Knopf „Alles geordnet schließen“ beendet die verwalteten Module nach Bestätigung. Nicht reagierende Prozesse melden; erzwungenes Beenden bleibt eine getrennte ausdrückliche Aktion. Fremde Prozesse werden nicht automatisch beendet. Der Webdienst bleibt mit Abschlussanzeige geöffnet, damit Fehler geprüft oder die Anlage erneut gestartet werden können.

### Ablauf für „Alles geordnet schließen“

Neue Bedienaktionen sperren, Master pausieren, verwaltete Geräte und Dienste unter Berücksichtigung ihrer Abhängigkeiten geordnet stoppen und Ergebnisse anzeigen. RabbitMQ bleibt bis zum Abschluss der benötigten Nachrichten und Rückmeldungen verfügbar und wird erst danach gestoppt, sofern durch diese Verwaltung gestartet und nicht anderweitig benötigt. Keine pauschale Beendigung von Docker oder fremden Containern. Der Webdienst und seine Abschlussanzeige bleiben erreichbar; der Knopf beendet den Webdienst nicht. Ein geschlossener Browser-Tab ist keine Voraussetzung für das erfolgreiche Stoppen.

## Bestätigte Technikentscheidung

**ASP.NET Core 10 mit einer Blazor Web App im Modus Interactive Server** als zusätzlicher Webdienst.
Die im Gespräch bestätigte Auswahl passt zum vorhandenen .NET-Aufbau: Oberfläche und Anwendungslogik überwiegend in C#,
lokaler Kestrel-Webserver, Browserdarstellung mit HTML/CSS und gezieltem JavaScript für Diagramme.
Keine separate Node.js-Laufzeit im Betrieb erforderlich. Statische Dateien, Schriftarten und eventuelle
Diagrammbibliotheken lokal ausliefern; keine Laufzeitabhängigkeit von CDNs.

Bei Interactive Server verarbeitet der Webdienst UI-Ereignisse; SignalR verbindet Browser und Webdienst.
Das ist ausschließlich die Oberflächenverbindung und kein zusätzlicher Datenweg zu Simulatoren.
Der Webdienst nutzt für Modulkommunikation MQTT über RabbitMQ, mit der bereits im Projekt verwendeten
Bibliothek MQTTnet. Instanzverwaltung, MQTT-Empfang und Verlaufsspeicher dürfen nicht an eine einzelne
Browser-Sitzung gebunden sein.

Abwägung: Die interaktive Oberfläche benötigt eine bestehende Verbindung zum Webdienst und hält
serverseitigen Zustand je Browser-Sitzung. Für den vorgesehenen lokalen Betrieb ist das vertretbar;
es ist keine gemessene Aussage über den minimalen Ressourcenverbrauch. Simulatoren bleiben auch bei
Verlust der Browser-Verbindung eigenständig. Die spätere Containerisierung der Oberfläche muss von
der Fähigkeit zur lokalen Prozessverwaltung getrennt betrachtet werden.

Offizielle Grundlagen, geprüft am 2026-11-09:
[Blazor-Hostingmodelle](https://learn.microsoft.com/en-us/aspnet/core/blazor/hosting-models?view=aspnetcore-10.0),
[unterstützte Plattformen](https://learn.microsoft.com/en-us/aspnet/core/blazor/supported-platforms?view=aspnetcore-10.0).
Konkrete Diagrammbibliothek und Speichergrenzen bleiben auszuwählen. Blazor Interactive Server ist beschlossen; Pakete
werden durch diese Planungsentscheidung noch nicht installiert. Eine Implementierung ist damit nicht nachgewiesen.

## Bestätigte gestalterische Grundlage und Geräteansichten

Der im Gespräch gezeigte anklickbare PV-Entwurf wurde als passende gestalterische Grundlage bestätigt.
Er zeigte zwei beispielhafte PV-Instanzen, Navigation zwischen den vier Ansichten, Kennzahlen,
einen Leistungsverlauf, Pause/Fortsetzen und eine beispielhafte Sollwertbestätigung. Alle Werte und
Rückmeldungen waren Beispieldaten; der Entwurf hatte keine Verbindung zu echten Modulen.
Die Darstellung ist damit abgestimmt, aber weder als Webanwendung implementiert noch funktional geprüft.

Ausdrücklicher Gestaltungsauftrag: Alle übrigen Geräte und Simulatoren erhalten ebenso passende,
individuelle Ansichten innerhalb derselben Navigation. Die nachstehende Ausgestaltung ist die
Planungsgrundlage; genaue Felder, Grenzwerte und bedienbare Funktionen müssen vor Umsetzung gegen
die jeweiligen Verträge und Registerpläne abgeglichen werden.

| Ansicht | Relevante Informationen und Darstellung | Vorgesehene Bedienung |
|---|---|---|
| Anlagenübersicht | Instanzen nach Typ; Prozess-, Verbindungs- und Simulationszustand; Startfehler je Modul | Anlage starten, Instanz hinzufügen, gezielt erneut starten, alle verwalteten Module stoppen |
| PV-Anlage | Wirkleistung und Ertrag, Einstrahlung, Modultemperatur, Auslastung, Leistungsdiagramm | Leistungsbegrenzung und weitere tatsächlich unterstützte Sollwerte |
| Batteriespeicher | Ladezustand als verständliche Füllanzeige, Lade-/Entladeleistung, Energie und Verlauf | Leistungssollwert und unterstützte Betriebsoptionen |
| DC-Ladesäule | Zwei klar getrennte Ladeanschlüsse mit Fahrzeugstatus, Ladefortschritt und Leistung; gemeinsame Säulenwerte | Unterstützte Aktionen je Anschluss sowie Leistungsgrenzen |
| Kühlhaus | Ist-/Solltemperatur gemeinsam im Verlauf, Kühlleistung und Betriebszustand | Solltemperatur und unterstützte Betriebsoptionen |
| Wetter | Standortauswahl, Wetterzustand, Einstrahlung, Temperatur, Wind und weitere verfügbare Wettergrößen; manuell/automatisch sichtbar | Standortbezogene Übersteuerung und Rückkehr zur Automatik |
| Master | Virtuelle Zeit, Tempo, Laufkennung sowie Synchronisationsstatus verbundener Geräte | Pause/Fortsetzen, Tempo, neuer Lauf mit Startzeit |
| Infrastruktur | RabbitMQ- und Gateway-Zustand, Verbindungen und verständliche Fehlermeldungen | Start-/Prozessaktionen im Rahmen der lokalen Verwaltung |

Gemeinsame Gestaltung: wenige Kennzahlen oben, fachlich passender Verlauf darunter, häufige Aktionen
direkt erreichbar. Zusätzliche Messwerte, Diagnose und Einstellungen dürfen die Übersicht nicht überladen.
Die Reiter „Register“ betreffen Feldgeräte mit Registerplan; Master und Wetter erhalten passende
Diagnoseinhalte statt erfundener Modbus-Register. Globale Master-Aktionen müssen auch aus einer
Geräteansicht klar als anlagenweit erkennbar sein.

### Farbkonzept

Messwerte und Diagramme dürfen ausdrücklich fachlich passende Farben erhalten, inspiriert von der
PV-Konsole. Als im PV-Entwurf gezeigte Grundlage gelten Grün für Leistung/Ertrag, Goldgelb für
Einstrahlung und Orange für Temperatur. Automatik, manuelle Eingriffe und Pause werden zusätzlich
mit Beschriftungen unterschieden; Grün/Gelb/Violett sind die vorgesehenen Zustandsfarben.
Störungen erhalten eine rote Kennzeichnung; fehlende oder nicht bestätigte Daten werden ausdrücklich
benannt. Exakte Farben und Kontraste für Hell-/Dunkelmodus sind noch zu prüfen.

Eine Messgröße behält ihre Diagrammfarbe. Fehlerzustände verändern nicht stillschweigend die Bedeutung
der Kurve. Farbe allein darf keine Information tragen; Einheiten, Legenden und Statusbeschriftungen
bleiben sichtbar. Für PV wird Einspeisung entsprechend dem Nachrichtenvertrag mit negativem Vorzeichen
und erklärender Beschriftung dargestellt.

## Kommunikationswege und notwendige Erweiterungen

```text
Browser ↔ lokaler Webdienst ↔ RabbitMQ
                                ├─ Master: Status und Bedienbefehle
                                ├─ Wetter: Messwerte, Status und Bedienbefehle
                                └─ Gateway ↔ Feldgeräte über Modbus

Lokale Startverwaltung → Prozesse / vorhandener RabbitMQ-Container
Bestehend: Master → Zeitverteilung; Feldgeräte → Wetterdienst über HTTP
```

Der Webdienst ist kein zusätzlicher Modbus- oder Wetter-HTTP-Client. Geräte-Messwerte kommen weiterhin
über Gateway und RabbitMQ. Der neue Rückweg führt Gerätebefehle über RabbitMQ zum Gateway und von dort
über Modbus zum Gerät. Master und Wetter benötigen eigenen MQTT-Befehlsempfang; der Master außerdem
die für die Oberfläche benötigten Zustandsmeldungen. Diese Erweiterungen sind geplant, nicht vorhanden.

Prozessstart ist getrennt von fachlicher Bedienung: Die lokale Startverwaltung kann den Broker starten,
bevor Nachrichtenverkehr möglich ist. Konfigurationen lassen sich speichern, ohne einen Prozess zu starten.
Bei geschlossenem Browser laufen Module weiter. Nach Webdienst-Neustart müssen Prozessidentität und
Verwaltungszuständigkeit sicher wiederhergestellt werden; eine gespeicherte Prozessnummer allein reicht
dafür nicht als Entwurf.

### Noch auszuarbeitender Befehlsvertrag

Für die Umsetzung vorzusehen und technisch zu konkretisieren: Befehlskennung, eindeutiges Ziel,
Prüfung der Parameter, Ablaufzeit, Behandlung doppelter Zustellung und dem Befehl zuordenbare Rückmeldung.
„An Broker gesendet“ darf nicht als „vom Gerät umgesetzt“ erscheinen. Ohne eindeutige Bestätigung ist
das Ergebnis als unklar darzustellen. Eine spätere automatische Ausführung alter Bedienaktionen nach
Verbindungsverlust ist zu vermeiden. Insbesondere neue Läufe oder Neustarts dürfen nicht durch doppelte
Zustellung mehrfach ausgelöst werden. Gleichzeitige Konsolen-/Web-/Diagnosezugriffe sind zu berücksichtigen.

## Diagramme

Begrenzter Verlauf im Webdienst, über Browser-Neuladen hinweg verfügbar, ohne dauerhafte Speicherung über Webdienst-Neustarts. X-Achse ist virtuelle Simulationszeit; neue Läufe werden getrennt dargestellt. Kapazität, Abtastrate und Darstellung mehrerer Änderungen zur gleichen virtuellen Zeit sind noch festzulegen. Fehlende oder ungültige Daten nicht als ununterbrochene gültige Kurve darstellen.

## Geprüfte Grundlagen und offene Umsetzung

- [Gateway](../../src/ModbusRabbitGateway/README.md): liest Geräte über Modbus und veröffentlicht per MQTT an RabbitMQ. Bestätigte Erweiterung, noch nicht umgesetzt: MQTT-Befehlsempfang, geprüfte Modbus-Schreibzugriffe und korrelierte Rückmeldungen; Webdienst selbst spricht kein Modbus. Dynamische Geräteaufnahme ist zusätzlich zu entwerfen.
- [Diagnose-Client](../../src/ModbusDiagnosticClient/README.md): bestehendes Vorbild für Sollwertprüfung und Rücklesen. Fehlerregister beziehen sich auf den letzten Schreibzugriff irgendeines Clients, nicht exklusiv auf einen Webauftrag; parallele Bedienung bei der Rückmeldung berücksichtigen.
- [Master](../../src/SimulationMaster/README.md): Zeitserver vorhanden; dokumentierte Bedienung über Konsole. MQTT-Status und MQTT-Bedienbefehle sind neu zu entwerfen.
- [Wetterdienst](../../src/WeatherSimulator/README.md): HTTP-Wetterversorgung und optional MQTT vorhanden; manuelle Bedienung bislang über Konsole. MQTT-Befehlsempfang und Rückmeldungen sind neu zu entwerfen.
- [Startskript](../../start-all.ps1): öffnet Windows-PowerShell-Fenster. Für die plattformübergreifende Prozessverwaltung ist ein eigener Startweg nötig.
- [PV-Anzeige](../../src/Devices/PvSimulator/Output/ConsoleDisplay.cs): Leistung/Ertrag grün, Automatik grün, manuell gelb, Pause magenta; Wetterfarben zustandsabhängig. Diese Konsolenfarben sind Inspiration, keine bereits festgelegte Webpalette.

Noch auszuarbeiten: konkrete lokale Prozessverwaltung, MQTT-Befehlsvertrag mit Ablaufzeit, Duplikatbehandlung und eindeutigen Ergebnissen, dynamische Geräteaufnahme, sichere Wiedererkennung von Prozessen, Konflikte paralleler Bedienung sowie Umfang der Geräteseiten. ASP.NET Core 10 mit Blazor Interactive Server ist beschlossen; weitere Bibliotheken und technische Detailverträge bleiben auszuarbeiten.

## Vorgeschlagene Umsetzungsschritte und Abnahme

Die Reihenfolge ist ein Umsetzungsvorschlag, keine bereits ausgeführte Arbeit:

1. Datenbedarf jeder Ansicht mit Register-/Nachrichtenverträgen abgleichen; Befehlsvertrag und dynamische Geräteaufnahme entwerfen.
2. Gemeinsame fachliche Bedienfunktionen für Konsole und Nachrichtenempfang anbinden; Gateway, Master und Wetter entsprechend erweitern.
3. Plattformübergreifende Instanzkonfiguration und Prozessverwaltung einschließlich Broker-Start, Erreichbarkeit und Wiedererkennung umsetzen.
4. Weboberfläche mit gemeinsamer Navigation, Statusmodell und individuellen Modulansichten aufbauen; begrenzte Verläufe nach Simulationslauf trennen.
5. Nach gesonderter Freigabe mit echten Modulen unter Windows und Linux prüfen.

Bei einer späteren Abnahme zu zeigen:

- Mehrere Instanzen desselben Typs mit eindeutigen Kennungen und Ports; Speichern ohne Start und Duplizieren ohne Konflikte.
- Start mit zunächst pausiertem Master; Teilfehler lassen erfolgreiche Starts bestehen und werden gezielt behandelbar angezeigt.
- Kein direkter Simulator-Datenweg des Webdiensts neben RabbitMQ; Broker-Ausfall wird sichtbar und erzeugt keine verspäteten überraschenden Befehle.
- Sollwerte mit tatsächlicher Rückmeldung; abgelehnte, doppelte, abgelaufene und unklar beantwortete Befehle verständlich behandeln.
- Browser schließen/öffnen und Webdienst-Neustart beenden keine laufenden Module; externe Prozesse bleiben außerhalb automatischer Stop-Aktionen.
- Eigenständige Konsolenbedienung bleibt nutzbar; fachlicher Zustand ist zwischen Bedienwegen konsistent.
- Gerätespezifische Ansichten, Hell-/Dunkelmodus, Kennzeichnungen ungültiger Daten und getrennte Diagrammläufe funktionieren.

**Dokumentationsprüfung:** Planungsinhalte und relative Links statisch geprüft. Keine Builds, Tests,
Probeläufe oder Prozessstarts für diesen Dokumentationsauftrag; die Abnahmepunkte sind noch nicht nachgewiesen.
