using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace kCura.PDD.Web.Enum
{
	public enum ErrorMode
	{
		General = 0,
		ReportDataPending = 1,
		AssemblySyncError = 2,
		AccessDenied = 3,
		Disabled = 51773
	}
}