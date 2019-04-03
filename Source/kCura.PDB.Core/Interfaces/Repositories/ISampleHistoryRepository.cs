namespace kCura.PDB.Core.Interfaces.Repositories
{
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Models;

	public interface ISampleHistoryRepository : IDbRepository
	{
		Task AddToCurrentSampleAsync(SampleHistory sampleHistory);

		Task AddToCurrentSampleAsync(IList<SampleHistory> sampleHistories);

		Task ResetCurrentSampleAsync(int serverId);

		Task<IList<SampleHistory>> ReadCurrentArrivalRateSampleAsync(int serverId);

		Task<IList<SampleHistory>> ReadCurrentConcurrencySampleAsync(int serverId);

		SampleHistoryRange ReadSampleRange();

		Task RemoveHourFromSampleAsync(int hourId);
	}
}
