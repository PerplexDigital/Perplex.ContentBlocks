# Emits Perplex.ContentBlocks version obtained from the DLL
$projectDir = "$PSScriptRoot\..\src\Perplex.ContentBlocks.Core"
$dll = Get-Item "$projectDir\bin\Release\*\Perplex.ContentBlocks.dll" | Select-Object -First 1
Write-Output $dll | % { $_.VersionInfo.ProductVersion }
