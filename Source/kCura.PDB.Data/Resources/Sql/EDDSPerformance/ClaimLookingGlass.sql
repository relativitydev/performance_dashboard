DECLARE @GlassAgent nvarchar(max)
BEGIN TRY
SET @GlassAgent =
	(SELECT C.[Value]
	FROM [EDDSPerformance].[eddsdbo].[Configuration] C
	JOIN [EDDS].[eddsdbo].[Agent] A WITH(NOLOCK)
	ON C.Value = A.ArtifactID
	WHERE C.Section='kCura.PDB'
	AND C.Name='LookingGlassAgent'
	AND A.MessageTime > DATEADD(hh, -1, getutcdate()))
IF (@GlassAgent is NULL) BEGIN
	SET @GlassAgent = @AgentID
	UPDATE [EDDSPerformance].[eddsdbo].[Configuration]
		SET Value = @GlassAgent
		WHERE Section = 'kCura.PDB' AND Name = 'LookingGlassAgent'
END
END TRY
BEGIN CATCH
	SET @GlassAgent = @AgentID
	UPDATE [EDDSPerformance].[eddsdbo].[Configuration]
		SET Value = @GlassAgent
		WHERE Section = 'kCura.PDB' AND Name = 'LookingGlassAgent'
END CATCH

SELECT @GlassAgent;