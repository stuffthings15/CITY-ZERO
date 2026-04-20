# Move large asset archives out of the repository working tree into Assets/ExternalAssets for archival.
# This script DOES NOT delete files; it moves them to a subfolder and preserves their timestamps.
# Usage: pwsh.exe -File tools/move_large_assets.ps1 -ThresholdMB 10
param(
    [int]$ThresholdMB = 10
)

$base = Resolve-Path .
$assets = Join-Path $base 'Assets'
$dest = Join-Path $assets 'ExternalAssets'
if (-not (Test-Path $dest)) { New-Item -ItemType Directory -Path $dest | Out-Null }

Write-Host "Scanning Assets for files larger than ${ThresholdMB}MB..."
$thresholdBytes = $ThresholdMB * 1MB
$files = Get-ChildItem -Path $assets -Recurse -File | Where-Object { $_.Length -ge $thresholdBytes }

if ($files.Count -eq 0) { Write-Host "No large files found."; exit 0 }

foreach ($f in $files) {
    $rel = $f.FullName.Substring($assets.Length + 1)
    $target = Join-Path $dest ($rel -replace '[\\/]', '_')
    Write-Host "Moving $rel -> $target"
    Move-Item -Path $f.FullName -Destination $target -Force
}

Write-Host "Done. Moved $($files.Count) files to $dest"
