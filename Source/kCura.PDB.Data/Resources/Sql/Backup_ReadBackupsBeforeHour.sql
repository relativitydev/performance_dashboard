declare @hourEndDate90DayWindow datetime = dateadd(day, -90, @hourEndDate)

;WITH LastDatabaseBackups (database_name, backup_finish_date, backup_start_date, [type], rowid)	AS
	(
		SELECT database_name, backup_finish_date, backup_start_date, [type]
			, ROW_NUMBER() OVER(PARTITION BY database_name ORDER BY backup_finish_date DESC) AS rowid 
		FROM msdb.dbo.backupset bh WITH(NOLOCK)
		WHERE backup_finish_date < @hourEndDate
			and backup_finish_date >= @hourEndDate90DayWindow
			AND database_name in @databaseNames
			AND [type] in @backupTypes
	)
select
	database_name as [DatabaseName],
	backup_start_date as [Start],
	backup_finish_date as [End],
	[type] as [BackupType]
from LastDatabaseBackups
where rowid = 1