select e.*, cs.*, c.*, md.*, m.*, h2.*
from eddsdbo.[Events] as e with (nolock)
left outer join eddsdbo.CategoryScores as cs with (nolock) on cs.ID = e.SourceID and (e.SourceTypeID = 6 or e.SourceTypeID = 7)
left outer join eddsdbo.Categories as c with (nolock) on c.ID = cs.CategoryID or (e.SourceID = c.ID and e.SourceTypeID = 5)
left outer join eddsdbo.CategoryTypes as ct with (nolock) on ct.ID = c.CategoryTypeID
left outer join eddsdbo.MetricData as md with (nolock) on md.ID = e.SourceID and (e.SourceTypeID = 2 or e.SourceTypeID = 3)
left outer join eddsdbo.Metrics as m with (nolock) on m.ID = md.MetricID or (e.SourceID = m.ID and e.SourceTypeID = 1)
left outer join eddsdbo.MetricTypes as mt with (nolock) on mt.ID = m.MetricTypeID
inner join eddsdbo.[Hours] as h1 with (nolock) on h1.ID = c.HourID or h1.ID = m.HourID
left outer join eddsdbo.[Hours] as h2 with (nolock) on (e.SourceID = h2.ID and (e.SourceTypeID = 8 or e.SourceTypeID = 9))
where h1.HourTimeStamp = '2017-02-27 22:00:00.000' and e.TimeStamp > '2017-02-27 22:00:00.000' and e.TimeStamp < '2017-02-27 23:00:00.000'
and (e.SourceTypeID = 2)

select * 
from eddsdbo.Categories as c 
left outer join eddsdbo.CategoryScores as cs on c.ID = cs.CategoryID
inner join eddsdbo.[Hours] as h on h.ID = c.HourID
right outer join eddsdbo.CategoryTypes as ct on ct.ID = c.CategoryTypeID
where h.HourTimeStamp = '2017-02-27 22:00:00.000'

select * 
from eddsdbo.Metrics as m 
left outer join eddsdbo.MetricData as md on m.ID = md.MetricID
inner join eddsdbo.[Hours] as h on h.ID = m.HourID
right outer join eddsdbo.MetricTypes as mt on mt.ID = m.MetricTypeID
where h.HourTimeStamp = '2017-02-27 22:00:00.000'

select top(336) * from eddsdbo.[Hours] as h
left outer join eddsdbo.[Events] as e with (nolock) on (e.SourceID = h.ID and (e.SourceTypeID = 8 or e.SourceTypeID = 9))
order by h.HourTimeStamp desc
