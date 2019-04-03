-- EDDSPerformance

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_BackupCheckServerMonTest]') AND type in (N'P', N'PC'))
	DROP PROCEDURE [eddsdbo].[QoS_BackupCheckServerMonTest]
GO

--Push backup data from this server to QoS_BackupHistory on the primary SQL instance

CREATE PROCEDURE [eddsdbo].[QoS_BackupCheckServerMonTest]
	@summaryDayHour DATETIME
	,@currentServerName nvarchar (255)
	,@eddsPerformanceServerName nvarchar(255)
AS
BEGIN	
	declare @backupHistoryDataDays int = -30;	
	declare @lastServerBackup datetime
		
		set @lastServerBackup = (select top(1) isnull(LastServerBackup, DATEADD(dd, @backupHistoryDataDays, @summaryDayHour)) from eddsdbo.Server as s
		inner join eddsdbo.MockServer as ms on s.ServerId = ms.ServerId
			where s.ServerName = @currentServerName and ServerTypeID = 3 and DeletedOn IS NULL and ISNULL(IgnoreServer, 0) = 0
			order by LastServerBackup desc)
	
		INSERT INTO eddsdbo.QoS_BackupHistory
			(DBName, CompletedOn, Duration, BackupType, LoggedDate)
		SELECT [Database], BackupEndDate, DATEDIFF(ss, BackupStartDate, BackupEndDate), BackupType, @summaryDayHour
		FROM eddsdbo.MockBackupSet bs WITH(NOLOCK)
		WHERE BackupEndDate > DATEADD(dd, @backupHistoryDataDays, @summaryDayHour)
			AND BackupEndDate > @lastServerBackup
			AND (
				[Database] = 'EDDS'
				OR [Database] LIKE 'EDDS[0-9]%'
				OR [Database] LIKE 'INV%'
			)
			and Server = @currentServerName

		delete from eddsdbo.QoS_BackupHistory
		where [BackupType] not in ('D','I','L')
			
		declare @newLastServerBackup datetime = (select top(1) CompletedOn from eddsdbo.QoS_BackupHistory order by CompletedOn desc)
		set @newLastServerBackup = isnull(@newLastServerBackup, @lastServerBackup)
		
		update eddsdbo.Server		
		set LastServerBackup = @newLastServerBackup
		where ServerName = @currentServerName and ServerTypeID = 3 and DeletedOn IS NULL and ISNULL(IgnoreServer, 0) = 0
		and ServerID in (select ServerID from eddsdbo.MockServer)	
END