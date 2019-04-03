namespace kCura.PDB.Service.Audits
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Interfaces.Apm;
	using kCura.PDB.Core.Interfaces.Audits;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.Audits;

	public class WorkspaceAuditApmReporter : IWorkspaceAuditApmReporter
	{
		private readonly IApmMetricsService apmMetricsService;

		public WorkspaceAuditApmReporter(IApmMetricsService apmMetricsService)
		{
			this.apmMetricsService = apmMetricsService;
		}

		public void ReportAuditDataToApm(IList<SearchAuditGroup> workspaceSearchAudits, Hour hour)
		{
			if (!workspaceSearchAudits.Any()) return;
			var workspaceId = workspaceSearchAudits.First().WorkspaceId;
			var correlationId = $"{hour.Id}-{workspaceId}";
			var additionalData = new { Hour = hour.HourTimeStamp, HourId = hour.Id, Workspace = workspaceId };
			this.SendAuditMetric(Names.Apm.TotalCount, this.TotalCount, workspaceSearchAudits, correlationId, additionalData);
			this.SendAuditMetric(Names.Apm.TotalAuditGroupCount, this.TotalAuditGroupCount, workspaceSearchAudits, correlationId, additionalData);
			this.SendAuditMetric(Names.Apm.SimpleQueryNonAdhoc, this.SimpleQueryNonAdhocCount, workspaceSearchAudits, correlationId, additionalData);
			this.SendAuditMetric(Names.Apm.SimpleQueryAdhoc, this.SimpleQueryAdhocCount, workspaceSearchAudits, correlationId, additionalData);
			this.SendAuditMetric(Names.Apm.ComplexQueryNonAdhoc, this.ComplexQueryNonAdhocCount, workspaceSearchAudits, correlationId, additionalData);
			this.SendAuditMetric(Names.Apm.ComplexQueryAdhoc, this.ComplexQueryAdhocCount, workspaceSearchAudits, correlationId, additionalData);
			this.SendAuditMetric(Names.Apm.SimpleLonRunningQueryNonAdhoc, this.SimpleLonRunningQueryNonAdhoc, workspaceSearchAudits, correlationId, additionalData);
			this.SendAuditMetric(Names.Apm.SimpleLonRunningQueryAdhoc, this.SimpleLonRunningQueryAdhoc, workspaceSearchAudits, correlationId, additionalData);
			this.SendAuditMetric(Names.Apm.ComplexLonRunningQueryNonAdhoc, this.ComplexLonRunningQueryNonAdhoc, workspaceSearchAudits, correlationId, additionalData);
			this.SendAuditMetric(Names.Apm.ComplexLonRunningQueryAdhoc, this.ComplexLonRunningQueryAdhoc, workspaceSearchAudits, correlationId, additionalData);
		}

		internal void SendAuditMetric<T>(string gaugeName, Func<IList<SearchAuditGroup>, int> func, IList<SearchAuditGroup> workspaceSearchAudits, string correlationId, T additionalData) =>
			this.apmMetricsService.RecordGauge(
				gaugeName,
				func(workspaceSearchAudits),
				Names.Apm.AuditUnitOfMeasure,
				correlationId,
				additionalData);

		internal int TotalCount(IList<SearchAuditGroup> workspaceSearchAudits) => workspaceSearchAudits.SelectMany(a => a.Audits).Count();

		internal int TotalAuditGroupCount(IList<SearchAuditGroup> workspaceSearchAudits) => workspaceSearchAudits.Count;

		internal IEnumerable<SearchAuditGroup> SimpleQueryNonAdhoc(IList<SearchAuditGroup> workspaceSearchAudits) =>
			workspaceSearchAudits.Where(a =>
			!a.IsComplex
			&& a.Audits.Any()
			&& !(a.Audits.First().Search?.IsAdhoc ?? true));
		internal IEnumerable<SearchAuditGroup> SimpleQueryAdhoc(IList<SearchAuditGroup> workspaceSearchAudits) =>
			workspaceSearchAudits.Where(a =>
			!a.IsComplex
			&& a.Audits.Any()
			&& (a.Audits.First().Search?.IsAdhoc ?? true));
		internal IEnumerable<SearchAuditGroup> ComplexQueryNonAdhoc(IList<SearchAuditGroup> workspaceSearchAudits) =>
			workspaceSearchAudits.Where(a =>
			a.IsComplex
			&& a.Audits.Any()
			&& !(a.Audits.First().Search?.IsAdhoc ?? true));
		internal IEnumerable<SearchAuditGroup> ComplexQueryAdhoc(IList<SearchAuditGroup> workspaceSearchAudits) =>
			workspaceSearchAudits.Where(a =>
			a.IsComplex
			&& a.Audits.Any()
			&& (a.Audits.First().Search?.IsAdhoc ?? true));

		internal int SimpleQueryNonAdhocCount(IList<SearchAuditGroup> workspaceSearchAudits) =>
			this.SimpleQueryNonAdhoc(workspaceSearchAudits)
				.Count();
		internal int SimpleQueryAdhocCount(IList<SearchAuditGroup> workspaceSearchAudits) =>
			this.SimpleQueryAdhoc(workspaceSearchAudits)
				.Count();
		internal int ComplexQueryNonAdhocCount(IList<SearchAuditGroup> workspaceSearchAudits) =>
			this.ComplexQueryNonAdhoc(workspaceSearchAudits)
				.Count();
		internal int ComplexQueryAdhocCount(IList<SearchAuditGroup> workspaceSearchAudits) =>
			this.ComplexQueryAdhoc(workspaceSearchAudits)
				.Count();

		internal int SimpleLonRunningQueryNonAdhoc(IList<SearchAuditGroup> workspaceSearchAudits) =>
			this.SimpleQueryNonAdhoc(workspaceSearchAudits)
				.Count(SearchAnalysisService.IsLongRunning);
		internal int SimpleLonRunningQueryAdhoc(IList<SearchAuditGroup> workspaceSearchAudits) =>
			this.SimpleQueryAdhoc(workspaceSearchAudits)
				.Count(SearchAnalysisService.IsLongRunning);

		internal int ComplexLonRunningQueryNonAdhoc(IList<SearchAuditGroup> workspaceSearchAudits) =>
			this.ComplexQueryNonAdhoc(workspaceSearchAudits)
				.Count(SearchAnalysisService.IsLongRunning);
		internal int ComplexLonRunningQueryAdhoc(IList<SearchAuditGroup> workspaceSearchAudits) =>
			this.ComplexQueryAdhoc(workspaceSearchAudits)
				.Count(SearchAnalysisService.IsLongRunning);

	}
}
