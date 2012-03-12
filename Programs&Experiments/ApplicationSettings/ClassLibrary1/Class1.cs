using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;

using System.Configuration;
using System.IO;

namespace ClassLibrary1
{
	public class Class1
	{
		public static string getApplicationSettingValue(string key)
		{
			string path = null;

			NameValueCollection appStgs = ConfigurationManager.AppSettings;

			for (int i = 0; i < appStgs.AllKeys.Length; i++)
			{
				if (appStgs.AllKeys[i] == key)
				{
					path = appStgs[i];
					break;
				}
			}

			//optional for directory
			if (path != null && path[path.Length - 1] != Path.DirectorySeparatorChar)
			{
				path += Path.DirectorySeparatorChar;
			}

			return path;
		}
	}
}
