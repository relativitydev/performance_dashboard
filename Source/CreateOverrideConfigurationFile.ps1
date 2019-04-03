function Get-ScriptDirectory {
    Split-Path -parent $PSCommandPath
}

$projectPath = Get-ScriptDirectory
$baseIntegrationFile = $projectPath + '\RelativityConnection.config'
$overrideIntegrationFile = $projectPath + '\RelativityConnection.Override.config'

$baseIntegrationAppSettingsFile = $projectPath + '\AppSettings.config'
$overrideIntegrationAppSettingsFile = $projectPath + '\AppSettings.Override.config'

if(-Not (Test-Path $overrideIntegrationFile))
{
    Copy-Item $baseIntegrationFile $overrideIntegrationFile
}

if(-Not (Test-Path $overrideIntegrationAppSettingsFile))
{
    Copy-Item $baseIntegrationAppSettingsFile $overrideIntegrationAppSettingsFile
}