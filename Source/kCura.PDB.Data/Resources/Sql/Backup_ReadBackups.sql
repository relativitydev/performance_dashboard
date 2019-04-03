SELECT
	database_name as [DatabaseName],
	backup_start_date as [Start],
	backup_finish_date as [End],
	[type] as [BackupType]
FROM msdb.dbo.backupset bs WITH(NOLOCK)
WHERE backup_finish_date >= @hourStartDate
	AND backup_finish_date < @hourEndDate
	AND database_name in @databaseNames