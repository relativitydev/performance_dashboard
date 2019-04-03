namespace kCura.PDB.Service.Logging
{
	using System.Text;
	using kCura.PDB.Core.Extensions;
	using kCura.PDB.Core.Interfaces.Services;

	public class TextLogger : GenericLogger, ILogger
	{
		public TextLogger()
		{
			this.stringBuilder = new StringBuilder();
		}

		private StringBuilder stringBuilder;

		public string Text => this.stringBuilder.ToString();

		public void Clear()
		{
			this.stringBuilder.Clear();
		}

		protected override void Log(int level, string message, params string[] categories)
		{
			this.stringBuilder.AppendLineWithDelimiter(message);
		}
	}
}
