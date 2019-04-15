namespace kCura.PDB.Service.Tests.Queuing
{
	using System;
	using Data.Services;
	using NUnit.Framework;
	using kCura.PDB.Data.Repositories;
	using Ninject;
	using Service.Queuing;
	using Service.Services;

	[TestFixture, Category("Integration")]
	public class QueuingServiceTests
	{
		[SetUp]
		public void Setup()
		{
			var configService = new AppSettingsConfigurationService();
			var connectionFactory = new ConfiguredConnectionFactory(configService);
			var kernel = new StandardKernel();
			var configurationRepository = new ConfigurationRepository(connectionFactory);
			var configuration = new QueuingConfiguration(connectionFactory, kernel, configurationRepository);
			configuration.ConfigureSystem();
		}

		[Test]
		public void QueuingService_Enqueue_Service()
		{
			//Arrange
			var service = new QueuingService();

			//Act
			service.Enqueue<QueuingServiceTestService>(s => s.Run());

			//Assert
			Assert.Pass("No return result");
		}

		public class QueuingServiceTestService
		{
			public void Run()
			{
				Console.WriteLine("QueuingService_Enqueue_Service");
			}
		}
	}
}
