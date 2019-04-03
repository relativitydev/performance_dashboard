SELECT *
FROM [eddsdbo].[Server] with(nolock)
where 
	DeletedOn is null
	and (IgnoreServer is null or IgnoreServer = 0)
  