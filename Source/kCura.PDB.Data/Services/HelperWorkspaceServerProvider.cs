namespace kCura.PDB.Data.Services
{
	using System;
	using global::Relativity.API;
	using kCura.PDB.Core.Interfaces.Data;

	public class HelperWorkspaceServerProvider : IWorkspaceServerProvider
	{
		public HelperWorkspaceServerProvider(IHelper helper)
		{
			this.helper = helper;
			if (this.helper == null) throw new ArgumentNullException(nameof(helper));
		}

		private readonly IHelper helper;

		public string GetWorkspaceServer(int workspaceId)
		{
			var context = this.helper.GetDBContext(workspaceId);
			var serverName = context.ServerName;
			context.ReleaseConnection();
			return serverName;
		}
	}
}
