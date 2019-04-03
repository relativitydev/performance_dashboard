USE EDDSPerformance
GO

IF EXISTS (select 1 from sysobjects where [name] = 'TuningForkRel' and type = 'P')  
BEGIN
	DROP PROCEDURE EDDSDBO.TuningForkRel
END

GO

CREATE PROCEDURE EDDSDBO.[TuningForkRel]
AS
BEGIN
	/* use this portion of the code to develop a 'reset'
	SP_CONFIGURE 'show advanced options', 1;
	GO
	RECONFIGURE;
	GO
	*/
	/* ToDo
	cHeck locations of full text catalog
	Incorporate other performance based testing scripts.*/


	IF EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'TuningForkRelativityConfig')
		DROP TABLE EDDSDBO.TuningForkRelativityConfig

	SET ANSI_NULLS ON

	SET QUOTED_IDENTIFIER ON

	SET ANSI_PADDING ON
	
	IF EXISTS(SELECT TABLE_NAME from INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'TuningForkRelativityConfig') 
	begin
		DROP TABLE EDDSDBO.TuningForkRelativityConfig
	end

	CREATE TABLE [eddsdbo].[TuningForkRelativityConfig](
	[Section] [nvarchar](200) NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[ActualValue] [nvarchar](max) NOT NULL,
	[DefaultValue] [nvarchar](max) NULL,
	[kIE_Status] [varchar](100) NULL,
	[Note] [nvarchar](max) NULL,
	[MachineName] [varchar](100) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[Severity] [int] NULL
	)

	SET ANSI_PADDING OFF

	DECLARE @mothership NVARCHAR(MAX),
		@neglectedServerCount INT = (
				SELECT COUNT(*)
				FROM [EDDSPerformance].[eddsdbo].[Server] WITH(NOLOCK)
				WHERE ISNULL(IgnoreServer, 0) = 0
					AND DeletedOn IS NULL
					AND LastChecked < DATEADD(hh, -1, getutcdate())
		),
		@avgScoringStartMinute INT = ISNULL((
			SELECT AVG(DATEPART(MINUTE, RunDateTime))
			FROM [EDDSPerformance].[eddsdbo].[QoS_GlassRunHistory] WITH(NOLOCK)
			WHERE RunDateTime > DATEADD(dd, -7, getutcdate())
				AND RunDuration < 3600000
				AND GlassRunID > 1
		), 0);

	INSERT INTO EDDSDBO.TuningForkRelativityConfig 
		([Section], [Name], [ActualValue], [MachineName], [Description])  
	SELECT distinct
		c.[Section], c.[Name], c.[Value], c.[MachineName], isnull(dflts.[Description], c.[Description])
	FROM [EDDS].eddsdbo.Configuration as c WITH(NOLOCK)
	left outer join [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults] as dflts on c.[Section] = dflts.[Section] AND c.Name = dflts.[Name] 
	where c.Section IN (
			'kCura.Data.RowDataGateway',
			'kCura.EDDS.Agents',
			'kCura.EDDS.SqlServer',
			'kCura.EDDS.Web',
			'kCura.EDDS.Web.Distributed',
			'kCura.EDDS.WebAPI',
			'kCura.Notification',
			'kCura.Relativity',
			'Relativity.Core',
			'Relativity.Data'
		)
		AND c.Name IN (
			'LongRunningQueryTimeout',
			'AgentOffHourEndTime',
			'AgentOffHourStartTime',
			'BrandingManagerBatchAmount',
			'FileDeletionManagerBatchAmount',
			'OCRCompilationBatchSize',
			'OCRProcessingBatchSize',
			'OCRWorkerBatchAmount',
			'OCRWorkerInsertBatchSize',
			'WordIndexMaxWordSize',
			'DataDirectory',
			'FTDirectory',
			'LDFDirectory',
			'AdvancedSearchDefault',
			'AncestorCountWarningLevel',
			'FileBrowserMaximumElements',
			'ImageViewerBufferSize',
			'MassCopyBatchAmount',
			'MassDeleteBatchAmount',
			'MassEditBatchAmount',
			'MassExportBatchAmount',
			'MassMoveBatchAmount',
			'MassProduceBatchAmount',
			'MaximumListPageTextLength',
			'MaximumNativeSizeForViewerInBytes',
			'ShowStackTraceOnError',
			'TallyBatchAmount',
			'WebClientNativeViewerCacheAheadMaxSizeInBytes',
			'WebClientTimeout',
			'BufferAcquisitionTimeoutInMilliseconds',
			'BufferCount',
			'BufferSize',
			'ValidateAssemblyVersion',
			'MassCreateBatchSize',
			'MassDeleteBatchAmount',
			'PDVCacheLifetime',
			'PDVDefaultQueryCacheSize',
			'AllowBrandingMultiThread',
			'AuditCountQueries',
			'AuditFullQueries',
			'AuditIdQueries',
			'AuthenticationTokenLifetimeInMinutes',
			'BrandingRecordCreationBatchSize',
			'CancelRequestTimeDelay',
			'ChoiceLimitForUI',
			'dtSearchBatchSize',
			'EnableTransactionalImports',
			'MaxPDVQueryLength',
			'MaxQueryConditionCharacterLength',
			'ViewQueryOptimization',
			'dtSearchQueueBatchSize',
			'dtSearchStreamBufferSize',
			'dtSearchStreamThresholdInBytes',
			'FileDeleteChunkSizeOnDocumentDelete',
			'QueryCacheMode',
			'SearchIndexerLongRunningQueryTimeout',
			'SearchIndexerTextFromSQLChunkSizeInBytes'
		)

	DECLARE @agentStart INT
	DECLARE @agentEnd INT
	DECLARE @data varchar(1)
	DECLARE @fulltext varchar(1)
	DECLARE @log varchar(1)
	DECLARE @i INT
	DECLARE @iMax INT
	DECLARE @machineName varchar(100)
	DECLARE @recommendatonDefaultID uniqueidentifier = null

	CREATE TABLE #temp(
		[ID] [int] IDENTITY(1,1) NOT NULL,
		[MachineName] [varchar](100)
		)
	INSERT INTO #temp ([MachineName]) SELECT DISTINCT MachineName FROM EDDSDBO.TuningForkRelativityConfig WHERE Section = 'kCura.EDDS.SqlServer'

	SELECT @iMax = (SELECT COUNT (*) FROM #temp)
	SET @i = 1

	WHILE @i <= @iMax
		BEGIN
			SELECT @machineName = (SELECT MachineName FROM #temp WHERE [ID] = @i)
			SELECT @data = SUBSTRING(
								(SELECT ActualValue FROM EDDSDBO.TuningForkRelativityConfig
								WHERE Name = 'DataDirectory' AND MachineName = @machineName),
								1,1)
								
			SELECT @fulltext = SUBSTRING(
								(SELECT ActualValue FROM EDDSDBO.TuningForkRelativityConfig
								WHERE Name = 'FTDirectory' AND MachineName = @machineName),
								1,1)
								
			SELECT @log = SUBSTRING(
								(SELECT ActualValue FROM EDDSDBO.TuningForkRelativityConfig
								WHERE Name = 'LDFDirectory' AND MachineName = @machineName),
								1,1)
								
			UPDATE EDDSDBO.TuningForkRelativityConfig
			SET kIE_Status =
			CASE
				WHEN (@data != @fulltext AND @data != @log)
				AND  (@fulltext != @log) AND @data != 'C' AND @log != 'C' AND @fulltext != 'C'
				THEN 'Good'
				ELSE 'Warning'
			END
			WHERE Section = 'kCura.EDDS.SqlServer' AND MachineName = @machineName

			UPDATE EDDSDBO.TuningForkRelativityConfig
			SET Note =
			CASE
				WHEN kIE_Status = 'Warning' THEN 'Please ensure that your data, full text, and log directories all sit on separate physical disks.  Also be sure that none of this SQL data resides on the same disk as the OS.'
				ELSE 'None'
			END,
			MachineName = 
			case 
				when @machineName = '' then 'Relativity'
				else @machineName
			end
			WHERE Section = 'kCura.EDDS.SqlServer' AND MachineName = @machineName
				
			SET @i = @i+1
		END
	--SELECT * FROM #temp
	DROP TABLE #temp		

	SELECT @agentStart = 
		CASE
		WHEN ISNUMERIC(SUBSTRING(
								(SELECT ActualValue FROM EDDSDBO.TuningForkRelativityConfig 
								WHERE Name = 'AgentOffHourStartTime'), 
								1, 2)) = 1
								
								THEN CAST(SUBSTRING(
								(SELECT ActualValue FROM EDDSDBO.TuningForkRelativityConfig 
								WHERE Name = 'AgentOffHourStartTime'), 
								1, 2) AS INT)
		ELSE CAST(SUBSTRING(
								(SELECT ActualValue FROM EDDSDBO.TuningForkRelativityConfig 
								WHERE Name = 'AgentOffHourStartTime'), 
								1, 1) AS INT)
		END
								
	SELECT @agentEnd = 
		CASE
		WHEN ISNUMERIC(SUBSTRING(
								(SELECT ActualValue FROM EDDSDBO.TuningForkRelativityConfig 
								WHERE Name = 'AgentOffHourEndTime'), 
								1, 2)) = 1
								
								THEN CAST(SUBSTRING(
								(SELECT ActualValue FROM EDDSDBO.TuningForkRelativityConfig 
								WHERE Name = 'AgentOffHourEndTime'), 
								1, 2) AS INT)
		ELSE CAST(SUBSTRING(
								(SELECT ActualValue FROM EDDSDBO.TuningForkRelativityConfig 
								WHERE Name = 'AgentOffHourEndTime'), 
								1, 1) AS INT)
		END

	/*****************************************     Start setting default values       *********************************************/

	UPDATE kcct
	SET DefaultValue = dflts.[DefaultValue], kIE_Status = dflts.[Status], Note = dflts.[Recommendation]
	from EDDSDBO.TuningForkRelativityConfig as kcct
	inner join [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults] as dflts on kcct.[Section] = dflts.[Section] AND kcct.Name = dflts.[Name] 
	WHERE  dflts.[DefaultValue] is not null
	and kcct.[Name] in ( 'AgentOffHourEndTime','LongRunningQueryTimeout', 'AgentOffHourStartTime', 'BrandingManagerBatchAmount', 'FileDeletionManagerBatchAmount', 'OCRCompilationBatchSize', 'OCRProcessingBatchSize', 'OCRWorkerBatchAmount', 'OCRWorkerInsertBatchSize', 'WordIndexMaxWordSize', 'BackupDirectory', 'DataDirectory', 'FTDirectory', 'LDFDirectory', 'AdvancedSearchDefault', 'AncestorCountWarningLevel', 'FileBrowserMaximumElements', 'ImageViewerBufferSize', 'MassCopyBatchAmount', 'MassDeleteBatchAmount', 'MassEditBatchAmount', 'MassExportBatchAmount', 'MassMoveBatchAmount', 'MassProduceBatchAmount', 'MaximumListPageTextLength', 'MaximumNativeSizeForViewerInBytes', 'ShowStackTraceOnError', 'TallyBatchAmount', 'WebClientNativeViewerCacheAheadMaxSizeInBytes', 'WebClientTimeout', 'BufferAcquisitionTimeoutInMilliseconds', 'BufferCount', 'BufferSize', 'ValidateAssemblyVersion', 'MassCreateBatchSize', 'MassDeleteBatchAmount', 'PDVCacheLifetime', 'PDVDefaultQueryCacheSize', 'AllowBrandingMultiThread', 'AuditCountQueries', 'AuditFullQueries', 'AuditIdQueries', 'AuthenticationTokenLifetimeInMinutes', 'BrandingRecordCreationBatchSize', 'CancelRequestTimeDelay', 'ChoiceLimitForUI', 'dtSearchBatchSize', 'EnableTransactionalImports', 'MaxPDVQueryLength', 'MaxQueryConditionCharacterLength', 'ViewQueryOptimization', 'dtSearchQueueBatchSize', 'dtSearchStreamBufferSize', 'dtSearchStreamThresholdInBytes', 'FileDeleteChunkSizeOnDocumentDelete', 'QueryCacheMode', 'SearchIndexerLongRunningQueryTimeout', 'SearchIndexerTextFromSQLChunkSizeInBytes', 'QueryCacheMode', 'AuditCountQueries', 'AuditFullQueries', 'AuditIdQueries')

	
	/*****************************************     AgentOffHourEndTime     *********************************************/
	
	if (@agentEnd - @agentStart) < 4 OR (@agentEnd - @agentStart) > 10 OR @agentEnd > 6 OR (@agentStart < 20 AND @agentStart > 2)
		set @recommendatonDefaultID = 'f4b48f1f-b531-4bba-adad-a992e5434cc3'
	else
		set @recommendatonDefaultID = '63cb6880-dcc6-4b01-ace2-e2a251f78542'
	
	UPDATE kcct
	SET Note = dflts.Recommendation, kIE_status = dflts.[Status], Severity = dflts.[Severity]
	from EDDSDBO.TuningForkRelativityConfig as kcct
	inner join [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults] as dflts 
	on kcct.Name = 'AgentOffHourEndTime' and dflts.Name = kcct.Name and dflts.ID = @recommendatonDefaultID
	
		
	/*****************************************     AgentOffHourStartTime      *********************************************/
	
	if (@agentEnd - @agentStart) < 4 OR (@agentEnd - @agentStart) > 10 OR @agentEnd > 6 OR (@agentStart < 20 AND @agentStart > 2)
		set @recommendatonDefaultID = 'f4b48f1f-b531-4bba-adad-a992e5434cc3'
	else
		set @recommendatonDefaultID = '63cb6880-dcc6-4b01-ace2-e2a251f78542'
	
	UPDATE kcct
	SET Note = dflts.Recommendation, kIE_status = dflts.[Status], Severity = dflts.[Severity]
	from EDDSDBO.TuningForkRelativityConfig as kcct
	inner join [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults] as dflts
	on kcct.Name = 'AgentOffHourStartTime' and dflts.Name = kcct.Name and dflts.ID = @recommendatonDefaultID
	
	
	/*****************************************     QueryCacheMode      *********************************************/
	
	UPDATE kcct
	SET kcct.Note = dflts.Recommendation, kcct.kIE_status = dflts.[Status], kcct.Severity = dflts.[Severity]
	from EDDSDBO.TuningForkRelativityConfig as kcct
	inner join [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults] as dflts on kcct.[Section] = dflts.[Section] AND kcct.Name = dflts.[Name] 
	WHERE kcct.Name ='QueryCacheMode' and dflts.ID = 'affbc853-d852-44d0-979c-78411be8a767' and kcct.ActualValue != dflts.DefaultValue
	
	UPDATE kcct
	SET kcct.Note = dflts.Recommendation, kcct.kIE_status = dflts.[Status], kcct.Severity = dflts.[Severity]
	from EDDSDBO.TuningForkRelativityConfig as kcct
	inner join [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults] as dflts on kcct.[Section] = dflts.[Section] AND kcct.Name = dflts.[Name] 
	WHERE kcct.Name ='QueryCacheMode' and dflts.ID = '73bdd8f3-496e-43e7-94f0-12a879c34358' and kcct.ActualValue = dflts.DefaultValue
	
	
	/*****************************************     AuditCountQueries      *********************************************/
	
	UPDATE kcct
	SET kcct.Note = dflts.Recommendation, kcct.kIE_status = dflts.[Status], kcct.Severity = dflts.[Severity]
	from EDDSDBO.TuningForkRelativityConfig as kcct
	inner join [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults] as dflts on kcct.[Section] = dflts.[Section] AND kcct.Name = dflts.[Name] 
	WHERE kcct.Name ='AuditCountQueries' and dflts.ID = '5d279a91-b3a1-42b6-b702-d463b49d8973' and kcct.ActualValue != dflts.DefaultValue
	
	UPDATE kcct
	SET kcct.Note = dflts.Recommendation, kcct.kIE_status = dflts.[Status], kcct.Severity = dflts.[Severity]
	from EDDSDBO.TuningForkRelativityConfig as kcct
	inner join [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults] as dflts on kcct.[Section] = dflts.[Section] AND kcct.Name = dflts.[Name] 
	WHERE kcct.Name ='AuditCountQueries' and dflts.ID = '2d61c1d8-4a2c-49db-b2fc-7b2511c7934d' and kcct.ActualValue = dflts.DefaultValue
	
	
	/*****************************************     AuditFullQueries      *********************************************/
		
	UPDATE kcct
	SET kcct.Note = dflts.Recommendation, kcct.kIE_status = dflts.[Status], kcct.Severity = dflts.[Severity]
	from EDDSDBO.TuningForkRelativityConfig as kcct
	inner join [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults] as dflts on kcct.[Section] = dflts.[Section] AND kcct.Name = dflts.[Name] 
	WHERE kcct.Name ='AuditFullQueries' and dflts.ID = 'dc05ca38-2e66-459f-9b98-d8f3bb74f5e9' and kcct.ActualValue != dflts.DefaultValue
	
	UPDATE kcct
	SET kcct.Note = dflts.Recommendation, kcct.kIE_status = dflts.[Status], kcct.Severity = dflts.[Severity]
	from EDDSDBO.TuningForkRelativityConfig as kcct
	inner join [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults] as dflts on kcct.[Section] = dflts.[Section] AND kcct.Name = dflts.[Name] 
	WHERE kcct.Name ='AuditFullQueries' and dflts.ID = 'ae4d0bf0-58a5-4f87-bf48-1b97fa358703' and kcct.ActualValue = dflts.DefaultValue
	
	
	/*****************************************     AuditIdQueries      *********************************************/
		
	UPDATE kcct
	SET kcct.Note = dflts.Recommendation, kcct.kIE_status = dflts.[Status], kcct.Severity = dflts.[Severity]
	from EDDSDBO.TuningForkRelativityConfig as kcct
	inner join [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults] as dflts on kcct.[Section] = dflts.[Section] AND kcct.Name = dflts.[Name] 
	WHERE kcct.Name ='AuditIdQueries' and dflts.ID = '98f4a499-a514-4beb-8521-ee5f82635ed1' and kcct.ActualValue != dflts.DefaultValue
	
	UPDATE kcct
	SET kcct.Note = dflts.Recommendation, kcct.kIE_status = dflts.[Status], kcct.Severity = dflts.[Severity]
	from EDDSDBO.TuningForkRelativityConfig as kcct
	inner join [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults] as dflts on kcct.[Section] = dflts.[Section] AND kcct.Name = dflts.[Name] 
	WHERE kcct.Name ='AuditIdQueries' and dflts.ID = '82411e6d-9b8f-418b-b8a5-a0458d8adf2c' and kcct.ActualValue = dflts.DefaultValue
	
		
	/*****************************************    Defaults     *********************************************/
	
	
	UPDATE kcct
	SET kcct.kIE_Status = dflts.[Status], kcct.Note = dflts.[Recommendation], kcct.Severity = dflts.Severity
	from EDDSDBO.TuningForkRelativityConfig as kcct
	inner join [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults] as dflts on kcct.[Section] = dflts.[Section] AND kcct.Name = dflts.[Name] 
	WHERE  kcct.[Name] in ('LongRunningQueryTimeout', 'AgentOffHourEndTime', 'AgentOffHourStartTime', 'BrandingManagerBatchAmount', 'FileDeletionManagerBatchAmount', 'OCRCompilationBatchSize', 'OCRProcessingBatchSize', 'OCRWorkerBatchAmount', 'OCRWorkerInsertBatchSize', 'WordIndexMaxWordSize', 'BackupDirectory', 'DataDirectory', 'FTDirectory', 'LDFDirectory', 'AdvancedSearchDefault', 'AncestorCountWarningLevel', 'FileBrowserMaximumElements', 'ImageViewerBufferSize', 'MassCopyBatchAmount', 'MassDeleteBatchAmount', 'MassEditBatchAmount', 'MassExportBatchAmount', 'MassMoveBatchAmount', 'MassProduceBatchAmount', 'MaximumListPageTextLength', 'MaximumNativeSizeForViewerInBytes', 'ShowStackTraceOnError', 'TallyBatchAmount', 'WebClientNativeViewerCacheAheadMaxSizeInBytes', 'WebClientTimeout', 'BufferAcquisitionTimeoutInMilliseconds', 'BufferCount', 'BufferSize', 'ValidateAssemblyVersion', 'MassCreateBatchSize', 'MassDeleteBatchAmount', 'PDVCacheLifetime', 'PDVDefaultQueryCacheSize', 'AllowBrandingMultiThread', 'AuditCountQueries', 'AuditFullQueries', 'AuditIdQueries', 'AuthenticationTokenLifetimeInMinutes', 'BrandingRecordCreationBatchSize', 'CancelRequestTimeDelay', 'ChoiceLimitForUI', 'dtSearchBatchSize', 'EnableTransactionalImports', 'MaxPDVQueryLength', 'MaxQueryConditionCharacterLength', 'ViewQueryOptimization', 'dtSearchQueueBatchSize', 'dtSearchStreamBufferSize', 'dtSearchStreamThresholdInBytes', 'FileDeleteChunkSizeOnDocumentDelete', 'QueryCacheMode', 'SearchIndexerLongRunningQueryTimeout', 'SearchIndexerTextFromSQLChunkSizeInBytes', 'QueryCacheMode', 'AuditCountQueries', 'AuditFullQueries', 'AuditIdQueries')
	and dflts.[Status] = '<DEFAULT>'
	
	
	UPDATE EDDSDBO.TuningForkRelativityConfig
	SET kIE_Status = 
		CASE
			WHEN ActualValue != DefaultValue THEN 'Not Default'
			WHEN ActualValue = DefaultValue THEN 'Good'
			ELSE kIE_Status
		END,
	Severity = 
		CASE
			WHEN ActualValue != DefaultValue THEN 10
			WHEN ActualValue = DefaultValue THEN 0
			ELSE kIE_Status
		END
	where kIE_Status = '<DEFAULT>'
	
	UPDATE EDDSDBO.TuningForkRelativityConfig
	SET Note = 
		CASE
			WHEN kIE_Status = 'Good' THEN 'This is the default value.  No recommendations can be made without further environmental analysis.'
			WHEN kIE_Status = 'Not Default' THEN 'This is not the default value, but no action is necessarily required.'		
			ELSE Note
		END
	where Note = '<DEFAULT>'
	
		
	/*****************************************    Performance Dashboard configuration checks     *********************************************/
	
	INSERT INTO EDDSDBO.TuningForkRelativityConfig
		(Section, Name, [ActualValue], [DefaultValue], MachineName, [Description])
		select distinct dflts.[Section], dflts.[Name], [DefaultValue] as [ActualValue], [DefaultValue], '' as MachineName, dflts.[Description]
		from [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults] as dflts
		where Name in ('Number of QoS Manager Agents', 'Number of QoS Worker Agents', 'Number of WMI Worker Agents', 'Number of Trust Worker Agents')
		
	
	-- Number of QoS Manager Agents --
	
	UPDATE EDDSDBO.TuningForkRelativityConfig
	SET ActualValue = ISNULL((
		SELECT COUNT(*)
		FROM [EDDS].[eddsdbo].[Agent] a WITH(NOLOCK)
		INNER JOIN [EDDS].[eddsdbo].[AgentType] at WITH(NOLOCK) ON a.AgentTypeArtifactID = at.ArtifactID
		WHERE at.Name = 'Performance Dashboard - QoS Manager' AND a.[Enabled] = 1
	), 0)
	WHERE Name = 'Number of QoS Manager Agents'

	declare @actualAgentCount int = 0
	declare @defaultAgentCount int = (select [DefaultValue] from [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults] where ID = '7c8ed281-c4c7-4e02-8b0e-343c8266b677')
	
	SELECT @actualAgentCount= COUNT(*)
		FROM [EDDS].[eddsdbo].[Agent] a WITH(NOLOCK)
		INNER JOIN [EDDS].[eddsdbo].[AgentType] at WITH(NOLOCK) ON a.AgentTypeArtifactID = at.ArtifactID
		WHERE at.Name = 'Performance Dashboard - QoS Manager' AND a.[Enabled] = 1
	
	if(@actualAgentCount = '0')
		set @recommendatonDefaultID = '7c8ed281-c4c7-4e02-8b0e-343c8266b677'
	else if(@actualAgentCount != @defaultAgentCount)
		set @recommendatonDefaultID = 'b2960953-5a7b-42ce-a777-cc53dc7d2261'
	else
		set @recommendatonDefaultID = '16e0576b-6d28-4ca9-833a-8cf18981dc4d'
		
	UPDATE kcct
	SET kcct.Note = dflts.Recommendation, kcct.kIE_status = dflts.[Status], kcct.Severity = dflts.[Severity]
	from EDDSDBO.TuningForkRelativityConfig as kcct
	inner join [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults] as dflts on kcct.[Section] = dflts.[Section] AND kcct.Name = dflts.[Name] 
	WHERE kcct.Name ='Number of QoS Manager Agents' and dflts.ID = @recommendatonDefaultID
	

	-- Number of Trust Worker Agents --
	
	UPDATE EDDSDBO.TuningForkRelativityConfig
	SET ActualValue = ISNULL((
		SELECT COUNT(*)
		FROM [EDDS].[eddsdbo].[Agent] a WITH(NOLOCK)
		INNER JOIN [EDDS].[eddsdbo].[AgentType] at WITH(NOLOCK) ON a.AgentTypeArtifactID = at.ArtifactID
		WHERE at.Name = 'Performance Dashboard - Trust Worker' AND a.[Enabled] = 1
	), 0)
	WHERE Name = 'Number of Trust Worker Agents'

	set @defaultAgentCount = (select [DefaultValue] from [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults] where ID = '7b8c7d7c-edae-4677-8450-3e269e847235')
	
	SELECT @actualAgentCount= COUNT(*)
		FROM [EDDS].[eddsdbo].[Agent] a WITH(NOLOCK)
		INNER JOIN [EDDS].[eddsdbo].[AgentType] at WITH(NOLOCK) ON a.AgentTypeArtifactID = at.ArtifactID
		WHERE at.Name = 'Performance Dashboard - Trust Worker' AND a.[Enabled] = 1
	
	if(@actualAgentCount = '0')
		set @recommendatonDefaultID = '7b8c7d7c-edae-4677-8450-3e269e847235'
	else if(@actualAgentCount != @defaultAgentCount)
		set @recommendatonDefaultID = 'f1343c4d-e696-439c-9c0e-211bb294756a'
	else
		set @recommendatonDefaultID = '9d7e6c29-084e-4af6-b0ef-626776561a89'
		
	UPDATE kcct
	SET kcct.Note = dflts.Recommendation, kcct.kIE_status = dflts.[Status], kcct.Severity = dflts.[Severity]
	from EDDSDBO.TuningForkRelativityConfig as kcct
	inner join [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults] as dflts on kcct.[Section] = dflts.[Section] AND kcct.Name = dflts.[Name] 
	WHERE kcct.Name ='Number of Trust Worker Agents' and dflts.ID = @recommendatonDefaultID

	-- Number of WMI Worker Agents --
	
	UPDATE EDDSDBO.TuningForkRelativityConfig
	SET ActualValue = ISNULL((
		SELECT COUNT(*)
		FROM [EDDS].[eddsdbo].[Agent] a WITH(NOLOCK)
		INNER JOIN [EDDS].[eddsdbo].[AgentType] at WITH(NOLOCK) ON a.AgentTypeArtifactID = at.ArtifactID
		WHERE at.Name = 'Performance Dashboard - WMI Worker' AND a.[Enabled] = 1
	), 0)
	WHERE Name = 'Number of WMI Worker Agents'

	set @defaultAgentCount = (select [DefaultValue] from [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults] where ID = '2c1fb60f-701f-49b2-997b-617d44bfb4bb')
	
	SELECT @actualAgentCount= COUNT(*)
		FROM [EDDS].[eddsdbo].[Agent] a WITH(NOLOCK)
		INNER JOIN [EDDS].[eddsdbo].[AgentType] at WITH(NOLOCK) ON a.AgentTypeArtifactID = at.ArtifactID
		WHERE at.Name = 'Performance Dashboard - WMI Worker' AND a.[Enabled] = 1
	
	if(@actualAgentCount = '0')
		set @recommendatonDefaultID = '2c1fb60f-701f-49b2-997b-617d44bfb4bb'
	else if(@neglectedServerCount > 0)
		set @recommendatonDefaultID = 'baf830e5-1fd0-44bc-8c8c-87c9981d606e'
	else
		set @recommendatonDefaultID = 'e5a55396-c85e-40e2-8699-f4477b81ae42'
		
	UPDATE kcct
	SET kcct.Note = dflts.Recommendation, kcct.kIE_status = dflts.[Status], kcct.Severity = dflts.[Severity]
	from EDDSDBO.TuningForkRelativityConfig as kcct
	inner join [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults] as dflts on kcct.[Section] = dflts.[Section] AND kcct.Name = dflts.[Name] 
	WHERE kcct.Name ='Number of WMI Worker Agents' and dflts.ID = @recommendatonDefaultID
	
	
	-- Number of QoS Worker Agents --
	
	UPDATE EDDSDBO.TuningForkRelativityConfig
	SET ActualValue = ISNULL((
		SELECT COUNT(*)
		FROM [EDDS].[eddsdbo].[Agent] a WITH(NOLOCK)
		INNER JOIN [EDDS].[eddsdbo].[AgentType] at WITH(NOLOCK) ON a.AgentTypeArtifactID = at.ArtifactID
		WHERE at.Name = 'Performance Dashboard - QoS Worker' AND a.[Enabled] = 1
	), 0)
	WHERE Name = 'Number of QoS Worker Agents'

	set @defaultAgentCount = (select [DefaultValue] from [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults] where ID = '0e53d4fc-6a75-4697-8b04-feae0e5e72bc')
	
	SELECT @actualAgentCount= COUNT(*)
		FROM [EDDS].[eddsdbo].[Agent] a WITH(NOLOCK)
		INNER JOIN [EDDS].[eddsdbo].[AgentType] at WITH(NOLOCK) ON a.AgentTypeArtifactID = at.ArtifactID
		WHERE at.Name = 'Performance Dashboard - QoS Worker' AND a.[Enabled] = 1
	
	if(CAST(@actualAgentCount as int) = 0)
		set @recommendatonDefaultID = '0e53d4fc-6a75-4697-8b04-feae0e5e72bc'
	else if(CAST(@actualAgentCount as int) > 8 AND @avgScoringStartMinute <= 6)
		set @recommendatonDefaultID = 'ffb4954e-7b74-413b-8ef8-f110da1c1796'
	else if(@avgScoringStartMinute > 10)
		set @recommendatonDefaultID = '79e95b24-468e-4dda-a572-fc10592e9e3e'
	else
		set @recommendatonDefaultID = '8b8271d0-c987-4fd2-8548-ee4f1010617c'
	
	UPDATE kcct
	SET kcct.Note = dflts.Recommendation, kcct.kIE_status = dflts.[Status], kcct.Severity = dflts.[Severity]
	from EDDSDBO.TuningForkRelativityConfig as kcct
	inner join [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults] as dflts on kcct.[Section] = dflts.[Section] AND kcct.Name = dflts.[Name] 
	WHERE kcct.Name ='Number of QoS Worker Agents' and dflts.ID = @recommendatonDefaultID
	
	
	/*****************************************    Process Control     *********************************************/
	
	
	INSERT INTO EDDSDBO.TuningForkRelativityConfig
		(Section, Name, [ActualValue], [DefaultValue], [MachineName], [Description])
	SELECT
		dflts.[Section],
		'Task: ' + ProcessTypeDesc,
		CASE
			WHEN Frequency < 0 THEN 'Disabled'
			WHEN DATEADD(MINUTE, Frequency + 120, LastProcessExecDateTime) > getutcdate() THEN 'Enabled'
			ELSE 'Not Running'
		END,
		dflts.DefaultValue,
		'',
		dflts.Description
	FROM EDDSPerformance.eddsdbo.ProcessControl WITH(NOLOCK)
	inner join [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults] as dflts on dflts.ID = 'a0551000-c45c-40eb-9b16-6b0f76ac01ed'
	WHERE ProcessControlID NOT IN (4, 13, 23)

	UPDATE EDDSDBO.TuningForkRelativityConfig
	SET kIE_Status = dflts.[Status], Note = dflts.Recommendation, Severity = dflts.Severity
	from EDDSDBO.TuningForkRelativityConfig as kcct
	inner join [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults] as dflts on dflts.ID = 'a0551000-c45c-40eb-9b16-6b0f76ac01ed'
	WHERE kcct.Name LIKE 'Task:%' and ActualValue = 'Disabled'
	
	UPDATE EDDSDBO.TuningForkRelativityConfig
	SET kIE_Status = dflts.[Status], Note = dflts.Recommendation, Severity = dflts.Severity
	from EDDSDBO.TuningForkRelativityConfig as kcct
	inner join [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults] as dflts on dflts.ID = '16cc163e-f310-48ae-97c3-0a9d8dfb1b65'
	WHERE kcct.Name LIKE 'Task:%' and ActualValue = 'Not Running'
	
	UPDATE EDDSDBO.TuningForkRelativityConfig
	SET kIE_Status = dflts.[Status], Note = dflts.Recommendation, Severity = dflts.Severity
	from EDDSDBO.TuningForkRelativityConfig as kcct
	inner join [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults] as dflts on dflts.ID = '1a442de2-1f75-4f0b-b685-8c582f5ef878'
	WHERE kcct.Name LIKE 'Task:%' and ActualValue <> 'Disabled' and ActualValue <> 'Not Running'

	/*****************************************    Clean Results     *********************************************/
	
	delete from EDDSDBO.TuningForkRelativityConfig
	WHERE [kIE_Status] is null or Note is null
	
	/*****************************************    Save Results     *********************************************/
	

	--clear out previous results
	delete from eddsdbo.[EnvironmentCheckRecommendations]
	where [Section] <> 'SQL Configuration'
	
	--insert new results
	insert into eddsdbo.EnvironmentCheckRecommendations
		([Scope], [Name], [Description],[Status], [Recommendation], [Value], [Section], Severity)
	SELECT
		isnull(NULLIF([MachineName],''),'Relativity'), [Name], [Description], [kIE_Status], [Note], [ActualValue], [Section], isnull(Severity, 0)
	FROM EDDSDBO.TuningForkRelativityConfig

	IF EXISTS(SELECT TABLE_NAME from INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'TuningForkRelativityConfig') DROP TABLE EDDSDBO.TuningForkRelativityConfig

END