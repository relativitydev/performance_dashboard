namespace kCura.PDB.Service.DataGrid.Interfaces
{
	using System.Collections.Generic;
	using kCura.PDB.Core.Models.Audits;
	using global::Relativity.Services.Objects.DataContracts;
	public interface IDataGridResponseAuditMapper
	{
		IList<Audit> ResponseToAudits(QueryResult response, int workspaceId);
	}
}