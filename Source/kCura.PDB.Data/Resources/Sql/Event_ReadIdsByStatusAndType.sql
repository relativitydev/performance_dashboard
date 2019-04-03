
SELECT TOP (@count) [ID]
FROM [eddsdbo].[Events] with(nolock)
WHERE StatusID = @eventStatus
AND SourceTypeID not in @excludeEventTypes