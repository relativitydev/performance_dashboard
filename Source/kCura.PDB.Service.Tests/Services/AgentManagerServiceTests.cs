namespace kCura.PDB.Service.Tests.Services
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using global::Relativity.Services;
	using global::Relativity.Services.Agent;
	using global::Relativity.Services.ResourceServer;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Service.Agent;
	using Moq;
	using NUnit.Framework;
	
	[TestFixture]
	[Category("Unit")]
	//[Explicit("Runs too long for TeamCity")]
	public class AgentManagerServiceTests
	{
		private Mock<IAgentManager> agentManagerMock;
		private Mock<IAgentRepository> agentRepositoryMock;
		private Mock<ILogger> loggerMock;
		private AgentManagerService agentManagerService;

		[SetUp]
		public void Setup()
		{
			this.agentManagerMock = new Mock<IAgentManager>(); //this.Mocks.Create<IAgentManager>();
			this.agentRepositoryMock = new Mock<IAgentRepository>(); // this.Mocks.Create<IAgentRepository>();
			this.loggerMock = new Mock<ILogger>();
			this.agentManagerService = new AgentManagerService(this.agentManagerMock.Object, this.agentRepositoryMock.Object, this.loggerMock.Object);
		}

		[Test]
        [Category("Explicit")]
        [Explicit("Runs too long for TeamCity")]
		public async Task StartPerformanceDashboardAgentsAsync_Success()
		{
			// Arrange
			var agentArtifactId1 = 1;
			var agentArtifactId2 = 2;
			var agents = new List<int> { agentArtifactId1, agentArtifactId2 };
			this.agentRepositoryMock.Setup(m => m.ReadAgentsAsync(Guids.Agent.CurrentAgentGuids)).ReturnsAsync(agents);

			var enabledQueryAgentsResult =
				new AgentQueryResultSet
				{
					Success = true,
					Results =
						new List<Result<Agent>>
						{
							new Result<Agent> {Success = true, Artifact = new Agent {ArtifactID = agentArtifactId1, Enabled = true}},
							new Result<Agent> {Success = true, Artifact = new Agent {ArtifactID = agentArtifactId2, Enabled = true}}
						}
				};

			var disabledQueryAgentsResult = new AgentQueryResultSet
			{
				Success = true,
				Results =
					new List<Result<Agent>>
					{
						new Result<Agent> {Success = true, Artifact = new Agent {ArtifactID = agentArtifactId1, Enabled = false}},
						new Result<Agent> {Success = true, Artifact = new Agent {ArtifactID = agentArtifactId2, Enabled = false}}
					}
			};
			this.agentManagerMock.SetupSequence(m => m.QueryAsync(It.Is<Query>(q => q.Condition == new WholeNumberCondition(
																				"ArtifactID",
																				NumericConditionEnum.In,
																				agents.ToList()).ToQueryString())))
				.ReturnsAsync(disabledQueryAgentsResult) // First read the disabled agents
				.ReturnsAsync(enabledQueryAgentsResult) // After update, read the successfully on agents
				;
			this.agentManagerMock
				.Setup(m => m.UpdateSingleAsync(It.Is<Agent>(a => a.Enabled == true)))
				.Returns(Task.Delay(1));

			// Act
			await this.agentManagerService.StartPerformanceDashboardAgentsAsync();

			// Assert
			this.agentManagerMock.VerifyAll();
			this.agentRepositoryMock.VerifyAll();
			this.agentManagerMock.Verify(m => m.UpdateSingleAsync(It.Is<Agent>(a => a.Enabled == true)),
				Times.Exactly(agents.Count));
		}

		[Test]
        [Category("Explicit")]
        [Explicit("Runs too long for TeamCity")]
		public async Task StopPerformanceDashboardAgentsAsync_Success()
		{
			// Arrange
			var agentArtifactId1 = 1;
			var agentArtifactId2 = 2;
			var enabledAgents = new List<int> { agentArtifactId1, agentArtifactId2 };
			this.agentRepositoryMock.Setup(m => m.ReadAllEnabledAgentsAsync()).ReturnsAsync(enabledAgents);

			var enabledQueryAgentsResult =
				new AgentQueryResultSet
				{
					Success = true,
					Results =
						new List<Result<Agent>>
						{
							new Result<Agent> {Success = true, Artifact = new Agent {ArtifactID = agentArtifactId1, Enabled = true}},
							new Result<Agent> {Success = true, Artifact = new Agent {ArtifactID = agentArtifactId2, Enabled = true}}
						}
				};

			var disabledQueryAgentsResult = new AgentQueryResultSet
			{
				Success = true,
				Results =
					new List<Result<Agent>>
					{
						new Result<Agent> {Success = true, Artifact = new Agent {ArtifactID = agentArtifactId1, Enabled = false}},
						new Result<Agent> {Success = true, Artifact = new Agent {ArtifactID = agentArtifactId2, Enabled = false}}
					}
			};
			this.agentManagerMock.SetupSequence(m => m.QueryAsync(It.Is<Query>(q => q.Condition == new WholeNumberCondition(
																				"ArtifactID",
																				NumericConditionEnum.In,
																				enabledAgents.ToList()).ToQueryString())))
				.ReturnsAsync(enabledQueryAgentsResult) // First read the enabled agents
				.ReturnsAsync(disabledQueryAgentsResult) // After update, read the successfully stopped agents
				;
			this.agentManagerMock
				.Setup(m => m.UpdateSingleAsync(It.Is<Agent>(a => a.Enabled == false)))
				.Returns(Task.Delay(1));

			// Act
			var result = await this.agentManagerService.StopPerformanceDashboardAgentsAsync();

			// Assert
			this.agentManagerMock.VerifyAll();
			this.agentRepositoryMock.VerifyAll();
			this.agentManagerMock.Verify(m => m.UpdateSingleAsync(It.Is<Agent>(a => a.Enabled == false)),
				Times.Exactly(enabledAgents.Count));
		}

		[Test]
		public async Task CreatePerformanceDashboardAgents_FreshSuccess()
		{
			// Arrange
			var machineName = "TestServer";
			var agentServer = new ResourceServer { Name = machineName, ArtifactID = 123 };
			var servers = new List<ResourceServer> { agentServer };
			this.agentManagerMock.Setup(m => m.GetAgentServersAsync()).ReturnsAsync(servers);

			this.agentRepositoryMock.Setup(m =>
				m.ReadAgentWithTypeExists(It.Is<Guid>(g => Guids.Agent.CurrentAgentGuids.Contains(g)))).ReturnsAsync(false);

			var agentTypeId = 1234;
			var agentTypeRefs =
				Guids.Agent.CurrentAgentGuids.Select(cg => new AgentTypeRef(new List<Guid> { cg }) { ArtifactID = agentTypeId }).ToList();
			this.agentManagerMock.Setup(m => m.GetAgentTypesAsync()).ReturnsAsync(agentTypeRefs);

			this.agentManagerMock.Setup(m =>
				m.CreateSingleAsync(It.Is<Agent>(a => a.Server.ArtifactID == agentServer.ArtifactID))).ReturnsAsync(1);

			// Act
			var result = await this.agentManagerService.CreatePerformanceDashboardAgents(machineName);

			// Assert
			Assert.That(result.Count, Is.EqualTo(Guids.Agent.CurrentAgentGuids.Count));
			this.agentManagerMock.VerifyAll();
			this.agentRepositoryMock.VerifyAll();
		}
	}
}
