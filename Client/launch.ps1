# Script de lancement LABOR CONTROL - Frontend Blazor
Write-Host "=== LABOR CONTROL - Lancement Frontend Blazor ===" -ForegroundColor Cyan
Write-Host ""

# Aller dans le dossier du projet
Set-Location "C:\Dev\LC\LaborControl.Web"

# Restaurer les packages
Write-Host "Restauration des packages NuGet..." -ForegroundColor Yellow
dotnet restore

if ($LASTEXITCODE -ne 0) {
    Write-Host "Erreur lors de la restauration des packages" -ForegroundColor Red
    pause
    exit 1
}

Write-Host ""
Write-Host "Packages restaurés avec succes!" -ForegroundColor Green
Write-Host ""

# Lancer l'application
Write-Host "Demarrage de l'application..." -ForegroundColor Yellow
Write-Host ""
Write-Host "L'application sera disponible sur:" -ForegroundColor Cyan
Write-Host "  http://localhost:5000" -ForegroundColor White
Write-Host "  https://localhost:5001" -ForegroundColor White
Write-Host ""
Write-Host "Appuyez sur Ctrl+C pour arreter" -ForegroundColor Gray
Write-Host ""

dotnet run
