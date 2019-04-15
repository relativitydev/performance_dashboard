USE [{{resourcedbname}}]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[QoS_CycleErrorLog]') AND type in (N'P', N'PC'))
	DROP PROCEDURE eddsdbo.QoS_CycleErrorLog

GO

CREATE PROCEDURE eddsdbo.QoS_CycleErrorLog
WITH EXEC AS SELF
AS
BEGIN
	EXEC sp_cycle_errorlog;
END