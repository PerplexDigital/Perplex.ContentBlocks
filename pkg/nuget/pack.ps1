# Convert hashtable to nuget pack property format (key=value)
Function Get-NuGetProps() {
    Param([Parameter(Mandatory = $true)] [hashtable]$Tokens)
    ($Tokens.Keys | %{ "$_=$($Tokens[$_])" }) -join ";"
}

Push-Location $PSScriptRoot

Write-Host "Packing for NuGet ..."

$version = ..\version.ps1

$tokens = @{
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

$tokensCore = $tokens.Clone()
$tokensCore.description = "Perplex.ContentBlocks assembly only"

$props = Get-NuGetProps($tokens)
$propsCore = Get-NuGetProps($tokensCore)

# Perplex.ContentBlocks
.\nuget pack Perplex.ContentBlocks.nuspec -p "$props" -p NoWarn=NU5105,NU5128

# Perplex.ContentBlocks.Core
.\nuget pack Perplex.ContentBlocks.Core.nuspec -p "$propsCore" -p NoWarn=NU5105

Write-Host "Done!"

# Write resulting .nupkgs file to pipeline
$nugetFiles = Get-ChildItem -Filter "*$version.nupkg"
Write-Output $nugetFiles

Pop-Location
