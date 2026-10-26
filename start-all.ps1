[CmdletBinding(SupportsShouldProcess)]
param(
    [ValidateRange(1, 100000)]
    [int]$Speed = 600,
    [switch]$Autostart,
    [switch]$NoClient
)

$ErrorActionPreference = 'Stop'
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8
$OutputEncoding = [System.Text.Encoding]::UTF8
$root = Split-Path -Parent $MyInvocation.MyCommand.Path

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

function Wait-Port {
    param(
        [int]$Port,
        [string]$Name,
        [int]$TimeoutSeconds = 30
    )

    $deadline = (Get-Date).AddSeconds($TimeoutSeconds)
    while ((Get-Date) -lt $deadline) {
        if (Test-Port -Port $Port) { return $true }
        Start-Sleep -Milliseconds 250
    }

    Write-Warning "$Name ist nach $TimeoutSeconds s nicht erreichbar (Port $Port)."
    return $false
}

function Start-Program {
    [CmdletBinding(SupportsShouldProcess)]
    param(
        [string]$Title,
        [string[]]$Arguments
    )

    if ($PSCmdlet.ShouldProcess($Title, 'in einem eigenen Fenster starten')) {
        $command = "try { `$Host.UI.RawUI.WindowTitle = '$Title' } catch { }; dotnet $($Arguments -join ' ')"
        Start-Process -FilePath 'powershell.exe' `
            -ArgumentList @('-NoExit', '-NoProfile', '-Command', $command) `
            -WorkingDirectory $root | Out-Null
    }
}

function Start-TwinService {
    param(
        [string]$Title,
        [int]$Port,
        [string[]]$Arguments
    )

    if (Test-Port -Port $Port) {
        Write-Host "  $Title läuft bereits (Port $Port) – wird nicht erneut gestartet." -ForegroundColor DarkYellow
        return
    }

    Start-Program -Title $Title -Arguments $Arguments
    if (-not $WhatIfPreference) {
        [void](Wait-Port -Port $Port -Name $Title)
    }
}

Push-Location $root
try {
    Write-Host 'EnergyTwin – alle Programme starten' -ForegroundColor Cyan
    Write-Host "  Repository: $root"
    Write-Host "  Tempo:      x$Speed"
    Write-Host ''

    if ($PSCmdlet.ShouldProcess('EnergyTwin.sln', 'bauen')) {
        dotnet build EnergyTwin.sln
        if ($LASTEXITCODE -ne 0) {
            throw 'Build fehlgeschlagen – die Programme werden nicht gestartet.'
        }
        Write-Host ''
    }

    Write-Host '1. Wettersimulator und Simulations-Master' -ForegroundColor Cyan
    Start-TwinService -Title 'Wetter' -Port 8080 -Arguments @(
        'run', '--no-build', '--project', 'src/WeatherSimulator'
    )

    $masterArguments = @('run', '--no-build', '--project', 'src/SimulationMaster', '--', '--speed', "$Speed")
    if ($Autostart) { $masterArguments += '--autostart' }
    Start-TwinService -Title 'Simulations-Master' -Port 5100 -Arguments $masterArguments

    Write-Host ''
    Write-Host '2. Feldgeräte' -ForegroundColor Cyan
    Start-TwinService -Title 'PV-Anlage' -Port 1502 -Arguments @(
        'run', '--no-build', '--project', 'src/Devices/PvSimulator', '--', '--id', 'PV-01'
    )
    Start-TwinService -Title 'Batterie' -Port 1503 -Arguments @(
        'run', '--no-build', '--project', 'src/Devices/BatterySimulator', '--', '--id', 'BAT-01', '--modbus-port', '1503'
    )
    Start-TwinService -Title 'Ladesäule' -Port 1504 -Arguments @(
        'run', '--no-build', '--project', 'src/Devices/DualPortCharger', '--', '--id', 'DC-01', '--modbus-port', '1504'
    )
    Start-TwinService -Title 'Kühlhaus' -Port 1505 -Arguments @(
        'run', '--no-build', '--project', 'src/Devices/KuehlhausSimulator', '--', '--id', 'KH-01'
    )

    if (-not $NoClient) {
        Write-Host ''
        Write-Host '3. Modbus-Diagnose-Client (PV, Port 1502)' -ForegroundColor Cyan
        Start-Program -Title 'Modbus-Diagnose-Client' -Arguments @(
            'run', '--no-build', '--project', 'src/ModbusDiagnosticClient', '--', '--port', '1502'
        )
    }

    Write-Host ''
    if (-not $WhatIfPreference) {
        Write-Host 'Alle Fenster sind gestartet.' -ForegroundColor Green
        if (-not $Autostart) {
            Write-Host 'Der Master startet pausiert: im Master-Fenster die Leertaste drücken.' -ForegroundColor Yellow
        }
        Write-Host 'Beenden: Q in den Geräten, Q im Master, Strg+C im Wettersimulator; Fenster danach schließen.'
        Write-Host 'Hinweis: Ohne -NoClient läuft der Diagnose-Client im eigenen Fenster (read, write, watch, quit).'
    }
}
finally {
    Pop-Location
}
