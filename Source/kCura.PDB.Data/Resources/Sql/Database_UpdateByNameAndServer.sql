UPDATE [eddsdbo].[Databases]
   SET	[DeletedOn] = @DeletedOn
		,[Ignore] = @Ignore
		,[LastDbccDate] = @lastDbccDate
		,[lastBackupLogDate] = @lastBackupLogDate
		,[lastBackupDiffDate] = @lastBackupDiffDate
		,[lastBackupFullDate] = @lastBackupFullDate
		,[LastBackupFullDuration] = @lastBackupFullDuration
		,[LastBackupDiffDuration] = @lastBackupDiffDuration
		,[LogBackupsDuration] = @logBackupsDuration
 WHERE [Name] = @name
	and [ServerId] = @serverId