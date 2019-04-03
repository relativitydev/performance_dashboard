namespace kCura.PDB.Core.Extensions
{
    using System;
    using System.Diagnostics;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Services;

	public static class LoggerExtensions
	{
		public static ILogger WithCategory(this ILogger logger, string category)
		{
			return new CategorizedLogger(logger, category);
		}

		public static ILogger WithClassName(this ILogger logger)
		{
			var method = new StackTrace()?.GetFrame(1)?.GetMethod();
			var classname = method?.ReflectedType?.Name;
			if (classname == null)
				return logger;
			return new CategorizedLogger(logger, classname);
		}

	    public static ILogger WithTypeName<T>(this ILogger logger, T o)
	    {
	        var className = o.GetType().Name;
	        return new CategorizedLogger(logger, className);
	    }
    }
}
