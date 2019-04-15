-- EDDSPerformance

select top 1 1
from eddsdbo.[Events]
where [HourId] = @hourId
and [SourceTypeId] = @sourceTypeId
and [StatusId] = @statusId