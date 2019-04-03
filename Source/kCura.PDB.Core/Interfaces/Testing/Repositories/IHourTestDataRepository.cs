namespace kCura.PDB.Core.Interfaces.Testing.Repositories
{
	using System.Collections.Generic;
	using System.Threading.Tasks;

	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.Testing;

	public interface IHourTestDataRepository
    {
        Task CreateAsync(IList<MockHour> testData);

        Task ClearAsync();

        Task<IList<Hour>> ReadHoursAsync();
    }
}
