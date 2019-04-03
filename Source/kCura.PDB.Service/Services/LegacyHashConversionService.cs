namespace kCura.PDB.Service.Services
{
	using System.Data.SqlClient;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;

	public class LegacyHashConversionService : IHashConversionService
	{
		public void ConvertHashes(ISqlServerRepository sqlRepository, ServerInfo[] servers)
		{
			//If there are legacy hashes in RHScriptsRun (Base64 MD5), we need to convert hashes in EDDSPerformance and EDDSQoS
			if (sqlRepository.IsVersionedWithLegacyHashes())
			{
				sqlRepository.ConvertLegacyRHScriptHashes(Names.Database.EddsPerformance);

				foreach (var server in servers)
				{
					if (sqlRepository.CanConnect(Names.Database.EddsQoS, server.Name))
					{
						sqlRepository.ConvertLegacyRHScriptHashes(Names.Database.EddsQoS, server.Name);
					}
				}
			}
		}
	}
}
