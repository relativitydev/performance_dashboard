namespace kCura.PDB.Service.Testing
{
	using System;
	using System.Collections.Generic;
	using System.Threading;
	using System.Threading.Tasks;

	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Interfaces.Testing.Services;

	public class NewBackupDbccServiceExecution : IServiceExecution
    {
        private readonly ITestAgentRunner testAgentRunner;
		private readonly ITestPollingService testPollingService;
		private readonly ITestEventPrimer testEventPrimer;
		private readonly ICancellationTokenSourceFactory cancellationTokenSourceFactory;

		public NewBackupDbccServiceExecution(
            ITestAgentRunner testAgentRunner,
            ITestPollingService testPollingService,
            ITestEventPrimer testEventPrimer,
            ICancellationTokenSourceFactory cancellationTokenSourceFactory)
        {
            this.testAgentRunner = testAgentRunner;
	        this.testPollingService = testPollingService;
	        this.testEventPrimer = testEventPrimer;
	        this.cancellationTokenSourceFactory = cancellationTokenSourceFactory;
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
			// Setup Timeout for cancellation token
	        var timeoutDuration = new TimeSpan(0, 2, 0); // Where do we get this?
	        var timeoutTokenSource =
		        this.cancellationTokenSourceFactory.GetTimeoutCancellationTokenSource(timeoutDuration);
			var timeoutToken = timeoutTokenSource.Token;

	        var combinedTokenSource =
		        this.cancellationTokenSourceFactory.CreateLinkedTokenSource(cancellationToken, timeoutToken);
			var combinedToken = combinedTokenSource.Token;

			// Setup needed events to prime the system for the test
			await this.testEventPrimer.CreateEventDataAsync();

			// Startup the Manager and Worker -- Should we await on this if we don't care how it's running?  How do we catch exception if it's thrown?
	        var tasksToExecute = new List<Task>();
	        var agentTasks = this.testAgentRunner.GetAgentExecutionTasks(combinedToken);
			tasksToExecute.AddRange(agentTasks);

			// Get event polling task -- know when to stop successfully
	        var pollingTask = this.testPollingService.WaitUntilEventCompletionAsync(combinedToken);
			tasksToExecute.Add(pollingTask);

			// Poll until it should be stopped
	        var finishedTask = await Task.WhenAny(tasksToExecute);

	        try
	        {
		        // Check for exceptions
		        await finishedTask;
	        }
	        catch (TaskCanceledException)
	        {
		        if (timeoutTokenSource.IsCancellationRequested == false)
		        {
			        throw; // We weren't expecting this
		        }
	        }

	        // Cancel the rest
			combinedTokenSource.Cancel();

	        // TODO -- Take care of data clean up?
        }
	}
}
