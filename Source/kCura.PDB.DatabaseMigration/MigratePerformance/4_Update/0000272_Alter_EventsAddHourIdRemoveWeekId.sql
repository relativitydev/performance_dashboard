USE [EDDSPerformance];

-- Add new column
IF COL_LENGTH ('eddsdbo.Events' ,'HourId') IS NULL
BEGIN
	ALTER TABLE eddsdbo.[Events]
    ADD [HourId] int null
	
	ALTER TABLE [eddsdbo].[Events]
	WITH CHECK ADD CONSTRAINT [FK_Events_Hours] FOREIGN KEY([HourId])
	REFERENCES [eddsdbo].[Hours] ([ID])
END

--Remove FK
if exists (select 1 from sys.objects where name = 'FK_Events_TrustWeeks' and type='F')
begin
	ALTER TABLE eddsdbo.[Events]
	drop constraint  FK_Events_TrustWeeks
end

--Remove old column
IF COL_LENGTH ('eddsdbo.Events' ,'WeekId') IS NOT NULL
BEGIN

	ALTER TABLE eddsdbo.[Events]
    DROP column [WeekId]
END