namespace kCura.PDB.Core.Models
{
    using System.Collections.Generic;
    using System.Linq;

    public class GeneralCheckGrid<T>
	{
		public GeneralCheckGrid()
		{
            Data = new List<T>().AsQueryable();
		}

		public int Count;
		public IQueryable<T> Data;
	}
}
