using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using kCura.Relativity.Client;

namespace ResourceFileUpload
{
	public class ResourceFileUploader
	{
			public ResourceFileUploader()
			{
			}

			public event EventHandler<CustomEventArgs> RaiseCustomEvent;
			
			protected virtual void OnRaiseCustomEvent(CustomEventArgs e)
			{
				EventHandler<CustomEventArgs> handler = RaiseCustomEvent;

				if (handler != null)
				{
					e.Message += String.Format(" at {0}", DateTime.UtcNow.ToShortTimeString());
					handler(this, e);
				}
			}
			public void UpdateResourceFiles(ArgsToProcess parsedArgs)
			{
				ArtifactManagerProxy apiProxy;
				try
				{
					apiProxy = GetLoggedInRSAPIProxy(parsedArgs);
				}
				catch (Exception ex)
				{
					throw new Exception("Couldnt connect to RSAPI!", ex);
				}
				UpdateResourceFiles(apiProxy, parsedArgs.assemblies, parsedArgs.applicationGuid);
			}

			public void DiagnosticsCheck(ArgsToProcess parsedArgs)
			{
				try
				{
					var apiProxy = GetLoggedInRSAPIProxy(parsedArgs);
				}
				catch (Exception ex)
				{
					throw new Exception("Couldnt connect to RSAPI!",ex);
					throw;
				}
			}
			public void RunUpdater(ArgsToProcess parsedargs)
			{
				try
				{
					System.Data.SqlClient.SqlConnection connCase = GetSQLConnection(parsedargs.dbCaseServer, string.Format("EDDS{0}", parsedargs.caseID), parsedargs.dbCaseUserName, parsedargs.dbCaseUserPassword);
					connCase.Open();

					Int32 appID = GetApplicationArtifactID(connCase, parsedargs.applicationGuid);

					log("Step 1: Connecting the RSAPI");
					var apiProxy = GetLoggedInRSAPIProxy(parsedargs);
					
					log("Step 2: Unlocking the Application");
					//RSAPI does NOT support updates on relativity application DTO's. Time for hard sql
					UnlockApp(connCase, appID);
					log("Step 3: Updating Assemblies");
					UpdateResourceFiles(apiProxy, parsedargs.assemblies, parsedargs.applicationGuid);
					log("Step 4: Updating Custom Pages");
					UpdateCustomPages(apiProxy, parsedargs.custompages);
					if (parsedargs.custompages.Count > 0)
					{
						PushToLibrary(apiProxy, parsedargs.applicationGuid, appID);
					}
					if (!String.IsNullOrEmpty(parsedargs.destinationPath))
					{	
						log("Step 5: Exporting Application to destination");
						//This makes me sad :(
						if(String.IsNullOrWhiteSpace(apiProxy.APIOptions.Token))
							apiProxy = GetLoggedInRSAPIProxy(parsedargs);
						ExportApplication(apiProxy, appID, parsedargs.destinationPath);
					}

					log("Done");
				}
				catch (Exception ex)
				{
					log(string.Format("Error: {0}\r\n{1}", ex.Message, ex.StackTrace));
					throw;
				}
				finally
				{
					//Close RSAPI connection
				}
			}

		private ArtifactManagerProxy GetLoggedInRSAPIProxy(ArgsToProcess parsedargs)
		{
			var caseUserName = parsedargs.caseUserName;
			var caseUserPassword = parsedargs.caseUserPassword;
			var webURLServer = parsedargs.webURLServer;
			if (String.IsNullOrWhiteSpace(caseUserName)) throw new ArgumentException("CaseUserName cannot be null or empty!");
			if (String.IsNullOrWhiteSpace(caseUserPassword)) throw new ArgumentException("caseUserPassword cannot be null or empty!");
			if (String.IsNullOrWhiteSpace(webURLServer)) throw new ArgumentException("webURLServer cannot be null or empty!");

			ArtifactManagerProxy apiProxy;
			if (String.IsNullOrWhiteSpace(parsedargs.certificateFindValue))
			{
				apiProxy = new ArtifactManagerProxy(new System.Uri(webURLServer), EndpointType.IntegratedHTTP);
			}
			else
			{
				var settings = new ArtifactManagerSettings() { CertificateFindValue = parsedargs.certificateFindValue };
				apiProxy = new ArtifactManagerProxy(new System.Uri(webURLServer), EndpointType.IntegratedHTTP, settings);
			}
			
			apiProxy.LoginWithCredentials(caseUserName, caseUserPassword);
			apiProxy.APIOptions.WorkspaceID = Convert.ToInt32(parsedargs.caseID);
			return apiProxy;
		}

