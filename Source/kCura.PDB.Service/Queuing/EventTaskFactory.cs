namespace kCura.PDB.Service.Queuing
{
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Interfaces.Queuing;
	using kCura.PDB.Core.Models;
	using Ninject;

	public class EventTaskFactory : IEventTaskFactory
	{
		public IEventTask GetEventTask(IKernel eventKernel, EventSourceType eventType) =>
			EventConstants.LoopEventTypes.Contains(eventType)
				? eventKernel.Get<CheckEventTask>() as IEventTask
				: eventKernel.Get<EventTask>();
	}
}
