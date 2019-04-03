namespace kCura.PDB.Core.Interfaces.HealthChecks
{
	using System;
	using System.Collections.Generic;
	using kCura.PDB.Core.Models.HealthChecks;

	public interface IHealthTask
	{
		/// <summary>
		/// Returns colunmns associated with measuretype
		/// </summary>
		/// <param name="measureType"></param>
		/// <returns></returns>
		IList<KeyValue> GetColumnsList(MeasureTypes measureType);

		/// <summary>
		/// Get the application health data
		/// </summary>
		/// <returns></returns>
		IList<HealthBase> GetHealthData(MeasureTypes measureType, DateTime? startDate, DateTime? endDate, int timeZoneOffset);
	}
}
