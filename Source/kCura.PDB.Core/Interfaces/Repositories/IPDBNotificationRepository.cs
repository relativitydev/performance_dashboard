namespace kCura.PDB.Core.Interfaces.Repositories
{
	using System.Data;

	public interface IPDBNotificationRepository : IDbRepository
	{
		DataTable GetFailingProcessControls();

		DataTable GetAgentsAlert();

		int? GetSecondsSinceLastAgentHistoryRecord();
	}
}
