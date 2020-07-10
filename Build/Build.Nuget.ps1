
Set-Location $(Get-Item "$PSScriptRoot").Parent.FullName

Get-ChildItem -Path "Qsi.*" -Directory | ForEach-Object {
    $GrammarDirectory = [System.IO.Path]::Combine($PSItem.FullName, "Antlr")

    if (!(Test-Path $GrammarDirectory)) {
        Write-Host "Skip $($PSItem.Name)" -ForegroundColor DarkGray
        return
    }

    Write-Host "[Nuget] Pack $($PSItem.Name).." -ForegroundColor Green

    dotnet pack "$PSItem" `
        --nologo `
        -c Release `
        -o ".\Build\Archive"
}
