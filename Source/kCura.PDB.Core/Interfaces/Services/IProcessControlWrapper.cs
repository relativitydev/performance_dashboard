namespace kCura.PDB.Core.Interfaces.Services
{
	using System;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Enumerations;

	public interface IProcessControlWrapper
	{
		Task Execute<TR>(ProcessControlId processControlId, Func<TR> func);

		Task Execute(ProcessControlId processControlId, Action func);

		Task Execute<TR>(ProcessControlId processControlId, Func<Task<TR>> func);

		Task Execute(ProcessControlId processControlId, Action<DateTime> func);
	}
}
