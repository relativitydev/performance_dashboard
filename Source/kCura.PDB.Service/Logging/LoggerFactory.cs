namespace kCura.PDB.Service.Logging
{
	using System.Collections.Generic;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Services;
	using kCura.PDB.Service.Agent;
	using Ninject;

	public static class LoggerFactory
	{
		public static ILogger GetLogger(Ninject.Activation.IContext context)
		{
			return new CompositeLogger(new List<ILogger>()
			{
				context.Kernel.Get<AgentLogger>(),
				context.Kernel.Get<DatabaseLogger>()
			});
		}

		public static ILogger GetEventHandlerLogger(Ninject.Activation.IContext context)
		{
			return new CompositeLogger(new List<ILogger>
			{
				context.Kernel.Get<TextLogger>()
			});
		}
	}
}
