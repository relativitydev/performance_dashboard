namespace kCura.PDB.Core.Interfaces.Repositories
{
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.RecoverabilityIntegrity;

	public interface IDatabaseGapsRepository
	{
		/// <summary>
		/// Creates database gaps if they don't already exist
		/// </summary>
		/// <param name="gaps">List of database gaps</param>
		/// <returns>Task</returns>
		Task CreateDatabaseGapsAsync<TGap>(IList<TGap> gaps)
			where TGap : Gap;

		Task<TGap> ReadLargestGapsForHourAsync<TGap>(Server server, Hour hour, GapActivityType activityType);
		
		/// <summary>
		/// Reads the gaps that are larger than supplied value.
		/// </summary>
		/// <typeparam name="TGap">Gap type (ie backup/dbcc)</typeparam>
		/// <param name="server">Filters the gaps by server</param>
		/// <param name="hour">Filters the gaps to just this hour</param>
		/// <param name="activityType">Gap activity type (ie backup/dbcc)</param>
		/// <param name="minDuration">Filters the gaps that have durations longer than this value. Duration is in seconds.</param>
		/// <returns>Task of list of gaps that meet the criteria</returns>
		Task<IList<TGap>> ReadGapsLargerThanForHourAsync<TGap>(
			Server server,
			Hour hour,
			GapActivityType activityType,
			int minDuration);

		/// <summary>
		/// Reads the largest gaps for each database for a given server/hour/activityType
		/// </summary>
		/// <typeparam name="TGap">Gap type (ie backup/dbcc)</typeparam>
		/// <param name="server">Filters the gaps by server</param>
		/// <param name="hour">Filters the gaps to just this hour</param>
		/// <param name="activityType">Gap activity type (ie backup/dbcc)</param>
		/// <returns>Task of list of gaps that meet the criteria</returns>
		Task<IList<TGap>> ReadLargestGapsForEachDatabaseAsync<TGap>(Server server, Hour hour, GapActivityType activityType);
	}
}
