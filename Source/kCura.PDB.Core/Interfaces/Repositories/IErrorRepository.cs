namespace kCura.PDB.Core.Interfaces.Repositories
{
	using System;

	public interface IErrorRepository
	{
		int LogError(Exception ex, string url, int workspaceId, string source);
	}
}
