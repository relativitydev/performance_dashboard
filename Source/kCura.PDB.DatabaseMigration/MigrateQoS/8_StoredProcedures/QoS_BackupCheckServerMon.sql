USE [EDDSQoS]
GO


IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_BackupCheckServerMon]') AND type in (N'P', N'PC'))
	DROP PROCEDURE [eddsdbo].[QoS_BackupCheckServerMon]
GO

--Push backup data from this server to QoS_BackupHistory on the primary SQL instance

CREATE PROCEDURE [eddsdbo].[QoS_BackupCheckServerMon]
	@summaryDayHour DATETIME
	,@currentServerName nvarchar (255)
	,@eddsPerformanceServerName nvarchar(255)
AS
BEGIN


	DECLARE
		@USESQL nvarchar(max),
		@params NVARCHAR (500);
	declare @backupHistoryDataDays int = -30;
	
	SET @params = '@summaryDayHour datetime, @currentServerName nvarchar(255)'
	SET @USESQL = '
		declare @lastServerBackup datetime
		
		set @lastServerBackup = (select top(1) isnull(LastServerBackup, DATEADD(dd, ' + convert(nvarchar(4), @backupHistoryDataDays ) + ', @summaryDayHour)) from ' + QUOTENAME(@eddsPerformanceServerName) + '.EDDSPerformance.eddsdbo.Server as s
			where s.ServerName = @currentServerName and ServerTypeID = 3 and DeletedOn IS NULL and ISNULL(IgnoreServer, 0) = 0
			order by LastServerBackup desc)
	
		INSERT INTO ' + QUOTENAME(@eddsPerformanceServerName) + '.EDDSPerformance.eddsdbo.QoS_BackupHistory
			(DBName, CompletedOn, Duration, BackupType, LoggedDate)
		SELECT
			database_name,
			backup_finish_date,
			DATEDIFF(ss, backup_start_date, backup_finish_date),
			[type],
			@summaryDayHour
		FROM msdb.dbo.backupset bs WITH(NOLOCK)
		WHERE backup_finish_date > DATEADD(dd, ' + convert(nvarchar(4), @backupHistoryDataDays ) + ', @summaryDayHour)
			AND backup_finish_date > @lastServerBackup
			AND (
				database_name = ''EDDS''
				OR database_name LIKE ''EDDS[0-9]%''
				OR database_name LIKE ''INV%''
			)

		delete from ' + QUOTENAME(@eddsPerformanceServerName) + '.EDDSPerformance.eddsdbo.QoS_BackupHistory
		where [BackupType] not in (''D'',''I'',''L'')
			
		declare @newLastServerBackup datetime = (select top(1) CompletedOn from ' + QUOTENAME(@eddsPerformanceServerName) + '.EDDSPerformance.eddsdbo.QoS_BackupHistory order by CompletedOn desc)
		set @newLastServerBackup = isnull(@newLastServerBackup, @lastServerBackup)
		
		update ' + QUOTENAME(@eddsPerformanceServerName) + '.EDDSPerformance.eddsdbo.Server
		set LastServerBackup = @newLastServerBackup
		where ServerName = @currentServerName and ServerTypeID = 3 and DeletedOn IS NULL and ISNULL(IgnoreServer, 0) = 0
	'

	-- Execute SQL statement to update table
	EXEC sp_executeSQL @USESQL, @params, @summaryDayHour, @currentServerName;
	

	
END