using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace kCura.PDD.Web.Enum
{
	/// <summary>
	/// The names of these must match the strings used for filtering in the backing procedure,
	/// and the order of the columns must match the order in the grid.
	/// </summary>
	public enum UptimeViewColumns
	{
		SummaryDayHour = 0,
		Score = 1,
		Status = 2,
		Uptime = 3
	}
}