using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace kCura.PDD.Web.Models
{
	public class DataTableResponse
	{
		public int draw;
		public int recordsTotal;
		public int recordsFiltered;

		public string sEcho;
		public string[][] aaData;
	}
}