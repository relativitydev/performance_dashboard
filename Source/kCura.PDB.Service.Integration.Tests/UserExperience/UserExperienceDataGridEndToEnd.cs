namespace kCura.PDB.Service.Integration.Tests.UserExperience
{
	using System;
	using System.Diagnostics;
	using System.Linq;
	using System.Threading.Tasks;
	using kCura.PDB.Data.Repositories;
	using kCura.PDB.Data.Services;
	using kCura.PDB.Service.Services;
	using Moq;
	using Ninject;
	using NUnit.Framework;

	[TestFixture, Category("Integration"), Explicit]
	public class UserExperienceDataGridEndToEnd : UserExperienceBaseTestFixture
	{
		[SetUp]
		public void Setup()
		{
			// Get the initial data?
			this.SetUpAuditRepository();
			base.SetupServices();
			base.SetUpData();
		}

		private void SetUpAuditRepository()
		{
			DataGridEndToEnd.SetUpDataGridOnlyWorkspaceAuditServiceFactory(this.Kernel);
			/*
			var dataGridWorkspaceAuditService = DataGridEndToEnd.SetUpWorkspaceAuditService();

			var workspaceAuditServiceFactoryMock = new Mock<IWorkspaceAuditServiceFactory>();
			workspaceAuditServiceFactoryMock.Setup(
					m => m.GetAuditService(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
				.ReturnsAsync(dataGridWorkspaceAuditService);
			Kernel.Rebind<IWorkspaceAuditServiceFactory>().ToConstant(workspaceAuditServiceFactoryMock.Object);
			//*/
		}

		[Test]
		public async Task DataGrid_SearchAuditAnalysis_Success()
		{
			await base.SearchAuditAnalysis_Success();
		}

		[Test]
		public async Task DataGrid_UserExperienceEndToEnd_Success()
		{
			await base.UserExperienceEndToEnd_Success();
		}

	}
}
