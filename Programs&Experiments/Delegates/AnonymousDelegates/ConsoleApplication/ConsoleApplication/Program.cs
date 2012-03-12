using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace ConsoleApplication
{
	class Program
	{
		static void Main(string[] args)
		{
			Func<int> deleg = delegate { return 4; };
			Func<int> deleg2 = new Func<int>(delegate() { return 5; });


			Func<object[], object[]> deleg3 = delegate(object[] objs)
			{
				List<object> list = new List<object>(objs);
				
				list.Add(list.Count + 1);

				return list.ToArray();
			};

			Console.WriteLine(deleg());
			Console.WriteLine(deleg2());

			Console.WriteLine();

			foreach(object obj in deleg3(new object[]{1, 2, 3}))
			{
				Console.WriteLine(obj);
			}
		}
	}
}
