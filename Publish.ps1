Param (
    [Parameter(Mandatory = $true)][Version] $Version
)

Set-Location $(Get-Item "$PSScriptRoot").FullName

Remove-Module Common -Force
Remove-Module Antlr -Force
Import-Module ".\Build\Common.ps1"
Import-Module ".\Build\Antlr.ps1"

# git rev-parse HEAD
$GitTagVersion = [Version]$(git describe --tags --abbrev=0).trimstart('v')

if ($(git rev-parse --abbrev-ref HEAD) -ne "master") {
    throw "Publish is only allow in 'master' branch."
}

$GitCurrentCommitId = git rev-parse HEAD
$GitLatestCommitId = git rev-parse origin/master

if ($GitCurrentCommitId -ne $GitLatestCommitId) {
    throw "Not in the latest commit. (latest: $GitLatestCommitId)"
}

if ($(git diff --name-only).Length -gt 0) {
    throw "There are files that have been changed."
}

if ($GitTagVersion -ge $Version) {
    throw "The version is lower than the git tag. ($GitTagVersion >= $Version)"
}

Class Task {
    [string] $Project
    [bool] $Antlr
 
    Task ([string] $Project, [bool] $Antlr) {
        $this.Project = $Project
        $this.Antlr = $Antlr
    }
}

$PublishDirectory = $(Resolve-Path-Safe ".\Publish")
$Tasks = 
[Task]::new("Qsi", $false),
[Task]::new("Qsi.MySql", $true),
[Task]::new("Qsi.PostgreSql", $false)

Function DotNet-Pack {
    Param (
        [Parameter(Mandatory = $true)][string] $ProjectName
    )

    Write-Host "[.NET] $($ProjectName) Pack.." -ForegroundColor Cyan

    dotnet pack $ProjectName `
        --nologo `
        -v=q `
        -c Release `
        -o $PublishDirectory `
        -p:Version=$Version `
        -p:PackageVersion=$Version
}

Function Nuget-Publish {
    Param (
        [Parameter(Mandatory = $true)][System.IO.FileInfo] $PackageFile,
        [Parameter(Mandatory = $true)][string] $Source
    )

    Write-Host "[NuGet] $($PackageFile.Name) Push to $Source.." -ForegroundColor Cyan

    #dotnet nuget push $PackageFile -s $Source
}

# Clean publish
Remove-Directory-Safe $PublishDirectory

$Tasks | ForEach-Object {
    if ($PSItem.Antlr) {
        Antlr-Generate $PSItem.Project
    }

    DotNet-Pack $PSItem.Project
}

# Publish
Get-ChildItem -Path $PublishDirectory/*.nupkg | ForEach-Object {
    Nuget-Publish $PSItem "github"
    Nuget-Publish $PSItem "nuget.org"
}

# Tag
