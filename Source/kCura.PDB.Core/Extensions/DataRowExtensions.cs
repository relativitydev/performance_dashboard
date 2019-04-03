namespace kCura.PDB.Core.Extensions
{
	using System;
	using System.Data;

	public static class DataRowExtensions
	{
		/// <summary>
		/// Same as DataRowExtensions.Field&lt;T&gt;(row, column) expect on error it includes column name in error message
		/// </summary>
		/// <typeparam name="T">case type</typeparam>
		/// <param name="row">the data row</param>
		/// <param name="columnName">name of the column</param>
		/// <returns>Data as explicit type</returns>
		public static T GetField<T>(this DataRow row, string columnName)
		{
			try
			{
				return System.Data.DataRowExtensions.Field<T>(row, columnName);
			}
			catch (Exception ex)
			{
				throw new Exception($"{columnName}: {ex.Message}", ex);
			}
		}
	}
}
