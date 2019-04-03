---Delete an hour
declare @hourTimeStamp datetime = '2017-02-28 17:00:00.000'

Delete from eddsdbo.MetricData
where ID in
(select md.ID
from eddsdbo.Metrics as m 
left outer join eddsdbo.MetricData as md on m.ID = md.MetricID
inner join eddsdbo.[Hours] as h on h.ID = m.HourID
where h.HourTimeStamp = @hourTimeStamp)

Delete from eddsdbo.Metrics
where ID in
(select m.ID
from eddsdbo.Metrics as m 
inner join eddsdbo.[Hours] as h on h.ID = m.HourID
where h.HourTimeStamp = @hourTimeStamp)

Delete from eddsdbo.CategoryScores
where ID in
(select cs.ID 
from eddsdbo.Categories as c 
left outer join eddsdbo.CategoryScores as cs on c.ID = cs.CategoryID
inner join eddsdbo.[Hours] as h on h.ID = c.HourID
where h.HourTimeStamp = @hourTimeStamp)

Delete from eddsdbo.Categories
where ID in
(select c.ID 
from eddsdbo.Categories as c
inner join eddsdbo.[Hours] as h on h.ID = c.HourID
where h.HourTimeStamp = @hourTimeStamp)

Delete from eddsdbo.[Hours]
where HourTimeStamp = @hourTimeStamp