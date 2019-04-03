namespace kCura.PDB.Core.Interfaces.Services
{
	using System.Collections.Generic;
	using System.Threading.Tasks;

	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.RecoverabilityIntegrity;

	public interface IGapReporter
	{
		/// <summary>
		/// Create Gap Report for hour/server/gapType
		/// </summary>
		/// <typeparam name="TGap"></typeparam>
		/// <param name="hour"></param>
		/// <param name="server"></param>
		/// <param name="gaps"></param>
		/// <param name="gapType"></param>
		/// <returns></returns>
		Task CreateGapReport<TGap>(Hour hour, Server server, IList<TGap> gaps, GapActivityType gapType)
			where TGap : Gap;
	}
}
