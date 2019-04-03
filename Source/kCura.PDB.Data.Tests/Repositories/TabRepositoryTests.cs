namespace kCura.PDB.Data.Tests.Repositories
{
	using kCura.PDB.Core.Models;
	using kCura.PDB.Data.Repositories;
	using kCura.PDB.Tests.Common;
	using NUnit.Framework;

	[TestFixture, Category("Integration")]
	public class TabRepositoryTests
	{
		private TabRepository tabRepository;

		[SetUp]
		public void SetUp()
		{
			var connectionFactory = TestUtilities.GetIntegrationConnectionFactory();
			this.tabRepository = new TabRepository(connectionFactory);
		}

		[TearDown]
		public void TearDown()
		{
			
		}

		[Test]
		public void ReadPerformanceDashboardParentTab()
		{
			var result = this.tabRepository.ReadPerformanceDashboardParentTab();

			Assert.That(result, Is.Not.Null);
		}

		[Test]
		public void ReadChildren()
		{
			var parent = this.tabRepository.ReadPerformanceDashboardParentTab();
			var result = this.tabRepository.ReadChildren(parent.ArtifactId);

			Assert.That(result, Is.Not.Null);
			Assert.That(result, Is.Not.Empty);
		}
		
		[Test, Ignore("Test this with the TabServiceIntegrationTests"), Category("Ignore")]
		public void CreateTab()
		{
			//this.tabRepository.CreateTab();
		}

		[Test, Ignore("Test this with the TabServiceIntegrationTests"), Category("Ignore")]
		public void UpdateTab()
		{
			//this.tabRepository.UpdateTab();
		}

		[Test, Explicit("Tool to delete tabs from bad environments")]
		//[TestCase(99999999999)]
		public void DeleteTabRecursively(int parentTabId)
		{
			this.tabRepository.DeleteTabRecursively(new Tab {ArtifactId = parentTabId});
		}

		[Test]
		public void ApplyGroupTabPermissions()
		{
			this.tabRepository.ApplyGroupTabPermissions();
		}
	}
}
