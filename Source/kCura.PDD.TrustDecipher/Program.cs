using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace TrustDecipher
{
    class Decipher
    {
        static void Main(string[] args)
        {
            var location = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var keyFilePath = String.Format(@"{0}\key", location);
            var trustDataPath = String.Format(@"{0}\TrustData", location);
            var decipheredPath = String.Format(@"{0}\Trust.csv", location);

            Console.WriteLine("Checking for key and encrypted data package in {0}...", location);
            if (!File.Exists(keyFilePath))
            {
                Console.WriteLine(String.Format("Unable to locate key file: {0}", keyFilePath));
                Console.ReadKey();
                return;
            }

            if (!File.Exists(trustDataPath))
            {
                Console.WriteLine(String.Format("Unable to locate TrustData package: {0}", trustDataPath));
                Console.ReadKey();
                return;
            }

            if (File.Exists(decipheredPath))
            {
                Console.WriteLine("Deleting existing CSV...");
                File.Delete(decipheredPath);
            }

            Console.WriteLine("Reading key and encrypted data package...");
            var key = File.ReadAllBytes(keyFilePath);
            var package = File.ReadAllBytes(trustDataPath);

            Console.WriteLine("Opening encrypted package...");
            var decrypted = Sodium.SecretBox.Open(package, new Byte[24], key);

            Console.WriteLine("Creating decrypted CSV...");
            File.WriteAllBytes(decipheredPath, decrypted);

            Console.WriteLine("Done!");
            Console.ReadKey();
        }
    }
}
