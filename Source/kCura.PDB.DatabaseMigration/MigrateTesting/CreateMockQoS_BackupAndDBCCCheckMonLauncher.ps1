$file = (join-path -Path $PSScriptRoot -ChildPath '..\MigratePerformance\8_StoredProcedures\QoS_BackupAndDBCCCheckMonLauncher.sql')
$outFile = join-path $PSScriptRoot '8_StoredProcedures\Mock_QoS_BackupAndDBCCCheckMonLauncher.sql'

$PSScriptRoot
$file

$str1 = 'DECLARE @now datetime = getdate();'
$str2 = 'DECLARE @nowUtc datetime = getutcdate();'
$str3 = 'FROM eddsdbo.[Server] WITH(NOLOCK)'

$strReplacement1 = 'DECLARE @now datetime = (select top 1 HourTimeStamp from eddsdbo.MockHours order by HourTimeStamp desc);'
$strReplacement2 = 'DECLARE @nowUtc datetime = (select top 1 HourTimeStamp from eddsdbo.MockHours order by HourTimeStamp desc);'
$strReplacement3 = 'FROM eddsdbo.[MockServer] WITH(NOLOCK)'

(Get-Content $file) |
ForEach-Object{ 
	$_.replace($str1, $strReplacement1).replace($str2, $strReplacement2).replace($str3, $strReplacement3)
	} |
Out-File $outFile


'Dont forget to re-generate hash in intgrity assembly.'