namespace kCura.PDB.Data.Tests.Repositories
{
	using System.Linq;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Data.Repositories;
	using NUnit.Framework;
	using PDB.Tests.Common;

	[TestFixture]
	[Category("Integration")]
	public class PoisonWaitRepositoryTests
	{
		[OneTimeSetUp]
		public async Task SetUp()
		{
			connectionFactory = TestUtilities.GetIntegrationConnectionFactory();
			var processControlRepo = new ProcessControlRepository(connectionFactory);

			repo = new PoisonWaitRepository(connectionFactory, processControlRepo);

			var serverRepo = new ServerRepository(connectionFactory);
			server = (await serverRepo.ReadAllActiveAsync()).FirstOrDefault();

			var hourRepo = new HourRepository(connectionFactory);
			hour = await hourRepo.ReadLastAsync();
		}

		private IConnectionFactory connectionFactory;
		private PoisonWaitRepository repo;
		private Server server;
		private Hour hour;

		[Test]
		public async Task ReadIfPoisonWaitsForHourAsync()
		{
			// Act
			var result = await repo.ReadIfPoisonWaitsForHourAsync(this.hour);

			// Assert
			Assert.Pass("result can be valid true or false depending on the data");
		}

		[Test]
		public async Task ReadPoisonWaitsForHourAsync()
		{
			// Act
			var result = await repo.ReadPoisonWaitsForHourAsync(this.hour, this.server.ServerId);

			// Assert
			Assert.Pass("result can be valid true or false depending on the data");
		}
	}
}
