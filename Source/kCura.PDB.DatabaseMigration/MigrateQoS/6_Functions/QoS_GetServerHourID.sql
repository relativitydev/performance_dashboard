USE [EDDSQoS]
GO

/****** Object:  UserDefinedFunction [eddsdbo].[QoS_GetServerHourID]    Script Date: 4/9/2014 11:01:35 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.Routines WHERE SPECIFIC_SCHEMA = 'eddsdbo' AND SPECIFIC_NAME = 'QoS_GetServerHourID' AND ROUTINE_TYPE = 'FUNCTION')
	DROP FUNCTION eddsdbo.QoS_GetServerHourID;
GO

CREATE FUNCTION [eddsdbo].[QoS_GetServerHourID]
(
@ServerArtifactID INT,
@beginDate DATETIME
)
RETURNS BIGINT
AS
BEGIN
RETURN CAST(CAST(@ServerArtifactID as varchar) + CAST(DATEPART(yyyy,@begindate) as varchar) + CAST (RIGHT(N'000' + CONVERT(VARCHAR(3), DATEPART(dy, @begindate)),3) + RIGHT('00' + CONVERT(VARCHAR(2), DATEPART(HH, @begindate)), 2) as varchar)as bigint)
END
GO

