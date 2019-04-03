using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace kCura.PDD.Web.Enum
{
	public enum EnvironmentCheckRecommendationColumns : int
	{
		Status = 0,
		Scope = 1,
		Section = 2,
		Name = 3,
		Description = 4,
		Value = 5,
		Recommendation = 6
	}

	public enum EnvironmentCheckServerColumns : int
	{
		ServerName = 0,
		OSName = 1,
		OSVersion = 2,
		LogicalProcessors = 3,
		Hyperthreaded = 4,
	}

	public enum EnvironmentCheckDatabaseColumns : int
	{
		ServerName = 0,
		SQLVersion = 1,
		AdHocWorkload = 2,
		MaxServerMemory = 3,
		MaxDegreeOfParallelism = 4,
		TempDBDataFiles = 5,
		LastSQLRestart = 6,
	}
}