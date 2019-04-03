namespace kCura.PDB.Core.Interfaces.Services
{
	using System.Threading.Tasks;

	public interface ISendScoreAlertsService
	{
		int SendNotifications(int hourId);
	}
}
