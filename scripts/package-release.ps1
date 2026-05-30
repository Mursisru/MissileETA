# Build Release and create release zip for GitHub (run from repo root or any path).
$ErrorActionPreference = "Stop"
# Repo root = parent of scripts/
$root = Split-Path -Parent $PSScriptRoot
if (-not (Test-Path (Join-Path $root "MissileETA_Engine.slnx"))) {
    $root = Get-Location
}
Set-Location $root

$proj = Join-Path $root "MissileETA_Engine\MissileETA_Engine.csproj"
if (-not (Test-Path $proj)) { throw "Project not found: $proj" }

msbuild $proj /t:Build /p:Configuration=Release
$dll = Join-Path $root "MissileETA_Engine\bin\Release\MissileETA_Engine.dll"
if (-not (Test-Path $dll)) { throw "Build failed or DLL missing: $dll" }

$rel = Join-Path $root "release"
New-Item -ItemType Directory -Path $rel -Force | Out-Null
Copy-Item $dll (Join-Path $rel "MissileETA_Engine.dll") -Force
$zip = Join-Path $rel "MissileETA_Engine_v1.9.0.zip"
if (Test-Path $zip) { Remove-Item $zip -Force }
Compress-Archive -Path (Join-Path $rel "MissileETA_Engine.dll"), (Join-Path $rel "INSTALL.txt") -DestinationPath $zip -Force
Write-Host "OK: $zip"
