DELETE rg
FROM eddsdbo.Reports_RecoveryGaps rg
INNER JOIN eddsdbo.[Databases] db on rg.DatabaseId = db.id
WHERE GapResolutionDate is NULL 
AND ActivityType = @ActivityType
AND db.ServerId = @serverId