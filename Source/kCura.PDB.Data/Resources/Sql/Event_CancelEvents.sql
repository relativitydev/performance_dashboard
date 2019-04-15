

UPDATE [eddsdbo].[Events]
set [StatusID] = @cancelStatus
where
	[StatusID] in @statusesToCancel
	and [SourceTypeId] not in @typesToExclude