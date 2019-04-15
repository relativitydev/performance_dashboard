namespace kCura.PDB.Core.Constants
{
	using System;
	using System.Collections.Generic;
	using kCura.PDB.Core.Interfaces.Queuing;
	using kCura.PDB.Core.Interfaces.Services;

	public class AgentConfiguration
	{
		public static int ConnectionTimeout = 60;
		public static int CommandTimeout = 1000;

		public static IList<Type> DefaultBindingsExclusionList = new[] { typeof(ILogContext), typeof(IEventRunner) };
	}
}
