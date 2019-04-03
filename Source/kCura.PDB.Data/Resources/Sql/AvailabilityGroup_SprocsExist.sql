USE [master]

IF EXISTS (SELECT * FROM sys.databases WHERE name = '{0}')
BEGIN
	declare @sql nvarchar(max) = N'
	use [{0}]
	IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N''[eddsdbo].[RemoveDatabaseFromAvailabilityGroup]'') AND type in (N''P'', N''PC''))
		and EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N''[eddsdbo].[ReadAvailabilityGroupName]'') AND type in (N''P'', N''PC''))
		and EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N''[eddsdbo].[DatabaseJoinedToGroup]'') AND type in (N''P'', N''PC''))
	begin
		select 1
	end
	else
	begin
		select 0
	end
	'
	execute (@sql)
END
ELSE
BEGIN
	select 0
END

