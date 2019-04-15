

-- params
--declare @hourId int = (select top 1 h.Id from eddsdbo.[Hours] as h
--	left join eddsdbo.Reports_SearchAudits as sa on sa.HourId = h.id
--	where sa.ID is not null
-- order by HourTimeStamp)
--declare @serverId int = (select top 1 ServerID from eddsdbo.[Server] where DeletedOn is null and (IgnoreServer is null or IgnoreServer = 0))

-- query additional data
declare @summaryDayHour dateTime = (select top 1 HourTimeStamp from eddsdbo.[Hours] with(nolock) where id = @hourId)
declare @serverArtifactID int = (select top 1 ArtifactId from eddsdbo.[Server] with(nolock) where ServerID = @serverId and DeletedOn is null and (IgnoreServer is null or IgnoreServer = 0))
declare @qosHourId bigint = (select eddsdbo.QoS_GetServerHourID(@ServerArtifactID, @summaryDayHour) as QoSHourID)

INSERT INTO [eddsdbo].[QoS_VarscatOutputDetailCumulative]
           ([QoSHourID]
           ,[SummaryDayHour]
           ,[CaseArtifactID]
           ,[SearchArtifactID]
           ,[QoSAction]
           ,[IsComplex]
		   ,[ServerID])
	(select
			@QoSHourID as QoSHourID
			,@SummaryDayHour as SummaryDayHour
			,wsa.WorkspaceId as CaseArtifactID
			,wsa.SearchId as SearchArtifactID
			,281 as QoSAction
			,wsa.IsComplex as IsComplex
			,@serverId as ServerID
			from eddsdbo.Reports_WorkspaceSearchAudits as wsa with(nolock)
			where wsa.HourId = @hourId and wsa.ServerId = @serverId)



