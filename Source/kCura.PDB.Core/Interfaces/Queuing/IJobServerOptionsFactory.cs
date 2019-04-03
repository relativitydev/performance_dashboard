namespace kCura.PDB.Core.Interfaces.Queuing
{
	using System.Threading.Tasks;
	using Hangfire;

	public interface IJobServerOptionsFactory
	{
		BackgroundJobServerOptions GetOptions();
	}
}
