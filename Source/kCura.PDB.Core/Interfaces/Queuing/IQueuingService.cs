namespace kCura.PDB.Core.Interfaces.Queuing
{
	using System;
	using System.Linq.Expressions;
	using System.Threading.Tasks;
	using Hangfire;

	public interface IQueuingService
	{
		void Enqueue<T>(Expression<Action<T>> methodCall, int? delay = null);
	}
}
