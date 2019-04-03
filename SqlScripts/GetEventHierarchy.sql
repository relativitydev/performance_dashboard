/*
	Searches for the last hour where there was an hour created
*/

declare @sourceTypeId int = 100;
declare @secondSourceTypeId int = 300;

with EventHierarchy (PreviousEventId, EventId)
as
(
	-- select the first event
	select top(1) convert(bigint, null) as PreviousEventId, convert(bigint, e1.Id) as EventId
	from eddsdbo.[Events] as e1
	inner join eddsdbo.[Events] as e2 on e1.ID = e2.PreviousEventID
	where e1.sourceTypeId = @sourceTypeId and e2.SourceTypeID = @secondSourceTypeId
	order by e1.id desc

	union all

	select ec.PreviousEventId as PreviousEventId, convert(bigint, ec.id) as EventId
	from eddsdbo.[Events] as ec
	inner join EventHierarchy as ep on ec.PreviousEventID = ep.EventId-- or ec.Id = ep.PreviousEventID
)
select distinct et.Name, e.*
from eddsdbo.[Events] as e
inner join EventHierarchy as eh on e.Id = eh.EventId
inner join eddsdbo.EventTypes et on et.Id = e.SourceTypeID
option (maxrecursion 9000)


/*
	Searches for the last hour where there was an hour created
*/

declare @sourceTypeId int = 100;
declare @hourId int = 1234;

with EventHierarchy (PreviousEventId, EventId)
as
(
	-- select the first event
	select convert(bigint, null) as PreviousEventId, convert(bigint, e1.Id) as EventId
	from eddsdbo.[Events] as e1
	inner join eddsdbo.[Events] as e2 on e1.ID = e2.PreviousEventID
	where e1.sourceTypeId = @sourceTypeId and e2.SourceId = @hourId

	union all

	select ec.PreviousEventId as PreviousEventId, convert(bigint, ec.id) as EventId
	from eddsdbo.[Events] as ec
	inner join EventHierarchy as ep on ec.PreviousEventID = ep.EventId-- or ec.Id = ep.PreviousEventID
)
select distinct et.Name, e.*
from eddsdbo.[Events] as e
inner join EventHierarchy as eh on e.Id = eh.EventId
inner join eddsdbo.EventTypes et on et.Id = e.SourceTypeID
option (maxrecursion 9000)