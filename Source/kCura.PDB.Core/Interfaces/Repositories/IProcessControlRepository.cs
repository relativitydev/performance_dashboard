namespace kCura.PDB.Core.Interfaces.Repositories
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Enumerations;
	using Models;

	public interface IProcessControlRepository : IDbRepository
	{
		Task<ProcessControl> ReadByIdAsync(ProcessControlId processControlId);

		ProcessControl ReadById(ProcessControlId processControlId);

		Task UpdateAsync(ProcessControl processControl);

		void Update(ProcessControl processControl);

		Task<bool> HasRunSuccessfully(ProcessControlId processControlId, DateTime timeThreshold);

		Task<IList<ProcessControl>> ReadAllAsync();

		IList<ProcessControl> ReadAll();

		void SetProcessFrequency(ProcessControlId process, int frequency);
	}
}
