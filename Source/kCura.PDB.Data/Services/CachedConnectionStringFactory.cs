namespace kCura.PDB.Data.Services
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;
	using System.Linq;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Data;
	using kCura.PDB.Core.Models;

	public abstract class CachedConnectionStringFactory : GenericConnectionFactory
	{
		private const int expiresOnSeconds = 60 * 5; // five minutes
		private static readonly IList<CachedConnectionString> cachedConnectionStrings = new List<CachedConnectionString>();

		public CachedConnectionStringFactory(IWorkspaceServerProvider workspaceServerProvider) : base(workspaceServerProvider)
		{
		}

		protected abstract SqlConnectionStringBuilder GetConnectionString(string server = null, GenericCredentialInfo credentialInfo = null);

		protected override IDbConnection GetServerConnection(string server = null, GenericCredentialInfo credentialInfo = null) =>
			this.GetDatabaseConnection(null, server, credentialInfo);

		protected override IDbConnection GetDatabaseConnection(string initialCatalog, string server = null, GenericCredentialInfo credentialInfo = null)
		{
			lock (cachedConnectionStrings)
			{
				// Check the cache for a compatible connection
				var cached = cachedConnectionStrings.FirstOrDefault(
					c => c.Database == initialCatalog
						&& c.Server == server
						&& c.UserName == credentialInfo?.UserName
						&& c.Password == credentialInfo?.Password
						&& c.UseWindowsAuthentication == credentialInfo?.UseWindowsAuthentication);

				// If the result is expired then remove it
				if (cached != null && cached.ExpiresOn < DateTime.UtcNow)
				{
					cachedConnectionStrings.Remove(cached);
				}

				// if the result doesn't exist or is expired
				if (cached == null || cached.ExpiresOn < DateTime.UtcNow)
				{
					cached = CreateCachedConnectionString(initialCatalog, server, credentialInfo);
					cachedConnectionStrings.Add(cached);
				}

				// return the connection
				return cached.ConnectionString.ToDbConnection();
			}
		}

		private CachedConnectionString CreateCachedConnectionString(string database = null, string server = null, GenericCredentialInfo credentialInfo = null)
		{
			var connectionStringBuilder = this.GetConnectionString(server, credentialInfo);

			if (string.IsNullOrEmpty(database) && connectionStringBuilder.ContainsKey("Initial Catalog"))
			{
				connectionStringBuilder.Remove("Initial Catalog");
			}

			if (!string.IsNullOrEmpty(database))
			{
				connectionStringBuilder.InitialCatalog = database;
			}

			return new CachedConnectionString
			{
				Database = database,
				Server = server,
				UserName = credentialInfo?.UserName,
				Password = credentialInfo?.Password,
				UseWindowsAuthentication = credentialInfo?.UseWindowsAuthentication,
				ConnectionString = connectionStringBuilder.ToString(),
				ExpiresOn = DateTime.UtcNow.AddSeconds(expiresOnSeconds)
			};
		}

		private class CachedConnectionString
		{
			public string Database { get; set; }

			public string Server { get; set; }

			public string UserName { get; set; }

			public string Password { get; set; }

			public bool? UseWindowsAuthentication { get; set; }

			public string ConnectionString { get; set; }

			public DateTime ExpiresOn { get; set; }
		}
	}
}
