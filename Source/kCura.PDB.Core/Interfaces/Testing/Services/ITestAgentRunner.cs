namespace kCura.PDB.Core.Interfaces.Testing.Services
{
	using System.Collections.Generic;
	using System.Threading;
	using System.Threading.Tasks;

	public interface ITestAgentRunner
    {
	    IList<Task> GetAgentExecutionTasks(CancellationToken cancellationToken);
    }
}
