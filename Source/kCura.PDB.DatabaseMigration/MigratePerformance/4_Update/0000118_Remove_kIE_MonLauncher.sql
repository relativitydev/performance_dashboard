USE EDDSResource
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[kIE_BackupAndDBCCCheckMonLauncher]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[kIE_BackupAndDBCCCheckMonLauncher]