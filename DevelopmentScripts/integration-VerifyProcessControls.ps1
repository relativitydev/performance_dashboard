$overrideIntegrationFile = join-path $PSScriptRoot '..\Source\RelativityConnection.Override.config'
$xml = [xml](Get-Content $overrideIntegrationFile)
$primaryConnection = Select-Xml -Xml $xml -Xpath '//add[@name="relativity"]'
$connectionString = $primaryConnection.node.connectionString
if($connectionString -is [system.array])
{
    $connectionString = $connectionString[0]
}
$scriptsFolder = (join-path $PSScriptRoot "..\Source\kCura.PDB.Data\Resources\Sql\")

function InvokeSql ($filePath) {
    $fileContent = [System.IO.File]::ReadAllText(( $filePath ));
    $connection = New-Object System.Data.SqlClient.SqlConnection($connectionString);
    $adapter = New-Object System.Data.SqlClient.SqlDataAdapter($fileContent, $connection);
    $dt = new-object System.Data.DataTable
    $adapter.Fill($dt);
    return $dt;
}

# query all the process controls
$table = InvokeSql(join-path $scriptsFolder "ProcessControl_ReadAll.sql")

# parse any failed process controls
$failedProcessControls = new-object "System.Collections.Generic.List[String]"
foreach($obj in $table){
    if($obj.LastExecSucceeded -eq $FALSE)
    {
        $failedProcessControls.Add([string]::Format("{0} - {1}", $obj.ProcessControlID, $obj.ProcessTypeDesc))
    }
}

# error if any failed process controls exist
if([System.Linq.Enumerable]::Any($failedProcessControls))
{
    # output with details on what failed process controls there were
    $failedProcessControlIdText = [System.String]::Join(", ", $failedProcessControls)
    throw [string]::Format("Failed process controls found: {0}", $failedProcessControlIdText);
}