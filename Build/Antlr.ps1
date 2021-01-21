$AntlrJar = Resolve-Path ".\Build\antlr-4.8-complete.jar"

Function Antlr-Generate {
    Param (
        [Parameter(Mandatory = $true)][string] $ProjectName
    )
    
    $ProjectDirectory = $(Resolve-Path $ProjectName)
    $Namespace = "$($ProjectName).Internal"
    $GrammarDirectory = [System.IO.Path]::Combine($ProjectDirectory, "Antlr")
    $OutputDirectory = [System.IO.Path]::Combine($GrammarDirectory, "generated")

    if (!(Test-Path $GrammarDirectory)) {
        throw (New-Object System.IO.DirectoryNotFoundException("Grammar directory not found"))
    }
    
    Write-Host "[Antlr4] $($ProjectName) Generate" -ForegroundColor Green

    # Clean output
    if (Test-Path $OutputDirectory) {
        Remove-Item -Path $OutputDirectory -Recurse -Force -Confirm:$false -ErrorAction Ignore
    }

    # Clean grammar cache
    Remove-Item -Path $GrammarDirectory/* -Include *.interp

    Get-ChildItem -Path $GrammarDirectory/* -Include *.g4 | ForEach-Object  {
        $FileName = [System.IO.Path]::ChangeExtension($PSItem.Name, "tokens")
        Remove-Item -Path $GrammarDirectory/* -Include $FileName
    }

    # Generate
    java `
        -jar $AntlrJar `
        -Dlanguage=CSharp `
        -package $Namespace `
        -Xexact-output-dir `
        -o $OutputDirectory `
        $GrammarDirectory/*.g4 `
        -no-listener `
        -no-visitor

    if ($LASTEXITCODE -ne 0) {
        throw "[Antlr4] $($ProjectName) Failed generate"
    }

    # Move grammar cache (interp, tokens)
    Get-ChildItem -Path $OutputDirectory/* -Include *.interp, *.tokens | ForEach-Object {
        Move-Item $PSItem $GrammarDirectory
    }

    # Patch access modifier
    Get-ChildItem -Path $OutputDirectory/*.cs | ForEach-Object {
        Write-Host " Patch $($PSItem.Name)" -ForegroundColor Yellow

        $Content = Get-Content -Path $PSItem -Raw
        $Content = $Content -replace 'public(?= +(?:interface|(?:partial +)?class) +[\w<>]+)', 'internal'
        $Content = $Content -replace '\s+\[System\.CLSCompliant\(false\)\]', ''

        Set-Content -Path $PSItem -Value $Content
    }
}
