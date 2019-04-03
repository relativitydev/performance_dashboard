using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace kCura.PDD.Web.Models.BISSummary
{
	public class ScoreChartViewModel
	{
		public int MinValue;
		public int MinDisplay
		{
			get
			{
				return Math.Max(MinValue - 5, 0);
			}
		}
		public int MaxValue;
		public int MaxDisplay
		{
			get
			{
				return Math.Min(MaxValue + 5, 100);
			}
		}
		public int ScaleSteps
		{
			get
			{
				return MaxDisplay - MinDisplay > 10
					? 10
					: 1;
			}
		}
		public double ScaleStepWidth
		{
			get
			{
				return ((double)(MaxDisplay - MinDisplay)) / ScaleSteps;
			}
		}

		public IEnumerable<string> Labels;
		public IEnumerable<ScoreChartDataSet> DataSets;
	}

	public class ScoreChartDataSet
	{
		public string label;
		public IEnumerable<bool?> sample;
		public IEnumerable<int?> data;
		public string strokeColor;
		public string pointColor;
	}
}