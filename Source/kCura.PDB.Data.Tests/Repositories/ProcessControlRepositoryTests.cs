namespace kCura.PDB.Data.Tests.Repositories
{
	using kCura.PDB.Data.Repositories;
	using NUnit.Framework;

	[TestFixture]
	[Category("UnitPlatform")]
	[Category("Integration")] // For TeamCity to ignore this test
	public class ProcessControlRepositoryTests
	{
		[OneTimeSetUp]
		public void Setup()
		{
			this.processControlRepository = new ProcessControlRepository(ConnectionFactorySetup.ConnectionFactory);
		}

		private ProcessControlRepository processControlRepository;

		//TODO FINISH THESE TESTS
	}
}
