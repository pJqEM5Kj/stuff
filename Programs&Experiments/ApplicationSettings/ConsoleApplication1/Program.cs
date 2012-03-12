using System;
using System.Collections.Generic;
using System.Text;

using ClassLibrary1;


namespace ConsoleApplication1
{
	class Program
	{
		static void Main(string[] args)
		{
			string str = Class1.getApplicationSettingValue("FontDirectory");

			Console.WriteLine(str);
		}
	}
}
