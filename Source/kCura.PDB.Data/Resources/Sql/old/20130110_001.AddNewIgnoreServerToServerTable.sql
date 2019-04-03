USE EDDSPerformance

SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON

--Add the new IgnoreServer field to the Server table if it doesn't already exist
IF COL_LENGTH('eddsdbo.Server','IgnoreServer') IS NULL
	BEGIN
		ALTER TABLE eddsdbo.Server ADD IgnoreServer bit NULL DEFAULT(0)
	END

GO
	
--Update any existing null entries to zero (false)
UPDATE eddsdbo.Server SET IgnoreServer = 0 WHERE IgnoreServer is null
GO