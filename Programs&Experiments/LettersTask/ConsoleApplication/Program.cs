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
			//
			const int N = 7;

			Console.WriteLine("Besporyadky:");

			for (int i = 1; i < N; i++)
			{
				Console.WriteLine(string.Format("{0} - {1}", i, besporyadok(i)));
			}

			Console.WriteLine("Letter task solution:");

			for (int i = 1; i < N; i++)
			{
				Console.WriteLine(string.Format("{0} - {1}", i, letterTask(i)));
			}
		}

		public static double letterTask(int letterCount)
		{
			if (letterCount < 1)
			{
				throw new ArgumentException("Parameter 'letterCount', must be greater than 1.");
			}
			else if (letterCount == 1)
			{
				return 0;
			}

			List<int> list = new List<int>(letterCount);
			for (int i = 1; i <= letterCount; i++)
			{
				list.Add(i);
			}

			long letterFact = factorial(letterCount);

			int perestanovky_S_besporyadkom = perestanovki<int>(list).Sum(perestanovka =>
				perestanovka.All(x => perestanovka.IndexOf(x) != x) ? (int)1 : (int)0);

			return (letterFact - perestanovky_S_besporyadkom) / (double)letterFact;
		}

		public static long factorial(int n)
		{
			if (n < 0)
			{
				throw new ArgumentException("Parameter 'n', must be greater or equal than 0.");
			}

			if (n < 1)
			{
				return 1;
			}

			return n * factorial(n - 1);

			//List<int> list = new List<int>(n);
			//for (int i = 0; i <= n; i++)
			//{
			//    list.Add(i);
			//}

			//return list.Aggregate(1, (long x, int y) => x * y);
		}

		public static List<List<T>> perestanovki<T>(IEnumerable<T> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException();
			}

			var result = new List<List<T>>();

			if (source.Count() < 1)
			{
				return result;
			}

			if (source.Count() == 1)
			{
				result.Add(source.ToList());
				return result;
			}

			if (source.Count() == 2)
			{
				result.Add(source.ToList());
				result.Add(source.Reverse().ToList());
				return result;
			}

			for (int i = 0; i < source.Count(); i++)
			{
				int varForCapturing = i;

				var curElem = source.ElementAt(varForCapturing);

				var source_Minus_CurElem = source.Where((x, indx) => indx != varForCapturing);

				var source_Minus_CurElem_Perestanovki = perestanovki(source_Minus_CurElem);

				var perestanovki_Plus_CurElem = source_Minus_CurElem_Perestanovki.ConvertAll(
					perestanovka => (new List<T>() { curElem }).Concat(perestanovka).ToList());

				result.AddRange(perestanovki_Plus_CurElem);
			}

			return result;
		}
		
		public static long besporyadok(int n)
		{
			if (n < 1)
			{
				throw new ArgumentException("Parameter 'n' must be greater than 1.");
			}
			
			List<int> list = new List<int>(n + 1);
			for (int k = 0; k <= n; k++)
			{
				list.Add(k);
			}

			long factN = factorial(n);

			return list.Sum(x => (long)Math.Pow(-1, x) * (factN / factorial(x)));
		}
	}

	public class TestClass
	{
		public int i = 0;
	}
}