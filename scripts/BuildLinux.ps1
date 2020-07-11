param()

[string] $Product = "StarTrekCardMaker"
[string] $Target = "Linux"

& "$PSScriptRoot\Build.ps1" -Product $Product -Target $Target

& "$PSScriptRoot\TarRelease.ps1" -Product $Product -Target $Target