		private void log(string message)
			{
				//Console.WriteLine(message);
				OnRaiseCustomEvent(new CustomEventArgs(message + " started"));
			}

			public System.Data.SqlClient.SqlConnection GetSQLConnection(string sqlServer, string dbName, string userName, string userPassword)
			{
				return
					new System.Data.SqlClient.SqlConnection(
						string.Format(
							"data source={0};initial catalog={1};persist security info=False;user id={2};password={3}; workstation id=localhost;packet size=4096", sqlServer, dbName, userName, userPassword));
			}

			public int GetApplicationArtifactID(System.Data.SqlClient.SqlConnection connCase, Guid appguid)
			{
				SqlCommand command = new SqlCommand();
				command.Connection = connCase;
				command.CommandText = "SELECT [ArtifactID] FROM [EDDSDBO].[ArtifactGuid] WHERE ArtifactGuid=@applicationGUID";

				SqlParameter pAppGuid = new SqlParameter("@applicationGUID", SqlDbType.UniqueIdentifier);
				pAppGuid.Value = appguid;

				command.Parameters.Add(pAppGuid);
				try
				{
					return Convert.ToInt32(command.ExecuteScalar().ToString());
				}
				catch
				{
					throw new Exception("Failed to unlock application.");
				}
			}
			public void UnlockApp(System.Data.SqlClient.SqlConnection connCase, int appID)
			{
				SqlCommand command = new SqlCommand();
				command.Connection = connCase;
				command.CommandText = "UPDATE [EDDSDBO].[RelativityApplication] SET [Locked]=0, [ApplicationIsDirty]=1 WHERE [ArtifactID] =@appID";

				SqlParameter pAppID = new SqlParameter("@appID", SqlDbType.Int);
				pAppID.Value = appID;

				command.Parameters.Add(pAppID);
				var rows = command.ExecuteNonQuery();
				if (rows < 1)
				{
					throw new Exception("Failed to unlock application.");
				}
			}
			public void UpdateResourceFiles(ArtifactManagerProxy apiProxy, List<String> filePaths, Guid appGuid)
			{
				var invalidPaths = filePaths.Where(filepath => !System.IO.File.Exists(filepath)).ToList();
				if (invalidPaths.Any())
				{
					var errorMessage = new System.Text.StringBuilder();
					errorMessage.AppendLine("The following assemblies could not be found...");
					errorMessage.AppendLine();
					invalidPaths.ForEach(x => errorMessage.AppendFormat("* {0}{1}", x, Environment.NewLine));
					errorMessage.AppendLine();
					throw new Exception(errorMessage.ToString());
				}
				
				
				foreach (String filepath in filePaths)
				{
					List<ResourceFileRequest> rfUploadRequests = new List<ResourceFileRequest>();
					ResourceFileRequest rfRequest = new ResourceFileRequest();
					rfRequest.FullFilePath = filepath;
					rfRequest.AppGuid = appGuid;
					rfRequest.FileName = System.IO.Path.GetFileName(filepath);
					rfUploadRequests.Add(rfRequest);
					var res = apiProxy.PushResourceFiles(apiProxy.APIOptions, rfUploadRequests);
					if (!res.Success)
					{
						throw new Exception(String.Format("Uploading resource files through RSAPI failed!{0}{0}File:{1}{0}Message:{2}{0}", Environment.NewLine,filepath, res.Message));
					}
				}
				
			}
			public void UpdateCustomPages(ArtifactManagerProxy apiProxy, List<CustomPageInfo> folderPaths)
			{    
                //Zip the folders up
				//Look up file field info.
				int objectArtifactId;
				int fieldId = GetCustomPageFileFieldArtifactID(apiProxy);
				//Push out to the server
				FileInfo fileInfo;
				UploadRequest uploadReq;
				foreach (CustomPageInfo cpInfo in folderPaths)
				{
					objectArtifactId = GetCustomPageArtifactID(apiProxy, new Guid(cpInfo.GUID));
					
					//Zip the contents of directory containing the custom page:
					string zipName = GetCustomPageName(apiProxy, objectArtifactId) + ".zip";

					if (System.IO.File.Exists(zipName))
					{
						System.IO.File.Delete(zipName);
					}
					System.IO.Compression.ZipFile.CreateFromDirectory(cpInfo.FilePath, zipName, System.IO.Compression.CompressionLevel.Optimal, false);
					
					fileInfo = new FileInfo(zipName);
					uploadReq = new UploadRequest(apiProxy.APIOptions);
					uploadReq.Target.ObjectArtifactId = objectArtifactId;
					uploadReq.Target.FieldId = fieldId;

					uploadReq.Metadata.FileName = zipName;
					uploadReq.Metadata.FileSize = fileInfo.Length;

					uploadReq.Overwrite = true;

					apiProxy.Upload(uploadReq);					
				}
			}
			public void PushToLibrary(ArtifactManagerProxy apiProxy, Guid appGuid, Int32 appId)
			{
				string tempDestination = System.IO.Path.GetTempPath().TrimEnd('/') + "/" + appGuid + ".rap";
				if (System.IO.File.Exists(tempDestination))
				{
					System.IO.File.Delete(tempDestination);
				}
				ExportApplication(apiProxy, appId, tempDestination);
				apiProxy.InstallLibraryApplication(apiProxy.APIOptions, new AppInstallRequest() {FullFilePath= tempDestination });

				System.IO.File.Delete(tempDestination);
				
			}
			public string GetCustomPageName(ArtifactManagerProxy apiProxy, int customPageArtifactID)
			{
				string customPageName = string.Empty;

				ArtifactRequest artifactRequest = new ArtifactRequest("Custom Page", customPageArtifactID);
				artifactRequest.Fields = new List<Field>();
				artifactRequest.Fields.Add(new Field("Name"));
				List<ArtifactRequest> artifactRequestList = new List<ArtifactRequest> { artifactRequest };

				List<Artifact> results = new List<Artifact>();
				ReadResultSet readResultSet = new ReadResultSet();
				readResultSet = apiProxy.Read(apiProxy.APIOptions, artifactRequestList);

				if (readResultSet.Success)
				{
					customPageName = readResultSet.ReadResults[0].Artifact.Name;
				}

				return customPageName;
			}

