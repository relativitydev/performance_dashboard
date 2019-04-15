-- EDDSPerformance
SELECT
	[Database] as [DatabaseName],
	BackupStartDate as [Start],
	BackupEndDate as [End],
	[BackupType]
FROM eddsdbo.MockBackupSet bs WITH(NOLOCK)
WHERE [Server] = @ServerName
	AND BackupEndDate >= @hourStartDate
	AND BackupEndDate < @hourEndDate
	AND [Database] in @databaseNames