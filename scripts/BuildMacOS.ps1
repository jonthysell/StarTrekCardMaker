[string] $Product = "StarTrekCardMaker"
[string] $Target = "MacOS"

& "$PSScriptRoot\Build.ps1" -Product $Product -Target $Target -BuildArgs "-t:BundleApp -p:RuntimeIdentifier=osx-x64 -p:PublishSingleFile=true -p:PublishTrimmed=true"

Write-Host "Removing unbundled files"
Remove-Item "$PSScriptRoot\..\bld\$Product.$Target\$Product"
Remove-Item "$PSScriptRoot\..\bld\$Product.$Target\$Product.pdb"

& "$PSScriptRoot\TarRelease.ps1" -Product $Product -Target $Target