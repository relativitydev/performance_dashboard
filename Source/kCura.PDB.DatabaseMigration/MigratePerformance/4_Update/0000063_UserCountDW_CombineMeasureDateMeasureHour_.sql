USE EDDSPerformance
GO

--If UserCountDW exists, check for the schema change and rename it if we need to perform work
IF NOT EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'UserCountDW_Back' AND TABLE_SCHEMA = 'EDDSDBO')
AND (SELECT DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'UserCountDW' AND TABLE_SCHEMA = 'EDDSDBO' AND COLUMN_NAME = 'MeasureDate') <> 'datetime'
BEGIN
	EXEC sp_rename 'eddsdbo.UserCountDW', 'UserCountDW_Back';

	IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
		WHERE CONSTRAINT_NAME = 'PK_UserCountDW' AND TABLE_SCHEMA = 'eddsdbo' AND TABLE_NAME = 'UserCountDW_Back')
	BEGIN
		ALTER TABLE eddsdbo.UserCountDW_Back
		DROP CONSTRAINT PK_UserCountDW
	END
END

--At this point, if we need to do any work, UserCountDW will no longer exist, as it was renamed
IF NOT EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'UserCountDW' AND TABLE_SCHEMA = 'EDDSDBO')
BEGIN
	CREATE TABLE [eddsdbo].[UserCountDW](
		[UserCountDWID] [int] IDENTITY(1,1) NOT NULL,
		[MeasureDate] [datetime] NOT NULL,
		[CaseArtifactID] [int] NOT NULL,
		[UserCount] [int] NULL,
		[CreatedOn] [datetime] NOT NULL,
	 CONSTRAINT [PK_UserCountDW] PRIMARY KEY CLUSTERED 
	(
		[UserCountDWID] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]
END

IF EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'UserCountDW_Back' AND TABLE_SCHEMA = 'EDDSDBO')
BEGIN
	WHILE EXISTS (SELECT TOP 1 * FROM eddsdbo.UserCountDW_Back)
	BEGIN
		INSERT INTO [eddsdbo].[UserCountDW] (MeasureDate, CaseArtifactID, UserCount, CreatedOn)
		SELECT TOP 1000000
		  DATEADD(hh, MeasureHour, CAST(MeasureDate as datetime)) MeasureDate,
		  CaseArtifactID,
		  UserCount,
		  CreatedOn
		FROM eddsdbo.UserCountDW_Back
		
		;WITH CTE AS
		(
		SELECT TOP 1000000 *
		FROM eddsdbo.UsercountDW_Back
		)
		DELETE FROM CTE
	END
	
	--No more rows in the backup table, can safely remove
	DROP TABLE eddsdbo.UserCountDW_Back;
END