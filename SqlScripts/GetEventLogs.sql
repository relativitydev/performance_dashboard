select top(1000)
et.Name,
e.ID,
e.SourceID,
l.GrLogId,
l.Module,
l.TaskCompleted,
l.OtherVars,
l.[RunTimeUTC],
l.AgentID,
l.LogLevel
  from eddsdbo.Events e
  inner join eddsdbo.EventTypes et on et.Id = e.SourceTypeID
  inner join eddsdbo.EventLogs el on e.ID = el.EventId
  inner join eddsdbo.QoS_GlassRunLog l on l.GRLogID = el.LogId