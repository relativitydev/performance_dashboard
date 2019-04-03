namespace kCura.PDB.Service.Metrics.Uptime
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Net;
	using System.Net.Http;
	using System.Threading.Tasks;
	using kCura.PDB.Core.Attributes;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Metrics;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Models.MetricDataSources;

	[MetricType(MetricType.WebUptime)]
	public class WebUptimeMetricLogic : IMetricLogic
	{
		public WebUptimeMetricLogic(IHttpClientFactory httpClientFactory, IServerRepository serverRepository, IMetricDataService metricDataService, IConfigurationRepository configurationRepository, ILogger logger)
		{
			this.httpClientFactory = httpClientFactory;
			this.serverRepository = serverRepository;
			this.configurationRepository = configurationRepository;
			this.metricDataService = metricDataService;
			this.logger = logger.WithClassName().WithCategory(Names.LogCategory.Metric);
		}

		private readonly IHttpClientFactory httpClientFactory;
		private readonly IServerRepository serverRepository;
		private readonly IConfigurationRepository configurationRepository;
		private readonly IMetricDataService metricDataService;
		private readonly ILogger logger;

		public async Task<decimal> ScoreMetric(MetricData metricData)
		{
			await this.logger.LogVerboseAsync($"Scoring metric data id {metricData.Id}");
			var webUptime = this.metricDataService.GetData<WebUptime>(metricData);
			if (webUptime == null)
			{
				return Defaults.Scores.Uptime;
			}

			var percentUp = 100.0m * (decimal)webUptime.SuccessfulSamples / (decimal)webUptime.TotalSamples;
			return percentUp;
		}

		/// <summary>
		/// This process needs to happen as frequently as possible for the hour:
		/// * response = HttpClient(login page)
		/// * webIsUp = response.Status less than 400
		/// * previousWebUptimeData = dataSourceRepo.Read(...) = {Number of Successful Up samples for hour, Number of Total samples for hour}
		/// * newWebUptimeData.SuccessfulSamples = previousWebUptimeData.SuccessfulSamples + WebIsUp
		/// * newWebUptimeData.TotalSamples++
		/// * dataSourceRepo.Set({ metric, newWebUptimeData })
		/// </summary>
		/// <param name="metric">The metric to collect web uptime data for</param>
		/// <returns>Task with updated web uptime data</returns>
		public async Task<object> CollectMetricData(MetricData metricData)
		{
			var httpClient = this.httpClientFactory.GetDefaultWindowsAuthHttpClient();
			var userAgent = this.configurationRepository.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.WebUptimeUserAgent)
				?? "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2228.0 Safari/537.36";
			httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", userAgent);

			var webUptime = this.metricDataService.GetData<WebUptime>(metricData) ?? new WebUptime();
			var suggestedWebUptimeUrl = this.configurationRepository.ReadConfigurationValue(
				ConfigurationKeys.Section,
				ConfigurationKeys.SuggestedWebUptimeUrl);

			var serverWereUp = false;
			if (!string.IsNullOrEmpty(suggestedWebUptimeUrl))
			{
				await this.logger.LogVerboseAsync(
					$"WebUptimeMetricLogic - SuggestedWebUptimeUrl {suggestedWebUptimeUrl} found.  Trying connection...");
				try
				{
					var suggestedWebUptimeUri = new Uri(suggestedWebUptimeUrl);
					serverWereUp = await this.ReturnSuccessfulUriRequest(suggestedWebUptimeUri, httpClient);
				}
				catch (Exception ex)
				{
					// Swallow exception intentionally
					await this.logger.LogVerboseAsync(
						$"WebUptimeMetricLogic - Exception thrown when trying to create Uri for SuggestedWebUptimeUrl: {suggestedWebUptimeUrl} - {ex}");
				}
			}
			else
			{
				await this.logger.LogVerboseAsync(
					$"WebUptimeMetricLogic - SuggestedWebUptimeUrl not found.  Fallback to iterating over servers.");
			}

			// If the first link was not a success, try the old fashioned way.
			if (!serverWereUp)
			{
				var servers = await this.serverRepository.ReadAllActiveAsync();
				var activeWebServers = servers
						.Where(s => s.ServerType == ServerType.Web || s.ServerType == ServerType.WebApi || s.ServerType == ServerType.WebBackground)
						.GroupBy(s => s.ServerIpAddress)
						.Select(s => s.First());
				if (activeWebServers.Any())
				{
					var processServers = await activeWebServers.Select(s => this.ProcessServer(s, httpClient)).WhenAllStreamed();
					var goodServer = processServers.FirstOrDefault(s => s != null && !string.IsNullOrEmpty(s.ToString()));
					serverWereUp = goodServer != null && !string.IsNullOrEmpty(goodServer.ToString());

					// If we found a good one, make sure to save it off.
					if (serverWereUp)
					{
						this.configurationRepository.SetConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.SuggestedWebUptimeUrl, goodServer.ToString());
					}
				}
				else
				{
					this.logger.LogError("WebUptimeMetricLogic - No active web servers found.");
				}
			}

			// Check for success
			if (serverWereUp)
			{
				webUptime.SuccessfulSamples++;
			}
			else
			{
				this.logger.LogWarning("WebUptimeMetricLogic - All web servers down.  Use verbose logging for more details.");
			}

			webUptime.TotalSamples++;
			return webUptime;
		}

		internal async Task<Uri> ProcessServer(Server server, HttpClient httpClient)
		{
			ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

			Uri successfulUri = null;

			// Get each resource to try for the server
			var requestResourcesToTry = GetRequestResourcesToTry(server);
			foreach (var requestResourceToTry in requestResourcesToTry)
			{
				// Await the uri request to see if it's successful
				if (await this.ReturnSuccessfulUriRequest(requestResourceToTry, httpClient))
				{
					successfulUri = requestResourceToTry;
					break;
				}
			}

			// if result is failure for all
			if (successfulUri != null)
			{
				await this.SaveNewRequestSettings(server, successfulUri);
			}
			else
			{
				await this.logger.LogVerboseAsync($"ProcessServer Called for Web - Failure. Server: {server.ServerName}. Details: Web server unreachable.");
			}

			return successfulUri;
		}

		/// <summary>
		/// Attempts to call a GetAsync request out to the given uri and returns if the request was considered a success
		/// </summary>
		/// <param name="requestResourceToTry">Uri to ping</param>
		/// <param name="httpClient">Client to use for the request</param>
		/// <param name="throwExceptions">Set to true to rethrow exceptions, defaults to false</param>
		/// <returns>Whether or not the Uri request was considered a success</returns>
		internal async Task<bool> ReturnSuccessfulUriRequest(Uri requestResourceToTry, HttpClient httpClient, bool throwExceptions = false)
		{
			try
			{
				var response = await httpClient.GetAsync(requestResourceToTry);
				if ((int)response.StatusCode < 400)
				{
					// Return success immediately
					return true;
				}
				else
				{
					// Log verbose in order to diagnose any web-uptime issues
					await this.logger.LogVerboseAsync($"Failed to connect to {requestResourceToTry}, StatusCode {(int)response.StatusCode}-{response.StatusCode}, {response.Content}");
				}
			}
			catch (Exception ex)
			{
				// intentionally swallowing exceptions since a connection issue is a possibility and 'successfulUri' remains null
				await this.logger.LogVerboseAsync($"Failed to reach Uri: {requestResourceToTry}, Exception: {ex}");
				if (throwExceptions)
				{
					throw ex;
				}
			}

			return false;
		}

		/// <summary>
		/// Gets list of Uri resources to try for a given (web)server
		/// </summary>
		/// <param name="server">Server to request Uri resources for</param>
		/// <returns>List of Uri to attempt connection for a given (web)server</returns>
		internal static IList<Uri> GetRequestResourcesToTry(Server server)
		{
			var previousUseHttps = server.UptimeMonitoringResourceUseHttps;
			var previousHost = server.UptimeMonitoringResourceHost;

			// order field is so we can try the previous config first
			return new[]
			{
					new { UseHttps = false, Host = server.ServerIpAddress, Order = 1 },
					new { UseHttps = true, Host = server.ServerIpAddress, Order = 2 },
					new { UseHttps = false, Host = server.ServerName, Order = 3 },
					new { UseHttps = true, Host = server.ServerName, Order = 4 },
			}
			.OrderBy(r => previousHost != null && r.UseHttps == previousUseHttps && string.Equals(previousHost, r.Host, StringComparison.CurrentCultureIgnoreCase) ? 0 : r.Order)
			.Select(r => new Uri($"{(r.UseHttps ? "https" : "http")}://{r.Host}/Relativity/"))
			.ToList();
		}

		internal async Task SaveNewRequestSettings(Server server, Uri successfulUri)
		{
			var previousUseHttps = server.UptimeMonitoringResourceUseHttps;
			var previousHost = server.UptimeMonitoringResourceHost;

			// Save if successful request resource is different than previous
			var successfulHttps = successfulUri.Scheme == "https";
			if (successfulHttps != previousUseHttps || !string.Equals(previousHost, successfulUri.Host, StringComparison.CurrentCultureIgnoreCase))
			{
				server.UptimeMonitoringResourceHost = successfulUri.Host;
				server.UptimeMonitoringResourceUseHttps = successfulHttps;
				await this.serverRepository.UpdateAsync(server);
			}
		}
	}
}
