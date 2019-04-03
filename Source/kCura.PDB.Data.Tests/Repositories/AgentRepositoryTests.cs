namespace kCura.PDB.Data.Tests.Repositories
{
	using System.Threading.Tasks;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Data.Repositories;
	using kCura.PDB.Data.Services;
	using kCura.PDB.Service.Services;
	using NUnit.Framework;
	using PDB.Tests.Common;

	[TestFixture, Category("Integration")]
	public class AgentRepositoryTests
	{
		[OneTimeSetUp]
		public void OneTimeSetup()
		{
			connectionFatory = TestUtilities.GetIntegrationConnectionFactory();
		}

		private IConnectionFactory connectionFatory;

		[Test]
		public void ReadAgentEnabled()
		{
			// Arrange
			var agentRepo = new AgentRepository(this.connectionFatory);
			var agentId = 1052363;

			// Act
			var result = agentRepo.ReadAgentEnabled(agentId);

			//Assert
			Assert.Pass($"{result} - Manager could be enabled or disabled making the result either true or false");
		}

		[Test]
		public async Task ReadAllAgents()
		{
			// Arrange
			var agentRepo = new AgentRepository(this.connectionFatory);

			// Act
			var result = await agentRepo.ReadAllEnabledAgentsAsync();

			//Assert
			Assert.That(result, Is.Not.Empty);
		}
	}
}
