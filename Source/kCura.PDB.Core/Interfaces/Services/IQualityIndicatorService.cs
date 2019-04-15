namespace kCura.PDB.Core.Interfaces.Services
{
	using System.Drawing;
	using kCura.PDB.Core.Enumerations;

	public interface IQualityIndicatorService
	{
		QualityIndicator GetIndicatorForScore(int score);

		string GetCssClassForScore(int score, bool isBackground);

		Color GetColorByScore(int score);
	}
}
