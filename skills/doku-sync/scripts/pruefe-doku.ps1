# pruefe-doku.ps1 - read-only Doku-Pruefung fuer EnergyTwin
# Aufruf: powershell -File skills/doku-sync/scripts/pruefe-doku.ps1 [-Person dominik-Zach.md] [-Datum 2026-11-09]
# Aendert nichts, committet nichts, startet keine Programme.
[CmdletBinding()]
param(
    [string]$Person = '',
    [string]$Datum = (Get-Date -Format 'yyyy-MM-dd')
)

$ErrorActionPreference = 'Stop'
try {
    $utf8Console = New-Object System.Text.UTF8Encoding($false)
    [Console]::OutputEncoding = $utf8Console
    $OutputEncoding = $utf8Console
} catch { }

$root = (Get-Location).Path
if (-not (Test-Path -LiteralPath (Join-Path $root 'EnergyTwin.sln'))) {
    Write-Output 'Bitte aus dem Repository-Root starten (EnergyTwin.sln nicht gefunden).'
    exit 2
}

$utf8 = New-Object System.Text.UTF8Encoding($false)
$befunde = New-Object System.Collections.Generic.List[string]
$pruefungen = 0

function Add-Befund {
    param([string]$Text)
    $befunde.Add($Text)
}

# 1) Relative Markdown-Links
#    Ausgenommen: graphify-out/ (generiert) und docs/archive/ (bewahrt absichtlich den Originaltext
#    inklusive der damals gueltigen relativen Verweise).
$mdFiles = @(& git -c core.quotepath=false ls-files '*.md')
$linkCount = 0
$linkFiles = 0
foreach ($rel in $mdFiles) {
    if ($rel -like 'graphify-out/*') { continue }
    if ($rel -like 'docs/archive/*') { continue }
    $path = Join-Path $root $rel
    if (-not (Test-Path -LiteralPath $path)) { continue }
    $linkFiles++
    $dir = Split-Path -Parent $path
    $text = [IO.File]::ReadAllText($path, $utf8)
    # Fenced Code-Bloecke entfernen: Beispiele wie [Text](relativer-link) sind keine echten Links.
    $text = [regex]::Replace($text, '(?s)```.*?```', '')
    $text = [regex]::Replace($text, '(?s)~~~.*?~~~', '')
    foreach ($m in [regex]::Matches($text, '\]\(([^)\s]+)')) {
        $target = $m.Groups[1].Value.Trim('<', '>')
        if ($target -match '^(https?:|mailto:|#)') { continue }
        if ($target -match '://') { continue }
        $clean = [Uri]::UnescapeDataString(($target -split '#')[0])
        if ([string]::IsNullOrWhiteSpace($clean)) { continue }
        $linkCount++
        $full = [IO.Path]::GetFullPath((Join-Path $dir $clean))
        if (-not (Test-Path -LiteralPath $full)) {
            Add-Befund ("toter Link: {0} -> {1}" -f $rel, $target)
        }
    }
}
$pruefungen++
Write-Output ("1) Markdown-Links: {0} in {1} Dateien (ohne graphify-out/, docs/archive/)" -f $linkCount, $linkFiles)

# 2) Pfade in Backticks (AGENTS.md, README.md) - nur mit Dateiendung, ohne .env
$pruefungen++
$pfadCount = 0
foreach ($doc in @('AGENTS.md', 'README.md')) {
    $path = Join-Path $root $doc
    if (-not (Test-Path -LiteralPath $path)) { continue }
    $text = [IO.File]::ReadAllText($path, $utf8)
    foreach ($m in [regex]::Matches($text, '`((?:docs|src|tests|docker|skills|scripts)/[A-Za-z0-9_\-./]+\.(?:md|cs|csproj|ps1|json|jsonc|yml|yaml|txt|csv|sln|pdf))`')) {
        $ref = $m.Groups[1].Value
        if ($ref -match '(^|/)\.env') { continue }
        $pfadCount++
        $full = Join-Path $root $ref
        if (-not (Test-Path -LiteralPath $full)) {
            Add-Befund ("Pfad im Text fehlt: {0} -> {1}" -f $doc, $ref)
        }
    }
}
Write-Output ("2) Pfadangaben in AGENTS.md/README.md: {0}" -f $pfadCount)

# 3) *.log unter docs/ (Nachweise muessen .txt sein, .gitignore schliesst *.log aus)
$pruefungen++
$logs = @(Get-ChildItem (Join-Path $root 'docs') -Recurse -File -Filter '*.log' -ErrorAction SilentlyContinue)
foreach ($l in $logs) {
    Add-Befund ("*.log statt *.txt: " + $l.FullName.Replace($root + '\', ''))
}
Write-Output ("3) *.log unter docs/: {0}" -f $logs.Count)

# 4) versionierte .env-Datei (nur .env.example erlaubt)
$pruefungen++
$trackedEnv = @(& git ls-files | Where-Object { $_ -match '(^|/)\.env$' })
foreach ($e in $trackedEnv) {
    Add-Befund ("versionierte .env-Datei: " + $e)
}
Write-Output ("4) versionierte .env-Dateien: {0}" -f $trackedEnv.Count)

# 5) Journal-Eintrag der eigenen Person fuer das Datum
$pruefungen++
if ([string]::IsNullOrWhiteSpace($Person)) {
    Write-Output '5) Journal: uebersprungen (kein -Person angegeben)'
}
else {
    $jpath = Join-Path $root ("docs/development-journal/" + $Person)
    if (-not (Test-Path -LiteralPath $jpath)) {
        Add-Befund ("Journal fehlt: docs/development-journal/" + $Person)
    }
    else {
        $jtext = [IO.File]::ReadAllText($jpath, $utf8)
        $muster = '(?m)^#{2,3} (Nachtrag )?' + [regex]::Escape($Datum) + '\b'
        if ($jtext -notmatch $muster) {
            Add-Befund ("kein Journal-Eintrag fuer {0} in {1}" -f $Datum, $Person)
        }
    }
    Write-Output ("5) Journal {0} fuer {1}: geprueft" -f $Person, $Datum)
}

# Ergebnis
Write-Output ''
Write-Output ("Pruefungen: {0}   Befunde: {1}" -f $pruefungen, $befunde.Count)
foreach ($b in $befunde) {
    Write-Output ('  - ' + $b)
}
if ($befunde.Count -eq 0) {
    Write-Output '  keine Befunde'
}
Write-Output ''
Write-Output 'Hinweis: Der Pruefer kennt keine Absicht. Ein Befund ist ein Pruefauftrag, keine automatische Korrektur.'