			public int GetCustomPageArtifactID(ArtifactManagerProxy apiProxy, Guid instanceGUID)
			{
				int returnValue = -1;

				ArtifactRequest artifactRequest = new ArtifactRequest("Custom Page", instanceGUID);

				artifactRequest.Fields = new List<Field>();
				artifactRequest.Fields.Add(new Field("Artifact ID"));
				List<ArtifactRequest> artifactRequestList = new List<ArtifactRequest> { artifactRequest };

				List<Artifact> results = new List<Artifact>();
				ReadResultSet readResultSet = new ReadResultSet();

				readResultSet = apiProxy.Read(apiProxy.APIOptions, artifactRequestList);

				if (readResultSet.Success)
				{
					returnValue = readResultSet.ReadResults[0].Artifact.ArtifactID;
				}
				return returnValue;
			}
			public int GetCustomPageFileFieldArtifactID(ArtifactManagerProxy apiProxy)
			{
				int returnValue = -1;

				Query query = new Query();

				query.ArtifactTypeID = 14;
				query.ArtifactTypeName = "Field";

				query.Fields.Add(new Field("Artifact ID"));

				query.Condition = null;
				query.RelationalField = null;

				query.Condition = new CompositeCondition(new TextCondition("Object Type", TextConditionEnum.EqualTo, "Custom Page"),
																								 CompositeConditionEnum.And,
																								 new TextCondition("Name", TextConditionEnum.EqualTo, "File"));
				QueryResult result;

				result = apiProxy.Query(apiProxy.APIOptions, query, 0);
				if (result.Success)
				{
					returnValue = (int)result.QueryArtifacts[0].ArtifactID;
				}
				return returnValue;
			}

			public void ExportApplication(ArtifactManagerProxy apiProxy, Int32 appID, string destinationPath)
			{
				AppExportRequest appExportReq = new AppExportRequest(appID, destinationPath);
				ResultSet res = null;
				if(System.IO.File.Exists(destinationPath))
				{
					System.IO.File.Delete(destinationPath);
				}
				res = apiProxy.ExportApplication(apiProxy.APIOptions, appExportReq);
				if (!res.Success)
					throw new Exception(res.Message);
			}
	}

	public class CustomEventArgs : EventArgs
	{
		private string msg;

		public CustomEventArgs(string s) { msg = s; }
		
		public string Message 
		{ 
			get { return msg; }
			set { msg = value; }
		}
	}
}
