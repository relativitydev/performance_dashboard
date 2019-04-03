namespace kCura.PDB.Core.Extensions
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
	using System.Diagnostics;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Reflection;
	using kCura.PDB.Core.Helpers;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;

	public static class ObjectExtensions
	{
		public static string GetDisplayName(this Enum val)
		{
			return AttributeHelper.Get<DisplayAttribute>(val)?.Name
				  ?? val.ToString();
		}

		public static string ToJson<T>(this T value)
		{
			return Newtonsoft.Json.JsonConvert.SerializeObject(value);
		}

		public static IDisposableStopwatch AsMeter<T>(this T obj, Expression<Func<T, TimeSpan>> expression) => Stopwatch.StartNew().AsMeter(obj, expression);

		public static IMeterRecorder AsMeter<T>(this T obj, Expression<Func<T, Meter>> expression) => Stopwatch.StartNew().AsMeter(obj, expression);


		public static Dictionary<string, object> ToKeyValuePairs<T>(this T value) =>
			value
				.GetType()
				.GetProperties()
				.Select(p => new { Key = p.Name, Value = p.GetValue(value, null) })
				.ToDictionary(x => x.Key, x => x.Value);

		/* 	Uncomment if needed:
		public static T Get<T>(Enum type)
			where T : Attribute
		{
			var enumType = type.GetType();
			var name = Enum.GetName(enumType, type);
			var enumMemberAttribute = ((T[])GetList<T>(enumType.GetField(name))).SingleOrDefault();
			return enumMemberAttribute;
		}

		public static IEnumerable<T> GetList<T>(MemberInfo type)
			where T : Attribute
		{
			return (IEnumerable<T>)Attribute.GetCustomAttributes(type, typeof(T), true);
		}
		//*/
	}
}
