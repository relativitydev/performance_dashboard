namespace kCura.PDB.Core.Interfaces.Repositories
{
	using System;
	using System.Threading.Tasks;

	public interface IPdbVersionRepository
	{
		Task<Version> GetLatestVersionAsync();

		Task SetLatestVersionAsync(Version version);

		Task InitializeIfNotExists();
	}
}
