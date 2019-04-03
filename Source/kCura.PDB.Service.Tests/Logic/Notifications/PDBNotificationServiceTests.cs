namespace kCura.PDB.Service.Tests.Logic.Notifications
{
	using System;
	using System.Data;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Service.Notifications;
	using Moq;
	using NUnit.Framework;

	[TestFixture, Category("Unit")]
	public class PdbNotificationServiceTests
	{
		[SetUp]
		public void Setup()
		{
			this.eventRepository = new Mock<IEventRepository>();
			this.serverRepository = new Mock<IServerRepository>();
			this.pdbNotificationRepository = new Mock<IPDBNotificationRepository>();
			this.sqlServerRepository = new Mock<ISqlServerRepository>();
			this.sqlServerRepository.SetupGet(r => r.EventRepository).Returns(this.eventRepository.Object);
			this.sqlServerRepository.SetupGet(r => r.PerformanceServerRepository).Returns(this.serverRepository.Object);
			this.sqlServerRepository.SetupGet(r => r.PDBNotificationRepository).Returns(this.pdbNotificationRepository.Object);
			this.pdbNotificationService = new PdbNotificationService(this.sqlServerRepository.Object);
		}

		private PdbNotificationService pdbNotificationService;
		private Mock<IEventRepository> eventRepository;
		private Mock<IServerRepository> serverRepository;
		private Mock<IPDBNotificationRepository> pdbNotificationRepository;
		private Mock<ISqlServerRepository> sqlServerRepository;


		[Test]
		[TestCase(1, Messages.Notification.DeploymentFailure, NotificationType.Critical)]
		[TestCase(2, "PDB Process Control(s) Failed when executing.", NotificationType.Critical)]
		[TestCase(3, "There must be at least 1 ", NotificationType.Warning)]
		[TestCase(4, Messages.Notification.NoSqlAgentsReporting, NotificationType.Warning)]
		public void GetNext_CorrectPrecedence(int precedence, string expectedMessage, NotificationType expectedSeverity)
		{
			//Expected order
			//ProccessControlNotification
			var data = GetTestData_ProcesControlData();
			var data2 = GetTestData_ProcesControlDataNoErrors();
			if (precedence <= 2)
				this.pdbNotificationRepository.Setup(r => r.GetFailingProcessControls()).Returns(data);
			else
				this.pdbNotificationRepository.Setup(r => r.GetFailingProcessControls()).Returns(data2);

			//AgentsAlert
			data = GetTestData_DisabledAgent();
			data2 = GetTestData_OneOfEachAgent();
			if (precedence <= 3)
				this.pdbNotificationRepository.Setup(r => r.GetAgentsAlert()).Returns(data);
			else
				this.pdbNotificationRepository.Setup(r => r.GetAgentsAlert()).Returns(data2);
			//SqlAgentsAlert
			if (precedence <= 4)
				this.pdbNotificationRepository.Setup(r => r.GetSecondsSinceLastAgentHistoryRecord()).Returns((Int32?)null);

			// Deployment Alert
			this.serverRepository.Setup(r => r.ReadAllActiveAsync())
				.ReturnsAsync(new[] { new Server { ServerId = 1234, ServerType = ServerType.Database } });
			this.eventRepository.Setup(r => r.ReadLastBySourceIdAndTypeAsync(EventSourceType.DeployServerDatabases, 1234))
					.ReturnsAsync(new Event { Status = (precedence <= 1) ? EventStatus.Error : EventStatus.Completed });
			this.eventRepository.Setup(r => r.ReadEventSystemStateAsync())
				.ReturnsAsync(EventSystemState.Prerequisites);

			//Act
			var result = this.pdbNotificationService.GetNext();

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Message, Is.StringContaining(expectedMessage));
			Assert.That(result.Type, Is.EqualTo(expectedSeverity));
		}

		[Test]
		public void GetNext_NoErrorsOrWarnings()
		{
			//Expected order
			//Proccess Control Notification
			var data2 = GetTestData_ProcesControlDataNoErrors();
			this.pdbNotificationRepository.Setup(r => r.GetFailingProcessControls()).Returns(data2);
			//Agent sAlert
			data2 = GetTestData_OneOfEachAgent();
			this.pdbNotificationRepository.Setup(r => r.GetAgentsAlert()).Returns(data2);
			//Sql Agents Alert
			this.pdbNotificationRepository.Setup(r => r.GetSecondsSinceLastAgentHistoryRecord()).Returns(60);
			// Deployment Alert
			this.serverRepository.Setup(r => r.ReadAllActiveAsync())
				.ReturnsAsync(new[] { new Server { ServerId = 1234, ServerType = ServerType.Database } });
			this.eventRepository.Setup(r => r.ReadLastBySourceIdAndTypeAsync(EventSourceType.DeployServerDatabases, 1234))
					.ReturnsAsync(new Event { Status = EventStatus.Completed });
			this.eventRepository.Setup(r => r.ReadEventSystemStateAsync())
				.ReturnsAsync(EventSystemState.Prerequisites);

			//Act
			var result = this.pdbNotificationService.GetNext();

			//Assert
			Assert.That(result, Is.Null);
		}

		[Test]
		public void GetAgentsAlert()
		{
			var data = GetTestData_DisabledAgent();
			this.pdbNotificationRepository.Setup(r => r.GetAgentsAlert()).Returns(data);

			//Act
			var result = this.pdbNotificationService.GetAgentsAlert();

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Message, Is.Not.Null);
			Assert.That(result.Message, Is.Not.Empty);
			Assert.That(result.Type, Is.EqualTo(NotificationType.Warning));
		}

		[Test]
		public void GetAgentsAlert_TrustAgentIsIgnored()
		{
			var data = GetTestData_DisabledTrustAgent();
			this.pdbNotificationRepository.Setup(r => r.GetAgentsAlert()).Returns(data);

			//Act
			var result = this.pdbNotificationService.GetAgentsAlert();

			//Assert
			Assert.That(result, Is.Null); //should be null since trust agent is the only agent type with zero agents
		}

		[Test]
		public void GetAgentsAlert_OneOfEachAgent()
		{
			var data = GetTestData_OneOfEachAgent();
			this.pdbNotificationRepository.Setup(r => r.GetAgentsAlert()).Returns(data);

			//Act
			var result = this.pdbNotificationService.GetAgentsAlert();

			//Assert
			Assert.That(result, Is.Null); //should be null since there is at least one of each agent type
		}

		[Test]
		public void GetProccessControlNotification()
		{
			var data = GetTestData_ProcesControlData();
			this.pdbNotificationRepository.Setup(r => r.GetFailingProcessControls()).Returns(data);

			//Act
			var result = this.pdbNotificationService.GetProccessControlNotification();

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Message, Is.Not.Null);
			Assert.That(result.Message, Is.Not.Empty);
			Assert.That(result.Type, Is.EqualTo(NotificationType.Critical));
		}

		[Test]
		public void GetProccessControlNotification_NoErrors()
		{
			var data = GetTestData_ProcesControlDataNoErrors();
			this.pdbNotificationRepository.Setup(r => r.GetFailingProcessControls()).Returns(data);

			//Act
			var result = this.pdbNotificationService.GetProccessControlNotification();

			//Assert
			Assert.That(result, Is.Null);
		}

		[Test]
		public void GetSqlAgentsAlert()
		{
			this.pdbNotificationRepository.Setup(r => r.GetSecondsSinceLastAgentHistoryRecord()).Returns(int.MaxValue);

			//Act
			var result = this.pdbNotificationService.GetSqlAgentsAlert();

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Message, Is.Not.Null);
			Assert.That(result.Message, Is.Not.Empty);
			Assert.That(result.Type, Is.EqualTo(NotificationType.Warning));
		}

		[Test]
		public void GetSqlAgentsAlert_NoHistoryRecords()
		{
			this.pdbNotificationRepository.Setup(r => r.GetSecondsSinceLastAgentHistoryRecord()).Returns((int?)null);

			//Act
			var result = this.pdbNotificationService.GetSqlAgentsAlert();

			//Assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Message, Is.Not.Null);
			Assert.That(result.Message, Is.Not.Empty);
			Assert.That(result.Type, Is.EqualTo(NotificationType.Warning));
		}

		[Test]
		public void GetSqlAgentsAlert_SqlAgentIsReporting()
		{
			this.pdbNotificationRepository.Setup(r => r.GetSecondsSinceLastAgentHistoryRecord()).Returns(10);

			//Act
			var result = this.pdbNotificationService.GetSqlAgentsAlert();

			//Assert
			Assert.That(result, Is.Null);
		}

		[Test,
			TestCase(EventStatus.Error, EventSystemState.Prerequisites, NotificationType.Critical, true),
			TestCase(EventStatus.Error, EventSystemState.Normal, NotificationType.Warning, true),
			TestCase(EventStatus.Completed, EventSystemState.Prerequisites, null, false),
			TestCase(EventStatus.Completed, EventSystemState.Normal, null, false)]
		public void DatabaseDeploymentFailureAlert(EventStatus status, EventSystemState systemState, NotificationType? expctedType, bool expectedResult)
		{
			this.serverRepository.Setup(r => r.ReadAllActiveAsync())
				.ReturnsAsync(new[] { new Server { ServerId = 1234, ServerType = ServerType.Database } });
			this.eventRepository.Setup(r => r.ReadLastBySourceIdAndTypeAsync(EventSourceType.DeployServerDatabases, 1234))
				.ReturnsAsync(new Event { Status = status });
			this.eventRepository.Setup(r => r.ReadEventSystemStateAsync())
				.ReturnsAsync(systemState);

			//Act
			var result = this.pdbNotificationService.GetDatabaseDeploymentFailureAlert();

			//Assert
			Assert.That(result != null, Is.EqualTo(expectedResult));
			Assert.That(result?.Type, Is.EqualTo(expctedType));
		}


		private DataTable GetTestData_DisabledAgent()
		{
			var disabledAgentData = new DataTable();
			disabledAgentData.Columns.Add("NumberOfAgents", typeof(int));
			disabledAgentData.Columns.Add("Name", typeof(string));
			var row = disabledAgentData.Rows.Add();
			row["NumberOfAgents"] = 1;
			row["Name"] = "Performance Dashboard - QoS Manager";
			var row2 = disabledAgentData.Rows.Add();
			row2["NumberOfAgents"] = 2;
			row2["Name"] = "Performance Dashboard - QoS Worker";
			var row3 = disabledAgentData.Rows.Add();
			row3["NumberOfAgents"] = 0;
			row3["Name"] = "Performance Dashboard - WMI Worker";
			var row4 = disabledAgentData.Rows.Add();
			row4["NumberOfAgents"] = 0;
			row4["Name"] = "Performance Dashboard - Trust Worker";

			return disabledAgentData;
		}

		private DataTable GetTestData_DisabledTrustAgent()
		{
			var data = new DataTable();
			data.Columns.Add("NumberOfAgents", typeof(int));
			data.Columns.Add("Name", typeof(string));
			var row = data.Rows.Add();
			row["NumberOfAgents"] = 1;
			row["Name"] = "Performance Dashboard - QoS Manager";
			var row2 = data.Rows.Add();
			row2["NumberOfAgents"] = 2;
			row2["Name"] = "Performance Dashboard - QoS Worker";
			var row3 = data.Rows.Add();
			row3["NumberOfAgents"] = 1;
			row3["Name"] = "Performance Dashboard - WMI Worker";
			var row4 = data.Rows.Add();
			row4["NumberOfAgents"] = 0; //set to zero, but no warning should be alerted
			row4["Name"] = "Performance Dashboard - Trust Worker";

			return data;
		}

		private DataTable GetTestData_OneOfEachAgent()
		{
			var data = new DataTable();
			data.Columns.Add("NumberOfAgents", typeof(int));
			data.Columns.Add("Name", typeof(string));
			var row = data.Rows.Add();
			row["NumberOfAgents"] = 1;
			row["Name"] = "Performance Dashboard - QoS Manager";
			var row2 = data.Rows.Add();
			row2["NumberOfAgents"] = 2;
			row2["Name"] = "Performance Dashboard - QoS Worker";
			var row3 = data.Rows.Add();
			row3["NumberOfAgents"] = 1;
			row3["Name"] = "Performance Dashboard - WMI Worker";
			var row4 = data.Rows.Add();
			row4["NumberOfAgents"] = 1;
			row4["Name"] = "Performance Dashboard - Trust Worker";

			return data;
		}

		private DataTable GetTestData_ProcesControlData()
		{
			var data = new DataTable();
			data.Columns.Add("ProcessControlID", typeof(int));
			data.Columns.Add("ProcessTypeDesc", typeof(string));
			data.Columns.Add("LastProcessExecDateTime", typeof(DateTime));
			data.Columns.Add("Frequency", typeof(int));
			data.Columns.Add("LastExecSucceeded", typeof(bool));
			data.Columns.Add("LastErrorMessage", typeof(string));
			var row = data.Rows.Add();
			row["ProcessControlID"] = 24;
			row["ProcessTypeDesc"] = "Environment Check Server Info";
			row["LastProcessExecDateTime"] = DateTime.Parse("2016-02-08 20:00:00.000");
			row["Frequency"] = 1440;
			row["LastExecSucceeded"] = false;
			row["LastErrorMessage"] = "System.Data.SqlClient.SqlException ...";
			var row2 = data.Rows.Add();
			row2["ProcessControlID"] = 26;
			row2["ProcessTypeDesc"] = "PC2";
			row2["LastProcessExecDateTime"] = DateTime.Now;
			row2["Frequency"] = 60;
			row2["LastExecSucceeded"] = false;
			row2["LastErrorMessage"] = "System.Data.SqlClient.SqlException ...";

			return data;
		}

		private DataTable GetTestData_ProcesControlDataNoErrors()
		{
			var data = new DataTable();
			data.Columns.Add("ProcessControlID", typeof(int));
			data.Columns.Add("ProcessTypeDesc", typeof(string));
			data.Columns.Add("LastProcessExecDateTime", typeof(DateTime));
			data.Columns.Add("Frequency", typeof(int));
			data.Columns.Add("LastExecSucceeded", typeof(bool));
			data.Columns.Add("LastErrorMessage", typeof(string));

			return data;
		}
	}
}
