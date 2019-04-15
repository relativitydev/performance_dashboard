namespace kCura.PDB.Data.Repositories
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Transactions;
	using Dapper;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Data.Properties;

	public class TabRepository : ITabRepository
	{
		private readonly IConnectionFactory connectionFactory;

		public TabRepository(IConnectionFactory connectionFactory)
		{
			this.connectionFactory = connectionFactory;
		}

		public Tab ReadPerformanceDashboardParentTab()
		{
			using (var conn = this.connectionFactory.GetEddsConnection())
			{
				return conn.QueryFirst<Tab>(Resources.Tab_ReadByName, new { tabName = Names.Tab.PerformanceDashboard });
			}
		}
		
		public IList<Tab> ReadChildren(int parentArtifactId)
		{
			using (var conn = this.connectionFactory.GetEddsConnection())
			{
				return conn.Query<Tab>(Resources.Tab_ReadAllByParent, new { artifactId =  parentArtifactId }).ToList();
			}
		}

		public int CreateTab(Tab tab)
		{
			using (var transaction = new TransactionScope())
			{
				using (var conn = this.connectionFactory.GetEddsConnection())
				{
					// Create everything
					tab.ArtifactId = conn.QueryFirst<int>(Resources.Artifact_Create, new
					{
						tab.ParentArtifactId,
						tab.Name,
						artifactTypeId = 23,
						createdBy = 9,
						containerId = 62,
						keywords = "",
						notes = ""
					});
					if (tab.ArtifactId <= 0)
					{
						throw new Exception($"Artifact creation for Tab {tab.Name} failed");
					}

					conn.Execute(Resources.Tab_CreateOrUpdate, tab);
					conn.Execute(Resources.ArtifactAncestry_Create, new { tab.ArtifactId, tab.ParentArtifactId });
				}
				transaction.Complete();
			}
			return tab.ArtifactId;
		}

		public int UpdateTab(Tab tab)
		{
			using (var conn = this.connectionFactory.GetEddsConnection())
			{
				conn.Execute(Resources.Tab_Update, tab);
			}
			return tab.ArtifactId;
		}

		public void ApplyGroupTabPermissions()
		{
			using (var conn = this.connectionFactory.GetEddsConnection())
			{
				conn.Execute(Resources.GroupTab_ApplyGroupTabPermissions);
			}
		}

		public void DeleteTabRecursively(Tab tab)
		{
			var childTabs = this.ReadChildren(tab.ArtifactId);
			var artifactIds = childTabs.Select(t => t.ArtifactId).ToList();

			using (var transaction = new TransactionScope())
			{
				using (var conn = this.connectionFactory.GetEddsConnection())
				{
					// Get all the artifactIds we need to clean up
					artifactIds.AddRange(conn.Query<int>(Resources.ArtifactAncestry_ReadByParent, new {tab.ArtifactId}));
					artifactIds.Add(tab.ArtifactId);
					artifactIds = artifactIds.Distinct().ToList();
					
					conn.Execute(Resources.GroupTab_DeleteAll, new { artifactIds });
					conn.Execute(Resources.Tab_DeleteAll, new { artifactIds });
					conn.Execute(Resources.ArtifactAncestry_DeleteAll, new { artifactIds });
					conn.Execute(Resources.Artifact_DeleteAll, new { artifactIds });
				}

				transaction.Complete();
			}
		}
	}
}
