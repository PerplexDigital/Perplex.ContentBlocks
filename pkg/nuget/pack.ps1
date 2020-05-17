Push-Location $PSScriptRoot

Write-Host "Packing for NuGet ..."

.\nuget pack "..\..\src\Perplex.ContentBlocks\Perplex.ContentBlocks.csproj" -p NoWarn=NU5128 -p Configuration=Release

$version = ..\version.ps1
$nugetFile = Get-ChildItem -Filter "*$version.nupkg"
if(!$nugetFile) {
    Throw "No NuGet file found!"
}

$tmpDir = "_tmp"

if(Test-Path $tmpDir) {
    Remove-Item $tmpDir -Recurse
}

# Rename to .zip to use in Expand-Archive
$nugetZip = "$nugetFile.zip"
Rename-Item $nugetFile $nugetZip

# Unzip
Expand-Archive $nugetZip $tmpDir

# Read .nuspec
$nuspecFile = "$tmpDir\Perplex.ContentBlocks.nuspec"
[xml]$nuspec = Get-Content $nuspecFile

# Add <group targetFramework=".NETFramework4.7.2">
$group = $nuspec.CreateElement("group", $nuspec.package.NamespaceURI)
$group.SetAttribute("targetFramework", ".NETFramework4.7.2")

$dependencies = $nuspec.package.metadata.dependencies

# Add all <dependency> elements to <group
$dependencies.dependency | % { $group.AppendChild($_) } | Out-Null
$dependencies.AppendChild($group) | Out-Null

$nuspec.Save("$pwd\$nuspecFile")

# Update Zip
Compress-Archive $nuspecFile $nugetZip -Update

# Rename back to .nupkg
Rename-Item $nugetZip $nugetFile

# Remove temporary directory
Remove-Item $tmpDir -Recurse

Write-Host "Done!"

# Write resulting .nupkg file to Pipeline
Write-Output (Get-Item $nugetFile)

Pop-Location
