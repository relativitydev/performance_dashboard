USE [EDDSPerformance]
GO

IF EXISTS (select 1 from sysobjects where [name] = 'QoS_CollectAgentUptimeData' and type = 'P')  
BEGIN
	DROP PROCEDURE EDDSDBO.QoS_CollectAgentUptimeData
END

GO

CREATE PROCEDURE EDDSDBO.[QoS_CollectAgentUptimeData]
AS
BEGIN
	declare @agentId int
	declare @lastupdate datetime
	declare @currentLastUpdateNormilized datetime
	declare @dateNowNormilized datetime = DATEADD(MINUTE, DATEDIFF(MINUTE,0,getutcdate()),0)

	SELECT DISTINCT top(1) @agentId = artifactID, @lastupdate = [LastUpdate]
	FROM EDDS.eddsdbo.Agent (NOLOCK)
	order by [LastUpdate] desc

	select top(1) @currentLastUpdateNormilized = [timestamp]
	from EDDSPerformance.eddsdbo.agenthistory
	order by [timestamp] desc

	set @currentLastUpdateNormilized = isnull(@currentLastUpdateNormilized, '1901-01-01')
	set @currentLastUpdateNormilized = DATEADD(MINUTE, DATEDIFF(MINUTE,0,@currentLastUpdateNormilized),0) --normalize to minute


	--select @lastupdate as lastupdate,
	--@currentLastUpdateNormilized as currentLastUpdateNormilized,
	--@dateNowNormilized as dateNowNormilized,
	--GETUTCDATE() as dateNow,
	--DATEDIFF(minute,@currentLastUpdateNormilized, @dateNowNormilized) as diffCurrNow,
	--DATEDIFF(SECOND, @lastupdate, @dateNowNormilized) as diffLastNow

	insert into EDDSPerformance.eddsdbo.agenthistory
	(agentartifactid, [timestamp], successful)
	select @agentId, @dateNowNormilized, 0
	where DATEDIFF(MINUTE,@currentLastUpdateNormilized, @dateNowNormilized) >= 1

	update EDDSPerformance.eddsdbo.agenthistory
	set Successful = 1
	where 
		[timestamp] = @dateNowNormilized
		and DATEDIFF(SECOND, @lastupdate, @dateNowNormilized) <= 60 
		and Successful = 0
	
End