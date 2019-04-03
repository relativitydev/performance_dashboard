
-- This query groups all the events matching the event type and status filters. Then it filters the groups where there are status ids other than @status
-- An example use case is you want to query the next pending singleton events that do not have any actively running.
-- In this case, @eventTypes = single event types, @status = pending status, and @negativeStatuses = the active statuses (ie. in progress)

SELECT min([ID])
  FROM [eddsdbo].[Events] with(nolock)
  where SourceTypeID in @eventTypes and (StatusID = @status or StatusID in @negativeStatuses)
  group by SourceTypeID
  having min(StatusID) = @status and max(StatusID) = @status