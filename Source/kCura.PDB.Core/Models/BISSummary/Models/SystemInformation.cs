namespace kCura.PDB.Core.Models.BISSummary.Models
{
	public class SystemInformation
	{
		public int ServerArtifactId;
		public string ServerName;
		public string Processor;
		public double? TotalMemoryGB;
		public double? FreeDiskSpaceGB;

		public string SummaryHtml
		{
			get
			{
				var cpuName = string.IsNullOrEmpty(Processor) ? "N/A" : Processor;
				var totalMemory = TotalMemoryGB.HasValue ? string.Format("{0} GB", TotalMemoryGB.Value.ToString()) : "N/A";
				var freeDisk = FreeDiskSpaceGB.HasValue ? string.Format("{0} GB", FreeDiskSpaceGB.Value.ToString()) : "N/A";

				return string.Format("<b>System Information</b><br/><br/>CPU: {0}<br/>Installed RAM: {1}<br/>Disk Space Free: {2}", cpuName, totalMemory, freeDisk);
			}
		}
	}
}
