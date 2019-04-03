namespace kCura.PDB.Core.Interfaces.Testing.Services
{
	using System.Threading.Tasks;

	using kCura.PDB.Core.Models.Testing;

	public interface ITestDataSetupService
	{
		/// <summary>
		/// Setup Backup Test data tables
		/// </summary>
		/// <param name="backupDbccData">Excel file to import</param>
		/// <param name="throwOnNullData">Throw when sheets are null</param>
		/// <returns>Data read from the excel sheet that was setup in the tables</returns>
		Task<TestBackupDbccData> SetupBackupDbccDataAsync(byte[] backupDbccData, bool throwOnNullData = false);

	    Task SetupBackupDbccDataAsync(TestBackupDbccData data);

		void SetupBackupDbccReportData(byte[] backupDbccData);

        void SetupQoSReportData(byte[] qosReportData);

        Task CleanupBackupDbccTestDataAsync();
    }
}
