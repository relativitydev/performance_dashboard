namespace kCura.PDB.Service.Integration.Tests.RecoverabilityIntegrity
{
	using System.Threading.Tasks;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.DependencyInjection;
	using kCura.PDB.Service.RecoverabilityIntegrity;
	using kCura.PDB.Tests.Common;
	using Ninject;
	using Ninject.Modules;
	using NUnit.Framework;

	[TestFixture]
	[Category("Integration")]
	public class ServerRecoverabilityProcessorIntTests
	{
		[Test]
		public async Task ServerRecoverabilityProcessorInt_ProcessRecoverabilityForServer()
		{
			// Arrange
			using (var kernel = KernelFactory.GetKernel(new TestBindings()))
			{
				var serverRecoverabilityProcessor = kernel.Get<ServerRecoverabilityProcessor>();
				
				// Act
				await serverRecoverabilityProcessor.ProcessRecoverabilityForServer(2280);

				// Assert
				Assert.Pass("No return result or mocks to verify");
			}
		}

		private class TestBindings : NinjectModule
		{
			public override void Load()
			{
				this.Bind<IConnectionFactory>().ToMethod(c => TestUtilities.GetIntegrationConnectionFactory());
				this.Bind<ILogger>().ToMethod(c => TestUtilities.GetMockLogger().Object);
			}
		}
	}
}
