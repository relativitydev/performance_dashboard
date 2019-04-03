namespace kCura.PDB.Core.Models.Audits
{
	public class AuditQueryBatch
	{
		public AuditQuery Query { get; set; }

		public int Size { get; set; }

		public long Start { get; set; }
	}
}
