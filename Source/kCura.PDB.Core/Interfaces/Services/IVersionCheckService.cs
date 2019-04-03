namespace kCura.PDB.Core.Interfaces.Services
{
	using System;
	using System.Threading.Tasks;

    public interface IVersionCheckService
	{
	    Task<bool> CurrentVersionIsInstalled(Version executingVersion);

	    Task UpdateLatestVersion(Version executingVersion);
	}
}
