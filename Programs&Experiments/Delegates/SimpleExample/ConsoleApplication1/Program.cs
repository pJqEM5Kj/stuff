using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApplication1
{
	
	
	class Program
	{
		public delegate void delegateMethod();

		static void myMethod1()
		{
			Console.WriteLine("Hello world 1");
		}

		static void myMethod2()
		{
			Console.WriteLine("Hello world 2");
		}

		static void myMethod3(delegateMethod x)
		{
			Console.WriteLine("Now we call delegate from method 3:");
			x();
		}
		
		static void Main(string[] args)
		{
			delegateMethod q = myMethod1;

			q();
			
			q = myMethod2;

			myMethod3(q);
		}


	}
}
