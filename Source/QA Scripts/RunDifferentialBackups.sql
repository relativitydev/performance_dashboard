DECLARE @Baksql VARCHAR(8000)
DECLARE @BackupFolder VARCHAR(100)
DECLARE @BackupFile VARCHAR(100)
DECLARE @BAK_PATH VARCHAR(4000)
DEclare @BackupDate varchar(100)

-- Setting value of  backup date and folder of the backup
SET @BackupDate =  REPLACE(REPLACE(REPLACE(CONVERT(VARCHAR,GETDATE(),120),'-',''),':',''),' ','_') -- 20110517_182551
SET @BackupFolder = '\\TT-MIL-9-0\BCPPath\'
SET @Baksql = ''

-- Declaring cursor
DECLARE c_bakup CURSOR FAST_FORWARD READ_ONLY FOR 
SELECT NAME FROM SYS.DATABASES
WHERE state_desc = 'ONLINE' -- Consider databases which are online
AND database_id > 4  -- Exluding system databases

OPEN c_bakup
FETCH NEXT FROM c_bakup INTO @BackupFile

WHILE @@FETCH_STATUS = 0
BEGIN
SET @BAK_PATH = @BackupFolder + @BackupFile
-- Creating dynamic script for every databases backup
SET @Baksql = 'BACKUP DATABASE ['+@BackupFile+'] TO DISK = '''+@BAK_PATH+'_DiffBackup_'+@BackupDate+'.bak'' WITH DIFFERENTIAL;'
-- Executing dynamic query
PRINT (@Baksql)
EXEC(@Baksql)
-- Opening and fetching next values from sursor
FETCH NEXT FROM c_bakup INTO @BackupFile
END

-- Closing and Deallocating cursor
CLOSE c_bakup
DEALLOCATE c_bakup