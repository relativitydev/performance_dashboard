namespace kCura.PDB.Core.Models
{
	public class Metric
	{
		public int Id { get; set; }

		public int MetricTypeId { get; set; }

		public int HourId { get; set; }

		public int SampleTypeId { get; set; }

		public SampleType SampleType
		{
			get { return (SampleType)this.SampleTypeId; }
			set { this.SampleTypeId = (int)value; }
		}

		public MetricType MetricType
		{
			get { return (MetricType)this.MetricTypeId; }
			set { this.MetricTypeId = (int)value; }
		}

		public Hour Hour { get; set; }
	}
}
