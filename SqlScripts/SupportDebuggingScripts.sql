-- Grab errored events
select * 
from eddsdbo.Events e with(nolock)
inner join eddsdbo.EventTypes et on e.SourceTypeID = et.Id
where StatusID = 4

-- Grab all events
select * 
from eddsdbo.Events e with(nolock)
inner join eddsdbo.EventTypes et on e.SourceTypeID = et.Id
order by e.ID desc

-- Grab event logs
select *
from eddsdbo.QoS_GlassRunLog with(nolock)
where Module like '%event%'
order by GRLogID desc

-- Grab category scoring logs
select *
from eddsdbo.QoS_GlassRunLog with(nolock)
where Module like '%categor%'
order by GRLogID desc

-- Grab hours
select * from eddsdbo.Hours order by ID desc

-- Grab all logs
select *
from eddsdbo.QoS_GlassRunLog with(nolock)
order by GRLogID desc

-- Grab current time of the server for reference
SELECT GETUTCDATE() as CurrentTime

select COUNT(*)
from EDDS1016785.EDDSDBO.AuditRecord with(nolock)
where [TimeStamp] <= GETUTCDATE() and [TimeStamp] >  '2017-09-20 06:00:00.000'




select m.ID as MetricID
	,h.HourTimeStamp
	,md.Data as Data
	,md.Score
	,aa.*
from eddsdbo.Metrics m with(nolock)
inner join eddsdbo.Hours h on m.HourID = h.ID
inner join eddsdbo.MetricTypes mt on m.MetricTypeID = mt.ID
inner join eddsdbo.MetricData md with(nolock) on m.ID = md.MetricID
left join eddsdbo.MetricData_AuditAnalysis aa with(nolock) on md.ID = aa.MetricDataId
where mt.ID = 12
order by m.ID desc

select * from eddsdbo.HourSearchAuditBatches with(nolock)
order by HourId desc

select * from eddsdbo.QoS_SampleHistoryUX with(nolock)

select * from eddsdbo.MetricData_AuditAnalysis as aa 
--where aa.MetricDataId = @metricDataId