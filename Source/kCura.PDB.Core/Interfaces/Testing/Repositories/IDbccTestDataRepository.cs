namespace kCura.PDB.Core.Interfaces.Testing.Repositories
{
	using System.Collections.Generic;
	using System.Threading.Tasks;

	using kCura.PDB.Core.Models.Testing;

	public interface IDbccTestDataRepository
    {
        Task CreateAsync(IList<MockDbccServerResults> testData);

        Task ClearAsync();
    }
}
