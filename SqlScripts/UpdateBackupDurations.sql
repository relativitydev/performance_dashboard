declare @backupDuration int = 10; -- in minutes

DECLARE @name VARCHAR(50); -- database name  

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
   
	update msdb.dbo.backupset
	set	backup_start_date = dateadd(minute, -@backupDuration, backup_finish_date)
	where	database_name = @name
			AND backup_finish_date is not null

	-- go to the next database
   FETCH NEXT FROM db_cursor INTO @name   
END   

 
CLOSE db_cursor
DEALLOCATE db_cursor


select database_name, backup_start_date, backup_finish_date
from msdb.dbo.backupset
order by backup_finish_date desc
