
UPDATE [eddsdbo].[Events]
SET   [StatusID] = @updateStatus,
      [Retries] = ISNULL([Retries], 0) + 1,
	  [LastUpdated] = getutcdate()
WHERE ID in @Ids