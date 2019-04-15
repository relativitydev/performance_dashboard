namespace kCura.PDB.Agent.Tests
{
	using kCura.PDB.Agent.Bindings;
	using kCura.PDB.Core.Interfaces.Agent;
	using Moq;
	using Ninject;
	using NUnit.Framework;
	using Relativity.API;

	[TestFixture]
	[Category("Unit")]
	[Category("Ignore")]
	[Ignore("Takes too long for TC and is not testing a lot")]
	public class AgentBindingsTests
	{
		[Test]
		public void AgentBindings_Load()
		{
			//Arrange
			var agentService = new Mock<IAgentService>();
			var kernel = new Mock<IKernel>();
			kernel.SetupGet(k => k.Settings).Returns(new Mock<INinjectSettings>().Object);

			//Act
			var bindings = new AgentBindings(agentService.Object);
			bindings.OnLoad(kernel.Object);

			//Asset
			Assert.Pass();
		}
	}
}
