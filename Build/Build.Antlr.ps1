
Set-Location $(Get-Item "$PSScriptRoot").Parent.FullName

$AntlrJar = Resolve-Path ".\Build\antlr-4.8-complete.jar"

Get-ChildItem -Path "Qsi.*" -Directory | ForEach-Object {
    $Namespace = $PSItem.Name
    $GrammarDirectory = [System.IO.Path]::Combine($PSItem.FullName, "Antlr")
    $OutputDirectory = [System.IO.Path]::Combine($GrammarDirectory, "generated")

    if (!(Test-Path $GrammarDirectory)) {
        Write-Host "Skip $($PSItem.Name)" -ForegroundColor DarkGray
        continue
    }

    Write-Host "[Antlr4] Build $($PSItem.Name).." -ForegroundColor Green

    # Clean output
    if (Test-Path $OutputDirectory) {
        Remove-Item -Path $OutputDirectory -Recurse -Force -Confirm:$false -ErrorAction Ignore
    }

    java `
        -jar "$AntlrJar" `
        -Dlanguage=CSharp `
        -package "$Namespace" `
        -Xexact-output-dir `
        -o "$OutputDirectory" `
        -visitor `
        "$GrammarDirectory\*.g4"

    # Fetch access modifier
    Get-ChildItem -Path $OutputDirectory\* -Include *.cs | ForEach-Object {
        Write-Host "[Fetch] $($PSItem.Name)" -ForegroundColor Yellow

        $Content = Get-Content -Path $PSItem -Raw
        $Content = $Content -replace 'public(?= +(?:interface|(?:partial +)?class) +[\w<>]+)', 'internal'

        Set-Content -Path $PSItem -Value $Content
    }
}
