namespace kCura.PDB.Data.Tests.Repositories
{
	using System;
	using kCura.PDB.Data.Repositories;
	using kCura.PDB.Tests.Common;
	using kCura.PDB.Core.Constants;
	using NUnit.Framework;

	[TestFixture]
	[Category("Integration")]
	public class RelativityApplicationRepositoryTests
	{
		private RelativityApplicationRepository relativityApplicationRepository;

		[SetUp]
		public void SetUp()
		{
			this.relativityApplicationRepository = new RelativityApplicationRepository(TestUtilities.GetIntegrationConnectionFactory());
		}

		[Test]
		[TestCase(Guids.Application.DataGridString, "Data Grid")]
		[TestCase(Guids.Application.DataGridForAuditString, "Data Grid for Audit")]
		[TestCase(Guids.Application.PerformanceDashboardString, "Performance Dashboard")]
		public void ApplicationIsInstalledOnEnvironment_Test(string guidStr, string applicationName)
		{
			// Arrange
			var guid = new Guid(guidStr);

			// Act
			var result = this.relativityApplicationRepository.ApplicationIsInstalledOnEnvironment(guid);

			// Assert
			Assert.Pass($"{applicationName} installed: {result}");
		}

		[Test]
		[TestCase(Guids.Application.DataGridString, "Data Grid")]
		[TestCase(Guids.Application.DataGridForAuditString, "Data Grid for Audit")]
		[TestCase(Guids.Application.PerformanceDashboardString, "Performance Dashboard")]
		public void GetApplicationVersion_Test(string s, string applicationName)
		{
			// Arrange
			var workspaceId = Config.WorkSpaceId;
			var guid = new Guid(s);

			// Act
			var result = this.relativityApplicationRepository.GetApplicationVersion(workspaceId, guid);

			// Assert
			Assert.Pass($"{applicationName} installed in workspace {workspaceId}, Version: {result}");
		}
	}
}
