
Set-Location $(Get-Item "$PSScriptRoot").Parent.FullName

Function Pack {
    Param (
        [Parameter(Mandatory=$true)][System.IO.DirectoryInfo] $Project
    )

    Write-Host "[Nuget] Pack $($Project.Name).." -ForegroundColor Green

    dotnet pack "$Project" `
        --nologo `
        -c Release `
        -o ".\Build\Archive"
}

Pack(".\Qsi")
Pack(".\Qsi.MySql")
