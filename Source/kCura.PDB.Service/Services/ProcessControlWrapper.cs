namespace kCura.PDB.Service.Services
{
	using System;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Enumerations;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;

	public class ProcessControlWrapper : IProcessControlWrapper
	{
		private readonly IProcessControlRepository processControlRepository;
		private readonly ILogger logger;

		public ProcessControlWrapper(IProcessControlRepository processControlRepository, ILogger logger)
		{
			this.processControlRepository = processControlRepository;
			this.logger = logger.WithClassName();
		}

		public async Task Execute<TR>(ProcessControlId processControlId, Func<TR> func)
		{
			var processControl = await this.processControlRepository.ReadByIdAsync(processControlId);
			if (processControl == null)
			{
				this.logger.WithCategory(processControlId.ToString()).LogVerbose("Skipping due to process control interval.");
				return;
			}

			var execSucceeded = processControl.LastExecSucceeded;
			try
			{
				var utcDate = DateTime.UtcNow;
				this.logger.LogVerbose($"Calling '{func.Method.Name}'");
				var interval = processControl.Frequency.GetValueOrDefault(10080);

				if (interval > 0 && processControl.LastProcessExecDateTime.AddMinutes(interval) <= utcDate)
				{
					func.Invoke();

					processControl.LastProcessExecDateTime = utcDate;
					execSucceeded = true;
				}
				else
				{
					this.logger.LogVerbose($"'{func.Method.Name}' Called - Skipping due to interval");
				}
			}
			catch (Exception ex)
			{
				var message = ex.GetExceptionDetails();
				this.logger.LogError($"'{func.Method.Name}' Called - Failure. Details: {message}");
				execSucceeded = false;
				processControl.LastErrorMessage = ex.ToString();
			}
			finally
			{
				processControl.LastExecSucceeded = execSucceeded;
				await this.processControlRepository.UpdateAsync(processControl);
			}
		}

		public async Task Execute(ProcessControlId processControlId, Action func)
		{
			var processControl = await this.processControlRepository.ReadByIdAsync(processControlId);
			if (processControl == null)
			{
				this.logger.WithCategory(processControlId.ToString()).LogVerbose("Skipping due to process control interval.");
				return;
			}

			var execSucceeded = processControl.LastExecSucceeded;
			try
			{
				var utcDate = DateTime.UtcNow;
				this.logger.LogVerbose($"Calling '{func.Method.Name}'");
				var interval = processControl.Frequency.GetValueOrDefault(10080);

				if (interval > 0 && processControl.LastProcessExecDateTime.AddMinutes(interval) <= utcDate)
				{
					func.Invoke();

					processControl.LastProcessExecDateTime = utcDate;
					execSucceeded = true;
				}
				else
				{
					this.logger.LogVerbose($"'{func.Method.Name}' Called - Skipping due to interval");
				}
			}
			catch (Exception ex)
			{
				var message = ex.GetExceptionDetails();
				this.logger.LogError($"'{func.Method.Name}' Called - Failure. Details: {message}");
				execSucceeded = false;
				processControl.LastErrorMessage = ex.ToString();
			}
			finally
			{
				processControl.LastExecSucceeded = execSucceeded;
				await this.processControlRepository.UpdateAsync(processControl);
			}
		}

		public async Task Execute<TR>(ProcessControlId processControlId, Func<Task<TR>> func)
		{
			var processControl = await this.processControlRepository.ReadByIdAsync(processControlId);
			if (processControl == null)
			{
				this.logger.WithCategory(processControlId.ToString()).LogVerbose("Skipping due to process control interval.");
				return;
			}

			var execSucceeded = processControl.LastExecSucceeded;
			try
			{
				var utcDate = DateTime.UtcNow;
				this.logger.LogVerbose($"Calling '{func.Method.Name}'");
				var interval = processControl.Frequency.GetValueOrDefault(10080);

				if (interval > 0 && processControl.LastProcessExecDateTime.AddMinutes(interval) <= utcDate)
				{
					await func.Invoke();

					processControl.LastProcessExecDateTime = utcDate;
					execSucceeded = true;
				}
				else
				{
					this.logger.LogVerbose($"'{func.Method.Name}' Called - Skipping due to interval");
				}
			}
			catch (Exception ex)
			{
				var message = ex.GetExceptionDetails();
				this.logger.LogError($"'{func.Method.Name}' Called - Failure. Details: {message}");
				execSucceeded = false;
				processControl.LastErrorMessage = ex.ToString();
			}
			finally
			{
				processControl.LastExecSucceeded = execSucceeded;
				await this.processControlRepository.UpdateAsync(processControl);
			}
		}

		public async Task Execute(ProcessControlId processControlId, Action<DateTime> func)
		{
			var processControl = await this.processControlRepository.ReadByIdAsync(processControlId);
			if (processControl == null)
			{
				this.logger.WithCategory(processControlId.ToString()).LogVerbose("Skipping due to process control interval.");
				return;
			}

			var execSucceeded = processControl.LastExecSucceeded;
			try
			{
				var utcDate = DateTime.UtcNow;
				this.logger.LogVerbose($"Calling '{func.Method.Name}'");
				var interval = processControl.Frequency.GetValueOrDefault(10080);

				if (interval > 0 && processControl.LastProcessExecDateTime.AddMinutes(interval) <= utcDate)
				{
					func(processControl.LastProcessExecDateTime);

					processControl.LastProcessExecDateTime = utcDate;
					execSucceeded = true;
				}
				else
				{
					this.logger.LogVerbose($"'{func.Method.Name}' Called - Skipping due to interval");
				}
			}
			catch (Exception ex)
			{
				var message = ex.GetExceptionDetails();
				this.logger.LogError($"'{func.Method.Name}' Called - Failure. Details: {message}");
				execSucceeded = false;
				processControl.LastErrorMessage = ex.ToString();
			}
			finally
			{
				processControl.LastExecSucceeded = execSucceeded;
				await this.processControlRepository.UpdateAsync(processControl);
			}
		}
	}
}
