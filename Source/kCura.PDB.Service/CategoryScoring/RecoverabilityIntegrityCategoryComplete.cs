namespace kCura.PDB.Service.CategoryScoring
{
	using System.Threading.Tasks;

	using kCura.PDB.Core.Attributes;
	using kCura.PDB.Core.Interfaces.CategoryScoring;
	using kCura.PDB.Core.Models;

    [CategoryType(CategoryType.RecoverabilityIntegrity)]
	public class RecoverabilityIntegrityCategoryComplete : ICategoryCompleteLogic
	{
		public Task CompleteCategory(Category category)
		{
			return Task.FromResult(0);
		}
	}
}
