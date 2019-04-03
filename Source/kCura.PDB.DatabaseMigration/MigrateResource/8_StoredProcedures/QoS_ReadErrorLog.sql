USE [{{resourcedbname}}]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_ReadErrorLog]') AND type in (N'P', N'PC'))
	DROP PROCEDURE eddsdbo.QoS_ReadErrorLog

GO

CREATE PROCEDURE eddsdbo.QoS_ReadErrorLog
	@start DATETIME,
	@end DATETIME,
	@searchTerm NVARCHAR(4000)
WITH EXEC AS SELF
AS
BEGIN
	EXEC xp_readerrorlog 0, 1, @searchTerm, NULL, @start, @end
END