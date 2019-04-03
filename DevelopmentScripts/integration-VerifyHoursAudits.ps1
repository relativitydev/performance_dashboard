# this requires the SqlServer module.

$overrideIntegrationFile = join-path $PSScriptRoot '..\Source\RelativityConnection.Override.config'
$xml = [xml](Get-Content $overrideIntegrationFile)
$primaryConnection = Select-Xml -Xml $xml -Xpath '//add[@name="relativity"]'
$connectionString = $primaryConnection.node.connectionString
$connectionString = $connectionString.replace('initial catalog=EDDS;', 'initial catalog=EDDSPerformance;')
$scriptsFolder = (join-path $PSScriptRoot "..\Source\kCura.PDB.Data\Resources\Sql\")

function InvokeSql ($filePath, $parameters) {
    $fileContent = [System.IO.File]::ReadAllText(( $filePath ));
    if($parameters -ne $null)
    {
        $parameters.Keys | foreach { $fileContent = $fileContent -replace "@$_", $parameters[$_] }
    }
    return (Invoke-Sqlcmd -ConnectionString $connectionString -Query $fileContent)
}

# query count of hour batches completed
$batchesCompleted = (InvokeSql (join-path $scriptsFolder "Backfill_ReadHoursAuditsProcessed.sql") @{backFillHours=-7}).Column1;
if($batchesCompleted -lt 8) {throw "Not enough batches were created."}

# query count of sample history
$sampleHistorySize = (InvokeSql (join-path $scriptsFolder "SampleHistory_Count.sql")).Column1;
if($sampleHistorySize -lt 8) {throw "Not enough hours in the sample."}