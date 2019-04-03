USE [EDDSQoS]
GO

/****** Object:  UserDefinedFunction [eddsdbo].[GetEnvCheckRecommendationText]    Script Date: 4/9/2014 11:01:35 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.Routines WHERE SPECIFIC_SCHEMA = 'eddsdbo' AND SPECIFIC_NAME = 'GetEnvCheckRecommendationText' AND ROUTINE_TYPE = 'FUNCTION')
	DROP FUNCTION eddsdbo.GetEnvCheckRecommendationText;
GO


CREATE FUNCTION [eddsdbo].GetEnvCheckRecommendationText 
(
	@recommendationsDefaultID uniqueidentifier,
	@currentValue varchar(200) = null,
	@kieValue varchar(200) = null,
	@other1 varchar(200) = null,
	@other2 varchar(200) = null,
	@status varchar(50) = null
)
RETURNS varchar(max)
AS
BEGIN
	DECLARE @result varchar(max)

	select @result = [Recommendation]
	from eddsdbo.EnvironmentCheckRecommendationsDefaults where ID = @recommendationsDefaultID

	if(@currentValue is not null)
	begin
		set @result = Replace(@result,'<CURRENTVALUE>', @currentValue)
	end

	if(@kieValue is not null)
	begin
		set @result = Replace(@result,'<KIEVALUE>', @kieValue)
	end
	
	if(@other1 is not null)
	begin
		set @result = Replace(@result,'<OTHER1>', @other1)
	end

	if(@other2 is not null)
	begin
		set @result = Replace(@result,'<OTHER2>', @other2)
	end

	if(@other2 is not null)
	begin
		set @result = Replace(@result,'<OTHER2>', @other2)
	end

	if(@result = '<DEFAULT>' and @status = 'Good')
	begin
		set @result = 'This is the default value.  No recommendations can be made without further environmental analysis.'
	end
	
	if(@result = '<DEFAULT>' and @status = 'Not Default')
	begin
		set @result = 'This is not the default value, but no action is necessarily required.'
	end

	RETURN @result

END
GO







