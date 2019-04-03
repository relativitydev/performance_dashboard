param
(
    [String] [Parameter()] $primaryServer,
	[String] [Parameter()] $secondaryServer,
	[int] [Parameter()] $WorkspaceId,
	[String] [Parameter()] $WorkerAgentInRelativity,
	[String] [Parameter()] $MetricManagerAgentInRelativity,
	[String] [Parameter()] $eddsdboPassword,
	[String] [Parameter()] $saUserName,
	[String] [Parameter()] $saPassword,
	[String] [Parameter()] $RSAPIUserName,
	[String] [Parameter()] $RSAPIPassword
)


function Get-ScriptDirectory {
    Split-Path -parent $PSCommandPath
}

if (!$primaryServer)
{
	$primaryServer = 'localhost'
}
if (!$secondaryServer)
{
	$secondaryServer = $primaryServer
}
if (!$WorkspaceId)
{
	$WorkspaceId = 1016430
}

if (!$WorkerAgentInRelativity)
{
	$WorkerAgentInRelativity = "false"
}

if (!$MetricManagerAgentInRelativity)
{
	$MetricManagerAgentInRelativity = "false"
}

$boolOut = $null;
if(![bool]::TryParse($WorkerAgentInRelativity, [ref]$boolOut) -or ![bool]::TryParse($MetricManagerAgentInRelativity, [ref]$boolOut))
{
	throw "WorkerAgentInRelativity and/or MetricManagerAgentInRelativity do not evaluate to true/false.  Values are ($WorkerAgentInRelativity) and ($MetricManagerAgentInRelativity)"
}

$projectPath = Get-ScriptDirectory 
$projectPath = $projectPath + '\..\Source'

$configOverrideScript = $projectPath + '\CreateOverrideConfigurationFile.ps1'
Invoke-Expression $configOverrideScript

$overrideIntegrationFile = $projectPath + '\RelativityConnection.Override.config'
$overrideIntegrationAppSettingsFile = $projectPath + '\AppSettings.Override.config'

$intXml = [xml](Get-Content $overrideIntegrationFile)

$primaryConnection = Select-Xml -Xml $intXml -Xpath '//add[@name="relativity"]'
$primaryConnection.node.connectionString = "data source="+ $primaryServer +";initial catalog=EDDS;persist security info=False;user id=EDDSdbo;password="+$eddsdboPassword+";packet size=4096"

$secondaryConnectionString = "data source="+ $secondaryServer +";persist security info=False;user id=EDDSdbo;password="+$eddsdboPassword+";packet size=4096"
$secondaryConnection = Select-Xml -Xml $intXml -Xpath '//add[@name="relativityWorkspace"]'
if ($secondaryConnection) {
	$secondaryConnection.node.connectionString = $secondaryConnectionString
}
else {
	$intXml.connectionStrings.AppendChild($intXml.ImportNode(([xml]"<add name='relativityWorkspace' connectionString='$secondaryConnectionString'/>").DocumentElement, $true))	
}


$intXml.Save($overrideIntegrationFile)

$appSettingsXml = [xml](Get-Content $overrideIntegrationAppSettingsFile)
foreach ($setting in $appSettingsXml.appSettings.add) {
    if($setting.key -eq 'Server' -Or $setting.key -eq 'RSAPIServer' -Or $setting.key -eq 'RSAPIDomain')
	{
        $setting.value = $primaryServer
    }
	if($setting.key -eq 'MdfPath')
	{
        $setting.value = "\\\\" + $primaryServer + "\\Database"
    }
	if($setting.key -eq 'LdfPath')
	{
        $setting.value = "\\\\" + $primaryServer + "\\Database"
    }
	if($setting.key -eq 'WorkspaceId'){
        $setting.value = "$WorkspaceId"
    }
	if($setting.key -eq 'WorkerAgentInRelativity'){
		$setting.value = "$WorkerAgentInRelativity"
	}
	if($setting.key -eq 'MetricManagerAgentInRelativity'){
		$setting.value = "$MetricManagerAgentInRelativity"
	}
	if($setting.key -eq 'EddsdboPassword'){
        $setting.value = "$eddsdboPassword"
    }
	if($setting.key -eq 'SAPassword'){
        $setting.value = "$saPassword"
    }
	if($setting.key -eq 'RSAPIUserName'){
        $setting.value = "$RSAPIUserName"
    }
	if($setting.key -eq 'RSAPIPassword'){
        $setting.value = "$RSAPIPassword"
    }
}

$appSettingsXml.Save($overrideIntegrationAppSettingsFile)