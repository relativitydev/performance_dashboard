namespace kCura.PDB.Service.Services
{
	using System.Data.SqlClient;
	using System.Threading.Tasks;
	using global::Relativity.Toggles;
	using global::Relativity.Toggles.Providers;
	using kCura.PDB.Core.Interfaces.Services;

	public class PdbSqlToggleProvider : SqlServerToggleProvider
	{
		public PdbSqlToggleProvider(IConnectionFactory connectionFactory)
			: base(
				() => CreateConnectionFactory(connectionFactory),
				() => CreateAsyncConnectionFactory(connectionFactory))
		{
			this.DefaultMissingFeatureBehavior = MissingFeatureBehavior.Disabled;
		}

		private static async Task<SqlConnection> CreateAsyncConnectionFactory(
			IConnectionFactory connectionFactory)
		{
			var connection = (SqlConnection)connectionFactory.GetEddsConnection();
			await connection.OpenAsync();
			return connection;
		}

		private static SqlConnection CreateConnectionFactory(IConnectionFactory connectionFactory)
		{
			var connection = (SqlConnection)connectionFactory.GetEddsConnection();
			connection.Open();
			return connection;
		}
	}
}
