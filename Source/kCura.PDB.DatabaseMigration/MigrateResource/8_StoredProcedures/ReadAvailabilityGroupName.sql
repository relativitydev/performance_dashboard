USE [{{resourcedbname}}]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eddsdbo].[ReadAvailabilityGroupName]') AND type in (N'P', N'PC'))
	DROP PROCEDURE eddsdbo.ReadAvailabilityGroupName

GO

CREATE PROCEDURE [eddsdbo].[ReadAvailabilityGroupName]
WITH EXECUTE AS SELF AS
BEGIN
	SELECT TOP 1 name FROM sys.availability_groups
END