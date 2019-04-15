namespace kCura.PDB.Core.Interfaces.Services
{
	using System;
	using System.Collections.Generic;
	using Ninject;

	public interface IKernelFactory
	{
		IKernel GetKernel(IList<Type> excludedTypes = null);
	}
}
