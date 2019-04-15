namespace kCura.PDB.Core.Enumerations
{
	using System.ComponentModel.DataAnnotations;

	/// <summary>
	/// Enum of all of the AuditActionIDs that Performance Dashboard uses
	/// 1, 29, 32, 34, 35, 37 - Ones with execution times
	/// 3 - also RDC overlay, estimated concurrency of COUNT(UserID) * 2 AS ExecutionTime
	/// 4, 5, 6 - estimated concurrency of COUNT(UserID) * 2 AS ExecutionTime
	/// 28 - Search, needs to be parsed
	/// 33 - Average execution time among them all
	/// 47 - RDC Overlay, estimated concurrency of COUNT(UserID) * 10 AS ExecutionTime
	/// -- Display names are used for parsing against Data Grid.  They must be the same as the ones used by Data Grid in order to correctly filter.
	/// </summary>
	public enum AuditActionId
	{
		[Display(Name = "View")]
		View = 1,
		[Display(Name = "Update")]
		Update = 3,
		[Display(Name = "Update - Mass Edit")]
		UpdateMassEdit = 4,
		[Display(Name = "Update - Mass Replace")]
		UpdateMassReplace = 5,
		[Display(Name = "Update - Propagation")]
		UpdatePropagation = 6,
		[Display(Name = "Document Query")]
		DocumentQuery = 28,
		[Display(Name = "Query")]
		Query = 29,
		[Display(Name = "Import")]
		Import = 32,
		[Display(Name = "Export")]
		Export = 33,
		[Display(Name = "ReportQuery")]
		ReportQuery = 34,
		[Display(Name = "RelativityScriptExecution")]
		RelativityScriptExecution = 35,
		[Display(Name = "Pivot Query")]
		PivotQuery = 37,
		[Display(Name = "Update - Import")]
		UpdateImport = 47
	}
}
