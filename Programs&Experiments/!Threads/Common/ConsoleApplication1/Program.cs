using System;
using System.Collections.Generic;
using System.Text;

using System.Threading;
using System.Collections;

namespace ConsoleApplication1
{
	class Program
	{
		static void Main(string[] args)
		{
			QQQ qwe = new QQQ();

			DataForThreads.i = 500;

			DataForThreads.thread1 = new Thread(qwe.method1);
			DataForThreads.thread2 = new Thread(qwe.method2);
			DataForThreads.thread3 = new Thread(WWW.listenForAbort);


			DataForThreads.thread1.Start();
			DataForThreads.thread2.Start();
			DataForThreads.thread3.Start();

			DataForThreads.thread3.Join();
			Console.WriteLine("That is all!");
		}
	}


	class QQQ
	{
		public void method1()
		{
			while( 0 < 1 )
			{
				while (DataForThreads.i < 1000)
				{
					lock((object)DataForThreads.i)
					{
						DataForThreads.i++;
						Console.WriteLine("class: QQQ, method: method1, result: DataForThreads.i = " + DataForThreads.i);
					}
					Thread.Sleep(500);
				}
			}
		}

		public void method2()
		{
			while (DataForThreads.thread2.IsAlive)
			{
				while (DataForThreads.i > 0)
				{
					Interlocked.Decrement(ref DataForThreads.i);
					Console.WriteLine("class: QQQ, method: method2, result: DataForThreads.i = " + DataForThreads.i);
					Thread.Sleep(500);
				}
			}
		}
	}

	class DataForThreads
	{
		public static Thread thread1 = null;
		public static Thread thread2 = null;
		public static Thread thread3 = null;

		public static int i = 0;
		public static int j = 0;
		public static bool Is = false;
	}

	class WWW
	{
		public static void listenForAbort()
		{
			bool exit = false;
			while (!exit)
			{
				ConsoleKeyInfo qwe = Console.ReadKey(true);
				if (qwe.Key == ConsoleKey.Escape)
				{
					if (DataForThreads.thread1 != null)
					{
						DataForThreads.thread1.Abort();
					}

					if (DataForThreads.thread2 != null)
					{
						DataForThreads.thread2.Abort();
					}

					//DataForThreads.thread3.Abort();
					exit = true;
					//break;
				}
			}
		}
	}
}
