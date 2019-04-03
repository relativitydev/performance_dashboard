namespace kCura.PDB.Core.Interfaces.Repositories
{
	using System;
	using System.Data.SqlClient;
	using Services;

	public interface IDbRepository
	{
		int? GetWorkspaceServerId(int caseArtifactId);
	}
}
