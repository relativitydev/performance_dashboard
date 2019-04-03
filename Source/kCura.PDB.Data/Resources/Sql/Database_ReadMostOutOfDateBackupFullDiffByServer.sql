
SELECT
	Case when Min(LastBackupFullDate) < Min(LastBackupDiffDate)
		then Min(LastBackupFullDate)
		else Min(LastBackupDiffDate)
	end
	FROM [eddsdbo].[Databases] with(nolock)
	where serverId = @serverId and (LastBackupFullDate is not null OR LastBackupDiffDate is not null)
	and [DeletedOn] is null and [Ignore] = 0