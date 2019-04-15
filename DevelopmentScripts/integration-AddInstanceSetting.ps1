param
(
	[string] $Section,
	[string] $Name,
	[string] $Value,
	[string] $MachineName
)
# this requires the SqlServer module.

[regex]$regexSection = '@Section'
[regex]$regexName = '@Name'
[regex]$regexValue = '@Value'
[regex]$regexMachineName = '@MachineName'

$overrideIntegrationFile = join-path $PSScriptRoot '..\Source\RelativityConnection.Override.config'
$xml = [xml](Get-Content $overrideIntegrationFile)
$primaryConnection = Select-Xml -Xml $xml -Xpath '//add[@name="relativity"]'
$connectionString = $primaryConnection.node.connectionString

[string] $queryText = [System.IO.File]::ReadAllText((join-path $PSScriptRoot "PDB_InsertInstanceSetting.sql"))
$queryText = $queryText -replace($regexSection, "`'$Section`'")
$queryText = $queryText -replace($regexName, "`'$Name`'")
$queryText = $queryText -replace($regexValue, "`'$Value`'")
$queryText = $queryText -replace($regexMachineName, "`'$MachineName`'")

Write-Host $queryText

Invoke-SqlCmd -ConnectionString $connectionString -Query $queryText