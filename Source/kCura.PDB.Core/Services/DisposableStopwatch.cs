namespace kCura.PDB.Core.Services
{
	using System;
	using System.Diagnostics;
	using System.Linq.Expressions;
	using System.Reflection;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;

	public class DisposableStopwatch : IDisposableStopwatch
	{
		private readonly Stopwatch stopwatch;

		public DisposableStopwatch(Stopwatch stopwatch)
		{
			this.stopwatch = stopwatch;
		}

		public TimeSpan Elapsed => this.stopwatch.Elapsed;

		public void Dispose()
		{
			this.Dispose(true);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.stopwatch.Stop();
			}
		}
	}

	public class TimeSpanDisposableStopwatch<T> : DisposableStopwatch
	{
		private readonly PropertyInfo propertyInfo;
		private readonly T obj;
		public TimeSpanDisposableStopwatch(Stopwatch stopwatch, T obj, Expression<Func<T, TimeSpan>> expression)
			: base(stopwatch)
		{
			this.propertyInfo = expression.IsProperty() ? expression.ToPropertyInfo() : null;
			this.obj = obj;
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(true);
			if (disposing)
			{
				if (propertyInfo == null || propertyInfo.PropertyType != typeof(TimeSpan)) return;
				var currentValue = (TimeSpan)propertyInfo.GetValue(obj, null);
				propertyInfo.SetValue(obj, this.Elapsed + currentValue);
			}
		}
	}

	public class MeterDisposableStopwatch<T> : DisposableStopwatch, IMeterRecorder
	{
		private readonly PropertyInfo propertyInfo;
		private readonly T obj;
		public MeterDisposableStopwatch(Stopwatch stopwatch, T obj, Expression<Func<T, Meter>> expression)
			: base(stopwatch)
		{
			this.propertyInfo = expression.IsProperty() ? expression.ToPropertyInfo() : null;
			this.obj = obj;
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(true);
			if (disposing)
			{
				if (propertyInfo == null || propertyInfo.PropertyType != typeof(Meter)) return;
				var currentValue = ((Meter)propertyInfo.GetValue(obj, null)) ?? new Meter();
				currentValue.Increment(this.Elapsed);
				propertyInfo.SetValue(obj, currentValue);
			}
		}

		public void Increment()
		{
			if (propertyInfo == null || propertyInfo.PropertyType != typeof(Meter)) return;
			var currentValue = ((Meter)propertyInfo.GetValue(obj, null)) ?? new Meter();
			currentValue.Increment();
			propertyInfo.SetValue(obj, currentValue);
		}
	}
}
