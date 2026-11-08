# pruefe-ports.ps1 - read-only Portcheck fuer EnergyTwin-Probelaeufe
# Aufruf: powershell -File .opencode/skills/probelauf/scripts/pruefe-ports.ps1
# Aendert nichts und beendet keine Prozesse.
[CmdletBinding()]
param(
    [int[]]$Ports = @(8080, 5100, 1502, 1503, 1504, 1505, 1883, 15672)
)

$names = @{
    8080  = 'Wetter (HTTP)'
    5100  = 'Zeitverteilung (Master, TCP)'
    1502  = 'Modbus PV / Charger'
    1503  = 'Modbus Batterie'
    1504  = 'Modbus Charger (Demo)'
    1505  = 'Modbus Kuehlhaus'
    1883  = 'MQTT (RabbitMQ)'
    15672 = 'RabbitMQ Management'
    5672  = 'AMQP (nicht genutzt)'
}

function Test-Port {
    param([int]$Port)

    $client = [System.Net.Sockets.TcpClient]::new()
    try {
        $task = $client.ConnectAsync('127.0.0.1', $Port)
        if (-not $task.Wait(250)) { return $false }
        return $task.Status -eq [System.Threading.Tasks.TaskStatus]::RanToCompletion
    }
    catch {
        return $false
    }
    finally {
        $client.Dispose()
    }
}

'Port   Status   Dienst'
'----   ------   ------'
foreach ($port in $Ports) {
    $status = if (Test-Port -Port $port) { 'belegt' } else { 'frei' }
    $name = if ($names.ContainsKey($port)) { $names[$port] } else { '(unbekannt)' }
    '{0,-6} {1,-7}  {2}' -f $port, $status, $name
}

''
'Hinweis: "belegt" ist kein Fehler, solange klar ist, welcher Prozess den Port haelt.'
'Vor dem Lauf: alte Master/Geraete beenden. Nach dem Lauf: erneut pruefen.'
