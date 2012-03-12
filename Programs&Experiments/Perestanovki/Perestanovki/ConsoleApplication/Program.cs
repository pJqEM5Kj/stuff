using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
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


namespace ConsoleApplication
{
	class Program
	{
		public static void Main(string[] args)
		{
			List<int> list = new List<int>() { 0, 1, 2, 3, 4, 5, 6, }; //7, 8 };

			var sw = Stopwatch.StartNew();
			var v = perestanovki<int>(list);
			sw.Stop();


			Console.WriteLine(string.Format("Perestanovki count = {0}, elapsed time = {1} ms.",
				v.Count, sw.ElapsedMilliseconds));

			//foreach (var perestanovka in v)
			//{
			//    foreach (var elem in perestanovka)
			//    {
			//        Console.Write(elem + " ");
			//    }

			//    Console.WriteLine();
			//}
		}

		public static List<IEnumerable<T>> perestanovki<T>(IEnumerable<T> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException();
			}

			var result = new List<IEnumerable<T>>();

			if (source.Count() < 1)
			{
				return result;
			}

			if (source.Count() == 1)
			{
				result.Add(source);
				return result;
			}

			if (source.Count() == 2)
			{
				result.Add(source);
				result.Add(source.Reverse());
				return result;
			}

			for (int i = 0; i < source.Count(); i++)
			{
				int varForCapturing = i;

				var curElem = source.ElementAt(varForCapturing);

				var source_Minus_CurElem = source.Where((x, indx) => indx != varForCapturing);

				var source_Minus_CurElem_Perestanovki = perestanovki(source_Minus_CurElem);

				var perestanovki_Plus_CurElem = source_Minus_CurElem_Perestanovki.ConvertAll(
					perestanovka => (new List<T>() { curElem }).Concat(perestanovka));

				result.AddRange(perestanovki_Plus_CurElem);
			}

			return result;
		}
	}

	public class TestClass
	{
		public int i = 0;
	}
}