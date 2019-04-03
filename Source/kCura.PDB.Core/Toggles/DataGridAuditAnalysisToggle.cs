namespace kCura.PDB.Core.Toggles
{
	using System.Runtime.InteropServices;
	using global::Relativity.Toggles;

	/// <summary>
	/// Feature toggle for data grid audit analysis
	/// </summary>
	[Guid("A632524B-D809-4258-ADDC-48C910AE91C8")]
	[DefaultValue(false)]
	public class DataGridAuditAnalysisToggle : IToggle
	{
	}
}
