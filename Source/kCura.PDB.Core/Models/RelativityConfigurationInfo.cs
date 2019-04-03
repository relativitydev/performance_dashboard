namespace kCura.PDB.Core.Models
{
	using System;
	using System.Linq;

	public class RelativityConfigurationInfo
	{
		public string Section { get; set; }

		public string Name { get; set; }

		public string Value { get; set; }

		public string MachineName { get; set; }

		public static string GetValueByMachineName(string machineName, string key, RelativityConfigurationInfo[] rcis, bool avoidException = false)
		{
			var cfgs = from RelativityConfigurationInfo rci in rcis
					   where string.Compare(rci.Name, key, true) == 0
					   select rci;

			if (!cfgs.Any())
			{
				if (avoidException)
				{
					return "NoValue_ForKey";
				}

				throw new Exception();
			}

			if (cfgs.Count() == 1)
			{
				return cfgs.Single().Value;
			}

			var cfgs2 = from RelativityConfigurationInfo rci in cfgs
						where string.Compare(rci.MachineName, machineName, true) == 0
						select rci;

			if (!cfgs2.Any())
			{
				if (avoidException)
				{
					return "NoValue_ForKeyForMachine";
				}

				throw new Exception();
			}

			if (cfgs2.Count() == 1)
			{
				return cfgs2.Single().Value;
			}

			return cfgs2.First().Value;
		}
	}
}
