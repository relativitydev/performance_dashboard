namespace kCura.PDB.Service.Services
{
	using System;
	using System.Linq;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;

	public class TabService : ITabService
	{
		private readonly ITabRepository tabRepository;
		private readonly IRelativityOneService relativityOneService;

		public TabService(ITabRepository tabRepository, IRelativityOneService relativityOneService)
		{
			this.tabRepository = tabRepository;
			this.relativityOneService = relativityOneService;
		}

		/// <summary>
		/// Creates the Performance Dashboard parent tab at the admin level and all its children.
		/// To add a new tab to Performance Dashboard:
		///     * Add it to kCura.PDD.Model.Constants.TabNames.
		///     * Add the new TabNames member to the initial list below.
		///     * Using existing tab checks toward the bottom of this method as a template, check if the new tab exists.
		///         Be sure to increment the display order and use the correct custom page URL.
		/// </summary>
		public void CreateApplicationTabs()
		{
			// Identify parent tab
			var parentTabId = -1;
			try
			{
				// Grab parent tab
				parentTabId = this.tabRepository.ReadPerformanceDashboardParentTab().ArtifactId;
			}
			catch (InvalidOperationException)
			{
				// It doesn't exist, create the new parent tab
				parentTabId = this.tabRepository.CreateTab(Tab.PerformanceDashboard);
			}

			CreateChildTabs(parentTabId);

			// Apply group tab permissions if needed
			this.tabRepository.ApplyGroupTabPermissions();
		}

		private void CreateChildTabs(int parentTabId)
		{
			//find and identify tabs to add, delete, update
			var desiredTabs = Tab.AllChildTabs.ToDictionary(x => x.Name);
			if (this.relativityOneService.IsRelativityOneInstance())
			{
				desiredTabs.Remove(Tab.EnvironmentCheck.Name);
			}

			var existingTabs = this.tabRepository.ReadChildren(parentTabId).ToDictionary(x => x.Name);

			foreach (var tabName in desiredTabs.Keys.Union(existingTabs.Keys))
			{
				Tab desiredTab;
				Tab existingTab;
				desiredTabs.TryGetValue(tabName, out desiredTab);
				existingTabs.TryGetValue(tabName, out existingTab);

				if (desiredTab == null)
				{
					//delete
					this.tabRepository.DeleteTabRecursively(existingTab);
				}
				else if (existingTab == null)
				{
					//create
					desiredTab.ParentArtifactId = parentTabId;
					this.tabRepository.CreateTab(desiredTab);
				}
				else
				{
					//update
					this.tabRepository.UpdateTab(new Tab
					{
						ArtifactId = existingTab.ArtifactId,
						ParentArtifactId = parentTabId,
						DisplayOrder = desiredTab.DisplayOrder,
						ExternalLink = desiredTab.ExternalLink,
						Name = existingTab.Name
					});
				}
			}
		}

		public void DeleteApplicationTabs()
		{
			// Get the main tab
			var parentTab = this.tabRepository.ReadPerformanceDashboardParentTab();
			if (parentTab != null && parentTab.ArtifactId > 0)
			{
				this.tabRepository.DeleteTabRecursively(parentTab);
			}
		}
	}
}
