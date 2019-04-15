namespace kCura.PDD.Web.Services
{
	using System;
	using System.Drawing;
	using System.IO;
	using DevExpress.Web.ASPxGauges;
	using DevExpress.Web.ASPxGauges.Gauges.Circular;
	using DevExpress.XtraGauges.Base;
	using DevExpress.XtraGauges.Core.Drawing;
	using kCura.PDB.Core.Interfaces.Services;

	public class ScoreImageService
    {
			private IQualityIndicatorService _indicatorService;
		
			public ScoreImageService(IQualityIndicatorService indicatorService)
			{
				_indicatorService = indicatorService;
			}

			public Bitmap GetOverall(int score, bool isWeeklyScore)
			{
				var control = new ASPxGaugeControl();
				var gauge = AddGauge(control, score);
				AddCategoryScoreLabels(gauge, score, isWeeklyScore);
				foreach (var label in ((CircularGauge)gauge).Labels)
				{
					label.AppearanceText.TextBrush = new SolidBrushObject
					{
						Color = System.Drawing.Color.White
					};
				}
				control.BackColor = ColorTranslator.FromHtml("#102D4E");
				return RenderGaugeControl(control);
			}

			public Bitmap GetCategory(int score, bool isWeeklyScore)
			{
				var control = new ASPxGaugeControl();
				var gauge = AddGauge(control, score);
				control.BackColor = Color.White;
				AddCategoryScoreLabels(gauge, score, isWeeklyScore);
				return RenderGaugeControl(control);
			}

			private IGauge AddGauge(ASPxGaugeControl gaugeControl, int score)
			{
				var gauge = (CircularGauge)gaugeControl.AddGauge(GaugeType.Circular);
				var scale = gauge.AddScale();

				//Set angles so gauge fills from "North" in the clockwise direction
				scale.StartAngle = -90;
				scale.EndAngle = 270;

				//Set range and current value to scale
				var scaleValue = (float)Math.Max(score, 0.1);
				scale.Renderable = false;
				scale.MinValue = 0;
				if (scaleValue <= 10)
					scale.MaxValue = scaleValue;
				else
					scale.MaxValue = 100;
				scale.Value = scaleValue; //If the score is zero, we still want some red to show				
				scale.DataBind();

				//Set colors on ranges
				var backRange = gauge.AddRangeBar();
				backRange.AppearanceRangeBar.ContentBrush = new SolidBrushObject()
				{
					Color = _indicatorService.GetColorByScore(-1)
				};
				backRange.Value = 100;
				backRange.StartOffset = 80;
				backRange.EndOffset = -12;

				var frontRange = gauge.AddRangeBar();
				frontRange.AppearanceRangeBar.ContentBrush = new SolidBrushObject()
				{
					Color = _indicatorService.GetColorByScore(score)
				};
				frontRange.StartOffset = 80;
				frontRange.EndOffset = -12;

				return gauge;
			}

			private void AddOverallScoreLabel(IGauge gauge, int score)
			{
				var scoreLabel = ((CircularGauge)gauge).AddLabel();
				scoreLabel.AppearanceText.TextBrush = new SolidBrushObject
				{
					Color = System.Drawing.Color.White
				};
				scoreLabel.Text = score >= 0
					? $"{score.ToString()}%"
					: "N/A";
				scoreLabel.AppearanceText.Font = new Font("Open Sans", 34, FontStyle.Bold);
				scoreLabel.Size = new SizeF(300F, 300F);
			}

			private void AddCategoryScoreLabels(IGauge gauge, int score, bool isWeekly)
			{
				var scoreLabel = ((CircularGauge)gauge).AddLabel();
				scoreLabel.Text = score >= 0
					? $"{score.ToString()}%"
					: "N/A";
				scoreLabel.AppearanceText.Font = new Font("Open Sans", 32, FontStyle.Bold);
				scoreLabel.Size = new SizeF(300F, 300F);

				var infoLabel = ((CircularGauge)gauge).AddLabel();
				infoLabel.Text = isWeekly ? "\n\nWeekly" : "\n\nQuarterly";
				infoLabel.AppearanceText.Font = new Font("Open Sans", 17, FontStyle.Bold);
				infoLabel.Size = new SizeF(300F, 300F);
			}

			private Bitmap RenderGaugeControl(ASPxGaugeControl gauge)
			{
				using (var stream = new MemoryStream())
				{
					gauge.ExportToImage(stream, System.Drawing.Imaging.ImageFormat.Png);
					return new Bitmap(stream);
				}
			}
    }
}