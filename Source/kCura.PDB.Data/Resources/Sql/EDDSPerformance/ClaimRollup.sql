DECLARE @RollupAgent nvarchar(max)
BEGIN TRY
SET @RollupAgent =
	(SELECT C.[Value]
	FROM [EDDSPerformance].[eddsdbo].[Configuration] C
	JOIN [EDDS].[eddsdbo].[Agent] A WITH(NOLOCK)
	ON C.Value = A.ArtifactID
	WHERE C.Section='kCura.PDB'
	AND C.Name='RollupAgent'
	AND A.MessageTime > DATEADD(hh, -1, getutcdate()))
IF (@RollupAgent is NULL) BEGIN
	SET @RollupAgent = @AgentID
	UPDATE [EDDSPerformance].[eddsdbo].[Configuration]
		SET Value = @RollupAgent
		WHERE Section = 'kCura.PDB' AND Name = 'RollupAgent'
END
END TRY
BEGIN CATCH
	SET @RollupAgent = @AgentID
	UPDATE [EDDSPerformance].[eddsdbo].[Configuration]
		SET Value = @RollupAgent
		WHERE Section = 'kCura.PDB' AND Name = 'RollupAgent'
END CATCH

SELECT @RollupAgent;