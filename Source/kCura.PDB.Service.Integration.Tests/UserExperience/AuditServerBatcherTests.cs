namespace kCura.PDB.Service.Integration.Tests.UserExperience
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using kCura.AuditUI2.Services.ExternalAuditLog;
	using kCura.PDB.Core.Interfaces.Audits;
	using kCura.PDB.Core.Interfaces.Workspace;
	using kCura.PDB.Service.Audits;
	using kCura.PDB.Service.DataGrid.Interfaces;
	using kCura.PDB.Tests.Common;
	using Moq;
	using Ninject;
	using NUnit.Framework;

	[TestFixture]
	[Category("Integration")]
	[Explicit("Uses hard-coded variables")]
	public class AuditServerBatcherTests
    {
        [Test]
		public async Task CreateServerBatches()
		{
			var metricDataId = 8643;

			var kernel = IntegrationSetupFixture.CreateNewKernel(true);
			var helper = TestUtilities.GetMockHelper(Config.RelativityServiceUrl, Config.RelativityRestUrl, Config.RSAPIUsername, Config.RSAPIPassword);
			kernel.Rebind<global::Relativity.API.IHelper>().ToConstant(helper.Object);

			var mockServiceFactory = new Mock<IWorkspaceAuditServiceFactory>();
			mockServiceFactory.Setup(m => m.GetAuditService(It.IsAny<int>(), It.IsAny<int>()))
				.ReturnsAsync(kernel.Get<IDataGridWorkspaceAuditService>());
			kernel.Rebind<IWorkspaceAuditServiceFactory>().ToConstant(mockServiceFactory.Object);

			var mockFactory = new Mock<IAuditLogObjectManagerFactory>();
			mockFactory.Setup(m => m.GetManager()).Returns(() => new ApiClientHelper().GetKeplerServiceReference<IExternalAuditLogObjectManager>().Value);
			kernel.Rebind<IAuditLogObjectManagerFactory>().ToConstant(mockFactory.Object);

			var actualWorkspaceService = kernel.Get<IWorkspaceService>();
			var mockWorkspaceService = new Mock<IWorkspaceService>();
			mockWorkspaceService
				.Setup(x => x.ReadAvailableWorkspaceIdsAsync(It.IsAny<int>()))
				.Returns<int>(x => Task.FromResult<IList<int>>(
					actualWorkspaceService.ReadAvailableWorkspaceIdsAsync(x).Result
					.Intersect(new[] { Config.WorkSpaceId })
					.ToList()));
			kernel.Rebind<IWorkspaceService>().ToConstant(mockWorkspaceService.Object);

			var serverBatcher = kernel.Get<AuditServerBatcher>();

			var batchIds = await serverBatcher.CreateServerBatches(metricDataId);

			Assert.That(batchIds, Is.Not.Empty);
		}
    }
}