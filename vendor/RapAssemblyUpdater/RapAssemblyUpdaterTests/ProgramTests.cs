using NUnit.Framework;
using RapAssemblyUpdater;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RapAssemblyUpdaterTests
{
	[TestFixture]
	public class ProgramTests
	{
		[SetUp]
		public void Setup()
		{
			var dirPath = GetTestAssemblyDirectory();
			var originalAppFile = System.IO.Path.Combine(dirPath, @"..\..\..\..\..\Source\Build", @"application.8.2.xml");
			_tempAppFile = Path.GetTempFileName();
			File.Copy(originalAppFile, _tempAppFile, true);
			_tempAssembly = System.IO.Path.Combine(dirPath, @"..\..\", @"kCura.PDB.Agent.Package.dll");
		}

		private String _tempAppFile;
		private String _tempAssembly;

		[Test]
		public void UpdateRap()
		{
			//Arrange
			var args = new List<String>() { _tempAppFile, _tempAssembly };

			//Act
			var result = Program.Run(args);

			//Assert
			Assert.That(result, Is.EqualTo(1), "result should match number of assemblies updated");

			//Extra
			Console.WriteLine(_tempAppFile);
		}

		private String GetTestAssemblyDirectory()
		{
			String codeBase = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
			UriBuilder uri = new UriBuilder(codeBase);
			String path = Uri.UnescapeDataString(uri.Path);
			return System.IO.Path.GetDirectoryName(path);
		}

	}
}
