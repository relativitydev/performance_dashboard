namespace kCura.PDB.Service.ProcessControls.ProcessTasks
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.ProcessControls;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;

	public class ProcessControlTaskManager
	{
		public ProcessControlTaskManager(ISqlServerRepository sqlRepository, ILogger logger)
		{
			this.sqlRepository = sqlRepository;
			this.observers = new List<IProcessControlTask>();
			this.logger = logger.WithCategory(Names.LogCategory.ProcessControl);
		}

		private readonly ISqlServerRepository sqlRepository;
		private readonly ILogger logger;
		private readonly List<IProcessControlTask> observers;

		public void Subscribe(params IProcessControlTask[] tasks)
		{
			this.observers.AddRange(tasks);
		}

		public void Execute()
		{
			var processControls = this.sqlRepository.ProcessControlRepository.ReadAll();
			var utcDateNow = this.sqlRepository.ReadServerUtcTime().AddSeconds(-15);
			foreach (var processControl in processControls)
			{
				var execSucceeded = processControl.LastExecSucceeded;
				try
				{
					Log("Checking", processControl);

					// read the process control last run time
					var interval = processControl != null ? processControl.Frequency.GetValueOrDefault(60) : 0;
					var processControlObservers = this.observers.Where(o => o.ProcessControlID == processControl.Id).ToList();

					if (processControl != null
						&& processControlObservers.Any()
						&& interval > 0
						&& processControl.LastProcessExecDateTime.AddMinutes(interval) <= utcDateNow)
					{
						// run the tasks
						execSucceeded = processControlObservers
								.Select(o => this.RunTask(o, processControl))
								.All(result => result == true);

						// save the process control last run time
						processControl.LastProcessExecDateTime = GetLastExecDate(processControl);
					}
					else
					{
						this.Log("Skipping due to Interval", processControl);
					}
				}
				catch (Exception ex)
				{
					var message = ex.GetExceptionDetails();
					this.LogError("Failure Running tasks. Details: {0}", processControl, message);
					execSucceeded = false;
					processControl.LastErrorMessage = ex.ToString();
				}
				finally
				{
					processControl.LastExecSucceeded = execSucceeded;
					this.sqlRepository.ProcessControlRepository.Update(processControl);
				}
			}
		}

		public bool RunTask(IProcessControlTask task, ProcessControl processControl)
		{
			try
			{
				this.Log("Called", task);

				// run the task
				var result = task.Execute(processControl);

				this.Log("Completed with result: {0}", task, result);
				return result;
			}
			catch (Exception ex)
			{
				var message = ex.GetExceptionDetails();
				this.LogError("Called - Failure. Details: {0}", task, message);
				processControl.LastErrorMessage = ex.ToString();
				return false;
			}
		}

		public static DateTime GetLastExecDate(ProcessControl processControl)
		{
			if (processControl == null || processControl.Frequency.HasValue == false)
			{
				throw new ArgumentException("Cannot get last exec date for null process control or frequency.");
			}

			if (processControl.Frequency.Value < 60)
			{
				return DateTime.UtcNow;
			}

			// Normalize to beginning of hour
			var lastExecDate = DateTime.UtcNow;
			var lastExecNorm = new DateTime(lastExecDate.Year, lastExecDate.Month, lastExecDate.Day, lastExecDate.Hour, 0, 0);
			return lastExecNorm;
		}

		private void Log(string msg, IProcessControlTask task, params object[] args)
		{
			this.logger.LogVerbose(string.Format(task.Name + " - " + msg, args), task.Name);
		}

		private void LogError(string msg, IProcessControlTask task, params object[] args)
		{
			this.logger.LogError(string.Format(task.Name + " - " + msg, args), task.Name);
		}

		private void Log(string msg, ProcessControl processControl, params object[] args)
		{
			this.logger.LogVerbose(string.Format(processControl.ProcessTypeDesc + " - " + msg, args), processControl.ProcessTypeDesc);
		}

		private void LogError(string msg, ProcessControl processControl, params object[] args)
		{
			this.logger.LogError(string.Format(processControl.ProcessTypeDesc + " - " + msg, args), processControl.ProcessTypeDesc);
		}
	}
}
