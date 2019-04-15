-- Grab last install time
declare @installTime datetime
select @installTime = ISNULL(MAX(InstalledOn),'1800-01-01 00:00:00.000')
from EDDS.eddsdbo.ApplicationInstall (nolock) where OriginSignature = '2B71354C-DB67-4A4D-8985-B3AB4FF3C6C5' -- PDB OriginSignature

-- grab process control info
select * from EDDSPerformance.eddsdbo.ProcessControl

-- grab all logs after install (with some buffer)
select * from EDDSPerformance.eddsdbo.QoS_GlassRunLog (nolock)
where LogTimestampUTC >= DATEADD(hh, -6, @installTime)

-- grab all events after install (with some buffer)
select et.[Name], e.* 
from EDDSPerformance.eddsdbo.[Events] e 
inner join EDDSPerformance.eddsdbo.EventTypes et on e.SourceTypeID = et.Id
where e.[TimeStamp] >= DATEADD(hh, -6, @installTime)

-- Grab QoS_Ratings that exist
select *
from EDDSPerformance.eddsdbo.QoS_Ratings

-- Grab hours that exist
select *
from EDDSPerformance.eddsdbo.[Hours]

-- MetricData
select md.ID as MetricDataID, m.HourID, h.HourTimeStamp, m.ID as MetricID, mt.Name as MetricType, md.Data as MetricData, md.Score as DataScore, md.ServerID
from EDDSPerformance.eddsdbo.[Metrics] m
inner join EDDSPerformance.eddsdbo.[MetricTypes] mt on m.MetricTypeID = mt.ID
inner join EDDSPerformance.eddsdbo.[Hours] h on m.HourID = h.ID
left join EDDSPerformance.eddsdbo.[MetricData] md on m.ID = md.MetricID

--Server
select ServerID, IsQoSDeployed, ArtifactID
from EDDSPerformance.eddsdbo.[Server]
where ServerTypeID = 3 -- Database

-- BatchInformation
select * from EDDSPerformance.eddsdbo.HourSearchAuditBatches
select b.Id as BatchID, b.HourId, b.ServerId, b.WorkspaceId, b.BatchStart, b.BatchSize, 
	r.Id as BatchResultID, r.UserId, r.TotalComplexQueries, r.TotalLongRunningQueries, r.TotalSimpleLongRunningQueries, r.TotalQueries, r.TotalExecutionTime
from EDDSPerformance.eddsdbo.SearchAuditBatch b
left join EDDSPerformance.eddsdbo.SearchAuditBatchResult r on b.Id = r.BatchId