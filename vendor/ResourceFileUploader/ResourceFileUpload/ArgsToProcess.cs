using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceFileUpload
{
	public class ArgsToProcess
	{
		/*Properties*/
		public string destinationPath;
		public List<string> assemblies = new List<string>();
		public List<CustomPageInfo> custompages = new List<CustomPageInfo>();
		public string webURLServer;
		public string dbCaseServer;
		public string dbCaseUserName;
		public string dbCaseUserPassword;
		public string caseUserName;
		public string caseUserPassword;
		public string caseID;
		public string caseGuid;
		public string certificateFindValue;
		public Guid applicationGuid;
		public bool showHelp;
		public bool diagnosticsCheck;
		public bool updateDlls;

		public ArgsToProcess()
		{ }

		public ArgsToProcess(string[] args)
		{
			Initialize(args);
		}

		public void Initialize(string[] args)
		{
			showHelp = false;
			assemblies = new List<string>();
			custompages = new List<CustomPageInfo>();
			foreach (string arg in args)
			{
				try
				{
					string[] separator = new string[] { ":" };
					string[] key = arg.Split(separator, 2, StringSplitOptions.RemoveEmptyEntries);
					switch (key[0].ToLower())
					{
						case "/destinationpath":
							destinationPath = key[1];
							break;
						case "/assembly":
							assemblies.Add(key[1]);
							break;
						case "/custompage":
							custompages.Add(new CustomPageInfo(key[1]));
							break;
						case "/masterurl":
							webURLServer = key[1];
							break;
						case "/casedbserver":
							dbCaseServer = key[1];
							break;
						case "/casedbusername":
							dbCaseUserName = key[1];
							break;
						case "/casedbuserpassword":
							dbCaseUserPassword = key[1];
							break;
						case "/caseusername":
							caseUserName = key[1];
							break;
						case "/caseuserpassword":
							caseUserPassword = key[1];
							break;
						case "/certificatefindvalue":
							certificateFindValue = key[1];
							break;
						case "/caseid":
							caseID = key[1];
							break;
						case "/applicationguid":
							applicationGuid = new Guid(key[1]);
							break;
						case "/help":
						case "/?":
							showHelp = true;
							break;
						case "/diagnostics":
							diagnosticsCheck = true;
							break;
						case "/updatedlls":
							updateDlls = true;
							break;
					}
					Console.WriteLine(key.Length > 1
						                  ? string.Format("Passed parameter: {0} = {1}", key[0], key[1])
						                  : string.Format("Passed parameter: {0}", key[0]));
				}
				catch (Exception)
				{
					throw new Exception(String.Format("Failed trying to parse arg:{0}",arg));
				}
				
				
			}
		}
	}
}
