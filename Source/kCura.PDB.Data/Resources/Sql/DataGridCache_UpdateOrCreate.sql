

UPDATE [eddsdbo].[DataGridCache]
SET [StartHourToReadAuditsFromDataGrid] = @hourId
WHERE WorkspaceId = @workspaceId
IF @@ROWCOUNT=0
  INSERT INTO [eddsdbo].[DataGridCache] VALUES (@workspaceId, @hourId)