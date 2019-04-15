

UPDATE [eddsdbo].[MaintenanceSchedules]
	SET [IsDeleted] = 1
	WHERE ID = @id