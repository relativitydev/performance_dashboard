namespace kCura.PDB.Core.Interfaces.ProcessControls
{
	using System;
	using kCura.PDB.Core.Enumerations;
	using kCura.PDB.Core.Models;

	public interface IProcessControlTask
	{
		bool Execute(ProcessControl processControl);

		ProcessControlId ProcessControlID { get; }

		string Name { get; }
	}
}
