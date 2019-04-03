namespace kCura.PDB.Core.Interfaces.Audits
{
	using System.Threading.Tasks;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.MetricDataSources;

	public interface IUserExperienceService
	{
		/// <summary>
		/// Builds the UserExperience Data model for a given server/hour which reflects the server's Sample Eligibility
		/// </summary>
		/// <param name="serverId">serverId to build UX data model for</param>
		/// <param name="hour">Hour to build UX data model for</param>
		/// <returns>UserExperience data model representing Sample Eligibility for the given server/hour</returns>
		Task<UserExperience> BuildUserExperienceModel(int serverId, Hour hour);
	}
}
