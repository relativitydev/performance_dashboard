USE EDDSResource;
GO

IF EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'kIE_BackSummary')
BEGIN
	IF COL_LENGTH('dbo.kIE_BackSummary', 'GapResolved') IS NOT NULL
	BEGIN
		ALTER TABLE dbo.kIE_BackSummary ADD GapResolvedDate DATETIME NULL
	END
END

GO

IF EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'kIE_BackSummary')
BEGIN
	IF COL_LENGTH('dbo.kIE_BackSummary', 'GapResolved') IS NOT NULL
	BEGIN
		EXEC('UPDATE BS
		SET BS.GapResolvedDate = (SELECT TOP 1 LastBackupDate FROM dbo.kIE_BackResults WHERE DBName = BS.DBName AND LastBackupDate > BS.LastBackupDate ORDER BY LastBackupDate ASC)
		FROM dbo.kIE_BackSummary BS
		WHERE BS.GapResolved = 1 AND BS.GapResolvedDate IS NULL')
		
		ALTER TABLE dbo.kIE_BackSummary DROP COLUMN GapResolved		
	END
END

GO

IF EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'kIE_DBCCSummary')
BEGIN
	IF COL_LENGTH('dbo.kIE_DBCCSummary', 'GapResolved') IS NOT NULL
	BEGIN
		ALTER TABLE dbo.kIE_DBCCSummary ADD GapResolvedDate datetime
	END
END

GO

IF EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'kIE_DBCCSummary')
BEGIN
	IF COL_LENGTH('dbo.kIE_DBCCSummary', 'GapResolved') IS NOT NULL
	BEGIN
		EXEC('UPDATE DS
		SET DS.GapResolvedDate = (SELECT TOP 1 LastCleanDBCCDate FROM dbo.kIE_DBCCResults WHERE DBName = DS.DBName AND LastCleanDBCCDate > DS.LastDBCCDate ORDER BY LastCleanDBCCDate ASC)
		FROM dbo.kIE_DBCCSummary DS
		WHERE DS.GapResolved = 1 AND DS.GapResolvedDate IS NULL')
		
		ALTER TABLE dbo.kIE_DBCCSummary DROP COLUMN GapResolved
	END
END