/*
If PersistResponsibleAgents is True, this will just update LastChecked.
Otherwise, the ResponsibleAgent column is also cleared.
*/
DECLARE @persistAgentClaims bit = ISNULL((
		SELECT TOP 1
			CASE WHEN [Value] = 'True' THEN 1
				ELSE 0
			END
		FROM EDDSPerformance.eddsdbo.Configuration
		WHERE Section = 'kCura.PDB'
			AND Name = 'PersistResponsibleAgents'
	), 0);

UPDATE [EDDSPerformance].[eddsdbo].[Server]
SET [ResponsibleAgent] =
		CASE
			WHEN @persistAgentClaims = 0 THEN NULL --If the ResponsibleAgent column isn't permanent, release the claim
			ELSE @AgentID --Otherwise, keep it forever
		END,
	LastChecked = CASE WHEN @Delay = 0 THEN GETUTCDATE()
		ELSE DATEADD(HH, 1, GETUTCDATE())
	END
WHERE ServerID = @ServerID AND [ResponsibleAgent] = @AgentID