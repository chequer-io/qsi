Function Resolve-Path-Safe {
    Param (
        [Parameter(Mandatory=$true)][string] $Value
    )

    return $ExecutionContext.SessionState.Path.GetUnresolvedProviderPathFromPSPath($Value)
}

Function Remove-Directory-Safe {
    Param (
        [Parameter(Mandatory=$true)][string] $Value
    )

    if (Test-Path $Value) {
        Remove-Item -Path $Value -Recurse -Force -Confirm:$false -ErrorAction Ignore
    }
}
