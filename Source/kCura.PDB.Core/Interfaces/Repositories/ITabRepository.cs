namespace kCura.PDB.Core.Interfaces.Repositories
{
	using System;
	using System.Collections.Generic;
	using kCura.PDB.Core.Models;

	/// <summary>
	/// Repository to read tabs.  FYI - most of this interacts just via Tab name since there is no direct link to the application.  USE CAUTION WHEN USING UPDATE/DELETE
	/// </summary>
	public interface ITabRepository
	{
		Tab ReadPerformanceDashboardParentTab();

		IList<Tab> ReadChildren(int parentArtifactId);
		
		int CreateTab(Tab tab);
		
		/// <summary>
		/// Update given tab (Note: Ensure we are only updating PDB tabs)
		/// </summary>
		/// <param name="tab">Tab to update</param>
		/// <returns>ArtifactId</returns>
		int UpdateTab(Tab tab);

		void ApplyGroupTabPermissions();

		void DeleteTabRecursively(Tab tab);
	}
}
