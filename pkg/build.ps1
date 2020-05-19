Push-Location $PSScriptRoot

$msBuild = .\vswhere -latest -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe | Select-Object -First 1
if (!$msBuild) {
    Throw "MSBuild not found"
}

$nuget = "nuget\nuget.exe"
$projectFile = "..\src\Perplex.ContentBlocks\Perplex.ContentBlocks.csproj"

Write-Host "Restoring NuGet packages ..."
& $nuget restore $projectFile

Write-Host "Building project ..."
& $msBuild $projectFile -t:rebuild /p:Configuration=Release -v:minimal

Pop-Location
