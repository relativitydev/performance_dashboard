namespace kCura.PDB.Agent.Tests
{
	using System.Threading;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.DependencyInjection;
	using kCura.PDB.Service.Agent;
	using Moq;
	using Ninject;
	using NUnit.Framework;

	[TestFixture]
	public class MetricSystemManagerAgentLogicIntegrationTests
	{
		[SetUp]
		public void Setup()
		{

		}

		[Test]
		[Category("Integration")]
		[Explicit("Explicit till run time issue is fixed")]
		public async Task MetricSystemManagerAgentLogic_StartManager_Integration()
		{
			// Arrange
			var cancellationToken = new CancellationToken(false);
			var factory = new KernelFactory(new AgentIntegrationModule());
			using (var kernel = factory.GetKernel(AgentConfiguration.DefaultBindingsExclusionList))
			{
				var agentRepo = new Mock<IAgentRepository>();
				agentRepo.Setup(r => r.ReadAgentEnabled(It.IsAny<int>())).Returns(true);
				kernel.Rebind<IAgentRepository>().ToConstant(agentRepo.Object);

				var logic = kernel.Get<MetricSystemManagerAgentLogic>();

				// Act
				await logic.Execute(cancellationToken);
			}
			// Assert
			Assert.Pass();
		}
	}
}
