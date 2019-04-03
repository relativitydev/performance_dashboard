USE EDDSResource
GO

CREATE PROCEDURE dbo.SimulateBackups
	@backupSuite INT = 0
AS
BEGIN

	DECLARE @dbId INT = 0,
		@dbMax INT = -1,
		@dbName nvarchar(255),
		@SQL nvarchar(max);

	CREATE TABLE #databases (
		DatabaseId INT IDENTITY(1,1),
		PRIMARY KEY(DatabaseId),
		Name NVARCHAR(255) NULL
	)

	INSERT INTO #databases (Name)
	SELECT name
	FROM sys.databases
	WHERE name = 'EDDS'
		OR name LIKE 'EDDS[0-9]%'
		OR name LIKE 'INV%'

	SELECT
		@dbId = MIN(DatabaseId),
		@dbMax = MAX(DatabaseId)
	FROM #databases

	WHILE (@dbId <= @dbMax)
	BEGIN
		SELECT @dbName = Name
		FROM #databases
		WHERE DatabaseId = @dbId;

		SET @SQL = CASE
			WHEN @backupSuite = 0 THEN N'BACKUP DATABASE ' + @dbName + ' TO DISK = ''NUL'''
			WHEN @backupSuite = 1 THEN N'BACKUP DATABASE ' + @dbName + ' TO DISK = ''NUL'' WITH DIFFERENTIAL'
			WHEN @backupSuite = 2 THEN N'BACKUP LOG ' + @dbName + ' TO DISK = ''NUL'''
			ELSE ''
		END;

		PRINT @SQL

		EXEC sp_executesql @SQL

		SET @dbId = ISNULL((
			SELECT MIN(DatabaseId)
			FROM #databases
			WHERE DatabaseId > @dbId
		), @dbMax + 1);
	END

END