namespace kCura.PDB.Core.Models.BISSummary.Grids
{
	using System.Collections.Generic;
	using System.Linq;
	using kCura.PDB.Core.Models.BISSummary.Models;

	public class ServerViewGrid
	{
		public ServerViewGrid()
		{
			Data = new List<UserExperienceServerWorkspaceInfo>().AsQueryable();
		}

		public int Count;
		public IQueryable<UserExperienceServerWorkspaceInfo> Data;
	}
}
