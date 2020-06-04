Push-Location $PSScriptRoot

Write-Host "Packing for NuGet ..."

$version = ..\version.ps1

$nuspecTokens = @{
    id = "Perplex.ContentBlocks";
    version = $version;
    title = $id;
    authors = "Perplex Digital";
    owners = $authors;
    description = "Block based content editor for Umbraco";
    license = "MIT";
    requireLicenseAcceptance = "false";
    projectUrl = "https://github.com/PerplexDigital/Perplex.ContentBlocks";
    copyright = "© Perplex Digital";
    tags = "umbraco property editor content block";
    releaseNotes = "https://github.com/PerplexDigital/Perplex.ContentBlocks/blob/master/RELEASE_NOTES.md";
    repositoryUrl = "https://github.com/PerplexDigital/Perplex.ContentBlocks.git";
}

$props = ($nuspecTokens.Keys | %{ "$_=$($nuspecTokens[$_])" }) -join ";"

# Generate .nupkgs
.\nuget pack Perplex.ContentBlocks.nuspec -p "$props" -p NoWarn=NU5105,NU5128
.\nuget pack Perplex.ContentBlocks.Core.nuspec -p "$props" -p NoWarn=NU5105

Write-Host "Done!"

# Write resulting .nupkgs file to pipeline
$nugetFiles = Get-ChildItem -Filter "*$version.nupkg"
Write-Output $nugetFiles

Pop-Location
