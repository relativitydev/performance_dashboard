# this requires the SqlServer module.

$overrideIntegrationFile = join-path $PSScriptRoot '..\Source\RelativityConnection.Override.config'
$xml = [xml](Get-Content $overrideIntegrationFile)
$primaryConnection = Select-Xml -Xml $xml -Xpath '//add[@name="relativity"]'
$connectionString = $primaryConnection.node.connectionString
$connectionString = $connectionString.replace('initial catalog=EDDS;', 'initial catalog=EDDSPerformance;')
$scriptsFolder = $PSScriptRoot

function InvokeSql ($filePath, $parameters) {
    $fileContent = [System.IO.File]::ReadAllText(( $filePath ));
    $parameters.Keys | foreach { $fileContent = $fileContent -replace "@$_", $parameters[$_] }
    return (Invoke-Sqlcmd -ConnectionString $connectionString -Query $fileContent)
}

function InvokeSqlFromText ($textContent, $parameters) {
    $parameters.Keys | foreach { $textContent = $textContent -replace "@$_", $parameters[$_] }
    return (Invoke-Sqlcmd -ConnectionString $connectionString -Query $textContent)
}

$BackupGapsCount = (InvokeSqlFromText "SELECT count(1) as [DatabaseGapsCount] FROM [EDDSPerformance].[eddsdbo].[DatabaseGaps] where activitytype = 1" @{}).DatabaseGapsCount
if ($BackupGapsCount -lt 12) { throw "Backup gaps created. Expected 12 but was " + $BackupGapsCount; }

$DatabasesWithDbccCount = (InvokeSqlFromText "SELECT count(1) as DatabasesWithDbccCount FROM [EDDSPerformance].[eddsdbo].[Databases] where LastDbccDate is not null" @{}).DatabasesWithDbccCount
if ($DatabasesWithDbccCount -lt 1) { throw "Last Dbcc per DB not tracked. Expected at least 1 but was " + $DatabasesWithDbccCount; }

$BackupFullAndDiffGapsCount = (InvokeSqlFromText "SELECT count(1) as [DatabaseGapsCount] FROM [EDDSPerformance].[eddsdbo].[DatabaseGaps] where activitytype = 3" @{}).DatabaseGapsCount
if ($BackupFullAndDiffGapsCount -lt 4) { throw "Backup full and diff gaps created. Expected 4 but was " + $BackupFullAndDiffGapsCount; }

$MetricsWithValidData = (InvokeSql (join-path $scriptsFolder "PDB_GetRIMetricsWithValidData.sql") @{}).MetricsWithValidData
if ($MetricsWithValidData -lt 6) { throw "Metrics types didnt have valid data. Expected 6 but was " + $MetricsWithValidData; }