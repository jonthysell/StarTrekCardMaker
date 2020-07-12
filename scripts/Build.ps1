param([string]$Product, [string]$Target)

$StartingLocation = Get-Location

Set-Location -Path "$PSScriptRoot\.."

[string] $SolutionPath = "src\$Product.sln"
[string] $ProfilesPath = "src\$Product\Properties\PublishProfiles"

[string] $OutputRoot = "bld"
[string] $TargetOutputDirectory = "$Product.$Target"

Write-Host "Clean output folder..."

if (Test-Path "$OutputRoot\$TargetOutputDirectory") {
    Remove-Item "$OutputRoot\$TargetOutputDirectory" -Recurse
}

Write-Host "Build release..."

dotnet publish -p:PublishProfile="$ProfilesPath\$Target`Release.pubxml" -o "$OutputRoot\$TargetOutputDirectory" "$SolutionPath"

Write-Host "Copy readme and license..."

Copy-Item "README.md" -Destination "$OutputRoot\$TargetOutputDirectory\ReadMe.txt"
Copy-Item "LICENSE.md" -Destination "$OutputRoot\$TargetOutputDirectory\License.txt"

Set-Location -Path "$StartingLocation"
