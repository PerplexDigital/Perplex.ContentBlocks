# Parse version string to version info object
Function Get-SemVer() {
    [CmdLetBinding()]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [string]$Version
    )
    $Version -match "^(\d+)\.(\d+)\.(\d+)(?:-(.*?))?$" | Out-Null

    [PSCustomObject] @{
        Major = [int]$matches[1]
        Minor = [int]$matches[2]
        Patch = [int]$matches[3]
        Suffix = $matches[4]
        Version = $Version
    }
}

Push-Location $PSScriptRoot

Write-Host "Packing for Umbraco ..."

$projectDir = Get-Item ..\..\src\Perplex.ContentBlocks

$version = ..\version.ps1

# Get Umbraco version from NuGet packages.config file
[xml]$nugetPackages = Get-Content $projectDir\packages.config
$umbVersion = $nugetPackages.packages.package | ? {
    $_.GetAttribute("id") -eq "UmbracoCms.Web"
} | % { $_.version } | Get-SemVer

$packageXmlIn = "package.xml"
$packageXmlOut = "$tmpDir\package.xml"

[xml]$package = Get-Content $packageXmlIn
$package.umbPackage.info.package.version = $version
$requirements = $package.umbPackage.info.package.requirements
$requirements.major = "$($umbVersion.Major)"
$requirements.minor = "$($umbVersion.Minor)"
$requirements.patch = "$($umbVersion.Patch)"

$files = $package.umbPackage.SelectSingleNode("files")
$xmlns = $package.umbPackage.NamespaceURI

$dll = Get-Item "$projectDir\bin\Release\Perplex.ContentBlocks.dll"
$appPluginRoot = "$projectDir\App_Plugins\Perplex.ContentBlocks"
$appPluginFiles = Get-ChildItem $appPluginRoot\* -File -Exclude @("*.less") -Recurse

# Create temp dir
$tmpDir = "_tmp"
If(Test-Path $tmpDir) { Remove-Item $tmpDir -Recurse }
New-Item -Path . -Name $tmpDir -ItemType "directory" | Out-Null

# Copy DLL + App_Plugins files to package.xml + output dir
$($dll; $appPluginFiles) | % {
    $file = $package.CreateElement("file", $xmlns)
    $guidElement = $package.CreateElement("guid", $xmlns)
    $guid = "$([System.Guid]::NewGuid())$($_.Extension)"
    $guidElement.InnerText = $guid

    $orgName = $package.CreateElement("orgName", $xmlns)
    $orgName.InnerText = $_.Name
    $orgPath = $package.CreateElement("orgPath", $xmlns)

    if($_.Extension -eq ".dll") {
        $orgPath.InnerText = "~/bin"
    } else {
        $relativePath = $_.Directory.ToString(). `
            Replace($projectDir.FullName, "~"). `
            Replace("\", "/")

        $orgPath.InnerText = $relativePath
    }

    $file.AppendChild($guidElement) | Out-Null
    $file.AppendChild($orgName) | Out-Null
    $file.AppendChild($orgPath) | Out-Null

    $files.AppendChild($file) | Out-Null

    Copy-Item $_.FullName $tmpDir\$guid
}

$packageOut = "$tmpDir\package.xml"
$package.Save("$pwd\$packageOut")

# Zip
$packageOut = "Perplex.ContentBlocks_$version.zip"
Compress-Archive $tmpDir\* $packageOut -Force

Remove-Item $tmpDir -Recurse

Write-Host "Done!"
Write-Output (Get-Item $packageOut)

Pop-Location
