namespace kCura.PDB.Data.Repositories.BISSummary
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;
	using System.Linq;
	using System.Text.RegularExpressions;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Data.Properties;
	using kCura.PDB.Data.Services;

	public class AnalyticsRepository : BaseRepository, IAnalyticsRepository
	{
		public AnalyticsRepository(IConnectionFactory connectionFactory,
			IEnvironmentCheckRepository environmentCheckRepository)
			: base(connectionFactory)
		{

			_environmentCheckRepository = environmentCheckRepository;
		}

		private readonly IEnvironmentCheckRepository _environmentCheckRepository;


		public void SaveAnalyticsRecommendation(Server analyticsServer, Guid id, String value)
		{
			_environmentCheckRepository.SaveRecommendation(id, analyticsServer.ServerName, value);
		}

		public IEnumerable<String> ReadCaatPopTables(int workspaceId)
		{
			return ExecuteReadCaatPopTables(workspaceId).ToList();
		}

		private IEnumerable<String> ExecuteReadCaatPopTables(int workspaceId)
		{
			using (var conn = (SqlConnection)this.connectionFactory.GetWorkspaceConnection(workspaceId))
			{
				var data = SqlHelper.ExecuteDataset(conn, CommandType.Text, Properties.Resources.Read_CAATPopTables);

				if (data.Tables.Count > 0 && data.Tables[0].Rows.Count > 0)
				{
					foreach (DataRow record in data.Tables[0].Rows)
					{
						yield return record.Field<String>("name");
					}
				}
			}
		}

		public IEnumerable<int> ReadCaatIndexes(int workspaceId)
		{
			return ExecuteReadCaatIndexes(workspaceId).ToList();
		}
		private IEnumerable<int> ExecuteReadCaatIndexes(int workspaceId)
		{
			using (var conn = (SqlConnection)this.connectionFactory.GetWorkspaceConnection(workspaceId))
			{
				var data = SqlHelper.ExecuteDataset(conn, CommandType.Text, "select ID from eddsdbo.ContentAnalystIndex");

				if (data.Tables.Count > 0 && data.Tables[0].Rows.Count > 0)
				{
					foreach (DataRow record in data.Tables[0].Rows)
					{
						yield return record.Field<int>("ID");
					}
				}
			}
		}


		public Dictionary<Int32, long> ReadCaatDocuments(int workspaceId, IEnumerable<String> caatPopTables, IEnumerable<Int32> caatIndexIds, String popTableDocsQuery)
		{
			var results = new Dictionary<Int32, long>();
			if (caatPopTables == null || caatIndexIds == null || caatPopTables.Any() == false || caatIndexIds.Any() == false) return results;

			var popTableQueries = caatPopTables
				.Select(t => new { t, Matches = Regex.Matches(t, @"Zca_POP_(\d+)_\d+", RegexOptions.IgnoreCase) })//match the pop table name to a index
				.Where(a => a.Matches.Count == 1 && a.Matches[0].Groups.Count == 2) //make sure that regex returns correct results
				.Select(a => new { a.t, Index = a.Matches[0].Groups[1].Value.TryParse<Int32>() }) //parse out index in pop table name
				.Where(ti => ti.Index.HasValue && caatIndexIds.Contains(ti.Index.Value)) //make sure the parse was success
				.Select(ti => String.Format(popTableDocsQuery, ti.t, ti.Index)); //get select statement for pop table

			if (popTableQueries.Any() == false) return results;

			var tableUnions = popTableQueries.Join("{0} union {1}"); //join all the pop table select statements with a union

			var searchableDocsQuery = String.Format(Resources.Read_CAATIndexSearchableDocumentCount, tableUnions);
			
			using (var conn = (SqlConnection)this.connectionFactory.GetWorkspaceConnection(workspaceId))
			{
				var data = SqlHelper.ExecuteDataset(conn, CommandType.Text, searchableDocsQuery);

				if (data.Tables.Count > 0 && data.Tables[0].Rows.Count > 0)
				{
					foreach (DataRow record in data.Tables[0].Rows)
					{

						var resourceServerId = record.GetField<Int32>("ResourceServerId");
						var count = record.GetField<long>("Count");
						results.Add(resourceServerId, count);
					}
				}
			}

			return results;
		}

		public Dictionary<Int32, long> ReadCaatSearchableDocuments(int workspaceId, IEnumerable<String> caatPopTables, IEnumerable<Int32> caatIndexIds)
		{
			var selectPopTableDocsAndServer =
				@"select ID, ResourceServerId = (select ResourceServerArtifactID from eddsdbo.ContentAnalystIndex where ID = {1})
					from eddsdbo.[{0}]
					where Settype in (1,2)";
			return ReadCaatDocuments(workspaceId, caatPopTables, caatIndexIds, selectPopTableDocsAndServer);
		}

		public Dictionary<Int32, long> ReadCaatTrainingDocuments(int workspaceId, IEnumerable<String> caatPopTables, IEnumerable<Int32> caatIndexIds)
		{
			var selectPopTableDocsAndServer =
				@"select ID, ResourceServerId = (select ResourceServerArtifactID from eddsdbo.ContentAnalystIndex where ID = {1})
					from eddsdbo.[{0}]
					where Settype in (1,2)";
			return ReadCaatDocuments(workspaceId, caatPopTables, caatIndexIds, selectPopTableDocsAndServer);
		}


	}
}
