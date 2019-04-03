namespace kCura.PDB.Service.DatabaseDeployment
{
	using System;
	using System.IO;
	using System.IO.Compression;
	using kCura.PDB.Core.Interfaces.Services;

	public class RoundHouseFileService : IRoundHouseFileService
	{
		#region Directory Information Objects

		protected string _unzipDirectory { get; set; }

		#endregion

		/// <summary>
		/// Unzips the resource file that is zipped up through the post build event
		/// to the app data directory in memory, returns a file info to the Unzipped
		/// location in memory
		/// </summary>
		/// <returns></returns>
		public string UnzipResourceFile(byte[] zippedBytes)
		{
			//construct database unzip path
			//String constructedPath = Path.Combine(_workingDirectory, String.Format(@"{0}{1}", DATABASE_DIRECTORY_NAME, Guid.NewGuid().ToString("N")));
			_unzipDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
			var databaseDirectory = new DirectoryInfo(_unzipDirectory);
			if (!databaseDirectory.Exists)
			{
				databaseDirectory.Create();

				//decompress the zip file bytes to the database unzip path
				Decompress(zippedBytes, _unzipDirectory);
			}

			//create and verify unzipped database contents
			var databaseContentsDirectory = new DirectoryInfo(databaseDirectory.FullName);
			if (!databaseContentsDirectory.Exists) throw new Exception("The contents of the database folder do not exist");

			return databaseContentsDirectory.FullName;
		}

		/// <summary>
		/// Decompress zip file from resource folder
		/// </summary>
		/// <param name="zippedData"></param>
		/// <param name="filePath"></param>
		public static void Decompress(byte[] zippedData, String filePath)
		{
			using (var memoryStream = new MemoryStream(zippedData))
			using (var archive = new ZipArchive(memoryStream))
			{
				foreach (var entry in archive.Entries)
				{
					var zipPath = Path.Combine(filePath, entry.FullName);

					if (String.IsNullOrWhiteSpace(Path.GetFileName(zipPath)))
					{
						Directory.CreateDirectory(zipPath);
					}
					else
					{
						File.Create(zipPath).Close();

						using (var reader = new StreamReader(entry.Open()))
						{
							var scriptText = reader.ReadToEnd();
							File.WriteAllText(zipPath, scriptText);
						}
					}
				}
			}
		}


		/// <summary>
		/// Delete working directories when deployment completes
		/// </summary>
		public void CleanUpAppDataDirectory()
		{
			if (Directory.Exists(_unzipDirectory))
				Directory.Delete(_unzipDirectory, true);
		}

	}
}




