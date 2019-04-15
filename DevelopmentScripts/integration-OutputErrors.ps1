# this requires the SqlServer module.

$overrideIntegrationFile = join-path $PSScriptRoot '..\Source\RelativityConnection.Override.config'
$xml = [xml](Get-Content $overrideIntegrationFile)
$primaryConnection = Select-Xml -Xml $xml -Xpath '//add[@name="relativity"]'
$connectionString = $primaryConnection.node.connectionString
$scriptsFolder = (join-path $PSScriptRoot "..\Source\kCura.PDB.Data\Resources\Sql\")

function InvokeSql ($filePath, $parameters) {
    $fileContent = [System.IO.File]::ReadAllText(( $filePath ));
    $parameters.Keys | foreach { $fileContent = $fileContent -replace "@$_", $parameters[$_] }
    return (Invoke-Sqlcmd -ConnectionString $connectionString -Query $fileContent -As DataSet)
}

$DS = (InvokeSql (join-path $scriptsFolder "GlassRunLog_ReadLastWithEventInfoByEventStatus.sql") @{count=10; statusId=4});

if ($DS.Tables[0].Rows.Count -eq 0) {echo "No event errors found"}
else { $DS.Tables[0].Rows | %{ echo "----------------------------------------------------------------`r`n$($_['grlogid']), $($_['LogTimestampUTC']), $($_['Module']), $($_['NextTask']), $($_['AgentID']), $($_['LogLevel']), $($_['EventId']), $($_['EventType']), $($_['EventSourceId']), $($_['EventStatusId']), $($_['EventTimeStamp']), $($_['EventDelay']), $($_['PreviousEventID']), $($_['EventLastUpdated']), $($_['EventRetries']), $($_['EventExecutionTime']), $($_['EventHourId']), $($_['TaskCompleted']), $($_['OtherVars'])" }  }




