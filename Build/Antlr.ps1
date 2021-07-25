$AntlrJar = Resolve-Path ".\Build\antlr-4.8-complete.jar"

Function Antlr-Generate {
    Param (
        [Parameter(Mandatory = $true)][string] $ProjectName,
        [Parameter(Mandatory = $false)][bool] $NoListener = $true,
        [Parameter(Mandatory = $false)][bool] $NoVisitor  = $true,
        [Parameter(Mandatory = $false)][string[]] $Ignores,
        [Parameter(Mandatory = $false)][int] $Progress,
        [Parameter(Mandatory = $false)][int] $Total
    )
    
    $ProjectDirectory = $(Resolve-Path $ProjectName)
    $ProjectName = $(Get-Childitem $ProjectDirectory -Filter *.csproj | Select-Object -First 1).Basename
    $Namespace = "$($ProjectName).Internal"
    $GrammarDirectory = [System.IO.Path]::Combine($ProjectDirectory, "Antlr")
    $OutputDirectory = [System.IO.Path]::Combine($GrammarDirectory, "generated")
    $Header = If ($Total -gt 0) { "[ANTLR4] ($Progress/$Total)" } Else { "[ANTLR4]" }

    if (!(Test-Path $GrammarDirectory)) {
        throw (New-Object System.IO.DirectoryNotFoundException("Grammar directory not found"))
    }
    
    Write-Host "$Header $($ProjectName) Generate" -ForegroundColor Green

    # Clean output
    if (Test-Path $OutputDirectory) {
        Remove-Item -Path $OutputDirectory -Recurse -Force -Confirm:$false -ErrorAction Ignore
    }

    # Clean grammar cache
    Remove-Item -Path $GrammarDirectory/* -Include *.interp

    Get-ChildItem -Path $GrammarDirectory/* -Include *.tokens | ForEach-Object  {
        $FileName = [System.IO.Path]::GetFileName($PSItem.Name)

        if ($Ignores -notcontains $FileName) {
            Remove-Item $PSItem
        }
    }

    # Generate
    $GenArgs = @(
        "-jar $AntlrJar",
        "-Dlanguage=CSharp",
        "-package $Namespace",
        "-Xexact-output-dir",
        "-o $OutputDirectory",
        "$GrammarDirectory/*.g4"
    )

    if ($NoListener) {
        $GenArgs += "-no-listener";
    }

    if ($NoVisitor) {
        $GenArgs += "-no-visitor";
    }

    $proc = Start-Process "java" -ArgumentList $GenArgs -NoNewWindow -PassThru
    $proc.WaitForExit()

    if ($LASTEXITCODE -ne 0) {
        throw "$Header $($ProjectName) Failed generate"
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
