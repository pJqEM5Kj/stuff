using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Media3D;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.InteropServices;


namespace ConsoleApplication
{
	class Program
	{
		public static void Main(string[] args)
		{
			//
			//string path = @"D:\DataForWork";
			//string path = @"D:\Data\";
			string path = @"D:\";


			Stopwatch sw = Stopwatch.StartNew();
			var files = getAllFiles(path);
			sw.Stop();

			Console.WriteLine(string.Format("Files count = {0}, list build time = {1} ms.", 
				files.Count, sw.ElapsedMilliseconds));
		}

		public static List<string> getAllFiles(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				throw new ArgumentException("Parametr 'path' is null or empty.");
			}

			try
			{
				//List<string> files = Directory.GetFiles(path, "*.*", SearchOption.TopDirectoryOnly).ToList();
				//string[] dirs = Directory.GetDirectories(path, "*", SearchOption.TopDirectoryOnly);

				//foreach (string dir in dirs)
				//{
				//    files.AddRange(getAllFiles(dir));
				//}

				//return files;


				return Directory.GetDirectories(path, "*", SearchOption.TopDirectoryOnly).Aggregate(
					Directory.GetFiles(path, "*.*", SearchOption.TopDirectoryOnly).AsEnumerable(),
					(x, y) => x.Concat(getAllFiles(y))).ToList();
			}
			catch (UnauthorizedAccessException)
			{
				return new List<string>();
			}
		}
	}

	public class TestClass
	{
		public int i = 0;
	}
}