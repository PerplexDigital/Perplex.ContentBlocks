$configuration = "Release"

Write-Host "Packing Perplex.ContentBlocks.StaticAssets ..."

$staticAssetsDir = "..\src\Perplex.ContentBlocks.StaticAssets"

Push-Location $staticAssetsDir
try {
    npm install -g pnpm@latest-10 --no-fund --no-audit --prefer-offline
    pnpm install --frozen-lockfile --prefer-offline --config.confirmModulesPurge=false
    pnpm build
} finally {
    Pop-Location
}

dotnet pack $staticAssetsDir\Perplex.ContentBlocks.StaticAssets.csproj -c $configuration -o .

Write-Host "Packing Perplex.ContentBlocks.Core ..."
dotnet pack ..\src\Perplex.ContentBlocks.Core\Perplex.ContentBlocks.Core.csproj -c $configuration -o .

Write-Host "Packing Perplex.ContentBlocks ..."
dotnet pack ..\src\Perplex.ContentBlocks\Perplex.ContentBlocks.csproj -c $configuration -o .
