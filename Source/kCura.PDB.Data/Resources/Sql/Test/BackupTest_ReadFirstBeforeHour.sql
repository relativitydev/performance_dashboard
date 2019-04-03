declare @hourEndDate90DayWindow datetime = dateadd(day, -90, @hourEndDate)

;WITH LastDatabaseBackups ([Database], BackupEndDate, BackupStartDate, [BackupType], rowid)	AS
	(
		SELECT [Database], BackupEndDate, BackupStartDate, [BackupType]
			, ROW_NUMBER() OVER(PARTITION BY [Database] ORDER BY BackupEndDate DESC) AS rowid 
		FROM eddsdbo.MockBackupSet bh WITH(NOLOCK)
		WHERE BackupEndDate < @hourEndDate
			and BackupEndDate >= @hourEndDate90DayWindow
			AND [Database] in @databaseNames
			AND [BackupType] in @backupTypes
	)
select
	[Database] as [DatabaseName],
	BackupStartDate as [Start],
	BackupEndDate as [End],
	[BackupType]
from LastDatabaseBackups
where rowid = 1


