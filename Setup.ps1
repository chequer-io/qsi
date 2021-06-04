Set-Location $(Get-Item "$PSScriptRoot").FullName

if (Get-Module Antlr) {
    Remove-Module Antlr -Force
}

Import-Module ".\Build\Antlr.ps1"

Antlr-Generate Qsi.MySql
Antlr-Generate Qsi.Cql
Antlr-Generate Qsi.Oracle