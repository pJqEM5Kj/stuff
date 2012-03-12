using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.ComponentModel;
using System.Threading;

namespace ConsoleApplication1
{
	class Program
	{
		public static void Main()
		{
			//
			Console.WriteLine("Start");
			Console.WriteLine(Calc.method(16));
			Console.WriteLine("End");

			RunWorkerCompletedEventHandler qwe = new RunWorkerCompletedEventHandler(
				delegate(object sender, RunWorkerCompletedEventArgs e)
				{
					Console.WriteLine("Async call result: " + (double)e.Result);
				});

			Console.WriteLine("Start2");
			Calc.methodAsync(16, qwe);
			Console.WriteLine("End2");
		}
	}

	class Calc
	{
		public static double method(int i)
		{
			DoWorkEventArgs args = new DoWorkEventArgs(i);
			DoWorkEventHandler work = new DoWorkEventHandler(bw_DoWork);
			
			work(null, args);

			return (double)args.Result;
		}

		public static void methodAsync(int i, RunWorkerCompletedEventHandler onComplete)
		{
			BackgroundWorker bw = new BackgroundWorker();
			bw.DoWork += new DoWorkEventHandler(bw_DoWork);
			bw.RunWorkerCompleted += onComplete;

			bw.RunWorkerAsync(i);
		}

		static void bw_DoWork(object sender, DoWorkEventArgs e)
		{
			e.Result = Math.Sqrt((int)e.Argument);
		}
	}
}
