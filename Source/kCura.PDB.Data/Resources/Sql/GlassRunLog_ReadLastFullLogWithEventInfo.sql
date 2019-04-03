

Select top(@count) 
	l.grlogid
	,l.LogTimestampUTC
	,l.Module
	,l.TaskCompleted
	,l.OtherVars
	,l.NextTask
	,l.AgentID
	,l.LogLevel
	,e.ID [EventId]
	,e.SourceTypeID [EventType]
	,e.SourceID [EventSourceId]
	,e.StatusID [EventStatusId]
	,e.TimeStamp [EventTimeStamp]
	,e.Delay [EventDelay]
	,e.PreviousEventID [PreviousEventID]
	,e.LastUpdated [EventLastUpdated]
	,e.Retries [EventRetries]
	,e.ExecutionTime [EventExecutionTime]
	,e.HourId [EventHourId]
from eddsdbo.QoS_GlassRunLog l with(nolock)
left outer join eddsdbo.EventLogs el  with(nolock) on el.LogId = l.GRLogID
left outer join eddsdbo.Events e with(nolock) on e.ID = el.EventId
order by GRLogId desc