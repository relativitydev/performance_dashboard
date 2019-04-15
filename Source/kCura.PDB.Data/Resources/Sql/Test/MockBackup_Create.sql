-- EDDSPerformance

INSERT INTO eddsdbo.[MockBackupSet] (
	[Server],
	[Database],
	[BackupStartDate],
	[BackupEndDate],
	[BackupType]
) VALUES (
	@server,
	@database,
	@backupStartDate,
	@backupEndDate,
	@backupType)