namespace kCura.PDB.Core.Extensions
{
	using System;
	using System.Diagnostics;
	using System.Linq.Expressions;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Services;

	public static class StopwatchExtensions
	{
		public static IDisposableStopwatch AsDisposable(this Stopwatch stopwatch) =>
			new DisposableStopwatch(stopwatch);

		public static IDisposableStopwatch AsMeter<T>(this Stopwatch stopwatch, T obj, Expression<Func<T, TimeSpan>> expression) =>
			new TimeSpanDisposableStopwatch<T>(stopwatch, obj, expression);

		public static IMeterRecorder AsMeter<T>(this Stopwatch stopwatch, T obj, Expression<Func<T, Meter>> expression) =>
			new MeterDisposableStopwatch<T>(stopwatch, obj, expression);
	}
}
