namespace kCura.PDB.Core.Models
{
	using System.ComponentModel.DataAnnotations;

	public class EventSystemControl
	{
		public int Id { get; set; }

		public EventSystemState State { get; set; }
	}

	public enum EventSystemState
	{
		[Display(Name = "Running")]
		Normal = 1,
		[Display(Name = "Paused")]
		Paused = 2,
		[Display(Name = "Database Deployment")]
		Prerequisites = 3
	}
}
