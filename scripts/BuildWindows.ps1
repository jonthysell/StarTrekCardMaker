param()

[string] $Product = "StarTrekCardMaker"
[string] $Target = "Windows"

& "$PSScriptRoot\Build.ps1" -Product $Product -Target $Target

& "$PSScriptRoot\ZipRelease.ps1" -Product $Product -Target $Target