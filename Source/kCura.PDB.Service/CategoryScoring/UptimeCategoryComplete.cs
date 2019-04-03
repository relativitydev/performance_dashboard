namespace kCura.PDB.Service.CategoryScoring
{
    using System.Threading.Tasks;

    using kCura.PDB.Core.Attributes;
    using kCura.PDB.Core.Interfaces.CategoryScoring;
    using kCura.PDB.Core.Models;

    [CategoryType(CategoryType.Uptime)]
	public class UptimeCategoryComplete : ICategoryCompleteLogic
	{
		public Task CompleteCategory(Category category)
		{
			return Task.FromResult(0);
		}
	}
}
