param()

[string] $Product = "StarTrekCardMaker"
[string] $Target = "Windows"

& "$PSScriptRoot\Build.ps1" -Product $Product -Target $Target -PublishArgs "--runtime win-x86 -p:PublishSingleFile=true -p:PublishTrimmed=true --self-contained true"

& "$PSScriptRoot\ZipRelease.ps1" -Product $Product -Target $Target