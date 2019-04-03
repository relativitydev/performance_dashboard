

/*
Equals = 0,
LessThan = 1,
GreaterThan = 2,
LessThanEqual = 3,
GreaterThanEqual = 4
*/

/*
declare @startTimeFilter datetime
declare @endTimeFilter datetime
declare @reasonFilter varchar(255)
declare @commentFilter varchar(255)

declare @startTimeOperator datetime
declare @endTimeOperator datetime
declare @reasonOperator int
*/

SELECT [ID]
	  ,[StartTime]
	  ,[EndTime]
	  ,[Reason]
	  ,[Comments]
	  ,[IsDeleted]
FROM [eddsdbo].[MaintenanceSchedules] with(nolock)
WHERE [IsDeleted] = 0
	AND
	(@commentFilter is null or 
		@commentFilter = '' or
		[Comments] like '%' + @commentFilter + '%')
	AND
	(@startTimeFilter is null 
		or @startTimeFilter = ''
		or (@startTimeOperator = 0 and [StartTime] = convert(datetime, @startTimeFilter))
		or (@startTimeOperator = 1 and [StartTime] < convert(datetime, @startTimeFilter))
		or (@startTimeOperator = 2 and [StartTime] > convert(datetime, @startTimeFilter))
		or (@startTimeOperator = 3 and [StartTime] <= convert(datetime, @startTimeFilter))
		or (@startTimeOperator = 4 and [StartTime] >= convert(datetime, @startTimeFilter)))
	AND
	(@endTimeFilter is null 
		or @endTimeFilter = ''
		or (@endTimeOperator = 0 and [EndTime] = convert(datetime, @endTimeFilter))
		or (@endTimeOperator = 1 and [EndTime] < convert(datetime, @endTimeFilter))
		or (@endTimeOperator = 2 and [EndTime] > convert(datetime, @endTimeFilter))
		or (@endTimeOperator = 3 and [EndTime] <= convert(datetime, @endTimeFilter))
		or (@endTimeOperator = 4 and [EndTime] >= convert(datetime, @endTimeFilter)))
	AND
	(@reasonFilter is null 
		or @reasonFilter = ''
		or (@reasonOperator = 0 and [Reason] = convert(int, @reasonFilter))
		or (@reasonOperator = 1 and [Reason] < convert(int, @reasonFilter))
		or (@reasonOperator = 2 and [Reason] > convert(int, @reasonFilter))
		or (@reasonOperator = 3 and [Reason] <= convert(int, @reasonFilter))
		or (@reasonOperator = 4 and [Reason] >= convert(int, @reasonFilter)))