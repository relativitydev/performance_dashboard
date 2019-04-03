namespace kCura.PDB.Service.Integration.Tests.UserExperience
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using Core.Models;
	using kCura.PDB.Core.Enumerations;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Audits;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Models.Audits;
	using Moq;
	using Ninject;

	public static class MockAuditData
	{
		const int totalAudits = 25 * 1000;
		const int totalUsers = 100;

		public static async Task SetupAuditRepository(
			Mock<IWorkspaceAuditService> workspaceAuditService,
			Mock<IViewRepository> viewRepository,
			Mock<IViewCriteriaRepository> viewCriteriaRepository,
			Mock<IWorkspaceAuditServiceFactory> workspaceAuditServiceFactory,
			IKernel kernel,
			Hour hour)
		{
			// Get the database servers and workspaceIds 
			var serverRepo = kernel.Get<IServerRepository>();
			var servers = await serverRepo.ReadAllActiveAsync();
			var databaseServers = servers.Where(s => s.ServerType == ServerType.Database);
			var workspaceIdsTask = await databaseServers.Select(s => serverRepo.ReadServerWorkspaceIdsAsync(s.ServerId)).WhenAllStreamed();
			var workspaceIds = workspaceIdsTask.SelectMany(wids => wids).ToList();

			// Divide up the total number of mock audits per workspace
			var totalWorkspaceAudits = totalAudits / workspaceIds.Count;

			// Create a list of mock user ids
			var users = Enumerable.Range(0, totalUsers)
				.Select(i => i).ToList();

			// Setup all the methods of the workspaceAuditService to return mock data
			workspaceAuditService.Setup(r => r.ReadAuditsAsync(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<IList<AuditActionId>>(), It.IsAny<int>(), It.IsAny<long>()))
				.ReturnsAsync<int, DateTime, DateTime, IList<AuditActionId>, int, long, IWorkspaceAuditService, IList<Audit>>((wid, sd, ed, was, size, start) => Audits(wid, size, start, hour));
			workspaceAuditService.Setup(r => r.ReadTotalAuditExecutionTimeForHourAsync(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<IList<AuditActionId>>()))
				.ReturnsAsync(totalWorkspaceAudits * (2500 / 2));
			workspaceAuditService.Setup(r => r.ReadTotalAuditsForHourAsync(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<IList<AuditActionId>>()))
				.ReturnsAsync(totalWorkspaceAudits);
			workspaceAuditService.Setup(r => r.ReadTotalLongRunningQueriesForHourAsync(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<IList<AuditActionId>>()))
				.ReturnsAsync(totalWorkspaceAudits / 10);
			workspaceAuditService.Setup(r => r.ReadUniqueUsersForHourAuditsAsync(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<IList<AuditActionId>>()))
				.ReturnsAsync(users);

			// Setup search repository methods to return mock data
			viewRepository.Setup(r => r.ReadById(It.IsAny<int>(), It.IsAny<int>()))
				.Returns<int, int>((wid, sid) => AuditSearchTestCase(sid, 2, wid, 750));
			viewCriteriaRepository.Setup(r => r.ReadViewCriteriasForSearch(It.IsAny<int>(), It.Is<int>(i => i % 100 == 0)))
				.Returns<int, int>((wid, sid) => new[] { new ViewCriteria { IsSubSearch = true, Operator = "", Value = $"{sid + 1}" } });
			viewCriteriaRepository.Setup(r => r.ReadViewCriteriasForSearch(It.IsAny<int>(), It.Is<int>(i => i % 100 != 0)))
				.Returns(new[] { new ViewCriteria { IsSubSearch = false, Operator = "like", Value = "%Something really cool%" } });

			// Have the factory return the mocked workspaceAuditService
			workspaceAuditServiceFactory.Setup(f => f.GetAuditService(It.IsAny<int>(), It.IsAny<int>()))
				.ReturnsAsync(workspaceAuditService.Object);

			// Rebind the kernel to return the mocked objects instead
			kernel.Rebind<IWorkspaceAuditService>().ToConstant(workspaceAuditService.Object);
			kernel.Rebind<IViewRepository>().ToConstant(viewRepository.Object);
			kernel.Rebind<IViewCriteriaRepository>().ToConstant(viewCriteriaRepository.Object);
			kernel.Rebind<IWorkspaceAuditServiceFactory>().ToConstant(workspaceAuditServiceFactory.Object);
		}

		private static readonly Func<int, int, long, Hour, IList<Audit>> Audits = (wid, size, pageStart, hour) =>
				Enumerable.Range((int)pageStart, size)
				.Select(i => new Audit
				{
					Action = AuditActionId.DocumentQuery,
					ArtifactID = (9 * totalAudits) - i,
					AuditID = i,
					Details = i % 2 == 0
						? AuditAdHocSearchTextTestCase(i % totalUsers, wid, i % 2500)
						: AuditSearchTextTestCase((9 * totalAudits) - i, i % totalUsers, wid, i % 2500),
					ExecutionTime = i % 2500,
					RequestOrigination = "",
					TimeStamp = hour.HourTimeStamp.AddMilliseconds(i),
					UserID = i % totalUsers,
					WorkspaceId = wid
				})
				.ToList();

		private static readonly Func<int, int, int, int, Search> AuditAdHocSearchTestCase = (sid, uid, wid, et) =>
			new Search { ArtifactId = sid, SearchText = AuditAdHocSearchTextTestCase(uid, wid, et), Name = "Test Search" };


		private static readonly Func<int, int, int, string> AuditAdHocSearchTextTestCase = (uid, wid, et) => $@" <auditElement><QueryText>/* &lt;Comments&gt;
  &lt;ArtifactID /&gt;
  &lt;ArtifactTypeID&gt;10&lt;/ArtifactTypeID&gt;
  &lt;UserID&gt;{uid}&lt;/UserID&gt;
  &lt;WorkspaceID&gt;{wid}&lt;/WorkspaceID&gt;
  &lt;QueryType&gt;IdList&lt;/QueryType&gt;
  &lt;QueryID&gt;52b60df4-9476-4aab-a2ec-55474cd230ec&lt;/QueryID&gt;
  &lt;QuerySource&gt;Ad Hoc Search&lt;/QuerySource&gt;
&lt;/Comments&gt; */

SET NOCOUNT ON 
SELECT TOP 1000
	[Document].[ArtifactID]

FROM
	[Document] (NOLOCK)
WHERE
[Document].[AccessControlListID_D] IN (1)
AND
((NOT ([Document].[BatesBeg] IS NULL OR [Document].[BatesBeg] = N'') 
AND 
[Document].[BatesBeg] IN (N'sdfasdfsadfasdf')) 
AND 
NOT ([Document].[ExtractedText] IS NULL OR [Document].[ExtractedText] = N''))
ORDER BY 
	[Document].[ArtifactID] 


//-------------------
//-- records returned: 0
//-------------------
//</QueryText><Milliseconds>{et}</Milliseconds></auditElement>";

		private static readonly Func<int, int, int, int, Search> AuditSearchTestCase = (sid, uid, wid, et) =>
			new Search { ArtifactId = sid, SearchText = AuditSearchTextTestCase(sid, uid, wid, et), Name = "Test Search" };

		private static readonly Func<int, int, int, int, string> AuditSearchTextTestCase = (sid, uid, wid, et) => $@"<auditElement><QueryText>/* &lt;Comments&gt;
  &lt;ArtifactID&gt;{sid}&lt;/ArtifactID&gt;
  &lt;ArtifactTypeID&gt;10&lt;/ArtifactTypeID&gt;
  &lt;UserID&gt;{uid}&lt;/UserID&gt;
  &lt;WorkspaceID&gt;{wid}&lt;/WorkspaceID&gt;
  &lt;QueryType&gt;IdList&lt;/QueryType&gt;
  &lt;QuerySource&gt;View or Search&lt;/QuerySource&gt;
&lt;/Comments&gt; */

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
</QueryText><Milliseconds>{et}</Milliseconds></auditElement>";
	}
}
