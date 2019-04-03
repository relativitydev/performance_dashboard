namespace kCura.PDB.Core.Interfaces.Repositories
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Models.MetricDataSources;

	public interface IUserExperienceCacheRepository
	{
		Task<IList<UserExperience>> ReadAsync(int serverId, DateTime start, DateTime end);

		Task<UserExperience> CreateAsync(UserExperience userExperience);
	}
}
