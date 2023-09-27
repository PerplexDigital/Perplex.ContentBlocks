$configuration = "Release"

Write-Host "Packing Perplex.ContentBlocks.StaticAssets ..."
dotnet pack ..\src\Perplex.ContentBlocks.StaticAssets\Perplex.ContentBlocks.StaticAssets.csproj -c $configuration -o .

Write-Host "Packing Perplex.ContentBlocks.Core ..."
dotnet pack ..\src\Perplex.ContentBlocks.Core\Perplex.ContentBlocks.Core.csproj -c $configuration -o .

Write-Host "Packing Perplex.ContentBlocks ..."
dotnet pack ..\src\Perplex.ContentBlocks\Perplex.ContentBlocks.csproj -c $configuration -o .
