SELECT [Section], [Name], [Value], [MachineName]
FROM [eddsdbo].[Configuration] with(nolock)
WHERE [Section] = @section AND [Name] = @name