namespace kCura.PDB.Core.Models
{
	public class Category
	{
		public int Id { get; set; }

		public int CategoryTypeId { get; set; }

		public int HourId { get; set; }

		public CategoryType CategoryType
		{
			get { return (CategoryType)this.CategoryTypeId; }
			set { this.CategoryTypeId = (int)value; }
		}
	}
}
