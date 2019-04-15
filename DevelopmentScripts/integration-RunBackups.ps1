param
(
	[string] $eddsdboPassword,
	[string] $saPassword,
	[string] $primaryServer,
	[string] $secondaryServer,
	[int] $numberOfBackups,
	[int] $backupFrequency,
	[int] $firstBackupFinishDateHoursAgo,
	[double] $percentOfDatabasesToBackup,
	[int] $backupDuration
)
# this requires the SqlServer module.

$overrideIntegrationFile = join-path $PSScriptRoot '..\Source\RelativityConnection.Override.config'
$xml = [xml](Get-Content $overrideIntegrationFile)
$primaryConnection = Select-Xml -Xml $xml -Xpath '//add[@name="relativity"]'
$connectionString = $primaryConnection.node.connectionString
$connectionString = $connectionString.replace('initial catalog=EDDS;', 'initial catalog=msdb;')
$connectionString = $connectionString.replace('user id=EDDSdbo;', 'user id=sa;')
$connectionString = $connectionString.replace('password='+$eddsdboPassword+';', 'password='+$saPassword+';')
$connectionString = $connectionString + ';Connection Timeout=450;'

$scriptsFolder = $PSScriptRoot

function InvokeSql ($filePath, $server, $parameters) {
    $fileContent = [System.IO.File]::ReadAllText(( $filePath ));
    if($parameters -ne $null)
    {
        $parameters.Keys | foreach { $fileContent = $fileContent -replace "@$_", $parameters[$_] }
    }
	$conStr = $connectionString -ireplace [regex]::Escape($primaryServer), $server
	$conStr
    return (Invoke-Sqlcmd -ConnectionString $conStr -querytimeout 450 -Query $fileContent)
}

# run backups and dbccs
InvokeSql (join-path $scriptsFolder "PDB_RunBackupsAndDbccsForAllDatabasesOnServer.sql") $primaryServer @{backupsPath='\\'+$primaryServer+'\Backups\'; numberOfBackups = $numberOfBackups; backupFrequency = $backupFrequency; firstBackupFinishDateHoursAgo = $firstBackupFinishDateHoursAgo; percentOfDatabasesToBackup = $percentOfDatabasesToBackup; backupDuration=$backupDuration};
InvokeSql (join-path $scriptsFolder "PDB_RunBackupsAndDbccsForAllDatabasesOnServer.sql") $secondaryServer @{backupsPath='\\'+$secondaryServer+'\Backups\'; numberOfBackups = $numberOfBackups; backupFrequency = $backupFrequency; firstBackupFinishDateHoursAgo = $firstBackupFinishDateHoursAgo; percentOfDatabasesToBackup = $percentOfDatabasesToBackup; backupDuration=$backupDuration};
