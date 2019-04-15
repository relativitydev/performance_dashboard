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
	public enum SearchViewColumns
	{
		SummaryDayHour = 0,
		Search = 1,
		User = 2,
		PercentLongRunning = 3,
		IsComplex = 4,
		TotalRunTime = 5,
		AverageRunTime = 6,
		TotalRuns = 7,
		QoSHourID = 8,
		IsActiveWeeklySample = 9
	}
	public enum HoursViewColumns
	{
		SummaryDayHour = 0,
		DatabaseName = 1,
		SearchName = 2,
		IsComplex = 3,
		TotalLRQRunTime = 4,
		AverageRunTime = 5,
		TotalRuns = 6,
		IsActiveWeeklySample = 7
	}
	public enum ServerViewColumns
	{
		SummaryDayHour = 0,
		Server = 1,
		Score = 2,
		Workspace = 3,
		TotalLongRunning = 4,
		TotalUsers = 5,
		TotalSearchAudits = 6,
		TotalNonSearchAudits = 7,
		TotalAudits = 8,
		TotalExecutionTime = 9,
		IsActiveWeeklySample = 10
	}

}