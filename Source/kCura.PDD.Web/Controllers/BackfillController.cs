using kCura.PDB.Core.Constants;
using kCura.PDB.Core.Extensions;
using kCura.PDB.Core.Models;
using kCura.PDD.Web.Filters;
using kCura.PDD.Web.Models;
using kCura.PDD.Web.Models.BISSummary;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace kCura.PDD.Web.Controllers
{
    using System;
    using System.Globalization;
    using System.Net;
    using System.Net.Http;
    using global::Relativity.CustomPages;
    using kCura.PDB.Core.Interfaces.Repositories;
    using kCura.PDB.Data.Repositories;
    using kCura.PDB.Data.Services;
    using kCura.PDB.Service.BISSummary;
    using kCura.PDB.Service.Notifications;
    using kCura.PDD.Web.Extensions;
    using kCura.PDD.Web.Services;

    [AuthenticateUser]
    public class BackfillController : ApiController
    {
        private ISqlServerRepository _sqlRepo;

        // Used for tests
        public BackfillController(ISqlServerRepository sqlRepo)
        {
            this._sqlRepo = sqlRepo;
        }

        // Used by Relativity
        public BackfillController()
        {
            var connectionFactory = new HelperConnectionFactory(ConnectionHelper.Helper());
            _sqlRepo = new SqlServerRepository(connectionFactory);
        }
        [HttpGet]
        public async Task<BackfillStatus> GetBackfillStatus()
        {
            var service = new BestInServiceReportingService(this._sqlRepo);

            var lastLogEntryTask = _sqlRepo.LogRepository.ReadLastAsync();
            var lastEventTask = _sqlRepo.EventRepository.ReadLastAsync();
            var pendingEventsCountTask = _sqlRepo.EventRepository.ReadCountByStatusAsync(new List<EventStatus> { EventStatus.Pending, EventStatus.PendingHangfire });
            var errorEventsCountTask = _sqlRepo.EventRepository.ReadCountByStatusAsync(EventStatus.Error);
            var eventSystemStateTask = _sqlRepo.EventRepository.ReadEventSystemStateAsync();
            var hoursAwaitingAnalysisTask = _sqlRepo.BackfillRepository.ReadHoursAwaitingAnalysis();
            var hoursAwaitingDiscoveryTask = _sqlRepo.BackfillRepository.ReadHoursAwaitingDiscovery();
            var hoursAwaitingScoringTask = _sqlRepo.BackfillRepository.ReadHoursAwaitingScoring();
            var hoursCompletedScoringTask = _sqlRepo.BackfillRepository.ReadHoursCompletedScoring();

            await Task.WhenAll(
                lastLogEntryTask,
                lastEventTask,
                pendingEventsCountTask,
                errorEventsCountTask,
                eventSystemStateTask,
                hoursAwaitingAnalysisTask,
                hoursAwaitingDiscoveryTask,
                hoursAwaitingScoringTask,
                hoursCompletedScoringTask).ConfigureAwait(false);
            var lastLogEntry = await lastLogEntryTask;

            var status = new BackfillStatus()
            {
                LastMessage = string.IsNullOrEmpty(lastLogEntry?.TaskCompleted) ? "N/A" : lastLogEntry.TaskCompleted.Truncate(200),
                LastEventExecuted = (await lastEventTask)?.LastUpdated.ToString("s") ?? "N/A",
                PendingEvents = await pendingEventsCountTask,
                ErrorEvents = await errorEventsCountTask,
                SystemState = System.Enum.GetName(typeof(EventSystemState), await eventSystemStateTask),
                SampleRange = new LookingGlassInformation(service.GetSampleRange(RequestService.GetTimezoneOffset(this.Request))).SampleRange,
                HoursAwaitingAnalysis = await hoursAwaitingAnalysisTask,
                HoursAwaitingDiscovery = await hoursAwaitingDiscoveryTask,
                HoursAwaitingScoring = await hoursAwaitingScoringTask,
                HoursCompleted = await hoursCompletedScoringTask
            };

            return status;
        }

        [HttpGet]
        public async Task<PDBNotification> GetNotificationStatus()
        {
            var notificationService = new PdbNotificationService(this._sqlRepo);
            return await notificationService.GetDatabaseDeploymentFailureAlertAsync();
        }

		[HttpPost]
		public async Task<BackfillStatus> RetryErrorEvents([FromBody]string value)
		{
		    var erroredEventIds = await this._sqlRepo.EventRepository.ReadIdsByStatusAsync(
		                              EventStatus.Error,
		                              EventConstants.DefaultErroredEventsToEnqueue);
		    await this._sqlRepo.EventRepository.UpdateStatusAndRetriesAsync(erroredEventIds, EventStatus.Pending);
			return await this.GetBackfillStatus();
		}

        [HttpPost]
        public async Task<HttpResponseMessage> Log(LogLevel logLevel)
        {
            //Initialize service and model properties
            var connectionFactory = new HelperConnectionFactory(ConnectionHelper.Helper());
            var logRepo = new LogRepository(connectionFactory);
            var configRepo = new ConfigurationRepository(connectionFactory);
            var configLogSize = await configRepo.ReadValueAsync<int>(ConfigurationKeys.LogFileSize);
            var logFileSize = Math.Max(configLogSize ?? Defaults.Log.DefaultLogFileSize, Defaults.Log.MinLogFileSize);
            var logLevelVal = logLevel == LogLevel.Errors ? 2 : (logLevel == LogLevel.Warnings ? 5 : 10);
            var logEntries = await logRepo.ReadLastAsync(logFileSize, logLevelVal);

            //Get the data
            using (var sw = new System.IO.StringWriter())
            {
                var heaaderArr = new string[]
                {
                    "Log Id",
                    "Log Timestamp Utc",
                    "Module",
                    "Task Completed",
                    "Other Vars",
                    "Next Task",
                    "Agent Id",
                    "Log Level",
                    "Event Id",
                    "Event Type",
                    "Event Status",
                    "Event Time Stamp",
                    "Event Delay",
                    "Previous Event",
                    "Event Last Updated",
                    "Event Retries",
                    "Event Execution Time",
                    "Event Hour Id"
                };
                sw.WriteLine(string.Join(",", heaaderArr));
                foreach (var row in logEntries)
                    sw.WriteCsvSafeLine(new[]
                    {
                        row.GrLogId.ToString(),
                        row.LogTimestampUtc.ToString(CultureInfo.InvariantCulture),
                        row.Module,
                        row.TaskCompleted,
                        row.OtherVars,
                        row.NextTask,
                        row.AgentId?.ToString(),
                        row.LogLevel?.ToString(),
                        row.EventId.ToString(),
                        row.EventType?.ToString(),
                        row.EventStatus?.ToString(),
                        row.EventTimeStamp?.ToString(CultureInfo.InvariantCulture),
                        row.EventDelay?.ToString(),
                        row.PreviousEventId?.ToString(),
                        row.EventLastUpdated?.ToString(CultureInfo.InvariantCulture),
                        row.EventRetries?.ToString(),
                        row.EventExecutionTime?.ToString(),
                        row.EventHourId?.ToString()
                    });

                //Serialize response
                var message = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(sw.ToString())
                };
                message.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/csv");
                message.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                {
                    FileName = $"Log-{DateTime.UtcNow:yyyyMMdd-HHmmss}.csv"
                };
                return message;
            }
        }

    }
}
