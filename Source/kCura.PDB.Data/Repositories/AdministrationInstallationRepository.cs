namespace kCura.PDB.Data.Repositories
{
	using System;
	using Dapper;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Data.Properties;

	public class AdministrationInstallationRepository : BaseRepository, IAdministrationInstallationRepository
	{
		public AdministrationInstallationRepository(IConnectionFactory connectionFactory)
			: base(connectionFactory)
		{
		}

		public bool HasDbccPermissions(string targetServer, GenericCredentialInfo credentialInfo)
		{
			try
			{
				using (var conn = this.connectionFactory.GetTargetConnection(Names.Database.Msdb, targetServer, credentialInfo))
				{
					conn.Execute("DBCC DBINFO");
				}
			}
			catch (Exception ex)
			{
				throw new Exception($"Failed to get permissions for account [{credentialInfo.UserName ?? "Integrated"}] against server [{targetServer}]", ex);
			}

			return true;
		}

		public void InstallPrimaryServerAdminScripts(GenericCredentialInfo credentialInfo)
		{
			using (var conn = this.connectionFactory.GetEddsConnection(credentialInfo))
			{
				//Deploy SQL Server Agent for collecting Relativity Agent history
				var statements = SplitScript(Resources.Create_QoS_CollectAgentUptime);
				foreach (var statement in statements)
					conn.Execute(statement);
			}
		}

		public void UpdateAdminScriptsRun()
		{
			using (var conn = this.connectionFactory.GetEddsPerformanceConnection())
			{
				conn.Execute(Resources.UpdateAdminScriptsInstalled);
			}
		}

		/// <summary>
		/// Split single script into multiple scripts using "GO" as the delimiter
		/// </summary>
		/// <param name="script"></param>
		/// <returns></returns>
		internal static string[] SplitScript(string script)
		{
			return script.Split(new string[] { DatabaseConstants.GoStatement }, StringSplitOptions.None);
		}
	}
}
