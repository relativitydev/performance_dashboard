namespace kCura.PDB.Data.Tests.Testing
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;

	using kCura.PDB.Core.Models.Testing;
	using kCura.PDB.Data.Testing;
	using kCura.PDB.Data.Tests.Repositories;

	using NUnit.Framework;

	[TestFixture]
	[Category("UnitPlatform")]
	[Category("Integration")]
	public class ServerTestDataRepositoryTests
	{
		private ServerTestDataRepository serverTestDataRepository;

		[SetUp]
		public void Setup()
		{
			this.serverTestDataRepository = new ServerTestDataRepository(ConnectionFactorySetup.ConnectionFactory);
		}

		[Test]
		public async Task CreateAsync()
		{
			var testData = new MockServer
				               {
					               ServerName = "ServerNameTest",
					               ArtifactID = 999,
					               ServerTypeID = 99,
					               CreatedOn = DateTime.UtcNow, // Shouldn't be nullable?
					               DeletedOn = null
				               };
			var testDataList = new List<MockServer> { testData };
			await this.serverTestDataRepository.CreateAsync(testDataList);
			Assert.Pass("No return results");
		}

		[Test]
		public async Task ClearAsync()
		{
			await this.serverTestDataRepository.ClearAsync();
			Assert.Pass("No return results");
		}
	}
}
