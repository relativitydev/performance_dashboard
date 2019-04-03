using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceFileUpload
{
	public class CustomPageInfo
	{
		public string GUID { get; set;}
		public string FilePath { get; set; }

		public CustomPageInfo()
		{ }
		public CustomPageInfo(string args)
		{
			string[] separator = new string[] { "=" };
			string[] values = args.Split(separator, StringSplitOptions.RemoveEmptyEntries);
			if (values.Length == 2)
			{
				GUID = values[0];
				FilePath = values[1];
			}
		}
	}
}
