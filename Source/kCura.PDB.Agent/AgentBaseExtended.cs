namespace kCura.PDB.Agent
{
	using System;
	using System.Collections.Generic;
	using System.Threading;

	using kCura.PDB.Agent.Bindings;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Helpers;
	using kCura.PDB.Core.Interfaces.Agent;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Data;
	using kCura.PDB.DependencyInjection;
	using kCura.PDB.Service.Bindings;
	using kCura.PDB.Service.Services;
	using Ninject;
	using Ninject.Modules;

	using AgentBase = kCura.EDDS.Agents.AgentBase;

	public abstract class AgentBaseExtended : AgentBase, IAgentService
	{
		protected AgentBaseExtended(ICancellationTokenSourceFactory cancellationTokenFactory)
		{
			// DI
			this.cancellationTokenSourceFactory = cancellationTokenFactory;

			// Assembly Resolves
			if (assemblyResolverInitialized == false)
			{
				AssemblyHelper.InitResolves();
				assemblyResolverInitialized = true;
			}
		}

		protected AgentBaseExtended()
		{
			this.cancellationTokenSourceFactory = new CancellationTokenSourceFactory();
			this.OnAgentDisabled += AgentBaseExtended_OnAgentDisabled;
			

			// Assembly Resolves
			if (assemblyResolverInitialized == false)
			{
				AssemblyHelper.InitResolves();
				assemblyResolverInitialized = true;
			}
		}

		private void AgentBaseExtended_OnAgentDisabled()
		{
			this.cancellationTokenSource?.Cancel();
		}
		 	

		private static bool assemblyResolverInitialized;
		private readonly ICancellationTokenSourceFactory cancellationTokenSourceFactory;
		private CancellationTokenSource cancellationTokenSource;

		protected abstract void ExecutePdbAgent(CancellationToken token);

		protected override void ExecuteAgent()
		{
			try
			{
				using (var source = this.cancellationTokenSourceFactory.GetCancellationTokenSource())
				{
					this.cancellationTokenSource = source;
					DataSetup.Setup();
					this.ExecutePdbAgent(cancellationTokenSource.Token);
				}
			}
			catch (Exception ex)
			{
				string merged = $@"{this.Name} Failed.: {ex.ToString()}".Truncate(30000);
				this.RaiseError(merged, string.Empty);
			}
		}

		protected IKernel GetKernel()
		{
			var bindings = new List<INinjectModule> { new AgentBindings(this), new ProductionServiceBindings(), new RelativityBindings(this.Helper) };
#if IntegrationTest
			bindings.Add(new AgentIntegrationTestBindings());
#endif
			return KernelFactory.GetKernel(bindings, AgentConfiguration.DefaultBindingsExclusionList);
		}
	}
}
