namespace kCura.PDB.Core.Interfaces.Services
{
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Models;

	public interface IHashConversionService
	{
		void ConvertHashes(ISqlServerRepository sqlRepository, ServerInfo[] servers);
	}
}
