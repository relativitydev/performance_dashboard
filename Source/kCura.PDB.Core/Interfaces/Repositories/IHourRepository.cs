namespace kCura.PDB.Core.Interfaces.Repositories
{
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Models;

	public interface IHourRepository
	{
		Task<Hour> CreateAsync(Hour hour);

		IList<Hour> Create(IList<Hour> hours);

		Task<Hour> ReadAsync(int id);

		Task<Hour> ReadHourReadyForScoringAsync(int hourId);

		/// <summary>
		/// Only use for Integration Tests
		/// </summary>
		/// <returns>Next highest hour after min hour in EDDSPerformance.eddsdbo.Hours table</returns>
		Task<Hour> ReadHighestHourAfterMinHour();

		Task<Hour> ReadLastAsync();

		Task<IList<Hour>> ReadBackFillHoursAsync();

		Task<IList<Hour>> ReadPastWeekHoursAsync(Hour endHour);
		Task<IList<Hour>> ReadCompletedBackFillHoursAsync();

		Task UpdateAsync(Hour hour);

		Task DeleteAsync(Hour hour);

		Task ScoreHourWithBuildAndRateSampleAsync(Hour hour, decimal weekIntegrityScore, bool enableLogging);

		Task<Hour> ReadNextHourWithoutRatings();

		Task<bool> ReadAnyIncompleteHoursAsync();

		Task<IList<int>> ReadIncompleteHoursAsync();

		Task<Hour> ReadLatestCompletedHourAsync();
	}
}
