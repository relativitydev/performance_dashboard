namespace kCura.PDB.Service.DataGrid
{
	using global::Relativity.API;
	using kCura.AuditUI2.Services.ExternalAuditLog;
	using kCura.PDB.Service.DataGrid.Helpers;
	using kCura.PDB.Service.DataGrid.Interfaces;

	public class AuditLogObjectManagerFactory : IAuditLogObjectManagerFactory
	{
		private readonly IHelper helper;

		public AuditLogObjectManagerFactory(IHelper helper)
		{
			this.helper = helper;
		}

		public IExternalAuditLogObjectManager GetManager()
		{
			// DO NOT REMOVE -- Need this to get Kepler to load the deserializer correctly during integration tests.
			var helper = new KeplerSerializationHelperService();
			helper.ForceLoadQueryObject();
			helper.ForceLoadQueryResultObject();
			return this.helper.GetServicesManager().CreateProxy<IExternalAuditLogObjectManager>(ExecutionIdentity.System);
		}
	}
}
