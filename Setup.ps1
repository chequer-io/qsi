Param (
    [ValidateNotNullOrEmpty()]
    [string] $Target = 'All'
)

Class Task {
    [string] $Project
    [bool] $NoListener
    [bool] $NoVisitor
    [string[]] $Ignores

    Task ([string] $Project, [bool] $NoListener, [bool] $NoVisitor) {
        $this.Project = $Project
        $this.NoListener = $NoListener
        $this.NoVisitor = $NoVisitor
        $this.Ignores = @()
    }
 
    Task ([string] $Project, [bool] $NoListener, [bool] $NoVisitor, [string[]] $Ignores) {
        $this.Project = $Project
        $this.NoListener = $NoListener
        $this.NoVisitor = $NoVisitor
        $this.Ignores = $Ignores
    }
}

$Tasks = @(
    "Qsi.MySql",
    "Qsi.Cql",
    "Qsi.Hana",
    "Qsi.Impala",
    "Qsi.Trino",
    "Qsi.Oracle"
)

if ($Target -eq 'All') {
    # nothing
} else {
    $Tasks = $Tasks | Where-Object {$_.Project -eq $Target}
}

if ($Tasks.Length -eq 0) {
    throw "'$Target' is an invalid setup target."
}

Set-Location $(Get-Item "$PSScriptRoot").FullName

if (Get-Module Antlr) {
    Remove-Module Antlr -Force
}

Import-Module ".\Build\Antlr.ps1"

for ($i = 0; $i -lt $Tasks.Count; $i++) {
    $Task = $Tasks[$i]

    if ($Tasks.Count -eq 1) {
        Antlr-Generate $Task.Project $Task.NoListener $Task.NoVisitor $Task.Ignores
    } else {
        Antlr-Generate $Task.Project $Task.NoListener $Task.NoVisitor $Task.Ignores ($i + 1) $Tasks.Count
    }
}
