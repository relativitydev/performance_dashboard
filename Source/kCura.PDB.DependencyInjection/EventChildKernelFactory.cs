namespace kCura.PDB.DependencyInjection
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Ninject;
	using Ninject.Modules;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Interfaces.Agent;
	using kCura.PDB.Core.Interfaces.Queuing;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Service;
	using kCura.PDB.Service.Agent;
	using kCura.PDB.Service.Audits;
	using kCura.PDB.Service.DataGrid;
	using kCura.PDB.Service.Logging;
	using kCura.PDB.Service.Queuing;

	/// <summary>
	/// A factory that creates and manages child kernels for events
	/// </summary>
	public class EventChildKernelFactory : IEventChildKernelFactory, IDisposable
	{
		private readonly IKernel parentKernel;
		private readonly Queue<IKernelWrapper> availableChildKernels;
		private readonly IList<IKernelWrapper> childKernels;
		private readonly object obj = new object();

		public EventChildKernelFactory(IKernel parentKernel)
		{
			this.parentKernel = parentKernel;
			this.availableChildKernels = new Queue<IKernelWrapper>();
			this.childKernels = new List<IKernelWrapper>();
		}

		/// <summary>
		/// Gets a child kernel for event. Child kernels are re-used to help with performance since creating child kernels for every event causes performance issues.
		/// </summary>
		/// <param name="evnt">The event that the kernel will have context of.</param>
		/// <returns>A kernel wrapper that wraps the actual kernel.</returns>
		public IKernelWrapper CreateChildKernel(Event evnt)
		{
			lock (this.obj)
			{
				// Attempt to get an available child kernel to re-use or create a new one if none exist
				IKernelWrapper eventKernelWrapper = null;
				while (eventKernelWrapper == null)
				{
					eventKernelWrapper = this.availableChildKernels.Any()
						? this.availableChildKernels.Dequeue()
						: this.CreateNewChildKernel();
				}

				// Rebind the log context for the current event
				eventKernelWrapper.Kernel.Rebind<ILogContext>().ToConstant(new LogContext(evnt));
				return eventKernelWrapper;
			}
		}

		/// <summary>
		/// Creates a new child kernel with all the bindings
		/// </summary>
		/// <returns>A kernel wrapper that wraps the actual kernel.</returns>
		private IKernelWrapper CreateNewChildKernel()
		{
			INinjectModule dataGridBindings = null;
			try
			{
				dataGridBindings = new DataGridBindings();
			}
			catch
			{
				// Swallow exception and use alternate bindings
				dataGridBindings = new NoDataGridBindings();
			}

			var kernelFactory = new KernelFactory(new ServiceBindings(), dataGridBindings);
			var eventKernel = kernelFactory.GetChildKernel(this.parentKernel, AgentConfiguration.DefaultBindingsExclusionList);
			eventKernel.Bind<IEventRunner>().To<EventRunner>().InTransientScope();
			eventKernel.Bind<ILogger>().ToMethod(LoggerFactory.GetLogger).InTransientScope();
			eventKernel.Bind<DatabaseLogger>()
				.ToConstructor(x => new DatabaseLogger(
						x.Inject<ILogRepository>(),
						x.Inject<ILogService>(),
						x.Inject<ILogContext>(),
						x.Inject<IEventRepository>(),
						x.Inject<IAgentService>()))
						.InTransientScope();
			var childKernelWrapper = new ChildKernelWrapper(eventKernel, this.AddFreeChildKernels);
			this.childKernels.Add(childKernelWrapper);
			return childKernelWrapper;
		}

		/// <summary>
		/// Call back to add the kernel to the available child kernels
		/// </summary>
		/// <param name="kernelWrapper">The wrapped kernel to mark available</param>
		private void AddFreeChildKernels(IKernelWrapper kernelWrapper)
		{
			lock (this.obj)
			{
				this.availableChildKernels.Enqueue(kernelWrapper);
			}
		}

		/// <summary>
		/// Disposes of all the child kernels created
		/// </summary>
		public void Dispose()
		{
			foreach (var childKernel in this.childKernels)
			{
				childKernel.Kernel.Dispose();
			}
		}

		/// <summary>
		/// The kernel wrapped is used so we can use the `using` statement but not actually dispose of the kernel so it can be re-used.
		/// </summary>
		private class ChildKernelWrapper : IKernelWrapper
		{
			private readonly Action<ChildKernelWrapper> onDispose;

			public ChildKernelWrapper(IKernel childKernel, Action<ChildKernelWrapper> onDispose)
			{
				this.onDispose = onDispose;
				this.Kernel = childKernel;
			}

			public IKernel Kernel { get; }

			public void Dispose()
			{
				this.onDispose(this);
			}
		}
	}
}
