namespace kCura.PDD.Web.Services
{
	using System;
	using System.Collections.Generic;
	using System.Data.SqlClient;
	using System.Linq;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.BISSummary.Models;

	public class DbccTargetService
	{
		private readonly ISqlServerRepository _sqlRepository;

		public DbccTargetService(ISqlServerRepository sqlServerRepository)
		{
			_sqlRepository = sqlServerRepository;
		}

		public void RefreshTargets()
		{
			_sqlRepository.RefreshDbccTargets();
		}

		public List<DbccTargetInfo> ListTargets()
		{
			return _sqlRepository.ListDbccTargets();
		}

		/// <summary>
		/// Retrieves information about a DBCC target
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public DbccTargetInfo GetTarget(int id)
		{
			return ListTargets().FirstOrDefault(x => x.Id == id);
		}

		public void SaveTarget(DbccTargetInfo target, int userId)
		{
			var originalTarget = GetTarget(target.Id);
			var now = DateTime.UtcNow;

			_sqlRepository.UpdateDbccTarget(target.Id, target.Database, target.IsActive);

			var audits = new List<ConfigurationAudit>();
			if (!originalTarget.Database.Equals(target.Database))
				audits.Add(new ConfigurationAudit
				{
					ServerName = target.Server,
					FieldName = ConfigurationAuditFields.MonitoringTargetDatabase,
					OldValue = originalTarget.Database,
					NewValue = target.Database,
					UserId = userId,
					CreatedOn = now
				});
			if (!originalTarget.IsActive.Equals(target.IsActive))
				audits.Add(new ConfigurationAudit
				{
					ServerName = target.Server,
					FieldName = ConfigurationAuditFields.MonitoringEnabled,
					OldValue = originalTarget.IsActive.ToString(),
					NewValue = target.IsActive.ToString(),
					UserId = userId,
					CreatedOn = now
				});

			//If there are no more active targets, disable view-based monitoring
			if (_sqlRepository.ListDbccTargets().All(x => !x.IsActive))
			{
				var useViewBasedMonitoring = Convert.ToBoolean(_sqlRepository.ConfigurationRepository.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.UseDbccViewMonitoring) ?? "False");
				if (useViewBasedMonitoring)
					audits.Add(new ConfigurationAudit
					{
						FieldName = ConfigurationAuditFields.EnableViewBasedMonitoring,
						OldValue = "True",
						NewValue = "False",
						UserId = userId,
						CreatedOn = now
					});

				_sqlRepository.ConfigurationRepository.SetConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.UseDbccViewMonitoring, "False");
			}

			//Track audit history
			var triggerAlert = Convert.ToBoolean(_sqlRepository.ConfigurationRepository.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.SendConfigurationChangeNotifications) ?? "False");
			_sqlRepository.AuditConfigurationChanges(audits, triggerAlert);
		}

		/// <summary>
		/// Indicates whether the target server-database pair is valid by attempting to connect as eddsdbo
		/// </summary>
		/// <param name="server"></param>
		/// <param name="database"></param>
		/// <returns></returns>
		public ValidationResult ValidateTarget(DbccTargetInfo target)
		{
			var result = new ValidationResult();
			try
			{
				_sqlRepository.TestConnection(target.Database, target.Server);
			}
			catch (SqlException ex)
			{
				result.Valid = false;
				result.Details =
					$"The connection test for the specified target database {{{target.Database}}} failed. The server {{{target.Server}}} could be unreachable, the database may not exist, or eddsdbo might not have permission to access it.  Additional Details: {ex}";
				return result;
			}

			try
			{
				_sqlRepository.DeployDbccLogView(target.Database, target.Server);
			}
			catch (Exception e)
			{
				result.Valid = false;
				result.Details = string.Format("The eddsdbo.QoS_DBCCLog view does not exist in the target database. Additionally, an exception occurred while attempting to deploy the standard view: {0}",
					e.Message);
				return result;
			}

			try
			{
				_sqlRepository.TestDbccLogView(target.Database, target.Server);
			}
			catch (Exception e)
			{
				result.Valid = false;
				result.Details = string.Format("The eddsdbo.QoS_DBCCLog view exists, but eddsdbo has insufficient permissions to use it or the output columns are incorrect. The following exception occurred: {0}",
					e.Message);
				return result;
			}

			return result;
		}
	}
}