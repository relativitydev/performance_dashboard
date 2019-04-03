namespace kCura.PDB.Core.Interfaces.Repositories
{
	using System;
	using System.Collections.Generic;
	using global::kCura.Relativity.Client.DTOs;

	public interface IWorkspaceRepository
	{
		List<Workspace> ReadAll(List<string> fieldNames = null);
	}
}
