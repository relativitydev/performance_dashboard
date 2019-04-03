namespace kCura.PDB.Service.ProcessControls.ProcessTasks.EnvironmentCheck
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Linq;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Enumerations;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.ProcessControls;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Service.ProcessControls.ProcessTasks;
	using kCura.Relativity.Client.DTOs;

	[Description(AnalyticsCheckMemoryCheck.AnalyticsCheckMemoryCheckDescription)]
	public class AnalyticsCheckMemoryCheck : BaseProcessControlTask, IProcessControlTask
	{

		public AnalyticsCheckMemoryCheck(IWMIHelper wmiHelper, IWorkspaceRepository workspaceRepository, ILogger logger, ISqlServerRepository sqlRepo, Int32 agentId)
			: base(logger, sqlRepo, agentId)
		{
			this.wmiHelper = wmiHelper;
			this.workspaceRepository = workspaceRepository;
		}

		private const string AnalyticsCheckMemoryCheckDescription = "Environment Check - Analytics Memory Check";
		private readonly IWMIHelper wmiHelper;
		private readonly IWorkspaceRepository workspaceRepository;

		public bool Execute(ProcessControl processControl)
		{
			var workspaces = workspaceRepository.ReadAll();

			var allServers = GetServers();

			// FIRST: 
			// Where servers is Sql
			// select indexed documents for each analytics server
			// 
			// SECOND:
			// Where server is Analytics
			// select server memory
			// foreach server compare memory and indexed documents
			var analyticsServerDocuments = allServers
				.Where(s => s.ServerTypeId == (int)ServerType.Database)
				.Select(s => GetServerIndexedDocuments(s, workspaces))
				.SelectMany(d => d) // start to merge all the server documents
				.GroupBy(g => g.Key) // group by analytics server id
				.Select(g => new // select analytics server id and sum of all docs indexed on that server
				{
					g.Key,
					Searchable = g.Select(kvp => kvp.Value.Searchable).Sum(),
					Training = g.Select(kvp => kvp.Value.Training).Sum()
				})
				.ToDictionary(g => g.Key, v => new CaatIndexDocuments { Searchable = v.Searchable, Training = v.Training }); //back to dictionary

			// process the analytics servers
			allServers
				.Where(s => s.ServerTypeId == (int)ServerType.Analytics)
				.Select(s => new
					{
						Server = s,
						ServerMemory = GetServerMemory(s),
						SearchableDocuments = analyticsServerDocuments.ContainsKey(s.ServerId) ? analyticsServerDocuments[s.ServerId].Searchable : 0,
						TrainingDocuments = analyticsServerDocuments.ContainsKey(s.ServerId) ? analyticsServerDocuments[s.ServerId].Training : 0
					})
				.ForEach(s => SaveServerRecommendation(s.Server, s.ServerMemory, s.SearchableDocuments, s.TrainingDocuments));
			return true;
		}

		public void SaveServerRecommendation(Server server, int serverMemory, long searchableDocumentsOnServer, long trainingDocumentsOnServer)
		{
			if (serverMemory == 0)
				throw new ArgumentException("Invalid Server Memory. Cannot be zero.");

			var million = 1000.0 * 1000.0;
			
			// Recommendation: 1 Million searchable documents in the index require 6 GB of free RAM
			var searchableDocsPerSixGb = 6.0 * (searchableDocumentsOnServer / million) / (serverMemory / 1024.0);
			var searchableDocsRecomendation = (searchableDocumentsOnServer < million || searchableDocsPerSixGb <= 1.0)
					? Guids.EnvironmentCheck.CaatMemoryPerSearchableDocumentsDefaultGood
					: Guids.EnvironmentCheck.CaatMemoryPerSearchableDocumentsDefaultWarning;
			
			// Recommendation: 1 Million training documents in the index require 6 GB of free RAM
			var trainingDocsPerSixGb = 6.0 * (trainingDocumentsOnServer / million) / (serverMemory / 1024.0);
			var trainingDocsRecomendation = (trainingDocumentsOnServer < million || trainingDocsPerSixGb <= 1.0)
					? Guids.EnvironmentCheck.CaatMemoryPerTrainingDocumentsDefaultGood
					: Guids.EnvironmentCheck.CaatMemoryPerTrainingDocumentsDefaultWarning;

			SqlRepo.AnalyticsRepository.SaveAnalyticsRecommendation(
				server,
				searchableDocsRecomendation,
				$"Searchable Documents per 6 GB of Memory on Server: {searchableDocumentsOnServer:F2}");
			SqlRepo.AnalyticsRepository.SaveAnalyticsRecommendation(
				server,
				trainingDocsRecomendation,
				$"Training Documents per 6 GB of Memory on Server: {searchableDocumentsOnServer:F2}");
		}

		public int GetServerMemory(Server server)
		{
			var totalServerMemory = wmiHelper.CreateDiagnostics(server, ManagementField.TotalVisibleMemorySize, "Win32_OperatingSystem", string.Empty);
			if (totalServerMemory.Count > 0)
			{
				return (int)(decimal.Parse(totalServerMemory[0].Value) / 1024); //in MB
			}

			throw new Exception("Couldn't read analytics server's memory.");
		}

		public Dictionary<int, CaatIndexDocuments> GetServerIndexedDocuments(Server server, List<Workspace> workspaces)
		{
			var serverWorkspaces = workspaces.Where(w => w.ServerID == server.ArtifactId);
			var totalServerIndexedDocs = new Dictionary<int, CaatIndexDocuments>();
			foreach (var workspace in serverWorkspaces)
			{
				try
				{
					var workspaceCaatPopTables = SqlRepo.AnalyticsRepository.ReadCaatPopTables(workspace.ArtifactID);
					var workspaceCaatIndexes = SqlRepo.AnalyticsRepository.ReadCaatIndexes(workspace.ArtifactID);
					var workspaceSearchableDocs = SqlRepo.AnalyticsRepository.ReadCaatSearchableDocuments(workspace.ArtifactID, workspaceCaatPopTables, workspaceCaatIndexes);
					var workspaceTrainingDocs = SqlRepo.AnalyticsRepository.ReadCaatTrainingDocuments(workspace.ArtifactID, workspaceCaatPopTables, workspaceCaatIndexes);
					
					// merge results into total
					MergeIndexedDocuments(totalServerIndexedDocs,
						workspaceSearchableDocs,
						workspaceTrainingDocs);
				}
				catch (Exception ex)
				{
					// log error and skip
					LogError("Couldn't query workspace's analytics searchable documents. Workspace: {0} ({1}). {2}", workspace.Name, workspace.ArtifactID, ex.ToString());
				}
			}

			return totalServerIndexedDocs;
		}

		private static void MergeIndexedDocuments(Dictionary<int, CaatIndexDocuments> total, Dictionary<int, long> searchableDocs, Dictionary<int, long> trainingDocs)
		{
			// return new[] { total, workspaceDocs }
			//				.SelectMany(d => d) //Select all key value pairs
			//				.GroupBy(kvp => kvp.Key) //group by the server ids
			//				.Select(g => new { g.Key, Sum = g.Select(kvp => kvp.Value).Sum() }) //sum the indexes per server
			//				.ToDictionary(k => k.Key, v => v.Sum); //back dictionary
			searchableDocs
				.Where(wd => total.ContainsKey(wd.Key))
				.ForEach(wd => total[wd.Key].Searchable += wd.Value);
			searchableDocs
				.Where(wd => total.ContainsKey(wd.Key) == false)
				.ForEach(wd => total.Add(wd.Key, new CaatIndexDocuments { Searchable = wd.Value }));
			trainingDocs
				.Where(wd => total.ContainsKey(wd.Key))
				.ForEach(wd => total[wd.Key].Training += wd.Value);
			trainingDocs
				.Where(wd => total.ContainsKey(wd.Key) == false)
				.ForEach(wd => total.Add(wd.Key, new CaatIndexDocuments { Training = wd.Value }));
		}

		public ProcessControlId ProcessControlID => ProcessControlId.EnvironmentCheckServerInfo;

		public class CaatIndexDocuments
		{
			public long Searchable { get; set; }
			public long Training { get; set; }
		}
	}
}
