using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using kCura.PDD.Model.Services;

namespace kCura.PDB.KeyGenerator
{
	public class KeyGen
	{
		static void Main(string[] args)
		{
			//Initialize
			var keyService = new AuthorizationKeyService();
			var configService = new ConfigurationService();

			//Determine file paths
			var location = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			var outputPath = String.Format(@"{0}\AuthKey.txt", location);

			//Read configuration settings
			var seed = configService.ReadSeedConfigurationValue();
			var requestToken = configService.ReadRequestTokenConfigurationValue();

			//Use provided seed to determine the type of authorization key
			var keyType = keyService.DetermineKeyType(seed);
			if (keyType == AuthorizationKeyType.Invalid)
			{
				Console.WriteLine("ERROR: Unrecognized seed. Please update the configuration file with your unique seed value.");
				Console.ReadKey();
				return;
			}

			//Create an authorization key and write to a file
			var authorizationKey = keyService.GenerateAuthorizationKey(seed, keyType, requestToken);
			if (string.IsNullOrEmpty(authorizationKey))
			{
				Console.WriteLine("ERROR: Failed to generate an authorization key. Verify the request token in the configuration file.");
				Console.ReadKey();
				return;
			}

			keyService.WriteKeyFile(outputPath, authorizationKey);

			Console.WriteLine();
			Console.WriteLine(String.Format("Your authorization key is: {0}", authorizationKey));
			Console.ReadKey();
		}
	}
}
