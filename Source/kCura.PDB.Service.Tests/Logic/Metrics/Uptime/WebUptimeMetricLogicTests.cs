namespace kCura.PDB.Service.Tests.Logic.Metrics.Uptime
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using System.Net;
	using System.Net.Http;
	using System.Threading;
	using Core.Constants;
	using Core.Interfaces.Repositories;
	using Core.Interfaces.Services;
	using Core.Models;
	using Core.Models.MetricDataSources;
	using kCura.PDB.Core.Interfaces.Metrics;
	using kCura.PDB.Service.Metrics.Uptime;
	using Moq;
	using Moq.Protected;
	using NUnit.Framework;
	using kCura.PDB.Tests.Common;

	[TestFixture, Category("Unit")]
	public class WebUptimeMetricLogicTests
	{
		[SetUp]
		public void Setup()
		{
			httpClientHandler = new Mock<HttpClientHandler>();
			httpClientHandler.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK))
				.Callback<HttpRequestMessage, CancellationToken>((r, c) =>
				{
					Assert.That(HttpMethod.Get, Is.EqualTo(r.Method));
				});
			httpClientHandler.Object.UseDefaultCredentials = true;

			httpClient = new HttpClient(httpClientHandler.Object);
			serverRepository = new Mock<IServerRepository>();
			metricDataService = new Mock<IMetricDataService>();
			configRepository = new Mock<IConfigurationRepository>();
			httpClientFactoryMock = new Mock<IHttpClientFactory>();
			httpClientFactoryMock.Setup(m=>m.GetDefaultWindowsAuthHttpClient()).Returns(httpClient);

			servers = new List<Server>()
			{
				// same Web servers
				new Server { ServerType = ServerType.Web, ServerIpAddress = "127.0.0.1", ServerName = "1.com", },
				new Server { ServerType = ServerType.WebApi, ServerIpAddress = "127.0.0.1", ServerName = "1.com", },
				// different Web server
				new Server { ServerType = ServerType.Web, ServerIpAddress = "127.0.0.2", ServerName = "2.com", },
				// different non-web server
				new Server { ServerType = ServerType.Database, ServerIpAddress = "127.0.0.3", ServerName = "3.com", }
			};

			logger = TestUtilities.GetMockLogger();
		}

		//private Mock<HttpMessageHandler> httpMessageHandler;
		private Mock<HttpClientHandler> httpClientHandler;
		private Mock<IHttpClientFactory> httpClientFactoryMock;
		private HttpClient httpClient;
		private Mock<IServerRepository> serverRepository;
		private Mock<IConfigurationRepository> configRepository;
		private Mock<IMetricDataService> metricDataService;
		private Mock<ILogger> logger;
		private List<Server> servers;

		private void SetupHttpClientAdditionalResponses(Uri requestUri, HttpStatusCode responseStatus)
		{
			httpClientHandler.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(r => r.RequestUri == requestUri), ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(new HttpResponseMessage(responseStatus))
				.Callback<HttpRequestMessage, CancellationToken>((r, c) =>
				{
					Assert.That(HttpMethod.Get, Is.EqualTo(r.Method));
				});
		}

		[Test]
		public async Task WebUptimeMetricLogic_ScoreMetric()
		{
			//Arrange
			var metricData = new MetricData();
			var webUptime = new WebUptime { SuccessfulSamples = 9, TotalSamples = 10 };
			metricDataService.Setup(s => s.GetData<WebUptime>(metricData)).Returns(webUptime);
			var logic = new WebUptimeMetricLogic(httpClientFactoryMock.Object, serverRepository.Object, metricDataService.Object, configRepository.Object, logger.Object);

			//Act
			var result = await logic.ScoreMetric(metricData);

			//Assert
			Assert.That(result, Is.EqualTo(90.0m));
		}

		[Test]
		public async Task WebUptimeMetricLogic_ScoreMetric_NoUptime()
		{
			//Arrange
			var metricData = new MetricData();
			var logic = new WebUptimeMetricLogic(httpClientFactoryMock.Object, serverRepository.Object, metricDataService.Object, configRepository.Object, logger.Object);

			//Act
			var result = await logic.ScoreMetric(metricData);

			//Assert
			Assert.That(result, Is.EqualTo(Defaults.Scores.Uptime));
		}

		[Test]
		public async Task WebUptimeMetricLogic_CollectMetricData()
		{
			//Arrange
			var metricData = new MetricData();
			var webUptime = new WebUptime { SuccessfulSamples = 9, TotalSamples = 10 };
			metricDataService.Setup(s => s.GetData<WebUptime>(metricData)).Returns(webUptime);
			configRepository.Setup(r => r.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.WebUptimeUserAgent)).Returns("Chrome");
			serverRepository.Setup(r => r.ReadAllActiveAsync()).ReturnsAsync(servers);

			var logic = new WebUptimeMetricLogic(httpClientFactoryMock.Object, serverRepository.Object, metricDataService.Object, configRepository.Object, logger.Object);

			//Act
			var result = await logic.CollectMetricData(metricData);

			//Assert
			var resultWebUptime = (WebUptime)result;
			Assert.That(result, Is.Not.Null);
			Assert.That(resultWebUptime, Is.Not.Null);
			Assert.That(resultWebUptime.SuccessfulSamples, Is.EqualTo(10));
			Assert.That(resultWebUptime.TotalSamples, Is.EqualTo(11));
		}

		[Test]
		public async Task WebUptimeMetricLogic_CollectMetricData_SuggestedUrl()
		{
			//Arrange
			var metricData = new MetricData();
			var webUptime = new WebUptime { SuccessfulSamples = 9, TotalSamples = 10 };
			var suggestedUrl = "http://www.google.com";
			SetupHttpClientAdditionalResponses(new Uri(suggestedUrl), HttpStatusCode.OK);
			metricDataService.Setup(s => s.GetData<WebUptime>(metricData)).Returns(webUptime);
			configRepository.Setup(r => r.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.WebUptimeUserAgent)).Returns("Chrome");
			configRepository.Setup(r => r.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.SuggestedWebUptimeUrl)).Returns(suggestedUrl);

			var logic = new WebUptimeMetricLogic(httpClientFactoryMock.Object, serverRepository.Object, metricDataService.Object, configRepository.Object, logger.Object);

			//Act
			var result = await logic.CollectMetricData(metricData);

			//Assert
			var resultWebUptime = (WebUptime)result;
			Assert.That(result, Is.Not.Null);
			Assert.That(resultWebUptime, Is.Not.Null);
			Assert.That(resultWebUptime.SuccessfulSamples, Is.EqualTo(10));
			Assert.That(resultWebUptime.TotalSamples, Is.EqualTo(11));
		}

		[Test]
		public async Task WebUptimeMetricLogic_CollectMetricData_NewDataSource()
		{
			//Arrange
			var metricData = new MetricData();

			serverRepository.Setup(r => r.ReadAllActiveAsync()).ReturnsAsync(servers);
			configRepository.Setup(r => r.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.WebUptimeUserAgent)).Returns("Chrome");

			var logic = new WebUptimeMetricLogic(httpClientFactoryMock.Object, serverRepository.Object, metricDataService.Object, configRepository.Object, logger.Object);

			//Act
			var result = await logic.CollectMetricData(metricData);


			//Assert
			var resultWebUptime = (WebUptime)result;
			Assert.That(result, Is.Not.Null);
			Assert.That(resultWebUptime, Is.Not.Null);
			Assert.That(resultWebUptime.SuccessfulSamples, Is.EqualTo(1));
			Assert.That(resultWebUptime.TotalSamples, Is.EqualTo(1));
		}

		[Test]
		public async Task WebUptimeMetricLogic_CollectMetricData_ServerWasNotUp()
		{
			//Arrange
			var metricData = new MetricData();
			var webUptime = new WebUptime { SuccessfulSamples = 9, TotalSamples = 10 };
			metricDataService.Setup(s => s.GetData<WebUptime>(metricData)).Returns(webUptime);
			SetupHttpClientAdditionalResponses(new Uri("http://127.0.0.1/Relativity/"), HttpStatusCode.NotFound);
			SetupHttpClientAdditionalResponses(new Uri("https://127.0.0.1/Relativity/"), HttpStatusCode.NotFound);
			SetupHttpClientAdditionalResponses(new Uri("http://1.com/Relativity/"), HttpStatusCode.NotFound);
			SetupHttpClientAdditionalResponses(new Uri("https://1.com/Relativity/"), HttpStatusCode.NotFound);

			SetupHttpClientAdditionalResponses(new Uri("http://127.0.0.2/Relativity/"), HttpStatusCode.NotFound);
			SetupHttpClientAdditionalResponses(new Uri("https://127.0.0.2/Relativity/"), HttpStatusCode.NotFound);
			SetupHttpClientAdditionalResponses(new Uri("http://2.com/Relativity/"), HttpStatusCode.NotFound);
			SetupHttpClientAdditionalResponses(new Uri("https://2.com/Relativity/"), HttpStatusCode.NotFound);
			configRepository.Setup(r => r.ReadConfigurationValue(ConfigurationKeys.Section, ConfigurationKeys.WebUptimeUserAgent)).Returns("Chrome");
			serverRepository.Setup(r => r.ReadAllActiveAsync()).ReturnsAsync(servers);

			var logic = new WebUptimeMetricLogic(httpClientFactoryMock.Object, serverRepository.Object, metricDataService.Object, configRepository.Object, logger.Object);

			//Act
			var result = await logic.CollectMetricData(metricData);

			//Assert
			var resultWebUptime = (WebUptime)result;
			Assert.That(result, Is.Not.Null);
			Assert.That(resultWebUptime, Is.Not.Null);
			Assert.That(resultWebUptime.SuccessfulSamples, Is.EqualTo(9));
			Assert.That(resultWebUptime.TotalSamples, Is.EqualTo(11));
		}

		[Test]
		public async Task WebUptimeMetricLogic_CollectMetricData_SomeServersDown()
		{
			//Arrange
			var metricData = new MetricData();
			var webUptime = new WebUptime { SuccessfulSamples = 9, TotalSamples = 10 };
			metricDataService.Setup(s => s.GetData<WebUptime>(metricData)).Returns(webUptime);
			SetupHttpClientAdditionalResponses(new Uri("http://127.0.0.1/Relativity/"), HttpStatusCode.NotFound);
			SetupHttpClientAdditionalResponses(new Uri("https://127.0.0.1/Relativity/"), HttpStatusCode.NotFound);
			SetupHttpClientAdditionalResponses(new Uri("http://1.com/Relativity/"), HttpStatusCode.NotFound);
			SetupHttpClientAdditionalResponses(new Uri("https://1.com/Relativity/"), HttpStatusCode.NotFound);
			
			serverRepository.Setup(r => r.ReadAllActiveAsync()).ReturnsAsync(servers);

			var logic = new WebUptimeMetricLogic(httpClientFactoryMock.Object, serverRepository.Object, metricDataService.Object, configRepository.Object, logger.Object);

			//Act
			var result = await logic.CollectMetricData(metricData);

			//Assert
			var resultWebUptime = (WebUptime)result;
			Assert.That(result, Is.Not.Null);
			Assert.That(resultWebUptime, Is.Not.Null);
			Assert.That(resultWebUptime.SuccessfulSamples, Is.EqualTo(10));
			Assert.That(resultWebUptime.TotalSamples, Is.EqualTo(11));
		}

		[Test]
		public async Task WebUptimeMetricLogic_CollectMetricData_NoWebServers()
		{
			//Arrange
			var metricData = new MetricData();
			var webUptime = new WebUptime { SuccessfulSamples = 9, TotalSamples = 10 };
			metricDataService.Setup(s => s.GetData<WebUptime>(metricData)).Returns(webUptime);

			serverRepository.Setup(r => r.ReadAllActiveAsync()).ReturnsAsync(new List<Server>());

			var logic = new WebUptimeMetricLogic(httpClientFactoryMock.Object, serverRepository.Object, metricDataService.Object, configRepository.Object, logger.Object);

			//Act
			var result = await logic.CollectMetricData(metricData);

			//Assert
			var resultWebUptime = (WebUptime)result;
			Assert.That(result, Is.Not.Null);
			Assert.That(resultWebUptime, Is.Not.Null);
			Assert.That(resultWebUptime.SuccessfulSamples, Is.EqualTo(9));
			Assert.That(resultWebUptime.TotalSamples, Is.EqualTo(11));
		}

		[Test]
		public async Task WebUptimeMetricLogic_ProcessServer()
		{
			//Arrange
			var server = new Server
			{
				ServerIpAddress = "127.0.0.1",
				ServerName = "1.com",
				UptimeMonitoringResourceHost = null,
				UptimeMonitoringResourceUseHttps = null
			};
			SetupHttpClientAdditionalResponses(new Uri("http://127.0.0.1/Relativity/"), HttpStatusCode.NotFound);
			httpClient = new HttpClient(httpClientHandler.Object);
			var logic = new WebUptimeMetricLogic(httpClientFactoryMock.Object, serverRepository.Object, metricDataService.Object, configRepository.Object, logger.Object);

			//Act
			var result = await logic.ProcessServer(server, httpClient);

			//Assert
			Assert.That(result, Is.Not.Null);
			httpClientHandler.Protected()
				.Verify<Task<HttpResponseMessage>>("SendAsync", Times.Exactly(2), ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());
		}

		[Test]
		public async Task WebUptimeMetricLogic_ProcessServer_NoSuccessfulUrls()
		{
			//Arrange
			var server = new Server
			{
				ServerIpAddress = "127.0.0.1",
				ServerName = "1.com",
				UptimeMonitoringResourceHost = null,
				UptimeMonitoringResourceUseHttps = null
			};
			SetupHttpClientAdditionalResponses(new Uri("http://127.0.0.1/Relativity/"), HttpStatusCode.NotFound);
			SetupHttpClientAdditionalResponses(new Uri("https://127.0.0.1/Relativity/"), HttpStatusCode.NotFound);
			SetupHttpClientAdditionalResponses(new Uri("http://1.com/Relativity/"), HttpStatusCode.NotFound);
			SetupHttpClientAdditionalResponses(new Uri("https://1.com/Relativity/"), HttpStatusCode.NotFound);

			httpClient = new HttpClient(httpClientHandler.Object);
			var logic = new WebUptimeMetricLogic(httpClientFactoryMock.Object, serverRepository.Object, metricDataService.Object, configRepository.Object, logger.Object);

			//Act
			var result = await logic.ProcessServer(server, httpClient);

			//Assert
			Assert.That(result, Is.Null);
			httpClientHandler.Protected()
				.Verify<Task<HttpResponseMessage>>("SendAsync", Times.Exactly(4), ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());
			this.logger.Verify(l => l.LogVerboseAsync("ProcessServer Called for Web - Failure. Server: 1.com. Details: Web server unreachable.", It.IsAny<List<string>>()));
		}

		[Test]
		public async Task WebUptimeMetricLogic_ProcessServer_RequestThrowsException()
		{
			//Arrange
			var server = new Server
			{
				ServerIpAddress = "127.0.0.1",
				ServerName = "1.com",
				UptimeMonitoringResourceHost = null,
				UptimeMonitoringResourceUseHttps = null
			};
			SetupHttpClientAdditionalResponses(new Uri("http://127.0.0.1/Relativity/"), HttpStatusCode.NotFound);
			httpClientHandler.Protected()
				.Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(r => r.RequestUri == new Uri("https://127.0.0.1/Relativity/")), ItExpr.IsAny<CancellationToken>())
				.Throws(new HttpRequestException("An error occurred while sending the request."));

			httpClient = new HttpClient(httpClientHandler.Object);
			var logic = new WebUptimeMetricLogic(httpClientFactoryMock.Object, serverRepository.Object, metricDataService.Object, configRepository.Object, logger.Object);

			//Act
			var result = await logic.ProcessServer(server, httpClient);

			//Assert
			Assert.That(result, Is.Not.Null);
			httpClientHandler.Protected()
				.Verify<Task<HttpResponseMessage>>("SendAsync", Times.Exactly(3), ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());
		}

		[Test,
			TestCase("127.0.0.1", true, new[] { "https://127.0.0.1", "http://127.0.0.1", "http://stuff.com", "https://stuff.com" }),
			TestCase("127.0.0.1", false, new[] { "http://127.0.0.1", "https://127.0.0.1", "http://stuff.com", "https://stuff.com" }),
			TestCase("stuff.com", true, new[] { "https://stuff.com", "http://127.0.0.1", "https://127.0.0.1", "http://stuff.com" }),
			TestCase("stuff.com", false, new[] { "http://stuff.com", "http://127.0.0.1", "https://127.0.0.1", "https://stuff.com" }),
			// thing.net covers case previous host is not current server name or ip address
			TestCase("thing.net", true, new[] { "http://127.0.0.1", "https://127.0.0.1", "http://stuff.com", "https://stuff.com" }),
			TestCase("thing.net", false, new[] { "http://127.0.0.1", "https://127.0.0.1", "http://stuff.com", "https://stuff.com" }),
			//previous values are null
			TestCase(null, true, new[] { "http://127.0.0.1", "https://127.0.0.1", "http://stuff.com", "https://stuff.com" }),
			TestCase("thing.net", null, new[] { "http://127.0.0.1", "https://127.0.0.1", "http://stuff.com", "https://stuff.com" })]
		public void WebUptimeMetricLogic_GetRequestResourcesToTry(string previousHost, bool? previousHttps, string[] expectedResults)
		{
			//Arrange
			var server = new Server
			{
				ServerIpAddress = "127.0.0.1",
				ServerName = "stuff.com",
				UptimeMonitoringResourceHost = previousHost,
				UptimeMonitoringResourceUseHttps = previousHttps
			};

			//Act
			var result = WebUptimeMetricLogic.GetRequestResourcesToTry(server);

			//Assert
			Assert.That(result[0].AbsoluteUri, Is.EqualTo(expectedResults[0] + "/Relativity/"));
			Assert.That(result[1].AbsoluteUri, Is.EqualTo(expectedResults[1] + "/Relativity/"));
			Assert.That(result[2].AbsoluteUri, Is.EqualTo(expectedResults[2] + "/Relativity/"));
			Assert.That(result[3].AbsoluteUri, Is.EqualTo(expectedResults[3] + "/Relativity/"));
		}

		[Test,
			TestCase("stuff.com", true, "https", "stuff.com", false),
			TestCase("stuff.com", true, "https", "thing.net", true),
			TestCase("stuff.com", true, "http", "stuff.com", true),
			TestCase("stuff.com", true, "http", "thing.net", true),
			TestCase("stuff.com", false, "https", "stuff.com", true),
			TestCase("stuff.com", false, "https", "thing.net", true),
			TestCase("stuff.com", false, "http", "stuff.com", false),
			TestCase("stuff.com", false, "http", "thing.net", true),
			TestCase("thing.net", true, "https", "stuff.com", true),
			TestCase("thing.net", true, "https", "thing.net", false),
			TestCase("thing.net", true, "http", "stuff.com", true),
			TestCase("thing.net", true, "http", "thing.net", true),
			TestCase("thing.net", false, "https", "stuff.com", true),
			TestCase("thing.net", false, "https", "thing.net", true),
			TestCase("thing.net", false, "http", "stuff.com", true),
			TestCase("thing.net", false, "http", "thing.net", false)]
		public async Task WebUptimeMetricLogic_SaveNewRequestSettings(string previousHost, bool previousHttps, string urlScheme, string urlDomain, bool expectedResult)
		{
			//Arrange
			var server = new Server { UptimeMonitoringResourceHost = previousHost, UptimeMonitoringResourceUseHttps = previousHttps };
			var logic = new WebUptimeMetricLogic(httpClientFactoryMock.Object, serverRepository.Object, metricDataService.Object, configRepository.Object, logger.Object);
			serverRepository.Setup(r => r.UpdateAsync(It.IsAny<Server>())).Returns(Task.Delay(100));

			//Act
			await logic.SaveNewRequestSettings(server, new Uri($"{urlScheme}://{urlDomain}"));

			//Assert
			if (expectedResult)
				serverRepository.Verify(r => r.UpdateAsync(It.IsAny<Server>()), Times.Once);
			else
				serverRepository.Verify(r => r.UpdateAsync(It.IsAny<Server>()), Times.Never);
		}
	}
}
