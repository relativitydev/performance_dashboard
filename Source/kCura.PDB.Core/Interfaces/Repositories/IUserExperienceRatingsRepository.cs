namespace kCura.PDB.Core.Interfaces.Repositories
{
	using System;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Models.Audits;

	public interface IUserExperienceRatingsRepository
	{
		Task CreateAsync(int serverArtifactId, decimal arrivalRateUXScore, decimal concurrencyUXScore, int hourId);
	}
}
