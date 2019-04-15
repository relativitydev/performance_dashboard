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
	public enum LoadViewColumns
	{
		SummaryDayHour = 0,
		Server = 1,
		ServerType = 2,
		Score = 3,
		CPUScore = 4,
		RAMPctScore = 5,
		MemorySignalStateScore = 6,
        WaitsScore = 7,
        VirtualLogFilesScore = 8,
        LatencyScore = 9,
		IsActiveWeeklySample = 10
	}

    public enum WaitsViewColumns
    {
        SummaryDayHour = 0,
        ServerName = 1,
        WaitsScore = 2,
        SignalWaitsRatio = 3,
		ProcessorTimeUtilization = 4,
        WaitType = 5,
        DifferentialSignalWaitMs = 6,
        DifferentialWaitMs = 7,
		WaitingTaskCount = 8,
        IsPoisonWait = 9,
        IsActiveWeeklySample = 10
    }
}