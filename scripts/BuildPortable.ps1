param()

[string] $Product = "StarTrekCardMaker"
[string] $Target = "Portable"

& "$PSScriptRoot\Build.ps1" -Product $Product -Target $Target -BuildArgs "-target:Publish"

& "$PSScriptRoot\ZipRelease.ps1" -Product $Product -Target $Target
