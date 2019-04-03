


-- params
--declare @hourId int = (select top 1 h.Id from eddsdbo.[Hours] as h
--	left join eddsdbo.Reports_SearchAudits as sa on sa.HourId = h.id
--	where sa.ID is not null
-- order by HourTimeStamp)
--declare @serverId int = (select top 1 ServerID from eddsdbo.[Server] where DeletedOn is null and (IgnoreServer is null or IgnoreServer = 0))

-- query additional data
declare @summaryDayHour dateTime = (select top 1 HourTimeStamp from eddsdbo.[Hours] where id = @hourId)
declare @serverArtifactID int = (select top 1 ArtifactId from eddsdbo.[Server] where ServerID = @serverId and DeletedOn is null and (IgnoreServer is null or IgnoreServer = 0))
declare @qosHourId bigint = (select eddsdbo.QoS_GetServerHourID(@ServerArtifactID, @summaryDayHour) as QoSHourID)

--select
--	 sa.WorkspaceId as CaseArtifactID
--	,sa.SearchId as SearchArtifactID
--	,wsa.SearchName as Search
--	,sa.MinAuditId as LastAuditID
--	,sa.UserId as UserArtifactID
--	,ISNULL(u.FullName, 'Deleted User') as FullName
--	,sa.TotalExecutionTime as TotalRunTime
--	,sa.TotalExecutionTime / sa.TotalSearchAudits as AverageRunTime
--	,sa.TotalSearchAudits as TotalRuns
--	,sa.PercentLongRunning as PercentLongRunning
--	,sa.IsComplex as IsComplex
--	,@summaryDayHour as SummaryDayHour
--	,@qoSHourID as QoSHourID
--	from eddsdbo.Reports_SearchAudits as sa
--	inner join EDDS.eddsdbo.[AuditUser] as u on u.UserID = sa.UserId
--	inner join eddsdbo.Reports_WorkspaceSearchAudits as wsa on sa.HourId = wsa.HourId and sa.WorkspaceId = wsa.WorkspaceId and sa.SearchId = wsa.SearchId
--	where sa.HourId = @hourId


INSERT INTO [eddsdbo].[QoS_UserExperienceSearchSummary]
           ([CaseArtifactID]
           ,[SearchArtifactID]
           ,[Search]
           ,[LastAuditID]
           ,[UserArtifactID]
           ,[User]
           ,[TotalRunTime]
           ,[AverageRunTime]
           ,[TotalRuns]
           ,[PercentLongRunning]
           ,[IsComplex]
           ,[SummaryDayHour]
           ,[QoSHourID]
		   ,[ServerID])
     (select
			 sa.WorkspaceId as CaseArtifactID
			,sa.SearchId as SearchArtifactID
			,wsa.SearchName as Search
			,sa.MinAuditId as LastAuditID
			,sa.UserId as UserArtifactID
			,ISNULL(u.FullName, 'Deleted User') as FullName
			,sa.TotalExecutionTime as TotalRunTime
			,sa.TotalExecutionTime / sa.TotalSearchAudits as AverageRunTime
			,sa.TotalSearchAudits as TotalRuns
			,sa.PercentLongRunning as PercentLongRunning
			,sa.IsComplex as IsComplex
			,@summaryDayHour as SummaryDayHour
			,@qoSHourID as QoSHourID
			,@serverId as ServerID
		from eddsdbo.Reports_SearchAudits as sa with(nolock)
		inner join EDDS.eddsdbo.[AuditUser] as u with(nolock) on u.UserID = sa.UserId
		inner join eddsdbo.Reports_WorkspaceSearchAudits as wsa with(nolock) on sa.HourId = wsa.HourId and sa.WorkspaceId = wsa.WorkspaceId and sa.SearchId = wsa.SearchId
		where sa.HourId = @hourId and wsa.ServerId = @serverId)


