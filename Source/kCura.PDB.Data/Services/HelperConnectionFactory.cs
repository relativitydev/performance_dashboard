namespace kCura.PDB.Data.Services
{
	using System;
	using System.Data;
	using System.Data.Common;
	using System.Data.SqlClient;
	using System.Threading.Tasks;
	using Core.Constants;
	using global::Relativity.API;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Data;
	using kCura.PDB.Core.Models;

	public class HelperConnectionFactory : CachedConnectionStringFactory
	{
		public HelperConnectionFactory(IHelper helper)
			: base(new HelperWorkspaceServerProvider(helper))
		{
			this.helper = helper;
			if (this.helper == null) throw new ArgumentNullException(nameof(helper));
		}

		private readonly IHelper helper;

		protected override SqlConnectionStringBuilder GetConnectionString(string server = null, GenericCredentialInfo credentialInfo = null)
		{
			// Build up our string from the default context
			var builder = new SqlConnectionStringBuilder(GenericConnectionFactory.GetDefaultConnectionString());
			if (string.IsNullOrEmpty(server) == false)
				builder.DataSource = server;
			else
			{
				var context = this.helper.GetDBContext(-1);
				builder.DataSource = context.ServerName;
				context.ReleaseConnection();
			}

			builder.ApplicationName = Names.Application.PerformanceDashboard;

			return builder
				.ModifyCreditentals(credentialInfo)
				.AddDefaultTimeout();
		}
	}
}