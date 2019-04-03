namespace kCura.PDB.Service.BISSummary
{
	using System;
	using System.Data;
	using System.Linq;
	using kCura.PDB.Core.Enumerations;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.BISSummary.Grids;
	using kCura.PDB.Core.Models.BISSummary.Models;

	public class EnvironmentCheckService : BestInServiceReportingService
	{
		private readonly IEnvironmentCheckRepository environmentCheckRepository;

		public EnvironmentCheckService(ISqlServerRepository sqlServerRepository)
			: base(sqlServerRepository)
		{
			this.environmentCheckRepository = sqlServerRepository.EnvironmentCheckRepository;
		}

		public virtual GeneralCheckGrid<EnvironmentCheckRecommendation> Recommendations(GridConditions gridConditions, EnvironmentCheckRecommendationFilterConditions filterConditions)
		{
			var grid = new GeneralCheckGrid<EnvironmentCheckRecommendation>();
			var dt = this.environmentCheckRepository.GetRecomendations(gridConditions, filterConditions);
			var gridData = (from DataRow d in dt.Rows
							select new EnvironmentCheckRecommendation
							{
								Recommendation = d.Field<String>("Recommendation"),
								Scope = d.Field<String>("Scope"),
								Section = d.Field<String>("Section"),
								Status = d.Field<String>("Status"),
								Description = d.Field<String>("Description"),
								Name = d.Field<String>("Name"),
								Value = d.Field<String>("Value"),
							}).ToList();

			grid.Count = gridData.Count();

			grid.Data = gridData.AsQueryable();

			if (0 < gridConditions.StartRow)
				grid.Data = grid.Data.Skip(gridConditions.StartRow - 1);
			if (0 < gridConditions.EndRow)
				grid.Data = grid.Data.Take(gridConditions.EndRow - gridConditions.StartRow + 1);

			return grid;
		}


		public virtual GeneralCheckGrid<EnvironmentCheckServerDetails> ServerDetails(GridConditions gridConditions, EnvironmentCheckServerFilterConditions filterConditions, EnvironmentCheckServerFilterOperands filterOperands)
		{
			var grid = new GeneralCheckGrid<EnvironmentCheckServerDetails>();
			var dt = this.environmentCheckRepository.GetServerDetails(gridConditions, filterConditions, filterOperands);
			var gridData = (from DataRow d in dt.Rows
							select new EnvironmentCheckServerDetails
							{
								Hyperthreaded = d.Field<Boolean>("Hyperthreaded"),
								LogicalProcessors = d.Field<Int32>("LogicalProcessors"),
								OSName = d.Field<String>("OSName"),
								OSVersion = d.Field<String>("OSVersion"),
								ServerName = d.Field<String>("ServerName"),
							}).ToList();

			grid.Count = gridData.Count();

			grid.Data = gridData.AsQueryable();

			if (0 < gridConditions.StartRow)
				grid.Data = grid.Data.Skip(gridConditions.StartRow - 1);
			if (0 < gridConditions.EndRow)
				grid.Data = grid.Data.Take(gridConditions.EndRow - gridConditions.StartRow + 1);

			return grid;
		}

		public virtual GeneralCheckGrid<EnvironmentCheckDatabaseDetails> DatabaseDetails(GridConditions gridConditions, EnvironmentCheckDatabaseFilterConditions filterConditions, EnvironmentCheckDatabaseFilterOperands filterOperands)
		{
			var grid = new GeneralCheckGrid<EnvironmentCheckDatabaseDetails>();
			var dt = this.environmentCheckRepository.GetDatabaseDetails(gridConditions, filterConditions, filterOperands);
			var gridData = (from DataRow d in dt.Rows
							select new EnvironmentCheckDatabaseDetails
							{
								AdHocWorkLoad = d.Field<Int32?>("AdHocWorkLoad") ?? 0,
								LastSQLRestart = d.Field<DateTime?>("LastSQLRestart") ?? DateTime.MinValue,
								MaxDegreeOfParallelism = d.Field<Int32?>("MaxDegreeOfParallelism") ?? 0,
								MaxServerMemory = Convert.ToDouble(d["MaxServerMemory"]),
								ServerName = d.Field<String>("ServerName"),
								SQLVersion = d.Field<String>("SQLVersion"),
								TempDBDataFiles = d.Field<Int32?>("TempDBDataFiles") ?? 0,
							}).ToList();

			grid.Count = gridData.Count();

			grid.Data = gridData.AsQueryable();

			if (0 < gridConditions.StartRow)
				grid.Data = grid.Data.Skip(gridConditions.StartRow - 1);
			if (0 < gridConditions.EndRow)
				grid.Data = grid.Data.Take(gridConditions.EndRow - gridConditions.StartRow + 1);

			return grid;
		}

		public DateTime ResetTuningForkLastRun(params ProcessControlId[] lastRunProcessControlTypes)
		{
			return ResetProcessControlLastRun(lastRunProcessControlTypes);
		}

		public DateTime TuningForkLastRun(params ProcessControlId[] lastRunProcessControlTypes)
		{
			return ProcessControlLastRun(lastRunProcessControlTypes);
		}
	}
}
