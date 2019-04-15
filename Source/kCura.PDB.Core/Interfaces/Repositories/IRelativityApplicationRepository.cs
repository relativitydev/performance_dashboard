namespace kCura.PDB.Core.Interfaces.Repositories
{
	using System;

	public interface IRelativityApplicationRepository
	{
		bool ApplicationIsInstalledOnEnvironment(Guid applicationGuid);

		string GetApplicationVersion(int workspaceId, Guid applicationGuid);
	}
}
