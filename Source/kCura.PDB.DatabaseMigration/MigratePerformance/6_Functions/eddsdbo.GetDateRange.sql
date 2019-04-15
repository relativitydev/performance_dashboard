USE [EDDSPerformance]
GO

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'eddsdbo.GetDateRange')) DROP FUNCTION eddsdbo.GetDateRange
GO

CREATE function [eddsdbo].[GetDateRange]
(@StartDate datetime,@EndDate datetime)				
RETURNS @DateRangeTable Table (DateRange datetime)			
as
Begin
	declare @blEqual bit =0	
	declare @HourDifference int = 0	
	if (@StartDate = @EndDate)
	begin				
		set  @EndDate = DateAdd(S,-1,@EndDate) +1 ;        
		set  @blEqual = 1;
	end
	      
	while (@StartDate <= @EndDate)
	begin                
		insert into @DateRangeTable select(@StartDate)	    
		if (@blEqual = 1)
		begin
			set @StartDate =  DateAdd(HH,1,@StartDate);
		end
		else
		begin
			set @StartDate = @StartDate + 1;
		end
	end	
	return 
End
 