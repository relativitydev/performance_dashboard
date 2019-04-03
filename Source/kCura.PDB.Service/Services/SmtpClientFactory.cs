namespace kCura.PDB.Service.Services
{
	using System;
	using System.Net.Mail;
	using kCura.PDB.Core.Interfaces;
	using kCura.PDB.Core.Models;

	public class SmtpClientFactory : ISmtpClientFactory
	{
		public ISmtpClient CreateSmtpClient(SmtpSettings settings)
		{
			// Creates a new notification config but only when needed
			var notificationConfig = new Lazy<kCura.Notification.Config>(() => new kCura.Notification.Config(null));

			var client = new SmtpClient
			{
				Host = settings.Server,
				Port = settings.Port,
				EnableSsl = settings.SSLisRequired,
				UseDefaultCredentials = false,
				DeliveryMethod = SmtpDeliveryMethod.Network,
				Credentials = new System.Net.NetworkCredential(settings.Username, settings.Password ?? notificationConfig.Value.SMTPPassword),
				Timeout = 60000
			};
			return new WrappedSmtpClient(client);
		}

		private class WrappedSmtpClient : ISmtpClient
		{
			public WrappedSmtpClient(SmtpClient smtpClient)
			{
				this.smtpClient = smtpClient;
			}

			private readonly SmtpClient smtpClient;

			public void Dispose()
			{
				smtpClient.Dispose();
			}

			public void Send(MailMessage message)
			{
				smtpClient.Send(message);
			}
		}
	}
}
