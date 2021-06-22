Param (
    [ValidateNotNullOrEmpty()]
    [string] $Target = 'All'
)

$Tasks = @(
    "Qsi.MySql",
    "Qsi.Cql",
    "Qsi.Hana"
)

if ($Target -eq 'All') {
    # nothing
} elseif ($Tasks -contains $Target) {
    $Tasks = @($Target)
} else {
    throw "'$Target' is an invalid setup target."
}

Set-Location $(Get-Item "$PSScriptRoot").FullName

if (Get-Module Antlr) {
    Remove-Module Antlr -Force
}

Import-Module ".\Build\Antlr.ps1"

for ($i = 0; $i -lt $Tasks.Count; $i++) {
    Antlr-Generate $Tasks[$i] ($i + 1) $Tasks.Count
}
