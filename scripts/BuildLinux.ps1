param()

[string] $Product = "StarTrekCardMaker"
[string] $Target = "Linux"

& "$PSScriptRoot\Build.ps1" -Product $Product -Target $Target -PublishArgs "--runtime linux-x64 -p:PublishSingleFile=true -p:PublishTrimmed=true --self-contained true"

& "$PSScriptRoot\TarRelease.ps1" -Product $Product -Target $Target