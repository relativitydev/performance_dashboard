namespace kCura.PDB.Service.HealthChecks
{
	using kCura.PDB.Core.Interfaces.HealthChecks;

	public class SqlHealthPresenter
	{
		private IViewHealth _view;

		/// <summary>
		/// Initializes a new instance of the <see cref="SqlHealthPresenter"/> class.
		/// Assign view and task 
		/// </summary>
		/// <param name="view"></param>
		public SqlHealthPresenter(IViewHealth view)
		{
			this._view = view;
		}

		/// <summary>
		/// Get Application data and set to view property
		/// </summary>
		public void GetHealthData()
		{
			var task = new HealthTask();
			_view.HealthData = task.GetHealthData(_view.MeasureType, _view.StartDate, _view.EndDate, _view.TimeZoneOffset);
		}

		/// <summary>
		/// Get performance field list
		/// </summary>
		public void GetColumnsList()
		{
			var task = new HealthTask();
			_view.ColumnsList = task.GetColumnsList(_view.MeasureType);
		}
	}
}
