Push-Location $PSScriptRoot

$nuget = .\nuget\pack.ps1
Write-Host "NuGet -> $(Resolve-Path $nuget.FullName -Relative)"

$umbraco = .\umbraco\pack.ps1
Write-Host "Umbraco -> $(Resolve-Path $umbraco.FullName -Relative)"

Pop-Location
