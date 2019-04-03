namespace kCura.PDB.Service.Tests.Queuing
{
	using Core.Interfaces.Services;
	using Data.Services;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Data.Repositories;
	using Moq;
	using Ninject;
	using NUnit.Framework;
	using PDB.Tests.Common;
	using Service.Queuing;
	using Service.Services;

	[TestFixture, Category("Integration")]
	public class QueuingConfigurationTests
	{
		[SetUp]
		public void Setup()
		{
			connectionFactory = TestUtilities.GetIntegrationConnectionFactory();
			kernel = new Mock<IKernel>();
			configurationRepository = new ConfigurationRepository(connectionFactory);
		}

		private IConnectionFactory connectionFactory;
		private Mock<IKernel> kernel;
		private IConfigurationRepository configurationRepository;

		[Test]
		public void ConfigureSystem()
		{
			//Act
			var config = new QueuingConfiguration(connectionFactory, kernel.Object, configurationRepository);
			config.ConfigureSystem();

			//Assert
			Assert.Pass();
		}

	}
}
