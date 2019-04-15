USE [EDDSPerformance]
GO

/****** Object:  UserDefinedFunction [eddsdbo].[QoS_GetServerHourID]    Script Date: 4/21/2014 10:28:17 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id(N'[eddsdbo].[QoS_DecomposeServerHourID]') AND xtype IN (N'FN', N'IF', N'TF'))
	DROP FUNCTION [eddsdbo].[QoS_DecomposeServerHourID];
GO
CREATE FUNCTION [eddsdbo].[QoS_DecomposeServerHourID]
(
@part varchar(8),
@qosHourID bigint
)
RETURNS int
AS
BEGIN
--Accepts 'hour', 'date','year' and 'serverID' and returns the part
--1234567 + yyyy + ddd + hh
--Function to decompose the summaryDayHour
 --SELECT @hourBottom =  EDDSDBO.QoS_DecomposeServerHourID('hour',@qosHourID)
DECLARE @returnPart int
SET @returnPart = -1
IF @Part = 'hour'
SET @returnPart = REVERSE(SUBSTRING(REVERSE(@qosHourID),1,2))
IF @Part = 'date'   --Julian Date
SET @returnPart = REVERSE(SUBSTRING(REVERSE(@qosHourID),3,3))
IF @Part = 'year'
SET @returnPart = REVERSE(SUBSTRING(REVERSE(@qosHourID),6,4))
IF @Part = 'serverID'
SET @returnPart = REVERSE(SUBSTRING(REVERSE(@qosHourID),10,11))

Return @returnPart 

END


