

select md.* from eddsdbo.MetricData as md with(nolock)
inner join eddsdbo.Metrics as m with(nolock) on md.MetricID = m.ID
inner join eddsdbo.MetricTypesToCategoryTypes as mt2ct with(nolock) on mt2ct.MetricTypeID = m.MetricTypeID
inner join eddsdbo.Categories as c with(nolock) on c.CategoryTypeID = mt2ct.CategoryTypeID and c.HourID = m.HourID
inner join eddsdbo.CategoryScores as cs with(nolock) on cs.CategoryID = c.ID
where 
	(cs.ServerID = @ServerId or (cs.ServerID is null and @ServerId is null))
	and (md.ServerID = @ServerId or (md.ServerID is null and @ServerId is null))
	and c.ID = @CategoryId