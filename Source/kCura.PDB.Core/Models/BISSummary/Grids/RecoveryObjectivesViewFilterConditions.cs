namespace kCura.PDB.Core.Models.BISSummary.Grids
{
	public class RecoveryObjectivesViewFilterConditions
	{
		public string Server;
		public string DatabaseName;
		public int? RPOScore;
		public int? RTOScore;
		public int? PotentialDataLossMinutes;
		public int? EstimatedTimeToRecoverHours;
	}
}
