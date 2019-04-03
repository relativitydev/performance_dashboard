USE EDDSPerformance
GO

--If ErrorCountDW exists, check for the schema change and rename it if we need to perform work
IF NOT EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'ErrorCountDW_Back' AND TABLE_SCHEMA = 'EDDSDBO')
AND (SELECT DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'ErrorCountDW' AND TABLE_SCHEMA = 'EDDSDBO' AND COLUMN_NAME = 'MeasureDate') <> 'datetime'
BEGIN
	EXEC sp_rename 'eddsdbo.ErrorCountDW', 'ErrorCountDW_Back';

	IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
		WHERE CONSTRAINT_NAME = 'PK_ErrorCountDW' AND TABLE_SCHEMA = 'eddsdbo' AND TABLE_NAME = 'ErrorCountDW_Back')
	BEGIN
		ALTER TABLE eddsdbo.ErrorCountDW_Back
		DROP CONSTRAINT PK_ErrorCountDW
	END
END

--At this point, if we need to do any work, ErrorCountDW will no longer exist, as it was renamed
IF NOT EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'ErrorCountDW' AND TABLE_SCHEMA = 'EDDSDBO')
BEGIN
	CREATE TABLE [eddsdbo].[ErrorCountDW](
		[ErrorCountDWID] [int] IDENTITY(1,1) NOT NULL,
		[MeasureDate] [datetime] NOT NULL,
		[CaseArtifactID] [int] NOT NULL,
		[ErrorCount] [int] NULL,
		[CreatedOn] [datetime] NOT NULL,
	 CONSTRAINT [PK_ErrorCountDW] PRIMARY KEY CLUSTERED 
	(
		[ErrorCountDWID] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]
END

IF EXISTS(SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'ErrorCountDW_Back' AND TABLE_SCHEMA = 'EDDSDBO')
BEGIN
	WHILE EXISTS (SELECT TOP 1 * FROM eddsdbo.ErrorCountDW_Back)
	BEGIN
		INSERT INTO [eddsdbo].[ErrorCountDW] (MeasureDate, CaseArtifactID, ErrorCount, CreatedOn)
		SELECT TOP 1000000
		  DATEADD(hh, MeasureHour, CAST(MeasureDate as datetime)) MeasureDate,
		  CaseArtifactID,
		  ErrorCount,
		  CreatedOn
		FROM eddsdbo.ErrorCountDW_Back
		
		;WITH CTE AS
		(
		SELECT TOP 1000000 *
		FROM eddsdbo.ErrorcountDW_Back
		)
		DELETE FROM CTE
	END
	
	--No more rows in the backup table, can safely remove
	DROP TABLE eddsdbo.ErrorCountDW_Back;
END