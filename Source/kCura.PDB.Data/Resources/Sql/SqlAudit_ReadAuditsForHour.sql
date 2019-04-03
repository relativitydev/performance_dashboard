-- Run against workspace database
-- Arguments required (@beginDate, @endDate, @action, @pageStart (default to -1))
SELECT * FROM (
SELECT
	a.ID as AuditID,
    a.[ArtifactID],
    a.[Details],
    UserID,
    [TimeStamp],
    [Action],
    ExecutionTime,
    RequestOrigination,
	@workspaceId as WorkspaceID
	,ROW_NUMBER() OVER(ORDER BY (a.ID)) as RowNum
FROM [eddsdbo].[AuditRecord] a with(nolock)
WHERE a.[Action] in @action
	AND [TimeStamp] >= @beginDate
	AND [Timestamp] < @endDate
) AS RowConstrainedResult 
WHERE RowNum >= @pageStart+1
	AND RowNum < @pageStart+1+@batchSize