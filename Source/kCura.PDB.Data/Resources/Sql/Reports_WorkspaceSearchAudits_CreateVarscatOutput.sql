

-- params
--declare @hourId int = (select top 1 h.Id from eddsdbo.[Hours] as h
--	left join eddsdbo.Reports_SearchAudits as sa on sa.HourId = h.id
--	where sa.ID is not null
-- order by HourTimeStamp)
--declare @serverId int = (select top 1 ServerID from eddsdbo.[Server] where DeletedOn is null and (IgnoreServer is null or IgnoreServer = 0))

-- query additional data
declare @summaryDayHour dateTime = (select top 1 HourTimeStamp from eddsdbo.[Hours] with(nolock) where id = @hourId)
declare @serverArtifactID int = (select top 1 ArtifactId from eddsdbo.[Server] with(nolock) where ServerID = @serverId and DeletedOn is null and (IgnoreServer is null or IgnoreServer = 0))
declare @serverName nvarchar(255) = ( select top 1 ServerName from eddsdbo.[Server] with(nolock) where ServerID = @serverId and DeletedOn is null and (IgnoreServer is null or IgnoreServer = 0) order by ServerTypeID)
declare @qosHourId bigint = (select eddsdbo.QoS_GetServerHourID(@ServerArtifactID, @summaryDayHour) as QoSHourID)

INSERT INTO [eddsdbo].[QoS_VarscatOutputCumulative]
           ([ServerName]
           ,[QoSHourID]
           ,[DatabaseName]
           ,[SearchName]
           ,[SearchArtifactID]
           ,[TotalLRQRunTime]
           ,[TotalRuns]
           ,[SummaryDayHour]
		   ,[ServerID])
    (Select
		@serverName as ServerName
		,@QoSHourID as QoSHourID
		,'EDDS' + Convert(varchar(12),wsa.WorkspaceId) as DatabaseName
		,SearchName
		,SearchId as SearchArtifactID
		,TotalExecutionTime as TotalLRQRunTime
		,TotalSearchAudits as TotalRuns
		,@SummaryDayHour
		,@serverId as ServerID
		from eddsdbo.Reports_WorkspaceSearchAudits as wsa with(nolock)
		where wsa.HourId = @hourId and wsa.ServerId = @serverId)



