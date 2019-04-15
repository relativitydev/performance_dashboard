namespace kCura.PDB.Core.Attributes
{
	using System;

	[AttributeUsage(AttributeTargets.Class)]
	public abstract class ServiceMappingAttribute : Attribute
	{
		protected ServiceMappingAttribute(object obj)
		{
			this.Type = obj;
		}

		public object Type { get; set; }
	}
}
