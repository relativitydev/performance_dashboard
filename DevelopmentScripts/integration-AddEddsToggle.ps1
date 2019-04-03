param
(
	[string] $Name,
	[bool] $Value
)
# this requires the SqlServer module.

[regex]$regexName = '@Name'
[regex]$regexValue = '@Value'

$overrideIntegrationFile = join-path $PSScriptRoot '..\Source\RelativityConnection.Override.config'
$xml = [xml](Get-Content $overrideIntegrationFile)
$primaryConnection = Select-Xml -Xml $xml -Xpath '//add[@name="relativity"]'
$connectionString = $primaryConnection.node.connectionString

[string] $queryText = [System.IO.File]::ReadAllText((join-path $PSScriptRoot "PDB_InsertEddsToggle.sql"))
$queryText = $queryText -replace($regexName, "`'$Name`'")
$queryText = $queryText -replace($regexValue, "`'$Value`'")

Write-Host $queryText

Invoke-SqlCmd -ConnectionString $connectionString -Query $queryText