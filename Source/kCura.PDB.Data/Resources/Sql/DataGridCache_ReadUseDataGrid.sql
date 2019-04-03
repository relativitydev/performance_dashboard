

DECLARE @dataGridHourTimeStamp DATETIME;
SELECT TOP 1 @dataGridHourTimeStamp = [HourTimeStamp]
		FROM [eddsdbo].[DataGridCache] dgc
		INNER JOIN [eddsdbo].[Hours] h on h.ID = dgc.StartHourToReadAuditsFromDataGrid
		WHERE WorkspaceId = @workspaceId

SELECT CASE 
	WHEN (SELECT TOP 1 1
		FROM [eddsdbo].[Hours]
		WHERE [ID] = @hourId AND [HourTimeStamp] >= @dataGridHourTimeStamp) = 1 
		THEN 1 
	ELSE 0 
END