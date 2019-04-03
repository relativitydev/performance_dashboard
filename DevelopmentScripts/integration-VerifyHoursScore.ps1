# this requires the SqlServer module.

$overrideIntegrationFile = join-path $PSScriptRoot '..\Source\RelativityConnection.Override.config'
$xml = [xml](Get-Content $overrideIntegrationFile)
$primaryConnection = Select-Xml -Xml $xml -Xpath '//add[@name="relativity"]'
$connectionString = $primaryConnection.node.connectionString
$connectionString = $connectionString.replace('initial catalog=EDDS;', 'initial catalog=EDDSPerformance;')
$scriptsFolder = (join-path $PSScriptRoot "..\Source\kCura.PDB.Data\Resources\Sql\")

function InvokeSql ($filePath, $parameters) {
    $fileContent = [System.IO.File]::ReadAllText(( $filePath ));
    $parameters.Keys | foreach { $fileContent = $fileContent -replace "@$_", $parameters[$_] }
    return (Invoke-Sqlcmd -ConnectionString $connectionString -Query $fileContent)
}

$deadline = (Get-Date).AddMinutes(15);
while ((get-date) -le $deadline)
{
    $errors = (InvokeSql (join-path $scriptsFolder "Event_ReadCountByStatus.sql") @{status=4}).Column1;
    if ($errors -ne 0) {throw "Errors found during backfill"}

    $disabledAgents =
    (InvokeSql  (join-path $PSScriptRoot "PDB_GetDisabledAgents.sql") @{} |
        foreach { "Agent $($_.Name) disabled:`r`n$($_.Message)`r`n$($_.DetailMessage)" | Write-Error; $_ }).Count -gt 0;
    if ($disabledAgents -ne 0) {throw  "One or more agents were disabled" }

    $hoursCompleted = (InvokeSql (join-path $scriptsFolder "Backfill_ReadHoursCompletedScoring.sql") @{backFillHours=-7}).HoursCompletedScoring
    if ($hoursCompleted -gt 7) { return; }

    "Waiting 10 seconds" | Write-Host
    Start-Sleep -s 10
}
throw "Timed out. Took longer than 15 minutes."