namespace kCura.PDB.Core.Interfaces.Testing.Repositories
{
	using System.Collections.Generic;
	using System.Threading.Tasks;

	using kCura.PDB.Core.Models.Testing;

	public interface IServerTestDataRepository
    {
        Task CreateAsync(IList<MockServer> testData);

        Task ClearAsync();
    }
}
