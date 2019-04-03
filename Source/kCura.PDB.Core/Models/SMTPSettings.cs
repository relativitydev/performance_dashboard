namespace kCura.PDB.Core.Models
{
	public class SmtpSettings
	{
		public string Server { get; set; }

		public int Port { get; set; }

		public string EmailFrom { get; set; }

		public string EmailTo { get; set; }

		public string Username { get; set; }

		public string Password { get; set; }

		public string EncryptedPassword { get; set; }

		public bool SSLisRequired { get; set; }
	}
}
