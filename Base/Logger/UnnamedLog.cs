using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base
{
	public static class GlobalLog
	{
		private static readonly ILog globalLog = new NLogAdapter("global");

		public static void Trace(string message)
		{
			globalLog.Trace(message);
		}

		public static void Warning(string message)
		{
			globalLog.Warning(message);
		}

		public static void Info(string message)
		{
			globalLog.Info(message);
		}

		public static void Debug(string message)
		{
			globalLog.Debug(message);
		}

		public static void Error(Exception e)
		{
			globalLog.Error(e.ToString());
		}

		public static void Error(string message)
		{
			globalLog.Error(message);
		}

		public static void Fatal(Exception e)
		{
			globalLog.Fatal(e.ToString());
		}

		public static void Fatal(string message)
		{
			globalLog.Fatal(message);
		}
	}
}
