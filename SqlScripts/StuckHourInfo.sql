declare @nextHourToScore datetime
  
-- get the next hour to be scored
select top(1) @nextHourToScore = id from eddsdbo.hours with(nolock)
  where CompletedOn is null
  order by HourTimeStamp desc
  
-- Query all the events for the next hour to score
select et.Name, e.*
  from [EDDSPerformance].[eddsdbo].[events] e with(nolock)
  inner join eddsdbo.EventTypes et with(nolock) on et.Id = e.SourceTypeID
  where HourId = @nextHourToScore
  order by e.ID desc

-- Query the metrics for the next hour to score to see if they've all been collected
select mt.Name, md.* from eddsdbo.MetricData md with(nolock)
  inner join eddsdbo.Metrics m with(nolock) on m.id = md.MetricID
  inner join eddsdbo.MetricTypes mt with(nolock) on mt.ID = m.MetricTypeID
  where m.HourID = @nextHourToScore
  
-- Query the hour batches for the enxt hour to score to see if thye're all completed
SELECT TOP (1000) hab.*, s.ServerName
	from eddsdbo.[Server] s with(nolock)
	left outer join [EDDSPerformance].[eddsdbo].[HourSearchAuditBatches] hab with(nolock) on s.ServerID = hab.ServerId and hab.HourId = @nextHourToScore
	where s.ServerTypeID = 3