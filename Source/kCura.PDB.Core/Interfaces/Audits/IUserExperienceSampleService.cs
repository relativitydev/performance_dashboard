namespace kCura.PDB.Core.Interfaces.Audits
{
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.Audits;
	using kCura.PDB.Core.Models.MetricDataSources;

	public interface IUserExperienceSampleService
	{
		Task<PastWeekEligibleSample> CalculateSample(int serverId, int hourId);

		Task UpdateCurrentSample(PastWeekEligibleSample sample);
	}
}