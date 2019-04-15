namespace kCura.PDB.Core.Interfaces.Services
{
	public interface IRoundHouseFileService
	{
		void CleanUpAppDataDirectory();
		string UnzipResourceFile(byte[] zippedBytes);
	}
}
