namespace kCura.PDB.Data.Repositories
{
	using System.Data;
	using System.Linq;
	using Dapper;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;

	public class AvailabilityGroupRepository : BaseDbRepository, IAvailabilityGroupRepository
	{
		public AvailabilityGroupRepository(IConnectionFactory connectionFactory) : base(connectionFactory)
		{
		}

		public bool AvailabilityGroupsEnabled()
		{
			using (var connection = this.connectionFactory.GetPdbResourceConnection())
			{
				return connection.Query<bool?>(Properties.Resources.AvailabilityGroup_Aoag_Enabled).FirstOrDefault() ?? false;
			}
		}

		public bool AvailabilityGroupsSprocesExist()
		{
			using (var connection = this.connectionFactory.GetMasterConnection())
			{
				return connection.Query<bool?>(string.Format(Properties.Resources.AvailabilityGroup_SprocsExist, Names.Database.PdbResource)).FirstOrDefault() ?? false;
			}
		}

		public string ReadAvailabilityGroupName()
		{
			using (var connection = this.connectionFactory.GetPdbResourceConnection())
			{
				return connection.Query<string>("eddsdbo.ReadAvailabilityGroupName", commandType: CommandType.StoredProcedure).FirstOrDefault();
			}
		}

		public bool RemoveFromAvailabilityGroup(string databaseName)
		{
			if (!this.AvailabilityGroupsSprocesExist())
				return false;

			if (!this.AvailabilityGroupsEnabled())
				return false;

			var availabilityGroup = this.ReadAvailabilityGroupName();

			using (var connection = this.connectionFactory.GetPdbResourceConnection())
			{
				//Verify database is within the availability group
				var databaseJoinedToGroup =
					connection.Query<bool>("eddsdbo.DatabaseJoinedToGroup", new { databaseName, availabilityGroup }, commandType: CommandType.StoredProcedure)
					.FirstOrDefault();

				if (databaseJoinedToGroup)
				{
					connection.Execute("eddsdbo.RemoveDatabaseFromAvailabilityGroup", new { databaseName, availabilityGroup }, commandType: CommandType.StoredProcedure);
				}
			}

			return true;
		}
	}
}
