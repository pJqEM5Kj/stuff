using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.ComponentModel;
using System.Threading;

using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

//design teplates: safe [Equals (==)] overload
namespace ConsoleApplication1
{
	class Program
	{
		public static void Main()
		{
			//
			A a = new A();
			a.i = 1;

			A a2 = new A();
			a2.i = 1;

			object obj = a;
			object obj2 = new object();

			Console.WriteLine(obj.Equals(a2));

		}
	}

	class A
	{
		public int i = 0;

		public override bool Equals(object obj)
		{
			if (obj is A)
			{
				return Equals((A)obj);
			}

			return false;
		}

		public bool Equals(A x)
		{
			return this == x;
		}

		public static bool operator == (A x, A y)
		{
			return AComparison.Equals(x, y);
		}

		public static bool operator !=(A x, A y)
		{
			return !(x == y);
		}
	}

	static class AComparison
	{
		public static bool Equals(A x, A y)
		{
			// both are the same object or both are null
			if (object.ReferenceEquals(x, y))
			{
				return true;
			}

			// one is null but another is not
			if (object.ReferenceEquals(x, null) || object.ReferenceEquals(y, null))
			{
				return false;
			}

			// comparison no one of operands are null
			return x.i == y.i;
		}
	}
}
