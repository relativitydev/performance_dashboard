namespace kCura.PDB.Core.Interfaces.ProcessControls
{
	public interface IPerformanceTask
	{
		/// <summary>
		/// Used to perform the task and save into database
		/// </summary>
		void GetPerformanceMetrics();
	}
}
