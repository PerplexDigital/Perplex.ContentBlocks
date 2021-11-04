Push-Location $PSScriptRoot

Write-Host "Packing for NuGet ..."

# Perplex.ContentBlocks
dotnet pack ..\..\src\Perplex.ContentBlocks --no-build -o . -v q --nologo

# Perplex.ContentBlocks.Core
dotnet pack ..\..\src\Perplex.ContentBlocks.Core -c Release -o . -v q --nologo

Write-Host "Done!"

# Write resulting .nupkgs file to pipeline
$version = ..\version.ps1
$nugetFiles = Get-ChildItem -Filter "*$version.nupkg"
Write-Output $nugetFiles

Pop-Location
