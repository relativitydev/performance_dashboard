namespace kCura.PDB.Service.Integration.Tests.Services
{
	using kCura.PDB.Data.Repositories;
	using kCura.PDB.Service.Services;
	using kCura.PDB.Tests.Common;
	using NUnit.Framework;

	[TestFixture, Category("Integration")]
	public class TabServiceIntegrationTests
	{
		private TabService tabService;

		[SetUp]
		public void SetUp()
		{
			var connectionFactory = TestUtilities.GetIntegrationConnectionFactory();
			this.tabService = new TabService(new TabRepository(connectionFactory), new RelativityOneService(new ConfigurationRepository(connectionFactory)));
		}

		[Test, Explicit("Destructive")]
		public void CreateApplicationTabs()
		{
			this.tabService.CreateApplicationTabs();

			Assert.Pass();
		}

		[Test, Explicit("Destructive")]
		public void DeleteApplicationTabs()
		{
			this.tabService.DeleteApplicationTabs();

			Assert.Pass();
		}

	}
}
