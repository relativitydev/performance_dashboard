namespace kCura.PDB.Core.Attributes
{
	using System;
	using kCura.PDB.Core.Models;

	[AttributeUsage(AttributeTargets.Class)]
	public sealed class MetricTypeAttribute : ServiceMappingAttribute
	{
		public MetricTypeAttribute(MetricType type)
			:base(type)
		{

		}
	}
}