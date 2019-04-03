using System.Web;
using kCura.PDD.Web.Filters;

[assembly: Microsoft.Owin.OwinStartup(typeof(kCura.PDD.Web.Startup))]
namespace kCura.PDD.Web
{
	using System;
	using Owin;
	using Hangfire;
	using System.Data.SqlClient;
	using global::Relativity.CustomPages;
	using Hangfire.SqlServer;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Data.Repositories;
	using kCura.PDB.Data.Services;
	using kCura.PDD.Web.Services;

	public class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			// Map Dashboard to the `http://<your-app>/hangfire` URL.
			//ConnectionService.SetConnectionString();
			var connectionFactory = new HelperConnectionFactory(ConnectionHelper.Helper());
			var configurationRepo = new ConfigurationRepository(connectionFactory);
			var queuePollInterval =
				Math.Max(
					configurationRepo.ReadValue<int>(ConfigurationKeys.QueuePollInterval) ?? Defaults.Queuing.DefaultQueuePollInterval,
					Defaults.Queuing.MinQueuePollInterval);
			var sqlHangfireOptions = new SqlServerStorageOptions
			{
				QueuePollInterval = TimeSpan.FromSeconds(queuePollInterval)
			};

			using (var conn = connectionFactory.GetEddsPerformanceConnection())
			{
				GlobalConfiguration.Configuration.UseSqlServerStorage(conn.ConnectionString, sqlHangfireOptions);
			}

			var options = new DashboardOptions
			{
				AppPath = VirtualPathUtility.ToAbsolute("~"),
				Authorization = new[] { new AuthenticateUserAttribute() }
			};
			app.UseHangfireDashboard("/hangfire", options);
		}
	}
}