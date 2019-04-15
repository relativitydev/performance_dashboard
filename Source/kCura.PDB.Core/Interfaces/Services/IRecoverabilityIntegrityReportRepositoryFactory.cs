namespace kCura.PDB.Core.Interfaces.Services
{
	using kCura.PDB.Core.Interfaces.Repositories;

	public interface IRecoverabilityIntegrityReportReaderFactory
	{
		IRecoverabilityIntegrityReportReader Get();
	}
}
