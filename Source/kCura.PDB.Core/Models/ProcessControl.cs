namespace kCura.PDB.Core.Models
{
	using System;
	using kCura.PDB.Core.Enumerations;

	public class ProcessControl
	{
		public ProcessControlId Id { get; set; }

		public int ProcessControlId
		{
			get { return (int)Id; }
			set { Id = (ProcessControlId)value; }
		}

		public DateTime LastProcessExecDateTime { get; set; }

		public int? Frequency { get; set; }

		public bool? LastExecSucceeded { get; set; }

		public string ProcessTypeDesc { get; set; }

		public string LastErrorMessage { get; set; }
	}
}
