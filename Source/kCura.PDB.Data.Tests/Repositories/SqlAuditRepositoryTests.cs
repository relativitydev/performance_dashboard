namespace kCura.PDB.Data.Tests.Repositories
{
	using System;
	using System.Linq;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Enumerations;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Data.Repositories;
	using kCura.PDB.Data.Services;
	using kCura.PDB.Service.Services;
	using NUnit.Framework;
	using kCura.PDB.Tests.Common;

	[TestFixture]
	[Category("Integration")]
	public class SqlAuditRepositoryTests
	{
		private SqlAuditRepository sqlAuditRepository;

		[SetUp]
		public void SetUp()
		{
			this.sqlAuditRepository = new SqlAuditRepository(TestUtilities.GetIntegrationConnectionFactory());
		}

		[Test]
		[TestCase(new[] { AuditActionId.DocumentQuery }, 
			TestName = "Document Search")]
		[TestCase(new[] { AuditActionId.View, AuditActionId.Query, AuditActionId.Import, AuditActionId.ReportQuery, AuditActionId.RelativityScriptExecution, AuditActionId.PivotQuery },
			TestName = "Ones with Execution Times")]
		[TestCase(new[] { AuditActionId.Update }, 
			TestName = "RDC Overlay 2")]
		[TestCase(new[] { AuditActionId.UpdateMassEdit, AuditActionId.UpdateMassReplace, AuditActionId.UpdatePropagation },
			TestName = "456")]
		[TestCase(new[] { AuditActionId.Export }, 
			TestName = "Average Execution Time among all")]
		[TestCase(new[] { AuditActionId.UpdateImport }, 
			TestName = "RDC Overlay")]
		public async Task ReadAudits(AuditActionId[] actionsToRead)
		{
			// Arrange
			var startHour = new DateTime(2017, 2, 2, 3, 0, 0).NormilizeToHour();
			var endHour = DateTime.UtcNow.NormilizeToHour();
			var batchSize = 5000;
			var pageStart = 0;

			// Act
			var audits = await this.sqlAuditRepository.ReadAuditsAsync(Config.WorkSpaceId, startHour, endHour, actionsToRead.ToList(), batchSize, pageStart);

			// Assert
			Assert.Pass($"Audits not guaranteed to exist.  Returned {audits.Count} Audits.", audits);
		}

		[Test]
		public async Task ReadTotalAuditExecutionTimeForHour()
		{
			var workspaceId = Config.WorkSpaceId;
			var startHour = new DateTime(2017, 2, 2, 3, 0, 0).NormilizeToHour();
			var endHour = DateTime.UtcNow.NormilizeToHour();
			var actionTypes = AuditConstants.RelevantAuditActionIdsOtherThanUpdate;

			var result = await this.sqlAuditRepository.ReadTotalAuditExecutionTimeForHourAsync(workspaceId, startHour, endHour, actionTypes);

			Assert.Pass($"Audits not guaranteed to exist.  Returned {result} ExecutionTime.");
		}

		[Test]
		public void ReadTotalAuditExecutionTime_Exception()
		{
			var workspaceId = Config.WorkSpaceId;
			var startHour = new DateTime(2017, 2, 2, 3, 0, 0).NormilizeToHour();
			var endHour = DateTime.UtcNow.NormilizeToHour();
			var actionTypes = AuditConstants.UpdateAuditActionIds;

			Assert.ThrowsAsync<ArgumentException>(
				() => this.sqlAuditRepository.ReadTotalAuditExecutionTimeForHourAsync(workspaceId,
					startHour, endHour, actionTypes));
		}

		[Test]
		public async Task ReadTotalAuditsForHour()
		{
			var workspaceId = Config.WorkSpaceId;
			var startHour = new DateTime(2017, 2, 2, 3, 0, 0).NormilizeToHour();
			var endHour = DateTime.UtcNow.NormilizeToHour();
			var actionTypes = AuditConstants.RelevantAuditActionIds;

			var result = await this.sqlAuditRepository.ReadTotalAuditsForHourAsync(workspaceId, startHour, endHour, actionTypes);

			Assert.Pass($"Audits not guaranteed to exist.  Returned {result} audits.");

		}

		[Test]
		public async Task ReadTotalLongRunningQueriesForHour()
		{
			var workspaceId = Config.WorkSpaceId;
			var startHour = new DateTime(2017, 2, 2, 3, 0, 0).NormilizeToHour();
			var endHour = DateTime.UtcNow.NormilizeToHour();
			var actionTypes = AuditConstants.RelevantAuditActionIdsOtherThanUpdate;
			actionTypes.Remove(AuditActionId.DocumentQuery);

			var result = await this.sqlAuditRepository.ReadTotalLongRunningQueriesForHourAsync(workspaceId, startHour, endHour, actionTypes);

			Assert.Pass($"Audits not guaranteed to exist.  Returned {result} long running queries.");
		}

		[Test]
		public async Task ReadUniqueUsersForHourAuditsAsync()
		{
			var workspaceId = Config.WorkSpaceId;
			var startHour = new DateTime(2017, 2, 2, 3, 0, 0).NormilizeToHour();
			var endHour = DateTime.UtcNow.NormilizeToHour();
			var actionTypes = AuditConstants.RelevantAuditActionIds;

			var result = await this.sqlAuditRepository.ReadUniqueUsersForHourAuditsAsync(workspaceId, startHour, endHour, actionTypes);
			Assert.Pass($"Audits not guaranteed to exist.  Returned {result.Count} unique users ids. {string.Join(", ", result)}");
		}

		[Test]
		public async Task ReadAnyAudits()
		{
			var workspaceId = Config.WorkSpaceId;
			var startHour = new DateTime(2017, 2, 2, 3, 0, 0).NormilizeToHour();
			var endHour = DateTime.UtcNow.NormilizeToHour();
			var actionTypes = AuditConstants.RelevantAuditActionIds;

			var result = await this.sqlAuditRepository.ReadAnyAuditsAsync(workspaceId, startHour, endHour, actionTypes);
			Assert.Pass($"Audits not guaranteed to exist.  Audits exist: {result}");
		}
	}
}
