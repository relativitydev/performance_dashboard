namespace kCura.PDB.Core.Interfaces.Repositories
{
	using System;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Models;

	public interface ILegacyRecoverabilityIntegrityRepository
	{
		Task<Decimal?> ReadWeekRecoverabilityIntegrityScore(Hour hour);
	}
}
