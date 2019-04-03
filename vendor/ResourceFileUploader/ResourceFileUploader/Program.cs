using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using kCura.Relativity.Client;
using ResourceFileUpload;

namespace ResourceFileUploader
{
	class Program
	{
		static void Main(string[] args)
		{
			try
			{
				ArgsToProcess parsedargs = new ArgsToProcess(args);

				if (parsedargs.showHelp)
				{
					ShowHelp();
				}
				else if(parsedargs.diagnosticsCheck)
				{
					CheckDiagnostics(parsedargs);
				}
				else if (parsedargs.updateDlls)
				{
					UpdateDlls(parsedargs);
				}
				else
				{
					RunUpdater(parsedargs);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine();
				Console.WriteLine();
				Console.WriteLine(ex.Message + Environment.NewLine + ex);
				System.Environment.Exit(-1);
			}

		}

		private static void CheckDiagnostics(ArgsToProcess parsedargs)
		{
			new ResourceFileUpload.ResourceFileUploader().DiagnosticsCheck(parsedargs);
		}
		private static void UpdateDlls(ArgsToProcess parsedargs)
		{
			new ResourceFileUpload.ResourceFileUploader().UpdateResourceFiles(parsedargs);
		}

		private static void RunUpdater(ArgsToProcess parsedargs)
		{
			new ResourceFileUpload.ResourceFileUploader().RunUpdater(parsedargs);
		}

		private static void ShowHelp()
		{
			Console.WriteLine("Help is under construction...");
		}
	}
}
