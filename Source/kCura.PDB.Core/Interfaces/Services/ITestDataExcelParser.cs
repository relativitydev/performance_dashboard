namespace kCura.PDB.Core.Interfaces.Services
{
	using kCura.PDB.Core.Models.Testing;

	public interface ITestDataExcelParser
	{
		TestBackupDbccData ParseExcelBackupDbccData(byte[] excelData, bool throwOnNullData = false);
	}
}
