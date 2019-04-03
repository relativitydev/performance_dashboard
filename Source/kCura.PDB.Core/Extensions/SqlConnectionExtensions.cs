namespace kCura.PDB.Core.Extensions
{
	using System;
	using System.Data;
	using System.Data.Common;
	using System.Data.SqlClient;
	using System.Threading;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Models;

	public static class SqlConnectionExtensions
	{
		public static SqlConnectionStringBuilder ModifyCreditentals(this SqlConnectionStringBuilder connBuilder, GenericCredentialInfo credentialInfo)
		{
			if (credentialInfo != null)
			{
				if (credentialInfo.UseWindowsAuthentication)
				{
					connBuilder.Remove("User ID");
					connBuilder.Remove("Password");
					connBuilder.IntegratedSecurity = true;
				}
				else
				{
					connBuilder.UserID = credentialInfo.UserName;
					connBuilder.Password = credentialInfo.Password;
				}
			}

			return connBuilder;
		}

		public static SqlConnectionStringBuilder AddDefaultTimeout(this SqlConnectionStringBuilder connBuilder)
		{
			connBuilder.ConnectTimeout = Defaults.Database.ConnectionTimeout;

			return connBuilder;
		}

		public static IDbConnection ToDbConnection(this SqlConnectionStringBuilder builder)
		{
			return new SqlConnection(builder.ToString());
		}

		public static async Task<IDbConnection> OpenAsync(this IDbConnection connection, string initialDatabase)
		{
			return await OpenAsync(connection, initialDatabase, CancellationToken.None);
		}

		public static async Task<IDbConnection> OpenAsync(this IDbConnection connection, string initialDatabase, CancellationToken cancellationToken)
		{
			var dbConnection = connection as DbConnection;
			if (dbConnection != null)
			{
				var isClosed = dbConnection.State == ConnectionState.Closed;
				if (isClosed)
				{
					await dbConnection.OpenAsync(cancellationToken).ConfigureAwait(false);
				}

				dbConnection.ChangeDatabase(initialDatabase);

				return connection;
			}

			throw new InvalidOperationException($"Async operations require use of a DbConnection and cannot be null");
		}

		public static IDbConnection Open(this IDbConnection connection, string intialDatabase)
		{
			if (connection != null)
			{
				var isClosed = connection.State == ConnectionState.Closed;
				if (isClosed)
				{
					connection.Open();
				}

				connection.ChangeDatabase(intialDatabase);

				return connection;
			}

			throw new InvalidOperationException($"Connection cannot be null");
		}


		public static IDbConnection ToDbConnection(this string connectionString)
		{
			return new SqlConnection(connectionString);
		}
	}
}
