namespace kCura.PDB.Core.Attributes
{
	using System;
	using kCura.PDB.Core.Models;

	[AttributeUsage(AttributeTargets.Class)]
	public sealed class CategoryTypeAttribute : ServiceMappingAttribute
	{
		public CategoryTypeAttribute(CategoryType type)
			: base(type)
		{

		}
	}
}