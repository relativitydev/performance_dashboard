-- requires @numberOfBackups, @backupFrequency, @firstBackupFinishDateHoursAgo, @backupsPath, @percentOfDatabasesToBackup

DECLARE @name VARCHAR(50), -- database name  
		@fileName VARCHAR(256), -- filename for backup  
		@fileDate VARCHAR(20), -- used for file name
		@lastDatabaseBackupDate datetime,
		@currentIteration int = 0,
		@minutesAgo int = 0,
		@databaseIterator int = 0,
		@firstBackupFinishDate datetime = dateadd(hour, -@firstBackupFinishDateHoursAgo, getdate());

SELECT @fileDate = CONVERT(VARCHAR(20),GETDATE(),112) 
DECLARE db_cursor CURSOR READ_ONLY FOR  
SELECT name 
FROM master.dbo.sysdatabases 
WHERE name NOT IN ('master','model','msdb','tempdb', 'EDDSLogging', 'EDDSMetrics')  -- exclude these databases
	and (name = 'EDDS' -- include edds
		OR name LIKE 'EDDS[0-9]%') -- include workspace dbs like EDDS101234

OPEN db_cursor   
FETCH NEXT FROM db_cursor INTO @name   

WHILE @@FETCH_STATUS = 0   
BEGIN   
   print(@name)
   -- checks if the database should be skipped based on the percent of databases to backup
   if (@percentOfDatabasesToBackup >= 1.0) OR ((@databaseIterator + 1) % (1.0 / (1.0 - @percentOfDatabasesToBackup)) >= 1.0)
   BEGIN 
	   -- iterate for each of the number of backups needed to be taken per database
   	   while @currentIteration < @numberOfBackups
	   BEGIN
		   -- gets the file path for the backup
		   SET @fileName = '@backupsPath' + @name + '_' + @fileDate + '_' + convert(nvarchar(10), @currentIteration) + '.BAK'  
		   	  
		   -- take the backup. Uncomment the following if you want full/diff backup
		   BACKUP DATABASE @name TO DISK = @fileName -- <= Full backup
		   --BACKUP DATABASE @name TO DISK = @fileName with DIFFERENTIAL  -- <= differential backup

		   -- get the finish date for the backup we just took. this lets us identify the backup in the update
		   select top(1) @lastDatabaseBackupDate = backup_finish_date
		   from msdb.dbo.backupset
		   where database_name = @name
		   order by backup_finish_date desc

		   -- calculate how much many minutes ago we want to change the backup
		   set @minutesAgo = -(@numberOfBackups * @backupFrequency) - (@backupFrequency * (@currentIteration + 1))

		   update msdb.dbo.backupset
			set	backup_start_date
				 = dateadd(minute, @minutesAgo,
						isnull(
							dateadd(second, datediff(second, backup_finish_date, backup_start_date) ,@firstBackupFinishDate),
							backup_start_date)),
				backup_finish_date = dateadd(minute, @minutesAgo, isnull(@firstBackupFinishDate, backup_finish_date))
			where	database_name = @name
					AND backup_finish_date = @lastDatabaseBackupDate

			-- increment the iterator
			set @currentIteration = @currentIteration + 1
			-- wait for 2 seconds
			WAITFOR DELAY '00:00:02'
		END
		
		-- run DBCC Check for the database
		DBCC CHECKDB (@name) WITH NO_INFOMSGS

		-- update the backups duration
		update msdb.dbo.backupset
		set	backup_start_date = dateadd(minute, -@backupDuration, backup_finish_date)
		where	database_name = @name
				AND backup_finish_date is not null
		
	END

	set @databaseIterator = @databaseIterator + 1
	set @currentIteration = 0
	-- go to the next database
   FETCH NEXT FROM db_cursor INTO @name   
END   

 
CLOSE db_cursor   
DEALLOCATE db_cursor