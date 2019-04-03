

declare @minData datetime = '1/1/1753 12:00:00 AM';

with hourTimeStamps (HourTimeStamp)
as
(
select HourTimeStamp
from eddsdbo.hours as h
where HourTimeStamp = @minData and Status != 4

union all

select h.HourTimeStamp
from eddsdbo.hours as h
inner join hourTimeStamps hcte on hcte.HourTimeStamp = DateAdd(hour, -1, h.HourTimeStamp)
where h.HourTimeStamp > @minData and Status != 4
)
select top (1) * 
from eddsdbo.Hours as h
where h.HourTimeStamp in (select HourTimeStamp from hourTimeStamps)
order by h.HourTimeStamp desc
option(MAXRECURSION 1000)