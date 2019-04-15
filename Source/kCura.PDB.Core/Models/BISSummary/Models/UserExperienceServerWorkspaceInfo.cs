namespace kCura.PDB.Core.Models.BISSummary.Models
{
	using System;

	public class UserExperienceServerWorkspaceInfo
	{
		public int Index;
		public int ServerId;
		public string Server;
		public int WorkspaceId;
		public string Workspace;
		public DateTime SummaryDayHour;
		public int Score;
		public int TotalUsers;
		public int TotalLongRunning;
		public int TotalSearchAudits;
		public int TotalNonSearchAudits;
		public int TotalAudits;
		public Int64 TotalExecutionTime;
		public bool IsActiveWeeklySample;
	}
}
