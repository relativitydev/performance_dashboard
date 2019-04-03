
UPDATE [eddsdbo].[Events]
SET   [StatusID] = @updateStatus,
	  [LastUpdated] = getutcdate()
OUTPUT Inserted.*
WHERE ID in @Ids