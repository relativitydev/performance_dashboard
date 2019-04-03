namespace kCura.PDB.Core.Interfaces.Testing.Repositories
{
	using System.Collections.Generic;
	using System.Threading.Tasks;

	using kCura.PDB.Core.Models.Testing;

	public interface IBackupTestDataRepository
    {
        Task CreateAsync(IList<MockBackupSet> backups);

        Task ClearAsync();
    }
}
