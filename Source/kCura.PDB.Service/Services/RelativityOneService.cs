namespace kCura.PDB.Service.Services
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;

	public class RelativityOneService : IRelativityOneService
	{
		private IConfigurationRepository configurationRepository;

		public RelativityOneService(IConfigurationRepository configurationRepository)
		{
			this.configurationRepository = configurationRepository;
		}

		public bool IsRelativityOneInstance()
		{
			return bool.Parse(configurationRepository.ReadEddsConfigurationInfo("Relativity.Core", "CloudInstance").FirstOrDefault()?.Value ?? "False");
		}
	}
}
