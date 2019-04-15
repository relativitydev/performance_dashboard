namespace kCura.PDB.Core.Models
{
	using System.Collections.Generic;
	using kCura.PDB.Core.Constants;

	public class Tab
	{
		public string Name { get; set; }

		public int ArtifactId { get; set; }

		public int DisplayOrder { get; set; }

		public string ExternalLink { get; set; }

		public int ParentArtifactId { get; set; }

		public static readonly Tab PerformanceDashboard = new Tab
		{
			Name = Names.Tab.PerformanceDashboard,
			DisplayOrder = 10000,
			ExternalLink = null,
			ParentArtifactId = 62
		};

		public static readonly Tab QualityOfService = new Tab
		{
			Name = Names.Tab.QualityOfService,
			DisplayOrder = 10100,
			ExternalLink = @"%ApplicationPath%/CustomPages/60a1d0a3-2797-4fb3-a260-614cbfd3fa0d/mvc/servicequality"
		};

		public static readonly Tab UserExperience = new Tab
		{
			Name = Names.Tab.UserExperience,
			DisplayOrder = 10200,
			ExternalLink = @"%ApplicationPath%/CustomPages/60a1d0a3-2797-4fb3-a260-614cbfd3fa0d/UserExperienceServer.aspx?StandardsCompliance=true"
		};

		public static readonly Tab InfrastructurePerformance = new Tab
		{
			Name = Names.Tab.InfrastructurePerformance,
			DisplayOrder = 10300,
			ExternalLink = @"%ApplicationPath%/CustomPages/60a1d0a3-2797-4fb3-a260-614cbfd3fa0d/SystemLoadServer.aspx?StandardsCompliance=true"
		};

		public static readonly Tab RecoverabilityIntegrity = new Tab
		{
			Name = Names.Tab.RecoverabilityIntegrity,
			DisplayOrder = 10400,
			ExternalLink = @"%ApplicationPath%/CustomPages/60a1d0a3-2797-4fb3-a260-614cbfd3fa0d/Backup.aspx?StandardsCompliance=true"
		};

		public static readonly Tab Uptime = new Tab
		{
			Name = Names.Tab.Uptime,
			DisplayOrder = 10500,
			ExternalLink = @"%ApplicationPath%/CustomPages/60a1d0a3-2797-4fb3-a260-614cbfd3fa0d/Uptime.aspx?StandardsCompliance=true"
		};

		public static readonly Tab ServerHealth = new Tab
		{
			Name = Names.Tab.ServerHealth,
			DisplayOrder = 10700,
			ExternalLink = @"%ApplicationPath%/CustomPages/60a1d0a3-2797-4fb3-a260-614cbfd3fa0d/ServerHealth.aspx?StandardsCompliance=true"
		};

		public static readonly Tab Configuration = new Tab
		{
			Name = Names.Tab.Configuration,
			DisplayOrder = 10800,
			ExternalLink = @"%ApplicationPath%/CustomPages/60a1d0a3-2797-4fb3-a260-614cbfd3fa0d/mvc/configuration"
		};

		public static readonly Tab BackfillConsole = new Tab
		{
			Name = Names.Tab.BackfillConsole,
			DisplayOrder = 10900,
			ExternalLink = @"%ApplicationPath%/CustomPages/60a1d0a3-2797-4fb3-a260-614cbfd3fa0d/mvc/backfill"
		};

		public static readonly Tab EnvironmentCheck = new Tab
		{
			Name = Names.Tab.EnvironmentCheck,
			DisplayOrder = 10950,
			ExternalLink = @"%ApplicationPath%/CustomPages/60a1d0a3-2797-4fb3-a260-614cbfd3fa0d/EnvironmentCheck.aspx?StandardsCompliance=true"
		};

		public static readonly Tab MaintenanceScheduling = new Tab
		{
			Name = Names.Tab.MaintenanceScheduling,
			DisplayOrder = 11000,
			ExternalLink = @"%ApplicationPath%/CustomPages/60a1d0a3-2797-4fb3-a260-614cbfd3fa0d/MaintenanceWindows.aspx?StandardsCompliance=true"
		};

		public static readonly IList<Tab> AllChildTabs = new[]
		{
			ServerHealth,
			QualityOfService,
			UserExperience,
			InfrastructurePerformance,
			RecoverabilityIntegrity,
			Uptime,
			Configuration,
			BackfillConsole,
			EnvironmentCheck,
			MaintenanceScheduling
		};
	}
}
