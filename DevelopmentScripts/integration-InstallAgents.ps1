param
(
	[String[]] $AgentGuids
)
# this requires the SqlServer module.

[regex]$regex = '@AgentGuids'

$overrideIntegrationFile = join-path $PSScriptRoot '..\Source\RelativityConnection.Override.config'
$xml = [xml](Get-Content $overrideIntegrationFile)
$primaryConnection = Select-Xml -Xml $xml -Xpath '//add[@name="relativity"]'
$connectionString = $primaryConnection.node.connectionString

[string] $queryText = [System.IO.File]::ReadAllText((join-path $PSScriptRoot "PDB_CreateAgentsEDDS.sql"))
$queryText = $queryText -replace($regex, ($AgentGuids -join ","))

Write-Host $queryText

Invoke-SqlCmd -ConnectionString $connectionString -Query $queryText