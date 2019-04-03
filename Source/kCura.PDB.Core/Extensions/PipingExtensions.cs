namespace kCura.PDB.Core.Extensions
{
	using System;
	using System.Threading.Tasks;

	public static class PipingExtensions
	{
		/// <summary>
		/// Take an object, pipe it into a function, and return the result.
		/// </summary>
		/// <typeparam name="T">type of obj</typeparam>
		/// <typeparam name="T2">result type of func</typeparam>
		/// <param name="obj">object</param>
		/// <param name="func">method to call on obj</param>
		/// <returns>Result from func</returns>
		public static T2 Pipe<T, T2>(this T obj, Func<T, T2> func)
		{
			return func(obj);
		}

		/// <summary>
		///  Pipes object it into a function, and returns object.
		/// </summary>
		/// <typeparam name="T">type of obj</typeparam>
		/// <param name="obj">object</param>
		/// <param name="func">method to call on obj</param>
		/// <returns>original obj</returns>
		/// <remarks>useful for chaining into other method calls</remarks>
		public static T Pipe<T>(this T obj, Action<T> func)
		{
			func(obj);
			return obj;
		}

		/// <summary>
		///  Pipes object it into a function, and returns object.
		/// </summary>
		/// <typeparam name="T">type of obj</typeparam>
		/// <param name="obj">object</param>
		/// <param name="func">method to call on obj</param>
		/// <returns>original obj</returns>
		/// <remarks>useful for chaining into other method calls</remarks>
		public static async Task<T> PipeAsync<T>(this Task<T> obj, Action<T> func)
		{
			var o = await obj;
			func(o);
			return o;
		}

		/// <summary>
		///  Pipes object it into a function, and returns object.
		/// </summary>
		/// <typeparam name="T">type of obj</typeparam>
		/// <param name="obj">object</param>
		/// <param name="func">method to call on obj</param>
		/// <returns>original obj</returns>
		/// <remarks>useful for chaining into other method calls</remarks>
		public static async Task<T2> PipeAsync<T, T2>(this Task<T> obj, Func<T, T2> func)
		{
			var o = await obj;
			return func(o);
		}
	}
}
