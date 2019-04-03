namespace kCura.PDB.Core.Interfaces
{
	using kCura.PDB.Core.Models;

	public interface ISmtpClientFactory
	{
		ISmtpClient CreateSmtpClient(SmtpSettings settings);
	}
}
