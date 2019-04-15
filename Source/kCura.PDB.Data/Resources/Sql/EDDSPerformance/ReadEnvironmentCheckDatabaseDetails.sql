-- EDDSPerformance


/*
Equals = 0,
LessThan = 1,
GreaterThan = 2,
LessThanEqual = 3,
GreaterThanEqual = 4
*/

/*
declare @serverNameFilter varchar(255)
declare @sqlVersionFilter varchar(255)
declare @adhocWorkloadFilter varchar(255)
declare @maxServerMemoryFilter varchar(255)
declare @maxdegreeOfParallelismFilter varchar(255)
declare @tempDBDataFilesFilter varchar(255)
declare @lastSQLRestartFilter datetime

declare @adhocWorkloadOperator int
declare @maxServerMemoryOperator int
declare @maxDegreeOfParallelismOperator int
declare @tempDBDataFilesOperator int
declare @lastSQLRestartOperator datetime
*/

SELECT [ID]
      ,[ServerName]
      ,[SQLVersion]
      ,[AdHocWorkLoad]
      ,convert(float, [MaxServerMemory] / 1024.0) as [MaxServerMemory]
      ,[MaxDegreeOfParallelism]
      ,[TempDBDataFiles]
      ,[LastSQLRestart]
  FROM [eddsdbo].[EnvironmentCheckDatabaseDetails]
where
	(@serverNameFilter is null or [ServerName] like '%' + @serverNameFilter + '%') 
	
	and 
		(@adhocWorkloadFilter is null 
		or (@adhocWorkloadOperator = 0 and [AdHocWorkLoad] = convert(int, @adhocWorkloadFilter))
		or (@adhocWorkloadOperator = 1 and [AdHocWorkLoad] < convert(int, @adhocWorkloadFilter))
		or (@adhocWorkloadOperator = 2 and [AdHocWorkLoad] > convert(int, @adhocWorkloadFilter))
		or (@adhocWorkloadOperator = 3 and [AdHocWorkLoad] <= convert(int, @adhocWorkloadFilter))
		or (@adhocWorkloadOperator = 4 and [AdHocWorkLoad] >= convert(int, @adhocWorkloadFilter)))
	and 
		(@maxServerMemoryFilter is null 
		or (@maxServerMemoryOperator = 0 and ([MaxServerMemory] / 1024.0) = convert(float, @maxServerMemoryFilter))
		or (@maxServerMemoryOperator = 1 and ([MaxServerMemory] / 1024.0) < convert(float, @maxServerMemoryFilter))
		or (@maxServerMemoryOperator = 2 and ([MaxServerMemory] / 1024.0) > convert(float, @maxServerMemoryFilter))
		or (@maxServerMemoryOperator = 3 and ([MaxServerMemory] / 1024.0) <= convert(float, @maxServerMemoryFilter))
		or (@maxServerMemoryOperator = 4 and ([MaxServerMemory] / 1024.0) >= convert(float, @maxServerMemoryFilter)))
	and 
		(@maxdegreeOfParallelismFilter is null 
		or (@maxDegreeOfParallelismOperator = 0 and [MaxDegreeOfParallelism] = convert(int, @maxdegreeOfParallelismFilter))
		or (@maxDegreeOfParallelismOperator = 1 and [MaxDegreeOfParallelism] < convert(int, @maxdegreeOfParallelismFilter))
		or (@maxDegreeOfParallelismOperator = 2 and [MaxDegreeOfParallelism] > convert(int, @maxdegreeOfParallelismFilter))
		or (@maxDegreeOfParallelismOperator = 3 and [MaxDegreeOfParallelism] <= convert(int, @maxdegreeOfParallelismFilter))
		or (@maxDegreeOfParallelismOperator = 4 and [MaxDegreeOfParallelism] >= convert(int, @maxdegreeOfParallelismFilter)))
	and 
		(@tempDBDataFilesFilter is null 
		or (@tempDBDataFilesOperator = 0 and [TempDBDataFiles] = convert(int, @tempDBDataFilesFilter))
		or (@tempDBDataFilesOperator = 1 and [TempDBDataFiles] < convert(int, @tempDBDataFilesFilter))
		or (@tempDBDataFilesOperator = 2 and [TempDBDataFiles] > convert(int, @tempDBDataFilesFilter))
		or (@tempDBDataFilesOperator = 3 and [TempDBDataFiles] <= convert(int, @tempDBDataFilesFilter))
		or (@tempDBDataFilesOperator = 4 and [TempDBDataFiles] >= convert(int, @tempDBDataFilesFilter)))
	and 
		(@lastSQLRestartFilter is null 
		or (@lastSQLRestartOperator = 0 and [LastSQLRestart] = convert(datetime, @lastSQLRestartFilter))
		or (@lastSQLRestartOperator = 1 and [LastSQLRestart] < convert(datetime, @lastSQLRestartFilter))
		or (@lastSQLRestartOperator = 2 and [LastSQLRestart] > convert(datetime, @lastSQLRestartFilter))
		or (@lastSQLRestartOperator = 3 and [LastSQLRestart] <= convert(datetime, @lastSQLRestartFilter))
		or (@lastSQLRestartOperator = 4 and [LastSQLRestart] >= convert(datetime, @lastSQLRestartFilter)))
