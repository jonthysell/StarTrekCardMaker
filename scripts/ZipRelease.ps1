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

7za a "$TargetOutputPackageName" "$TargetOutputDirectory" -tzip

Set-Location -Path "$StartingLocation"
