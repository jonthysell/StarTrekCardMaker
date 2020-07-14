param([string]$Product, [string]$Target, [string]$PublishArgs)

[string] $SolutionPath = "src\$Product.sln"

[string] $OutputRoot = "bld"
[string] $TargetOutputDirectory = "$Product.$Target"

$StartingLocation = Get-Location
Set-Location -Path "$PSScriptRoot\.."

if (Test-Path "$OutputRoot\$TargetOutputDirectory") {
    Write-Host "Clean output folder..."
    Remove-Item "$OutputRoot\$TargetOutputDirectory" -Recurse
}

Write-Host "Build release..."

dotnet publish $PublishArgs.Split() -c Release -o "$OutputRoot\$TargetOutputDirectory" "$SolutionPath"
Copy-Item "README.md" -Destination "$OutputRoot\$TargetOutputDirectory\ReadMe.txt"
Copy-Item "LICENSE.md" -Destination "$OutputRoot\$TargetOutputDirectory\License.txt"

Set-Location -Path "$StartingLocation"
