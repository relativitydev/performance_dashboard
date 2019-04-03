namespace kCura.PDB.Core.Tests
{
	using System;
	using System.IO;
	using System.Reflection;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Helpers;
	using NUnit.Framework;

	[TestFixture, Explicit("Fix these for TeamCity"), Category("Unit")] //TODO: Fix these for TeamCity
	public class AssemblyHelperTests
	{
		[Test]
		public void AssemblyHelper_InitResolve_Success()
		{
			// Act
			AssemblyHelper.InitResolves();

			// Assert
			Assert.Pass();
		}

		[Test]
		public void GetLatestVersion()
		{
			var version = AssemblyHelper.GetLatestVersion(Assembly.GetExecutingAssembly());
			Assert.That(version, Is.Not.Null);
		}

		[Test, Ignore("TODO: Fix this for TeamCity")]
		public void Resolve_Success()
		{
			// Arrange
			var testAssembly = "TestAsm.dll";
			var currentDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase).Replace(@"file:\", "");
			var fullTestAssembly = Path.Combine(currentDir, testAssembly);
			File.Copy(fullTestAssembly, Path.Combine(currentDir, "TestAsm2.dll"), true);
			File.Delete(fullTestAssembly);
			Assert.That(File.Exists(fullTestAssembly), Is.False);

			// Act
			AssemblyHelper.Resolve("TestAsm", "TestAsm2.dll");
			TestAsmThingWrapper.Run();

			// Assert
			Assert.Pass();
		}

	}


	public class TestAsmThingWrapper
	{
		public static void Run()
		{
			new TestAsm.Thing().DoStuff();
		}
	}
}
