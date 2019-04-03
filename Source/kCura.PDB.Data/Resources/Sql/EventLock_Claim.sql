

INSERT INTO eddsdbo.[EventLocks] (EventTypeId, SourceId, EventId, WorkerId)
	VALUES (@eventTypeId, @sourceId, @eventId, @workerId)

SELECT * FROM eddsdbo.[EventLocks] WHERE Id = @@IDENTITY