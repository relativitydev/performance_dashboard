namespace kCura.PDB.Service.Tests.Services
{
	using System;
	using System.Linq;
	using System.Linq.Expressions;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Service.Services;
	using Moq;
	using NUnit.Framework;

	[TestFixture, Category("Unit")]
	public class TabServiceTests
	{
		[SetUp]
		public void Setup()
		{
			this.mockRepository = new MockRepository(MockBehavior.Strict);
			this.tabRepository = this.mockRepository.Create<ITabRepository>();
			this.relativityOneService = this.mockRepository.Create<IRelativityOneService>();
			this.tabService = new TabService(this.tabRepository.Object, this.relativityOneService.Object);
		}

		[TearDown]
		public void Teardown()
		{
			//Assert
			this.mockRepository.VerifyAll();
		}

		private MockRepository mockRepository;
		private Mock<ITabRepository> tabRepository;
		private Mock<IRelativityOneService> relativityOneService;
		private TabService tabService;

		[TestCase(true)]
		[TestCase(false)]
		public void CreateApplicationTabs_NewInstall(bool isRelativityOne)
		{
			// Arrange
			var parentTabId = 123456;
			var createdParentTab = Tab.PerformanceDashboard;
			createdParentTab.ArtifactId = parentTabId;

			this.tabRepository.Setup(r => r.ReadPerformanceDashboardParentTab()).Throws<InvalidOperationException>();
			this.tabRepository.Setup(r => r.ReadChildren(parentTabId)).Returns(new Tab[] {});

			this.relativityOneService.Setup(s => s.IsRelativityOneInstance()).Returns(isRelativityOne);

			this.tabRepository.Setup(
				r => r.CreateTab(Tab.PerformanceDashboard))
				.Returns(parentTabId);

			this.tabRepository.Setup(
				r => r.CreateTab(It.Is<Tab>(GetChildTabExpression(parentTabId, isRelativityOne)))).Returns(555);

			this.tabRepository.Setup(r => r.ApplyGroupTabPermissions());

			// Act
			this.tabService.CreateApplicationTabs();
		}

		[TestCase(true)]
		[TestCase(false)]
		public void CreateApplicationTabs_Upgrade(bool isRelativityOne)
		{
			// Arrange
			var parentTabId = 123456;
			var startTabId = parentTabId;
			var createdParentTab = Tab.PerformanceDashboard;
			createdParentTab.ArtifactId = parentTabId;
			var childTabs = Tab.AllChildTabs.Select(t =>
			{
				t.ArtifactId = ++startTabId;
				t.ParentArtifactId = parentTabId;
				return t;
			}).ToList();

			this.tabRepository.Setup(r => r.ReadPerformanceDashboardParentTab()).Returns(createdParentTab);
			this.tabRepository.Setup(r => r.ReadChildren(parentTabId)).Returns(childTabs);

			this.relativityOneService.Setup(s => s.IsRelativityOneInstance()).Returns(isRelativityOne);

			this.tabRepository.Setup(
				r => r.UpdateTab(It.Is<Tab>(GetChildTabExpression(parentTabId, isRelativityOne)))).Returns(555);

			if (isRelativityOne)
			{
				this.tabRepository.Setup(r => r.DeleteTabRecursively(It.Is<Tab>(t => t.Name == Tab.EnvironmentCheck.Name)));
			}

			this.tabRepository.Setup(r => r.ApplyGroupTabPermissions());

			// Act
			this.tabService.CreateApplicationTabs();
		}

		private static Expression<Func<Tab, bool>> GetChildTabExpression(int parentTabId, bool isRelativityOne)
		{
			return t => 
				t.Name != Names.Tab.PerformanceDashboard 
					&& t.ParentArtifactId == parentTabId
					&& !(isRelativityOne && t.Name == Names.Tab.EnvironmentCheck);
		}
	}
}