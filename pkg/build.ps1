Push-Location $PSScriptRoot

$projectFile = "..\src\Perplex.ContentBlocks.Core\Perplex.ContentBlocks.Core.csproj"

Write-Host "Cleaning project ..."
dotnet clean $projectFile -c Release -v q --nologo

Write-Host "Building project ..."
dotnet build $projectFile -c Release -v q --nologo

Pop-Location
