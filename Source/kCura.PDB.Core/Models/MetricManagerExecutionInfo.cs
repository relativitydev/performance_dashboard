namespace kCura.PDB.Core.Models
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class MetricManagerExecutionInfo
	{
		public DateTime Start { get; } = DateTime.UtcNow;

		public Guid ExecutionId { get; } = Guid.NewGuid();

		public Meter ResolveOrphanedEventLocks { get; set; } = new Meter();

		public Meter ResolveTimedOutEvents { get; set; } = new Meter();

		public Meter CreateHourProcessingEvents { get; set; } = new Meter();

		public Meter EnqueueTasksForPendingEvents { get; set; } = new Meter();

		public Meter CheckIfAgentIsDisabled { get; set; } = new Meter();

		public Meter InitialWork { get; set; } = new Meter();

		public Meter TimeoutDelays { get; set; } = new Meter();

		public Meter ManagerMainLoops { get; set; } = new Meter();

		public override string ToString() => ToStats().Select(MeterToString).Aggregate((x, acc) => $"{x};\r\n {acc}");

		public IList<MetricManagerExecutionStat> ToStats()
		{
			return new[]
			{
				ToStat(nameof(ResolveOrphanedEventLocks), ResolveOrphanedEventLocks),
				ToStat(nameof(ResolveTimedOutEvents), ResolveTimedOutEvents),
				ToStat(nameof(CreateHourProcessingEvents), CreateHourProcessingEvents),
				ToStat(nameof(EnqueueTasksForPendingEvents), EnqueueTasksForPendingEvents),
				ToStat(nameof(CheckIfAgentIsDisabled), CheckIfAgentIsDisabled),
				ToStat(nameof(InitialWork), InitialWork),
				ToStat(nameof(TimeoutDelays), TimeoutDelays),
				ToStat(nameof(ManagerMainLoops), ManagerMainLoops),
			};
		}

		private MetricManagerExecutionStat ToStat(string name, Meter meter)
		{
			return new MetricManagerExecutionStat
			{
				Start = this.Start,
				End = DateTime.UtcNow,
				ExecutionId = ExecutionId,
				Name = name,
				TotalTime = meter.ElapsedTime.TotalSeconds,
				Count = meter.Count,
				MaxTime = meter.MaxElapsedTime.TotalSeconds,
			};
		}

		private string MeterToString(MetricManagerExecutionStat stat) => $"{stat.Name}: Total={stat.TotalTime:N2}; Count={stat.Count}; Max={stat.MaxTime:N2}";

	}
}
