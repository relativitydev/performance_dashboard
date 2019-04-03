namespace kCura.PDB.Core.Interfaces.Testing.Repositories
{
	using System.Collections.Generic;
	using System.Threading.Tasks;

	using kCura.PDB.Core.Models.Testing;

	public interface IDatabasesCheckedTestDataRepository
    {
        Task CreateAsync(IList<MockDatabaseChecked> testData);

        Task ClearAsync();
    }
}
