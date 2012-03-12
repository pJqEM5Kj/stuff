using System;
using System.Collections.Generic;
using System.Text;

using System.Windows;
using System.Diagnostics;
using System.IO;
using System.Collections;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Reflection;
using System.Threading;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace ConsoleApplication1
{
    class Program
    {
		enum QQQ : long
		{
			// powers of 2
			aaa = 2,
			bbb = 4,
			ccc = bbb * 2 //8
		}
		
		static void Main(string[] args)
        {
			// init
			//QQQ qqq = QQQ.bbb;
			QQQ qqq = QQQ.aaa | QQQ.bbb;
			
			// adding
			qqq = qqq | QQQ.ccc;

			// is containing
			Console.WriteLine((qqq & QQQ.aaa) == QQQ.aaa);

			// inverting state
			qqq = qqq ^ QQQ.aaa;
			Console.WriteLine(qqq);

			// is containing
			Console.WriteLine((qqq & QQQ.aaa) == QQQ.aaa);
        }
    }
}
