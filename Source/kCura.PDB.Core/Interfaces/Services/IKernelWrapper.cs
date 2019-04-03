namespace kCura.PDB.Core.Interfaces.Services
{
	using System;
	using Ninject;

	public interface IKernelWrapper : IDisposable
	{
		IKernel Kernel { get; }
	}
}
