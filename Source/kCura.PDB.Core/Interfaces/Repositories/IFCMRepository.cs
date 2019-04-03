namespace kCura.PDB.Core.Interfaces.Repositories
{
	using System.Threading.Tasks;

	public interface IFcmRepository
	{
		Task ValidatePreBuildAndRateSample(int hourId, bool enableLogging);

		Task ApplySecondaryHashes();
	}
}
