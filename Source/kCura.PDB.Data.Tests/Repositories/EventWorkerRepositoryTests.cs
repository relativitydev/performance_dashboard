namespace kCura.PDB.Data.Tests.Repositories
{
	using System.Linq;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Data.Repositories;
	using NUnit.Framework;
	using PDB.Tests.Common;

	[TestFixture]
	[Category("UnitPlatform")]
	[Category("Integration")] // For TeamCity to ignore this test
	public class EventWorkerRepositoryTests
	{
		[OneTimeSetUp]
		public void Setup()
		{
			this.repository = new EventWorkerRepository(ConnectionFactorySetup.ConnectionFactory);
		}

		private EventWorkerRepository repository;

		[Test]
		public async Task Event_Worker_CreateReadAndDelete()
		{
			var id = 1234588;
			var worker = new EventWorker { Id = id, Name = "Delete me. Create from tests", Type = (EventWorkerType)1234 }; // using dummy Type value for testing
			var createResult = await repository.CreateAsync(worker);
			var readAllResult = await repository.ReadAllWorkersAsync();
			await repository.DeleteAsync(createResult);

			// Assert
			Assert.That(createResult, Is.Not.Null);
			Assert.That(createResult.Id, Is.EqualTo(id));
			Assert.That((int)createResult.Type, Is.EqualTo(1234));
			Assert.That(readAllResult, Is.Not.Empty);
			Assert.That(readAllResult.Any(r => r.Id == id), Is.True);
		}
	}
}
