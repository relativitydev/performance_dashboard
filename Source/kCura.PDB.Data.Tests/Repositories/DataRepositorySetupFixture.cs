namespace kCura.PDB.Data.Tests.Repositories
{
	using NUnit.Framework;

	[SetUpFixture]
	public class DataRepositorySetupFixture
	{
		[OneTimeTearDown]
		public void TearDown()
		{
			ConnectionFactorySetup.DeleteExisting();
		}
	}
}
