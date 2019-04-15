namespace kCura.PDB.Core.Toggles
{
	using System.Runtime.InteropServices;
	using global::Relativity.Toggles;

    /// <summary>
    /// Feature toggle for data grud audit analysis
    /// </summary>
    [Guid("A531D0E1-58E9-499E-AB4E-607BF3C38BB6")]
    [DefaultValue(false)]
	public class DatabaseDeploymentDeparallelizationToggle : IToggle
	{
	}
}
