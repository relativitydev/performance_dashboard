

UPDATE [eddsdbo].[Server]
   SET [DeletedOn] = getutcdate()
WHERE
	ServerID = @serverID
