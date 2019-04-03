using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace kCura.PDB.KeyGenerator
{
	public class ConfigurationService
	{
		public byte[] ReadSeedConfigurationValue()
		{
			//Read the seed from the configuration and decode it
			byte[] seed = null;
			try
			{
				var seed64 = ConfigurationManager.AppSettings["Seed"] ?? String.Empty;
				seed = Convert.FromBase64String(seed64);
			}
			catch (ConfigurationErrorsException)
			{
				Console.WriteLine("ERROR: Seed configuration value cannot be read.");
			}
			catch (FormatException)
			{
				Console.WriteLine("ERROR: Invalid seed value. Please verify the configuration value matches your unique seed value.");
			}

			return seed;
		}

		public byte[] ReadRequestTokenConfigurationValue()
		{
			//Read the request token from the configuration and decode it
			byte[] requestToken = null;
			try
			{
				var request64 = ConfigurationManager.AppSettings["RequestToken"];
				requestToken = Convert.FromBase64String(request64);
			}
			catch (ConfigurationErrorsException)
			{
				Console.WriteLine("ERROR: Request token configuration value cannot be read.");
			}
			catch (FormatException)
			{
				Console.WriteLine("ERROR: Invalid request token value. Please verify the configuration value matches the full request token provided by PDB.");
			}

			return requestToken;
		}
	}
}
