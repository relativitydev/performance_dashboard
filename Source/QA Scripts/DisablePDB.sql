USE EDDSPerformance;
GO
DECLARE @now datetime = getutcdate();

EXEC eddsdbo.DisableLookingGlass @now