namespace kCura.PDB.Core.Interfaces.HealthChecks
{
	using System;
	using System.Collections.Generic;
	using kCura.PDB.Core.Models.HealthChecks;

	public interface IViewHealth
	{
		/// <summary>
		/// Holds application health data
		/// </summary>
		IList<HealthBase> HealthData { get; set; }

		/// <summary>
		/// Holds list of fields (error,lqr,user stc.)
		/// </summary>
		IList<KeyValue> ColumnsList { get; set; }

		/// <summary>
		/// Holds start date
		/// </summary>
		DateTime? StartDate { get; set; }

		/// <summary>
		/// Holds end date
		/// </summary>
		DateTime? EndDate { get; set; }

		/// <summary>
		/// TimeZoneOffSet
		/// </summary>
		int TimeZoneOffset { get; set; }

		MeasureTypes MeasureType { get; }
	}
}
