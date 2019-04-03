USE [EDDSPerformance]
GO

IF EXISTS ( SELECT  1 FROM    sysobjects WHERE   [name] = 'QoS_HasSystemVersionChanged' AND type = 'P' ) 
BEGIN
    DROP PROCEDURE eddsdbo.QoS_HasSystemVersionChanged
END
GO

CREATE PROCEDURE [eddsdbo].[QoS_HasSystemVersionChanged]
(
	@summaryDayHour datetime,
	@upgradeWeeklyGracePeriod int output
)
AS
BEGIN

	DECLARE
			--@summaryDayHour datetime = getutcdate(),
			@relativityVersion varchar(200) = (select [Value] FROM [EDDS].[eddsdbo].[Relativity] WITH(NOLOCK) WHERE [Key] = 'Version'),
			@osVersion nvarchar(256) = (select windows_release from sys.dm_os_windows_info),
			@osServicePack nvarchar(256) = (select windows_service_pack_level from sys.dm_os_windows_info),
			@sqlServerVersion nvarchar(128) = (SELECT convert(varchar(128), SERVERPROPERTY('productversion'))),
			@sqlServerLevel nvarchar(128) = (SELECT convert(varchar(128), SERVERPROPERTY('productlevel'))),
			@rowHash binary(20),
			@firstSummaryDayHour int = (select top 1 [ID] from [eddsdbo].[SystemVersionHistory] order by [SummaryDayHour] ),
			@lastSummaryDayHour datetime = (select top 1 [SummaryDayHour] from [eddsdbo].[SystemVersionHistory] order by [SummaryDayHour] desc ),
			@summaryDayHourSevenDaysAgo datetime = DATEADD(dd, -7, @summaryDayHour),
			@relUpgradeWeeklyGracePeriod int = 0,
			@sysUpgradeWeeklyGracePeriod int = 0,
			@firstRelVer varchar(200) = (select top 1 RelativityVersion from [eddsdbo].[SystemVersionHistory] order by [SummaryDayHour] ),
			@firstOSVersion varchar(200) = (select top 1 [OSVersion] from [eddsdbo].[SystemVersionHistory] order by [SummaryDayHour] ),
			@firstOSServicePack varchar(200) = (select top 1 [OSServicePack] from [eddsdbo].[SystemVersionHistory] order by [SummaryDayHour] ),
			@firstSqlServerVersion varchar(200) = (select top 1 [SqlServerVersion] from [eddsdbo].[SystemVersionHistory] order by [SummaryDayHour] ),
			@firstSqlServerLevel varchar(200) = (select top 1 [SqlServerLevel] from [eddsdbo].[SystemVersionHistory] order by [SummaryDayHour] );

	set @upgradeWeeklyGracePeriod = 0

	--if any of the current versions are different from the last history record and the summaryDayHour is greater than the last summaryDayHour
	--then insert a new version history change
	if ((@lastSummaryDayHour is null or @lastSummaryDayHour < @summaryDayHour) and not exists (select * from [eddsdbo].[SystemVersionHistory] where
			   [RelativityVersion] = @relativityVersion
			   and [OSVersion] = @osVersion
			   and [OSServicePack] = @osServicePack
			   and [SqlServerVersion] = @sqlServerVersion
			   and [SqlServerLevel] = @sqlServerLevel
			   and [SummaryDayHour] = @lastSummaryDayHour))
	begin
		--calc hash
		set @rowHash = HASHBYTES('SHA1', @relativityVersion + @osVersion + @osServicePack + @sqlServerVersion + @sqlServerLevel + convert(varchar(50),@summaryDayHour));

		--insert new versions
		INSERT INTO [eddsdbo].[SystemVersionHistory]
			   ([SummaryDayHour] 
			   ,[RowHash]
			   ,[RelativityVersion]
			   ,[OSVersion]
			   ,[OSServicePack]
			   ,[SqlServerVersion]
			   ,[SqlServerLevel])
		 VALUES
			   (@summaryDayHour
			   ,@rowHash
			   ,@relativityVersion
			   ,@osVersion
			   ,@osServicePack
			   ,@sqlServerVersion
			   ,@sqlServerLevel)

	end
	
	--Allow a grace period for uptime of 10 hours/upgrade over the last 7 days for Relativity upgrades
	--Allow a grace period for uptime of 2 hours/upgrade over the last 7 days for OS or SQL Server upgrades

	--calculate relativity upgrade grace period
	select  @relUpgradeWeeklyGracePeriod = (count(distinct RelativityVersion) * 10)
	from [eddsdbo].[SystemVersionHistory] as currentSvh  
	where 
	currentSvh.SummaryDayHour <= @summaryDayHour
	and currentSvh.SummaryDayHour >  DATEADD(dd, -7, @summaryDayHour)
	and @firstRelVer <> currentSvh.RelativityVersion and @firstSummaryDayHour <> currentSvh.ID
	and currentSvh.RelativityVersion <> (select top 1 svh2.RelativityVersion from [eddsdbo].[SystemVersionHistory] as svh2 where svh2.ID < currentSvh.ID order by svh2.ID desc)

	--calculate os and sql upgrade grace period
	select @sysUpgradeWeeklyGracePeriod = count(*) * 2 from (
		select distinct [OSVersion], [OSServicePack], [SqlServerVersion], [SqlServerLevel]
		from [eddsdbo].[SystemVersionHistory] as currentSvh  
		where 
		currentSvh.SummaryDayHour <= @summaryDayHour
		and currentSvh.SummaryDayHour >  DATEADD(dd, -7, @summaryDayHour)
		and @firstSummaryDayHour <> currentSvh.ID
		and (@firstOSVersion <> currentSvh.[OSVersion] or @firstOSServicePack <> currentSvh.[OSServicePack] or @firstSqlServerVersion <> currentSvh.[SqlServerVersion] or @firstSqlServerLevel <> currentSvh.[SqlServerLevel] )
		and (currentSvh.[OSVersion] <> (select top 1 svh2.[OSVersion] from [eddsdbo].[SystemVersionHistory] as svh2 where svh2.ID < currentSvh.ID order by svh2.ID desc)
			or currentSvh.[OSServicePack] <> (select top 1 svh2.[OSServicePack] from [eddsdbo].[SystemVersionHistory] as svh2 where svh2.ID < currentSvh.ID order by svh2.ID desc)
			or currentSvh.[SqlServerVersion] <> (select top 1 svh2.[SqlServerVersion] from [eddsdbo].[SystemVersionHistory] as svh2 where svh2.ID < currentSvh.ID order by svh2.ID desc)
			or currentSvh.[SqlServerLevel] <> (select top 1 svh2.[SqlServerLevel] from [eddsdbo].[SystemVersionHistory] as svh2 where svh2.ID < currentSvh.ID order by svh2.ID desc))
	) as a

	
	set @upgradeWeeklyGracePeriod = @relUpgradeWeeklyGracePeriod + @sysUpgradeWeeklyGracePeriod
			
			
END
GO