namespace kCura.PDB.Core.Interfaces.Services
{
	using kCura.PDB.Core.Models;

	public interface IPDBNotificationService
	{
		PDBNotification GetNext();
	}
}
