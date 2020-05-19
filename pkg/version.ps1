# Emits Perplex.ContentBlocks version obtained from the DLL
$projectDir = "$PSScriptRoot\..\src\Perplex.ContentBlocks"
$dll = Get-Item "$projectDir\bin\Release\Perplex.ContentBlocks.dll"
Write-Output $dll | % { $_.VersionInfo.ProductVersion }
