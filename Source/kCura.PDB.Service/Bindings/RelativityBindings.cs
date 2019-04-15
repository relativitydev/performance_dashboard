namespace kCura.PDB.Service.Bindings
{
	using global::Relativity.API;
	using global::Relativity.Services.Agent;

	using Ninject.Modules;

	public class RelativityBindings : NinjectModule
	{
		public RelativityBindings(IHelper helper)
		{
			this.helper = helper;
		}

		private readonly IHelper helper;

		public override void Load()
		{
			this.Bind<IHelper>().ToConstant(this.helper);
			this.Bind<IServicesMgr>().ToMethod(c => this.helper.GetServicesManager());
			this.Bind<IAgentManager>()
				.ToMethod(c => this.helper.GetServicesManager().CreateProxy<IAgentManager>(ExecutionIdentity.System));
		}
	}
}
