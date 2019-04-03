

-- params
--declare @serverId int = (select top 1 ServerID from eddsdbo.[Server] where DeletedOn is null and IgnoreServer is null)
--declare @hourId int = (select top 1 Id from eddsdbo.[Hours] order by HourTimeStamp)
--declare @workspaceId int = (select top 1 ArtifactID from edds.eddsdbo.[Case])
--declare @score decimal(9,0) = 98.7
--declare @totalUsers int = 123
--declare @totalAudits bigint = 987654321000
--declare @totalLongRunning bigint = 1234 -- (select sum(1) from eddsdbo.report)

-- query additional data
declare @summaryDayHour dateTime = (select top 1 HourTimeStamp from eddsdbo.[Hours] with(nolock) where id = @hourId)
declare @serverArtifactID int = (select top 1 ArtifactId from eddsdbo.[Server] with(nolock) where ServerID = @serverId and DeletedOn is null and (IgnoreServer is null or IgnoreServer = 0))
declare @serverName nvarchar(255) = ( select top 1 ServerName from eddsdbo.[Server] with(nolock) where ServerID = @serverId and DeletedOn is null and (IgnoreServer is null or IgnoreServer = 0) order by ServerTypeID)
declare @qosHourId bigint = (select eddsdbo.QoS_GetServerHourID(@ServerArtifactID, @summaryDayHour) as QoSHourID)
declare @workspaceName varchar(50) = (select top 1 [Name] from edds.eddsdbo.[Case] with(nolock) where ArtifactID = @workspaceId)
declare @totalSearchAudits bigint = (select sum(TotalSearchAudits) from eddsdbo.Reports_WorkspaceSearchAudits with(nolock) where HourId = @hourId and WorkSpaceId = @workspaceId)
declare @totalNonSearchAudits bigint = @totalAudits - @totalSearchAudits
declare @totalExecutionTime bigint = (select sum(TotalExecutionTime) from eddsdbo.Reports_WorkspaceSearchAudits with(nolock) where HourId = @hourId and WorkSpaceId = @workspaceId)

-- downcast bigint to int overflow during casts
declare @totalAuditsInt int
BEGIN TRY
    set @totalAuditsInt = (select Cast(@totalAudits as int))
END TRY BEGIN CATCH
	set @totalAuditsInt = -1
END CATCH
declare @totalLongRunningInt int
BEGIN TRY
    set @totalLongRunningInt = (select Cast(@totalLongRunning as int))
END TRY BEGIN CATCH
	set @totalLongRunningInt = -1
END CATCH
declare @totalSearchAuditsInt int
BEGIN TRY
    set @totalSearchAuditsInt = (select Cast(@totalSearchAudits as int))
END TRY BEGIN CATCH
	set @totalSearchAuditsInt = -1
END CATCH
declare @totalNonSearchAuditsInt int
BEGIN TRY
    set @totalNonSearchAuditsInt = (select Cast(@totalNonSearchAudits as int))
END TRY BEGIN CATCH
	set @totalNonSearchAuditsInt = -1
END CATCH
declare @totalExecutionTimeInt int
BEGIN TRY
    set @totalExecutionTimeInt = (select Cast(@totalExecutionTime as int))
END TRY BEGIN CATCH
	set @totalExecutionTimeInt = -1
END CATCH

INSERT INTO [eddsdbo].[QoS_UserExperienceServerSummary]
           ([ServerArtifactID]
           ,[Server]
           ,[CaseArtifactID]
           ,[Workspace]
           ,[Score]
           ,[TotalLongRunning]
           ,[TotalUsers]
           ,[TotalSearchAudits]
           ,[TotalNonSearchAudits]
           ,[TotalAudits]
           ,[TotalExecutionTime]
           ,[SummaryDayHour]
           ,[QoSHourID])
	(select 
			@serverArtifactID as ServerArtifactID,
			@serverName as [server],
			@workspaceId as CaseArtifactID,
			@workspaceName as Workspace,
			@score as Score,
			@totalLongRunning as TotalLongRunning,
			@totalUsers as TotalUsers,
			@totalSearchAuditsInt as TotalSearchAudits,
			@totalNonSearchAuditsInt as TotalNonSearchAudits,
			@totalAuditsInt as TotalAudits,
			@totalExecutionTimeInt as TotalExecutionTime,
			@summaryDayHour as SummaryDayHour,
			@qosHourId as QosHourId)



