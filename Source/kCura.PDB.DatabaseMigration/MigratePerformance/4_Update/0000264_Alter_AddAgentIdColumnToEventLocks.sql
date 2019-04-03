USE [EDDSPerformance]

IF COL_LENGTH ('eddsdbo.EventLocks' ,'WorkerId') IS NULL
BEGIN
	delete from eddsdbo.[EventLocks] -- Clean up the event locks table since there should be no agent processing data during DB deployment. Any existing locks would be most like considered orphaned and deleted anyways.
	
	ALTER TABLE eddsdbo.[EventLocks]
    ADD [WorkerId] int not null
	
	ALTER TABLE [eddsdbo].[EventLocks]  WITH CHECK ADD  CONSTRAINT [FK_EventLocks_EventWorkers] FOREIGN KEY([WorkerId])
	REFERENCES [eddsdbo].[EventWorkers] ([Id])
	
	ALTER TABLE [eddsdbo].[EventLocks] CHECK CONSTRAINT [FK_EventLocks_EventWorkers]
END