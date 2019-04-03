using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace kCura.PDD.Web.Models
{
	public class BackfillStatus
	{
		public string SampleRange { get; set; }
		public string SystemState { get; set; }
		public int HoursAwaitingAnalysis { get; set; }
		public int HoursAwaitingScoring { get; set; }
		public int HoursAwaitingDiscovery { get; set; }
		public int HoursCompleted { get; set; }
		public string LastMessage { get; set; }
		public string LastEventExecuted { get; set; }
		public int PendingEvents { get; set; }
		public int ErrorEvents { get; set; }


	}
}