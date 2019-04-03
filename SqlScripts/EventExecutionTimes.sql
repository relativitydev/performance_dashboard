
-- compare execution times for category scores
select ct.Name, avg(e.ExecutionTime) avg, max(e.ExecutionTime) max
  from eddsdbo.Events e
  inner join eddsdbo.CategoryScores cs on cs.Id = e.SourceId and e.sourcetypeid = 7
  inner join eddsdbo.Categories as c on c.id = cs.CategoryID
  inner join eddsdbo.CategoryTypes as ct on c.CategoryTypeID = ct.ID
  where e.statusid = 3
  group by ct.Name

-- compare execution times for different event types
select name, SourceTypeID, AVG(ExecutionTime), max(ExecutionTime), count(*) 
	from eddsdbo.Events e
	inner join eddsdbo.EventTypes et on et.Id = e.SourceTypeID
	where StatusID in (3)
	group by SourceTypeID, name
