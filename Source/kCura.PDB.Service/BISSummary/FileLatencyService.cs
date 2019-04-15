namespace kCura.PDB.Service.BISSummary
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Linq;
	using kCura.PDB.Core.Enumerations;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.BISSummary.Grids;
	using kCura.PDB.Core.Models.BISSummary.Models;

	public class FileLatencyService : BestInServiceReportingService
	{
		private readonly IFileLatencyRepository fileLatencyRepository;

		public FileLatencyService(ISqlServerRepository sqlServerRepository)
			: base(sqlServerRepository)
		{
			this.fileLatencyRepository = sqlServerRepository.FileLatencyRepository;
		}

		public virtual GeneralCheckGrid<FileLatency> FileLatencies(GridConditions gridConditions, Dictionary<FileLatency.Columns, String> filterConditions, Dictionary<FileLatency.Columns, FilterOperand> filterOperands)
		{
			var grid = new GeneralCheckGrid<FileLatency>();
			var dt = this.fileLatencyRepository.GetFileLevelLatencyDetails(gridConditions, filterConditions, filterOperands);
			var gridData = (from DataRow d in dt.Rows
							select new FileLatency
							{
								ServerName = d.GetField<String>("ServerName"),
								DatabaseName = d.GetField<String>("DatabaseName"),
								Score = d.GetField<decimal?>("Score"),
								DataReadLatency = d.GetField<long?>("DataReadLatency"),
								DataWriteLatency = d.GetField<long?>("DataWriteLatency"),
								LogReadLatency = d.GetField<long?>("LogReadLatency"),
								LogWriteLatency = d.GetField<long?>("LogWriteLatency"),
							}).ToList();

			grid.Count = gridData.Count();

			grid.Data = gridData.AsQueryable();

			if (0 < gridConditions.StartRow)
				grid.Data = grid.Data.Skip(gridConditions.StartRow - 1);
			if (0 < gridConditions.EndRow)
				grid.Data = grid.Data.Take(gridConditions.EndRow - gridConditions.StartRow + 1);

			return grid;
		}

		public DateTime ResetFileLatencyLastRun()
		{
			return ResetProcessControlLastRun(ProcessControlId.CollectWaitStatistics);
		}

		public DateTime FileLatencyLastRun()
		{
			return ProcessControlLastRun(ProcessControlId.CollectWaitStatistics);
		}
	}
}
