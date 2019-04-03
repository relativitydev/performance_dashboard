

IF NOT EXISTS(select top(1) id from [eddsdbo].[CategoryScores] (UPDLOCK) where [CategoryID] = @categoryID and ([ServerID] = @serverID OR ([ServerID] is null and @serverID is null)))
BEGIN
	INSERT INTO [eddsdbo].[CategoryScores]
			([CategoryID]
			,[ServerID]
			,[Score])
		VALUES
			(@categoryID
			,@serverID
			,@score)
			
	SELECT * FROM [eddsdbo].[CategoryScores] WHERE ID = @@IDENTITY
END
ELSE
BEGIN
	SELECT * FROM [eddsdbo].[CategoryScores]
	where [CategoryID] = @categoryID and ([ServerID] = @serverID OR ([ServerID] is null and @serverID is null))
END