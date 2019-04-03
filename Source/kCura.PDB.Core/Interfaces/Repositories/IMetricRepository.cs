namespace kCura.PDB.Core.Interfaces.Repositories
{
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Models;

	public interface IMetricRepository
	{
		Task<Metric> CreateAsync(Metric metric);

		Task<Metric> ReadAsync(int id);

		Task UpdateAsync(Metric metric);

		Task DeleteAsync(Metric metric);
	}
}
