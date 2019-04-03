-- Expected Params
--declare @workspace int = 1189132;
--declare @endHour datetime = dateadd(hh, 1, @nowHour)
--declare @startHour datetime = dateadd(hh, @backfillhours, @endHour)
--declare @usersPerHour int = 3; -- Suggested users per hour (provided that many exist)
--declare @suggestedExecutionTime int = null; -- overrides all execution times with this value
--declare @minExecutionTime int; -- min range for execution times
--declare @maxExecutionTime int; -- max range for execution times
--declare @auditsPerHour int = 600;
--declare @primaryServer nvarchar(255)

-- Normalize the given datetimes
declare @startHourNormalized datetime = dateadd(hh, datepart(hh, getutcdate()), Convert(DateTime, DATEDIFF(DAY, 0, @startHour)))
declare @endHourNormalized datetime = dateadd(hh, datepart(hh, getutcdate()), Convert(DateTime, DATEDIFF(DAY, 0, @endHour)))

-- Initialize executionTime range if needed
-- min
-- max
-- value = min + (percentage * (max - min))
SET @minExecutionTime = isnull(@minExecutionTime, 100) -- Set default value if null
SET @maxExecutionTime = isnull(@maxExecutionTime, 10000) -- Set default value if null
SET @maxExecutionTime = CASE WHEN @minExecutionTime > @maxExecutionTime THEN @minExecutionTime ELSE @maxExecutionTime END; -- If min > max, set max to min

-- Init the auditActionTypes to work against
declare @auditActionType table(Id int not null)
INSERT @auditActionType(Id) VALUES (1),(3),(4),(5),(6),(28),(29),(32),(33),(34),(35),(37),(47),(2),(10),(11),(12)
declare @auditActionTypesCount int = (select count(distinct AuditActionId) from eddsdbo.auditaction a
			inner join @auditActionType t on a.AuditActionID = t.Id)

declare @actualAuditsPerHour int = @auditsPerHour / (@usersPerHour * @auditActionTypesCount); -- About how many search audits there'll be
declare @searchid int = 1003663; -- AdHoc = 1003663
			-- Non-adhoc: (SELECT top(1) artifactID FROM EDDSDBO.[view] WHERE ArtifactTypeID = 10 and artifactID in (SELECT ArtifactID from EDDSDBO.AuditRecord WHERE [action] = 28 ));
-- make temp table
create table #users (artifactid int not null)
-- call out to primary with dynamic sql
declare @sqlStatement nvarchar(max) = 'INSERT INTO #users select top(@usersPerHour) artifactid from ' + QUOTENAME(@primaryServerName) + '.edds.eddsdbo.[User]'
EXEC sp_executesql @sqlStatement, N'@usersPerHour int', @usersPerHour = @usersPerHour

BEGIN TRANSACTION createaudits;

--select hours in backfill range
with backfillhours (summaryDayHour)
as
(
	select @endHourNormalized as summaryDayHour
	union all
	select dateadd(hh, -1, summaryDayHour) from backfillhours
	where summaryDayHour >= @startHourNormalized
	and not exists
		(select *
		from eddsdbo.AuditRecord_PrimaryPartition
		where dateadd(hh, -1, summaryDayHour) < [TimeStamp] and dateadd(hh, -2, [summaryDayHour]) > TimeStamp)
),
-- select out base audit records
auditsForHour (x,[RequestOrigination],[RecordOrigination],[ExecutionTime])
as
(
	select top(1) 1 as x, [RequestOrigination],[RecordOrigination],isnull(@suggestedExecutionTime, [ExecutionTime]) [ExecutionTime]
		from eddsdbo.AuditRecord_PrimaryPartition as arpp
	union all
	select  x+1 as x,
			afh.RequestOrigination [RequestOrigination],
			afh.RecordOrigination [RecordOrigination],
			isnull(@suggestedExecutionTime, 
				@minExecutionTime + (x * @actualAuditsPerHour * (@maxExecutionTime - @minExecutionTime))) [ExecutionTime]
			from auditsForHour as afh
			where x+1 <= @actualAuditsPerHour
),
-- cross apply users so we can select final details with user information
auditsForHourWithUsers (x,[RequestOrigination],[RecordOrigination],[ExecutionTime],[UserID], [Details])
as
(
	select x, [RequestOrigination],[RecordOrigination],[ExecutionTime], u.ArtifactID [UserID],
	N'<auditElement><QueryText>/* &lt;Comments&gt;
	  &lt;ArtifactID&gt;'+convert(nvarchar(10),isnull(@searchid,1))+'&lt;/ArtifactID&gt;
	  &lt;ArtifactTypeID&gt;10&lt;/ArtifactTypeID&gt;
	  &lt;UserID&gt;'+convert(nvarchar(10),u.ArtifactID)+'&lt;/UserID&gt;
	  &lt;WorkspaceID&gt;'+convert(nvarchar(10),@workspace)+'&lt;/WorkspaceID&gt;
	  &lt;QueryType&gt;IdList&lt;/QueryType&gt;
	  &lt;QuerySource&gt;View or Search&lt;/QuerySource&gt;
	&lt;/Comments&gt; */
	'+(case x % 3 when 0 then '' when 1 then '&#xB;' when 2 then char(11) end)+'
	SET NOCOUNT ON 
	SELECT TOP 1000
		[Document].[ArtifactID]

	FROM
		[Document] (NOLOCK)
	WHERE
	[Document].[AccessControlListID_D] IN (1)
	ORDER BY 
		[Document].[ArtifactID] 


	-------------------
	-- records returned: 0
	-------------------
	</QueryText><Milliseconds>'+convert(nvarchar(12), isnull(afh.ExecutionTime,1))+'</Milliseconds></auditElement>' details
	from auditsForHour as afh
	cross apply (select top(@usersPerHour) artifactid from #users) as u
)
--insert mock records into auditrecord
insert into eddsdbo.AuditRecord_PrimaryPartition
([ArtifactID], [Action],[UserID], [TimeStamp],[RequestOrigination],[RecordOrigination],[ExecutionTime],[SessionIdentifier],[Details])
select @searchid [ArtifactID], aa.AuditActionID [Action], afh.UserID [UserID], bh.summaryDayHour [TimeStamp],[RequestOrigination],[RecordOrigination],afh.ExecutionTime [ExecutionTime],null [SessionIdentifier],[Details]
from auditsForHourWithUsers as afh
cross apply (select AuditActionId from eddsdbo.auditaction a
	inner join @auditActionType t on a.AuditActionID = t.Id) as aa
cross apply (select dateadd(MINUTE, 1, summaryDayHour) as summaryDayHour from backfillhours) as bh
order by [TimeStamp]
option (maxrecursion 5000);

commit transaction createaudits
--rollback transaction createaudits

DROP TABLE #users