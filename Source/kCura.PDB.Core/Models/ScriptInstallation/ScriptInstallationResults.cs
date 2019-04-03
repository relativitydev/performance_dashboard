namespace kCura.PDB.Core.Models.ScriptInstallation
{
	using System.Collections.Generic;

	public class ScriptInstallationResults
	{
		public bool Success { get; set; }
		public List<ScriptInstallationMessage> Messages;

		public ScriptInstallationResults()
		{
			Messages = new List<ScriptInstallationMessage>();
		}

		public void AppendMessage(string message, bool isVerbose = false)
		{
			Messages.Add(new ScriptInstallationMessage
				{
					Verbose = isVerbose,
					Text = message
				});
		}
	}
}