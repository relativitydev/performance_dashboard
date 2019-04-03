using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Linq;

namespace RapAssemblyUpdater
{
	public class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Starting RapAssemblyUpdater");
			Console.WriteLine("-------------------------------------------------------");

			foreach (var arg in args)
			{
				Console.WriteLine("arg: {0}", arg);
			}

			if (args.Count() < 2)
				throw new ArgumentException("Usage: RapAsseblyUpdater PathToAppXml FirstAssembly [AdditionalAssemblies]");

			Run(args.ToList());

			Console.WriteLine("-------------------------------------------------------");
			Console.WriteLine("Completed RapAssemblyUpdater");
		}

		public static Int32 Run(List<String> args)
		{
			var appXmlPath = args[0];
			var assemblies = args.Skip(1).ToList();

			var appXdoc = XDocument.Load(appXmlPath);

			var assembliesInApp = appXdoc.Descendants("Assemblies").Elements("Assembly");

			var assembliesToUpdate = assembliesInApp.Where(asm => assemblies.Any(a => Path.GetFileName(a).ToLower() == asm.Element("Name").Value.ToLower())).ToList();
			var assembliesSkipping = assembliesInApp.Where(asm => false == assemblies.Any(a => Path.GetFileName(a).ToLower() == asm.Element("Name").Value.ToLower())).ToList();

			foreach (var asm in assembliesSkipping)
			{
				Console.WriteLine("Skipping {0}", asm.Element("Name").Value);
			}

			foreach (var asm in assembliesToUpdate)
			{
				Console.WriteLine("Updating {0}", asm.Element("Name").Value);
				var asmPath = assemblies.FirstOrDefault(a => Path.GetFileName(a).ToLower() == asm.Element("Name").Value.ToLower());
				var updatedFileData = Convert.ToBase64String(File.ReadAllBytes(asmPath));
				if (null != asm.Element("FileData"))
					asm.Element("FileData").Value = updatedFileData;
				else
					asm.Add(new XElement("FileData", updatedFileData));
			}

			var appXml82Path = Path.Combine(Path.GetDirectoryName(appXmlPath), Path.GetFileNameWithoutExtension(appXmlPath) + ".8.2" + Path.GetExtension(appXmlPath));
			appXdoc.Save(appXml82Path);

			return assembliesToUpdate.Count();
		}
	}
}
