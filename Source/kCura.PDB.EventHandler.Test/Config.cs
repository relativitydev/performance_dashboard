using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kCura.PDB.EventHandler.Test
{
	public static class Config
	{
		static Config()
		{
			WorkSpaceId = int.Parse(ConfigurationManager.AppSettings["WorkSpaceId"]);
			RSAPIUserName = ConfigurationManager.AppSettings["RSAPIUserName"];
			RSAPIPassword = ConfigurationManager.AppSettings["RSAPIPassword"];
			RSAPIDomain = ConfigurationManager.AppSettings["RSAPIDomain"];
			RSAPIProtocol = ConfigurationManager.AppSettings["RSAPIProtocol"];
			RSAPIUseWindowsAuthentication = bool.Parse(ConfigurationManager.AppSettings["RSAPIUseWindowsAuthentication"]);
			EddsdboUsername = ConfigurationManager.AppSettings["EddsdboUsername"];
			EddsdboPassword = ConfigurationManager.AppSettings["EddsdboPassword"];
			MdfPath = ConfigurationManager.AppSettings["MdfPath"];
			LdfPath = ConfigurationManager.AppSettings["LdfPath"];
			SAUsername = ConfigurationManager.AppSettings["SAUserName"];
			SAPassword = ConfigurationManager.AppSettings["SAPassword"];
		}

		public static int WorkSpaceId { get; private set; }
		public static string RSAPIUserName { get; private set; }
		public static string RSAPIPassword { get; private set; }
		public static string RSAPIDomain { get; private set; }
		public static string RSAPIProtocol { get; private set; }
		public static bool RSAPIUseWindowsAuthentication { get; private set; }
		public static string EddsdboUsername { get; private set; }
		public static string EddsdboPassword { get; private set; }
		public static string MdfPath { get; private set; }
		public static string LdfPath { get; private set; }
		public static string SAUsername { get; private set; }
		public static string SAPassword { get; private set; }
	}
}
