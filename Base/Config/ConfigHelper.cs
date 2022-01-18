using Newtonsoft.Json;
using System;
using System.IO;

namespace Base
{
	public static class ConfigHelper
	{
		public static string GetText(string path)
		{
			try
			{
				string configStr = File.ReadAllText(path);
				return configStr;
			}
			catch (Exception e)
			{
				throw new Exception($"load config file fail, path: {path} {e}");
			}
		}

		public static T ToObject<T>(string str)
		{
			return JsonConvert.DeserializeObject<T>(str);
		}

		public static string ToJson(object obj)
        {
			return JsonConvert.SerializeObject(obj);
		}
	}
}
