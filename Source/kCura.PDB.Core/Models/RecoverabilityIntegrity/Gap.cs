namespace kCura.PDB.Core.Models.RecoverabilityIntegrity
{
	using System;

	public class Gap
	{
		public int Id { get; set; }

		public int DatabaseId { get; set; }

		public DateTime Start { get; set; }

		public DateTime? End { get; set; }

		private int? duration;
		public int? Duration
		{
			get
			{
				return duration ??
						(End.HasValue
							? (int)(End.Value - Start).TotalSeconds
							: (int?)null);
			}
			set { this.duration = value; }
		}

		public virtual GapActivityType ActivityType { get; set; }
	}
}
