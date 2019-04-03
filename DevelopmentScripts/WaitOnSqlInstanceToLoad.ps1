param
(
    [String] [Parameter(Mandatory = $true)] $ServerName
)

$overrideIntegrationFile = join-path $PSScriptRoot '..\Source\RelativityConnection.Override.config'
$xml = [xml](Get-Content $overrideIntegrationFile);
$secondaryConnection = Select-Xml -Xml $xml -Xpath '//add[@name="relativityWorkspace"]'
$connectionString = $secondaryConnection.node.connectionString

$builder = New-Object System.Data.SqlClient.SqlConnectionStringBuilder $connectionString

$builder["Data Source"] = $ServerName
$builder["Initial Catalog"] = "master"
$builder["Connect Timeout"] = 20
$connectionString = $builder.ToString()
#$connectionString

Write-Output "Checking if sql server $ServerName is loaded and ready."
    $result = 0;
do{
    $result = 0;
    try {
        $result = (Invoke-Sqlcmd -ConnectionString $connectionString -Query "select 1" -QueryTimeout 20).Column1
    }
	catch{
        $_.Exception.Message
        Write-Output "$ServerName is not loaded yet. Trying again in 10 seconds."
    }
	Start-Sleep 10
}while($result -ne 1)

 Write-Output "$ServerName has completed loading."