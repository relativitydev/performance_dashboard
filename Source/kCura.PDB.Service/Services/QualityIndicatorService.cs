namespace kCura.PDB.Service.Services
{
	using System.Drawing;
	using kCura.PDB.Core.Constants;
	using kCura.PDB.Core.Enumerations;
	using kCura.PDB.Core.Interfaces.Repositories;
	using kCura.PDB.Core.Interfaces.Services;
	using kCura.PDB.Core.Models;
	using kCura.PDB.Core.Services;

	public class QualityIndicatorService : IQualityIndicatorService
	{
		private QualityIndicatorLevel qualityLevels;


		public QualityIndicatorService(IQualityIndicatorConfigurationService indicatorConfigurationService)
		{
			this.qualityLevels = indicatorConfigurationService.GetIndictatorConfiguration();
		}

		public QualityIndicator GetIndicatorForScore(int score)
		{
			if (score >= this.qualityLevels.PassScore)
			{
				return QualityIndicator.Pass;
			}
			else if (score < this.qualityLevels.PassScore && score >= this.qualityLevels.WarnScore)
			{
				return QualityIndicator.Warn;
			}
			else if (score >= 0 && score < this.qualityLevels.WarnScore)
			{
				return QualityIndicator.Fail;
			}
			else
			{
				return QualityIndicator.None;
			}
		}

		public QualityIndicatorLevel GetIndicatorLevels()
		{
			return this.qualityLevels;
		}

		public Color GetColorByScore(int score)
		{
			if (score >= this.qualityLevels.PassScore)
			{
				return ColorTranslator.FromHtml("green");
			}
			else if (score < this.qualityLevels.PassScore && score >= this.qualityLevels.WarnScore)
			{
				return ColorTranslator.FromHtml("#CACD00"); // color 'yellow' is unreadable on most backgrounds, this is a darker yellow
			}
			else if (score >= 0 && score < this.qualityLevels.WarnScore)
			{
				return ColorTranslator.FromHtml("red");
			}
			else
			{
				return ColorTranslator.FromHtml("#C1C1C0"); // light gray
			}
		}

		public string GetCssClassForScore(int score, bool isBackground)
		{
			// See site.css for css class details
			var indicator = this.GetIndicatorForScore(score);
			switch (indicator)
			{
				case QualityIndicator.Pass:
					return isBackground ? "passBackground" : "passText";
				case QualityIndicator.Warn:
					return isBackground ? "warnBackground" : "warnText";
				case QualityIndicator.Fail:
					return isBackground ? "failBackground" : "failText";
				case QualityIndicator.None:
					return isBackground ? "noneBackground" : "noneText";
				default:
					return string.Empty;
			}
		}

	}
}