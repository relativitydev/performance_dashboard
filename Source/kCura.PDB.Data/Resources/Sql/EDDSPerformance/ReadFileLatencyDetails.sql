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
declare @databaseNameFilter varchar(255)
declare @scoreFilter varchar(255)
declare @dataReadFilter varchar(255)
declare @dataWriteFilter varchar(255)
declare @logReadFilter varchar(255)
declare @logWriteFilter varchar(255)

declare @scoreOperator int
declare @dataReadOperator int
declare @dataWriteOperator int
declare @logReadOperator int
declare @logWriteOperator int
*/

select distinct flld.* 
from eddsdbo.[FileLevelLatencyDetails] as flld
inner join eddsdbo.[Server] as s on s.ServerName = flld.ServerName
where
	(@serverNameFilter is null or (flld.[ServerName] like '%' + @serverNameFilter + '%' or convert(varchar(10), s.ArtifactID) = @serverNameFilter ) )
	and (@databaseNameFilter is null or [DatabaseName] like '%' + @databaseNameFilter + '%') 
	and (@scoreFilter is null 
		or (@scoreOperator = 0 and convert(int, [Score]) = convert(int, @scoreFilter))
		or (@scoreOperator = 1 and [Score] < convert(int, @scoreFilter))
		or (@scoreOperator = 2 and [Score] > convert(int, @scoreFilter))
		or (@scoreOperator = 3 and convert(int, [Score]) <= convert(int, @scoreFilter))
		or (@scoreOperator = 4 and convert(int, [Score]) >= convert(int, @scoreFilter))) 	
	and (@dataReadFilter is null 
		or (@dataReadOperator = 0 and [DataReadLatency] = convert(int, @dataReadFilter))
		or (@dataReadOperator = 1 and [DataReadLatency] < convert(int, @dataReadFilter))
		or (@dataReadOperator = 2 and [DataReadLatency] > convert(int, @dataReadFilter))
		or (@dataReadOperator = 3 and [DataReadLatency] <= convert(int, @dataReadFilter))
		or (@dataReadOperator = 4 and [DataReadLatency] >= convert(int, @dataReadFilter)))
	and (@dataWriteFilter is null 
		or (@dataWriteOperator = 0 and [DataWriteLatency] = convert(int, @dataWriteFilter))
		or (@dataWriteOperator = 1 and [DataWriteLatency] < convert(int, @dataWriteFilter))
		or (@dataWriteOperator = 2 and [DataWriteLatency] > convert(int, @dataWriteFilter))
		or (@dataWriteOperator = 3 and [DataWriteLatency] <= convert(int, @dataWriteFilter))
		or (@dataWriteOperator = 4 and [DataWriteLatency] >= convert(int, @dataWriteFilter)))
	and (@logReadFilter is null 
		or (@logReadOperator = 0 and [LogReadLatency] = convert(int, @logReadFilter))
		or (@logReadOperator = 1 and [LogReadLatency] < convert(int, @logReadFilter))
		or (@logReadOperator = 2 and [LogReadLatency] > convert(int, @logReadFilter))
		or (@logReadOperator = 3 and [LogReadLatency] <= convert(int, @logReadFilter))
		or (@logReadOperator = 4 and [LogReadLatency] >= convert(int, @logReadFilter)))
	and (@logWriteFilter is null 
		or (@logWriteOperator = 0 and [LogWriteLatency] = convert(int, @logWriteFilter))
		or (@logWriteOperator = 1 and [LogWriteLatency] < convert(int, @logWriteFilter))
		or (@logWriteOperator = 2 and [LogWriteLatency] > convert(int, @logWriteFilter))
		or (@logWriteOperator = 3 and [LogWriteLatency] <= convert(int, @logWriteFilter))
		or (@logWriteOperator = 4 and [LogWriteLatency] >= convert(int, @logWriteFilter)))
	
	
