using kCura.PDD.Web.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace kCura.PDD.Web.Models.BISSummary
{
    using kCura.PDB.Core.Models;
    using kCura.PDB.Core.Models.BISSummary.Grids;
    using kCura.PDB.Core.Models.BISSummary.Models;

	public class FileLatencyViewModel
	{
		public FileLatencyViewModel()
		{
			GridConditions = new GridConditions();
			FilterConditions = System.Enum
				.GetValues(typeof(FileLatency.Columns))
				.Cast<FileLatency.Columns>()
				.ToDictionary(k => k, v => (String)null);
			FilterOperands = System.Enum
				.GetValues(typeof(FileLatency.Columns))
				.Cast<FileLatency.Columns>()
				.ToDictionary(k => k, v => FilterOperand.Equals);
		}

		public GridConditions GridConditions;
		public Dictionary<FileLatency.Columns, String> FilterConditions;
		public Dictionary<FileLatency.Columns, FilterOperand> FilterOperands;
	}
}