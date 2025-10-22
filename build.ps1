# Script de build LABOR CONTROL - Frontend Blazor
Write-Host "=== LABOR CONTROL - Build Frontend Blazor ===" -ForegroundColor Cyan
Write-Host ""

# Aller dans le dossier du projet
Set-Location "C:\Dev\LC\LaborControl.Web"

# Restaurer les packages
Write-Host "Restauration des packages..." -ForegroundColor Yellow
dotnet restore

if ($LASTEXITCODE -ne 0) {
    Write-Host "Erreur lors de la restauration" -ForegroundColor Red
    pause
    exit 1
}

Write-Host "OK" -ForegroundColor Green
Write-Host ""

# Build
Write-Host "Compilation du projet..." -ForegroundColor Yellow
dotnet build

if ($LASTEXITCODE -ne 0) {
    Write-Host "Erreur lors de la compilation" -ForegroundColor Red
    pause
    exit 1
}

Write-Host ""
Write-Host "Build termine avec succes!" -ForegroundColor Green
Write-Host ""
pause
