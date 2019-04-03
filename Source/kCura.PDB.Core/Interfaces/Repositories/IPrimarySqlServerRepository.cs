namespace kCura.PDB.Core.Interfaces.Repositories
{
    using System.Threading.Tasks;
    using kCura.PDB.Core.Models;

    public interface IPrimarySqlServerRepository : IDbRepository
    {
        Task<ResourceServer> GetPrimarySqlServerAsync();

        ResourceServer GetPrimarySqlServer();
    }
}
