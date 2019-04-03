

-- params
--declare @hourId int = (select top 1 h.Id from eddsdbo.[Hours] as h
--	left join eddsdbo.Reports_SearchAudits as sa on sa.HourId = h.id
--	where sa.ID is not null
-- order by HourTimeStamp)
--declare @serverId int = (select top 1 ServerID from eddsdbo.[Server] where DeletedOn is null and (IgnoreServer is null or IgnoreServer = 0))

DELETE FROM [eddsdbo].[Reports_WorkspaceSearchAudits]
      WHERE HourId = @hourId and ServerId = @serverId

DELETE FROM [eddsdbo].[Reports_SearchAudits]
      WHERE HourId = @hourId and ServerId = @serverId




