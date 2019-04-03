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
declare @osnameFilter varchar(255)
declare @osversionFilter varchar(255)
declare @logicalProcessorsFilter varchar(255)
declare @hypterthreadedFilter varchar(255)

declare @logicalProcessorsOperator int
*/

select * from eddsdbo.[EnvironmentCheckServerDetails]
where
	(@serverNameFilter is null or [ServerName] like '%' + @serverNameFilter + '%') 
	and	(@osnameFilter is null or [OSName] like '%' + @osnameFilter + '%')  
	and	(@osversionFilter is null or [OSVersion] like '%' + @osversionFilter + '%')  
		
	and	(@hypterthreadedFilter is null or [Hyperthreaded] = @hypterthreadedFilter) 
	
	and 
	(@logicalProcessorsFilter is null 
		or (@logicalProcessorsOperator = 0 and [LogicalProcessors] = convert(int, @logicalProcessorsFilter))
		or (@logicalProcessorsOperator = 1 and [LogicalProcessors] < convert(int, @logicalProcessorsFilter))
		or (@logicalProcessorsOperator = 2 and [LogicalProcessors] > convert(int, @logicalProcessorsFilter))
		or (@logicalProcessorsOperator = 3 and [LogicalProcessors] <= convert(int, @logicalProcessorsFilter))
		or (@logicalProcessorsOperator = 4 and [LogicalProcessors] >= convert(int, @logicalProcessorsFilter)))