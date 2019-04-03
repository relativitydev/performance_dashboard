namespace kCura.PDB.Service.Tests.DatabaseDeployment
{
	using System;
	using System.IO;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Service.DatabaseDeployment;
	using kCura.PDB.Service.Tests.Properties;
	using NUnit.Framework;

	[TestFixture, Category("Unit")]
	public class RoundHouseFileServiceTests
	{
		[Test]
		public void Decompress()
		{
			var zipBytes = Resources.ExampleZip;

			//act
			RoundHouseFileService.Decompress(zipBytes, Path.GetTempPath());

			//Assert
			Assert.Pass();
		}

		/// <summary>
		/// Test to make sure the actual directory is being created
		/// </summary>
		[Test]
		public void RoundHouseFileService_SmokeTest()
		{
			var zipService = new RoundHouseFileService();
			var workingDirectory = zipService.UnzipResourceFile(Resources.TestDirectory);
			Assert.IsTrue(Directory.Exists(workingDirectory));
		}

		/// <summary>
		/// Make sure the actual files are being created
		/// </summary>
		[Test]
		public void RoundHouseFileService_AlterDirectory_Exists()
		{
			string folderName = DeploymentDirectoryStructureConstants.AlterDatabaseFolderName;
			var zipService = new RoundHouseFileService();
			var workingDirectory = zipService.UnzipResourceFile(Resources.TestDirectory);
			var direxistsTest = new DirectoryInfo(Path.Combine(workingDirectory, folderName));

			bool result = direxistsTest.Exists;
			zipService.CleanUpAppDataDirectory();
			Assert.IsTrue(result);
		}

		[Test]
		public void RoundHouseFileService_RunAfterCreateOrAlterDirectory_Exists()
		{
			string folderName = DeploymentDirectoryStructureConstants.RunAfterCreateDatabaseFolderName;
			var zipService = new RoundHouseFileService();
			var workingDirectory = zipService.UnzipResourceFile(Resources.TestDirectory);
			var direxistsTest = new DirectoryInfo(Path.Combine(workingDirectory, folderName));

			bool result = direxistsTest.Exists;
			zipService.CleanUpAppDataDirectory();
			Assert.IsTrue(result);
		}

		[Test]
		public void RoundHouseFileService_RunBeforeUpdateDirectory_Exists()
		{
			string folderName = DeploymentDirectoryStructureConstants.RunBeforeUpFolderName;
			var zipService = new RoundHouseFileService();
			var workingDirectory = zipService.UnzipResourceFile(Resources.TestDirectory);
			var direxistsTest = new DirectoryInfo(Path.Combine(workingDirectory, folderName));

			bool result = direxistsTest.Exists;
			zipService.CleanUpAppDataDirectory();
			Assert.IsTrue(result);
		}

		[Test]
		public void RoundHouseFileService_UpdateDirectory_Exists()
		{
			string folderName = DeploymentDirectoryStructureConstants.UpFolderName;
			var zipService = new RoundHouseFileService();
			var workingDirectory = zipService.UnzipResourceFile(Resources.TestDirectory);
			var direxistsTest = new DirectoryInfo(Path.Combine(workingDirectory, folderName));

			bool result = direxistsTest.Exists;
			zipService.CleanUpAppDataDirectory();
			Assert.IsTrue(result);
		}

		[Test]
		public void RoundHouseFileService_LegacyFolderName_Exists()
		{
			string folderName = DeploymentDirectoryStructureConstants.LegacyFolderName;
			var zipService = new RoundHouseFileService();
			var workingDirectory = zipService.UnzipResourceFile(Resources.TestDirectory);
			var direxistsTest = new DirectoryInfo(Path.Combine(workingDirectory, folderName));

			bool result = direxistsTest.Exists;
			zipService.CleanUpAppDataDirectory();
			Assert.IsTrue(result);
		}

		[Test]
		public void RoundHouseFileService_FunctionsDirectory_Exists()
		{
			string folderName = DeploymentDirectoryStructureConstants.FunctionsFolderName;
			var zipService = new RoundHouseFileService();
			var workingDirectory = zipService.UnzipResourceFile(Resources.TestDirectory);
			var direxistsTest = new DirectoryInfo(Path.Combine(workingDirectory, folderName));

			bool result = direxistsTest.Exists;
			zipService.CleanUpAppDataDirectory();
			Assert.IsTrue(result);
		}

		[Test]
		public void RoundHouseFileService_ViewsDirectory_Exists()
		{
			string folderName = DeploymentDirectoryStructureConstants.ViewsFolderName;
			var zipService = new RoundHouseFileService();
			var workingDirectory = zipService.UnzipResourceFile(Resources.TestDirectory);
			var direxistsTest = new DirectoryInfo(Path.Combine(workingDirectory, folderName));

			bool result = direxistsTest.Exists;
			zipService.CleanUpAppDataDirectory();
			Assert.IsTrue(result);
		}

		[Test]
		public void RoundHouseFileService_StoredProcedureDirectory_Exists()
		{
			string folderName = DeploymentDirectoryStructureConstants.SprocsFolderName;
			var zipService = new RoundHouseFileService();
			var workingDirectory = zipService.UnzipResourceFile(Resources.TestDirectory);
			var direxistsTest = new DirectoryInfo(Path.Combine(workingDirectory, folderName));

			bool result = direxistsTest.Exists;
			zipService.CleanUpAppDataDirectory();
			Assert.IsTrue(result);
		}

		[Test]
		public void RoundHouseFileService_IndexesDirectory_Exists()
		{
			string folderName = DeploymentDirectoryStructureConstants.IndexesFolderName;
			var zipService = new RoundHouseFileService();
			var workingDirectory = zipService.UnzipResourceFile(Resources.TestDirectory);
			var direxistsTest = new DirectoryInfo(Path.Combine(workingDirectory, folderName));

			bool result = direxistsTest.Exists;
			zipService.CleanUpAppDataDirectory();
			Assert.IsTrue(result);
		}

		[Test]
		public void RoundHouseFileService_RunAfterAllAnyTimeDirectory_Exists()
		{
			string folderName = DeploymentDirectoryStructureConstants.RunAfterOtherAnyTimeScriptsFolderName;
			var zipService = new RoundHouseFileService();
			var workingDirectory = zipService.UnzipResourceFile(Resources.TestDirectory);
			var direxistsTest = new DirectoryInfo(Path.Combine(workingDirectory, folderName));

			bool result = direxistsTest.Exists;
			zipService.CleanUpAppDataDirectory();
			Assert.IsTrue(result);
		}

		[Test]
		public void RoundHouseFileService_PermissionsDirectory_Exists()
		{
			string folderName = DeploymentDirectoryStructureConstants.PermissionsFolderName;
			var zipService = new RoundHouseFileService();
			var workingDirectory = zipService.UnzipResourceFile(Resources.TestDirectory);
			var direxistsTest = new DirectoryInfo(Path.Combine(workingDirectory, folderName));

			bool result = direxistsTest.Exists;
			zipService.CleanUpAppDataDirectory();
			Assert.IsTrue(result);
		}

		[Test]
		public void RoundHouseFileService_CustomCreateScript_Exists()
		{
			var zipService = new RoundHouseFileService();
			var workingDirectory = zipService.UnzipResourceFile(Resources.TestDirectory);
			var file = new FileInfo(Path.Combine(workingDirectory, DeploymentDirectoryStructureConstants.CreatePerformanceCustomScript));

			bool result = file.Exists;
			zipService.CleanUpAppDataDirectory();
			Assert.IsTrue(result);
		}

		[Test]
		public void RoundHouseFileService_VersionFileScript_Exists()
		{
			var zipService = new RoundHouseFileService();
			var workingDirectory = zipService.UnzipResourceFile(Resources.TestDirectory);
			var file = new FileInfo(Path.Combine(workingDirectory, DeploymentDirectoryStructureConstants.VersionFile));

			bool result = file.Exists;
			zipService.CleanUpAppDataDirectory();
			Assert.IsTrue(result);
		}
	}
}
