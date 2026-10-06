Write-Host "FreshSpin first-run setup" -ForegroundColor Cyan
Write-Host "Clearing Windows downloaded-file marks..." -ForegroundColor Yellow

Get-ChildItem -Path $PSScriptRoot -Recurse -File | Unblock-File -ErrorAction SilentlyContinue

Set-Location $PSScriptRoot

Write-Host "Cleaning old build output..." -ForegroundColor Yellow
if (Test-Path ".\bin") { Remove-Item ".\bin" -Recurse -Force }
if (Test-Path ".\obj") { Remove-Item ".\obj" -Recurse -Force }

Write-Host "Restoring packages..." -ForegroundColor Yellow
dotnet restore
if ($LASTEXITCODE -ne 0) {
    Write-Host "dotnet restore failed." -ForegroundColor Red
    exit $LASTEXITCODE
}

Write-Host "Starting FreshSpin..." -ForegroundColor Green
dotnet run
