DECLARE
	@ServerAgent nvarchar(max) = @AgentID,
	@LastChecked datetime = '',
	@persistAgentClaims bit = ISNULL((
		SELECT TOP 1
			CASE WHEN [Value] = 'True' THEN 1
				ELSE 0
			END
		FROM EDDSPerformance.eddsdbo.Configuration
		WHERE Section = 'kCura.PDB'
			AND Name = 'PersistResponsibleAgents'
	), 0);

/*
Determine whether another agent has already claimed this server and when it was last checked.
If another agent has claimed this server, but hasn't unclaimed it in an hour, it's fair game.
The PersistResponsibleAgents setting will prevent claims from changing hands; once the column is set,
it persists indefinitely.
*/
SELECT @ServerAgent = ISNULL(ResponsibleAgent, @AgentID),
	@LastChecked = ISNULL(LastChecked, '')
FROM [EDDSPerformance].[eddsdbo].[Server]
WHERE ServerID = @ServerID
AND (
	@persistAgentClaims = 1 --If the ResponsibleAgent column is permanent, we always want to see the agent currently assigned to a server
	OR LastChecked > DATEADD(hh, -1, getutcdate()) --Otherwise, we only want to see the agent currently assigned if it's been able to process the server
)

IF (@ServerAgent = @AgentID AND @LastChecked < DATEADD(MINUTE, -1, GETUTCDATE()))
BEGIN
	UPDATE [EDDSPerformance].[eddsdbo].[Server]
	SET ResponsibleAgent = @ServerAgent,
		LastChecked = GETUTCDATE()
	WHERE ServerID = @ServerID
END
ELSE
BEGIN
	SET @ServerAgent = NULL
END
	
SELECT @ServerAgent