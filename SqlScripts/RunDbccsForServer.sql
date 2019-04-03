declare @percentOfDatabasesToDbcc decimal(3,2) = 0.75; -- 1.0 = 100% and 0.2 = 20%

DECLARE @name VARCHAR(50), -- database name 
		@databaseIterator int = 0;


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
   if ((@databaseIterator + 1) % (1.0 / (1.0 - @percentOfDatabasesToDbcc)) >= 0.01)
   BEGIN 
		DBCC CHECKDB (@name)
	END

	set @databaseIterator = @databaseIterator + 1
	-- go to the next database
   FETCH NEXT FROM db_cursor INTO @name   
END   

 
CLOSE db_cursor   
DEALLOCATE db_cursor