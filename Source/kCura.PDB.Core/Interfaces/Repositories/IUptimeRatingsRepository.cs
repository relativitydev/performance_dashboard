namespace kCura.PDB.Core.Interfaces.Repositories
{
	using System;
	using System.Threading.Tasks;

	public interface IUptimeRatingsRepository
	{
		Task Create(decimal hoursDown, DateTime summaryDayHour, bool isWebDowntime, bool affectedByMaintenanceWindow);

		Task UpdateWeeklyScores();

		Task UpdateQuartlyScores(DateTime summaryDayHour);
	}
}
