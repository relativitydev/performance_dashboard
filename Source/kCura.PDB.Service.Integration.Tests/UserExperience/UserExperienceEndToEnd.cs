namespace kCura.PDB.Service.Integration.Tests.UserExperience
{
	using System.Threading.Tasks;
	using kCura.PDB.Core.Interfaces.Audits;
	using kCura.PDB.Core.Interfaces.Repositories;
	using Moq;
	using NUnit.Framework;

	[TestFixture, Category("Integration"), Explicit]
	public class UserExperienceEndToEnd : UserExperienceBaseTestFixture
	{
		[SetUp]
		public async Task Setup()
		{
			await SetupMockData();
			base.SetupServices();
			base.SetUpData();
		}

		public async Task<IWorkspaceAuditServiceFactory> SetupMockData()
		{
			var workspaceAuditService = new Mock<IWorkspaceAuditService>();
			var viewRepository = new Mock<IViewRepository>();
			var viewCriteriaRepository = new Mock<IViewCriteriaRepository>();
			var workspaceAuditServiceFactory = new Mock<IWorkspaceAuditServiceFactory>();
			await MockAuditData.SetupAuditRepository(
				workspaceAuditService,
				viewRepository,
				viewCriteriaRepository,
				workspaceAuditServiceFactory,
				Kernel,
				Hour);

			return workspaceAuditServiceFactory.Object;
		}

		[Test]
		public async Task Mock_SearchAuditAnalysis_Success()
		{
			await base.SearchAuditAnalysis_Success();
		}

		[Test]
		public async Task Mock_UserExperienceEndToEnd_Success()
		{
			await base.UserExperienceEndToEnd_Success();
		}
	}
}
