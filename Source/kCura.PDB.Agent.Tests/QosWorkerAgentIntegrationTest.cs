namespace kCura.PDB.Agent.Tests
{
	using System.Threading;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Models;
	using kCura.PDB.DependencyInjection;
	using kCura.PDB.Service;
	using kCura.PDB.Service.Agent;
	using Moq;
	using Ninject;
	using NUnit.Framework;

	[TestFixture]
	[Category("Integration")]
	public class QosWorkerAgentIntegrationTest
	{
		[Test]
		[Explicit("Explicit till run time issue is fixed")]
		public async Task RunWorkerLogic()
		{
			// start metric event system
			var cancellationToken = new CancellationToken(false);
			var factory = new KernelFactory(new AgentIntegrationModule(), new ServiceBindings());
			using (var kernel = factory.GetKernel(AgentConfiguration.DefaultBindingsExclusionList))
			{
				var agentRepo = new Mock<IAgentRepository>();
				agentRepo.Setup(r => r.ReadAgentEnabled(It.IsAny<int>())).Returns(true);
				kernel.Rebind<IAgentRepository>().ToConstant(agentRepo.Object);
				var managerLogic = kernel.Get<MetricSystemWorkerAgentLogic>();
				await managerLogic.Execute(cancellationToken, new EventWorker { Id = 123456, Name = "Integration test worker", Type = EventWorkerType.Other });
			}

			// Assert
			Assert.Pass();
		}
	}
}
