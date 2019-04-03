namespace kCura.PDB.Core.Models.BISSummary.Models
{
	public class RecoveryObjectivesInfo
	{
		public int Index;
		public int ServerId;
		public string ServerName;
		public string DatabaseName;
		public int RPOScore;
		public int RTOScore;
		public int PotentialDataLossMinutes;
		public int EstimatedTimeToRecoverHours;
	}
}
