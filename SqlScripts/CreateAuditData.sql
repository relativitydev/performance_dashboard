USE EDDS1189132

declare @workspace int = 1189132;
declare @backfilldays int = -7;
declare @backfillhours int = @backfilldays * 24;
declare @nowHour datetime = dateadd(hh, datepart(hh, getutcdate()), Convert(DateTime, DATEDIFF(DAY, 0, getutcdate())));
declare @endHour datetime = dateadd(hh, 1, @nowHour)
declare @startHour datetime = dateadd(hh, @backfillhours, @endHour)
declare @usersPerHour int = 3;
declare @suggestedExecutionTime int = null;
declare @auditsPerHour int = 600;
declare @auditActionTypes int = (select count(distinct AuditActionId) from eddsdbo.auditaction 
			where AuditActionID in (1,3,4,5,6,28,29,32,33,34,35,37,47) or AuditActionID in (2,10,11,12))
declare @actualAuditsPerHour int = @auditsPerHour / (@usersPerHour * @auditActionTypes);
declare @searchid int = 1003663; -- AdHoc = 1003663
			-- Non-adhoc: (SELECT top(1) artifactID FROM EDDSDBO.[view] WHERE ArtifactTypeID = 10 and artifactID in (SELECT ArtifactID from EDDSDBO.AuditRecord WHERE [action] = 28 ));

begin transaction createaudits;

--select hours in backfill range
with backfillhours (summaryDayHour)
as
(
	select @endHour as summaryDayHour
	union all
	select dateadd(hh, -1, summaryDayHour) from backfillhours
	where summaryDayHour >= @startHour
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
			isnull(@suggestedExecutionTime, x * @auditsPerHour / @actualAuditsPerHour) [ExecutionTime]
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
	cross apply (select top(@usersPerHour) artifactid from edds.eddsdbo.[User]) as u
)
--insert mock records into auditrecord
insert into eddsdbo.AuditRecord_PrimaryPartition
([ArtifactID], [Action],[UserID], [TimeStamp],[RequestOrigination],[RecordOrigination],[ExecutionTime],[SessionIdentifier],[Details])
select @searchid [ArtifactID], aa.AuditActionID [Action], afh.UserID [UserID], bh.summaryDayHour [TimeStamp],[RequestOrigination],[RecordOrigination],afh.ExecutionTime [ExecutionTime],null [SessionIdentifier],[Details]
from auditsForHourWithUsers as afh
cross apply (select AuditActionId from eddsdbo.auditaction
	where AuditActionID in (1,3,4,5,6,28,29,32,33,34,35,37,47) or AuditActionID in (2,10,11,12)) as aa
cross apply (select dateadd(MINUTE, 1, summaryDayHour) as summaryDayHour from backfillhours) as bh
order by [TimeStamp]
option (maxrecursion 5000);

commit transaction createaudits
--rollback transaction createaudits