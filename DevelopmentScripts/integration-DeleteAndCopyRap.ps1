 Param
(
    [Parameter(Mandatory=$true)]
    [string]$CopyTo
)

Remove-Item (Join-Path $CopyTo "*")
Copy-Item "Applications\*" -Destination $CopyTo