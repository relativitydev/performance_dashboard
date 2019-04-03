using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using kCura.PDD.Model.Services;

namespace kCura.PDB.KeyGenerator
{
	public class AuthorizationKeyService
	{
		private IKeyProvider _keyStore;

		public AuthorizationKeyService()
		{
			_keyStore = new KeyProvider();
		}

		public AuthorizationKeyType DetermineKeyType(byte[] seed)
		{
			if (seed == null || seed.Length <= 0)
				return AuthorizationKeyType.Invalid;

			//Compare seed to known sequences
			var isManager = seed.SequenceEqual(_keyStore.GenerateManagerSeed());
			var isArchitect = seed.SequenceEqual(_keyStore.GenerateArchitectSeed());
			var keyType = isManager
				? AuthorizationKeyType.Manager
				: isArchitect
					? AuthorizationKeyType.Architect
					: AuthorizationKeyType.Invalid;
			return keyType;
		}

		public string GenerateAuthorizationKey(byte[] seed, AuthorizationKeyType keyType, byte[] requestToken)
		{
			if (requestToken == null || requestToken.Length <= 0)
				return null;

			//Parse the request token for environment information
			var requestString = System.Text.Encoding.UTF8.GetString(requestToken);
			var requestParts = requestString.Split(',');
			if (requestParts.Length < 2)
				return null;

			var server = requestParts[0];
			var version = requestParts[1];

			//Sanity check: spit out the instance name we found in the request token, as this MUST match
			Console.WriteLine(String.Format("Generating reset key for {0} on version {1}", server, version));
			Console.WriteLine();

			//Get the base key shared by all types
			var key = _keyStore.GenerateKey();
			Array.Reverse(key);

			//Add time-sensitive information to the request key components to create an authorization string
			var culture = System.Globalization.CultureInfo.GetCultureInfo("en-US");
			var hour = DateTime.UtcNow.ToString(culture);
			var ephemeral = new byte[16];
			var random = new Random();
			random.NextBytes(ephemeral);
			var authorizationString = String.Format("{0},{1},{2},{3}", keyType.ToString(), server, version, hour);

			//Generate the authorization key and convert to base64
			var encrypted = Sodium.SecretBox.Create(System.Text.Encoding.UTF8.GetBytes(authorizationString), seed, key);
			Buffer.BlockCopy(ephemeral, 0, encrypted, 0, 16);
			return Convert.ToBase64String(encrypted);
		}

		public void WriteKeyFile(string outputPath, string authorizationKey)
		{
			try
			{
				File.WriteAllText(outputPath, authorizationKey);
				Console.WriteLine(String.Format("Authorization key successfully written to: {0}", outputPath));
			}
			catch (Exception e)
			{
				Console.WriteLine(String.Format("WARNING: An authorization key was generated, but could not be written to the file system. {0}", e.Message));
			}
		}
	}
}
