SELECT TOP (1) [Major]
      ,[Minor]
      ,[Build]
      ,[Revision]
  FROM [eddsdbo].[EddsPerformanceVersion]
  order by [Major] desc, [Minor] desc, [Build] desc, [Revision] desc