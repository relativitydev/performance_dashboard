namespace kCura.PDB.Core.Interfaces.Queuing
{
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using Ninject;

	public interface IEventChildKernelFactory
	{
		IKernelWrapper CreateChildKernel(Event evnt);
	}
}
