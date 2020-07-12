param([string]$Product, [string]$Target)

$StartingLocation = Get-Location

Set-Location -Path "$PSScriptRoot\.."

[string] $OutputRoot = "bld"
[string] $TargetOutputDirectory = "$Product.$Target"
[string] $TargetOutputPackageName = "$Product.$Target.zip"

Write-Host "Remove old package..."

if (Test-Path "$OutputRoot\$TargetOutputPackageName") {
    Remove-Item "$OutputRoot\$TargetOutputPackageName"
}

Write-Host "Create package..."

Set-Location -Path "$OutputRoot"

if (-Not (Test-Path ".\7za.exe")) {
    Write-Host "Get 7zip"
    curl https://www.7-zip.org/a/7zr.exe -o 7zr.exe 
    curl https://www.7-zip.org/a/7z1900-extra.7z -o 7z1900-extra.7z
.\7zr.exe e 7z1900-extra.7z -y 7za.exe
}

.\7za.exe a "$TargetOutputPackageName" "$TargetOutputDirectory" -tzip

Set-Location -Path "$StartingLocation"
