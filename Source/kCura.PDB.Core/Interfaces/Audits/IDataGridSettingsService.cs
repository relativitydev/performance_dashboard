namespace kCura.PDB.Core.Interfaces.Audits
{
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Enumerations;

	public interface IDataGridSettingsService
	{
		Task<IList<int>> ReadActionChoiceIds(int workspaceId, IList<AuditActionId> actions);
	}
}
