namespace kCura.PDB.Data.Services
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using kCura.PDB.Core.Interfaces.Data;

	public class CachedWorkspaceServerFactory : IWorkspaceServerProvider
	{
		private const int expiresOnSeconds = 60 * 5; // five minutes
		private static readonly IList<CachedWorkspaceServer> cachedWorkspaceServer = new List<CachedWorkspaceServer>();

		private readonly IWorkspaceServerProvider workspaceServerProvider;

		public CachedWorkspaceServerFactory(IWorkspaceServerProvider workspaceServerProvider)
		{
			this.workspaceServerProvider = workspaceServerProvider;
		}

		public string GetWorkspaceServer(int workspaceId)
		{
			lock (cachedWorkspaceServer)
			{
				var cached = cachedWorkspaceServer.FirstOrDefault(c => c.Workspace == workspaceId);

				// If the result is expired then remove it
				if (cached != null && cached.ExpiresOn < DateTime.UtcNow)
				{
					cachedWorkspaceServer.Remove(cached);
				}

				// if the result doesn't exist or is expired
				if (cached == null || cached.ExpiresOn < DateTime.UtcNow)
				{
					var server = workspaceServerProvider.GetWorkspaceServer(workspaceId);
					cached = new CachedWorkspaceServer
					{
						Workspace = workspaceId,
						Server = server,
						ExpiresOn = DateTime.UtcNow.AddSeconds(expiresOnSeconds)
					};
					cachedWorkspaceServer.Add(cached);
				}

				// return the server
				return cached.Server;
			}
		}

		private class CachedWorkspaceServer
		{
			public int Workspace { get; set; }

			public string Server { get; set; }

			public DateTime ExpiresOn { get; set; }
		}
	}
}
