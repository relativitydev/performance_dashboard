namespace kCura.PDB.Core.Interfaces.Queuing
{
	using kCura.PDB.Core.Models;
	using Ninject;

	public interface IEventTaskFactory
	{
		IEventTask GetEventTask(IKernel eventKernel, EventSourceType eventType);
	}
}
