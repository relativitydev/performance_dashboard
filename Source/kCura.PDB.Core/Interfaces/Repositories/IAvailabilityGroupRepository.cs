namespace kCura.PDB.Core.Interfaces.Repositories
{
	public interface IAvailabilityGroupRepository : IDbRepository
	{
		bool AvailabilityGroupsEnabled();

		bool AvailabilityGroupsSprocesExist();

		string ReadAvailabilityGroupName();

		bool RemoveFromAvailabilityGroup(string databaseName);
	}
}
