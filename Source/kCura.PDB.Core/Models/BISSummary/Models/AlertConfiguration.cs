namespace kCura.PDB.Core.Models.BISSummary.Models
{
	public class AlertConfiguration
	{
		public AlertConfiguration()
		{
			Recipients = string.Empty;
		}

		public int Frequency { get; set; }
		public int Threshold { get; set; }
		public string Recipients { get; set; }
		public bool Enabled { get; set; }

		public bool Valid
		{
			get
			{
				if (!Enabled)
					return true;
				if (string.IsNullOrEmpty(Recipients))
					return false;

				try
				{
					var addresses = Recipients.Split(',', ';');
					foreach (var address in addresses)
					{
						var a = new System.Net.Mail.MailAddress(address);
					}

					return true;
				}
				catch
				{
					return false;
				}
			}
		}
	}
}