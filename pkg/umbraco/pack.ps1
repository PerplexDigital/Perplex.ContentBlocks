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

..\build.ps1

$projectDir = Get-Item ..\..\src\Perplex.ContentBlocks

$projectDirCore = Get-Item ..\..\src\Perplex.ContentBlocks.Core
$csprojFileCore = "$projectDirCore\Perplex.ContentBlocks.Core.csproj"

$version = ..\version.ps1

# Create temp dir
$tmpDir = "_tmp"
If(Test-Path $tmpDir) { Remove-Item $tmpDir -Recurse }
New-Item -Path . -Name $tmpDir -ItemType "directory" | Out-Null

# Get Umbraco version from UmbracoCms.Web NuGet dependency
[xml]$csprojCore = Get-Content $csprojFileCore
$umbVersion = $csprojCore.Project.ItemGroup.PackageReference | ? {
    $_.Include -eq "UmbracoCms.Web"
} | %{ $_.Version } | Get-SemVer

$packageXmlIn = "package.xml"
$packageXmlOut = "$tmpDir\package.xml"

[xml]$packageXml = Get-Content $packageXmlIn
$packageXml.umbPackage.info.package.version = $version
$requirements = $packageXml.umbPackage.info.package.requirements
$requirements.major = "$($umbVersion.Major)"
$requirements.minor = "$($umbVersion.Minor)"
$requirements.patch = "$($umbVersion.Patch)"

$files = $packageXml.umbPackage.SelectSingleNode("files")
$xmlns = $packageXml.umbPackage.NamespaceURI

$dll = Get-Item "$projectDirCore\bin\Release\net472\Perplex.ContentBlocks.dll"
$appPluginRoot = "$projectDir\App_Plugins\Perplex.ContentBlocks"
$appPluginFiles = Get-ChildItem $appPluginRoot\* -File -Exclude @("*.less") -Recurse

# Copy DLL + App_Plugins files to package.xml + output dir
@($dll; $appPluginFiles) | % {
    $file = $packageXml.CreateElement("file", $xmlns)
    $guidElement = $packageXml.CreateElement("guid", $xmlns)
    $guid = "$([System.Guid]::NewGuid())$($_.Extension)"
    $guidElement.InnerText = $guid

    $orgName = $packageXml.CreateElement("orgName", $xmlns)
    $orgName.InnerText = $_.Name
    $orgPath = $packageXml.CreateElement("orgPath", $xmlns)

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

$packageXml.Save("$pwd\$packageXmlOut")

# Zip
$packageOut = "Perplex.ContentBlocks_$version.zip"
Compress-Archive $tmpDir\* $packageOut -Force

Remove-Item $tmpDir -Recurse

Write-Host "Done!"
Write-Output (Get-Item $packageOut)

Pop-Location
