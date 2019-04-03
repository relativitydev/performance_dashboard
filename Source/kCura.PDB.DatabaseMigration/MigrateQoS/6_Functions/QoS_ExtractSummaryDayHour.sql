USE [EDDSQoS]
GO

/****** Object:  UserDefinedFunction [eddsdbo].[QoS_GetServerHourID]    Script Date: 4/21/2014 10:28:17 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id(N'[eddsdbo].[QoS_ExtractSummaryDayHour]') AND xtype IN (N'FN', N'IF', N'TF'))
	DROP FUNCTION [eddsdbo].[QoS_ExtractSummaryDayHour];
GO
CREATE FUNCTION [eddsdbo].[QoS_ExtractSummaryDayHour]
(
@qosHourID bigint
)
RETURNS datetime
AS
BEGIN
--Given a QoSHourID, returns the SummaryDayHour used to generate it
DECLARE @summaryDayHour datetime
SET @summaryDayHour = DATEADD(hh, CAST(REVERSE(SUBSTRING(REVERSE(@qosHourID),1,2)) as int), CONVERT(datetime, CAST(CAST(REVERSE(SUBSTRING(REVERSE(@qosHourID),6,4)) as datetime) as int) + CAST(REVERSE(SUBSTRING(REVERSE(@qosHourID),3,3)) as int) - 1))

Return @summaryDayHour 

END


