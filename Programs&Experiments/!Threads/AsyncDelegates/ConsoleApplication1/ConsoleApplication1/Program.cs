using System;
using System.Collections;
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
using System.Collections.Specialized;
using System.Security.Cryptography;

//using System.Reflection.Emit;

namespace ConsoleApplication1
{
	class Program
	{
		delegate string MethodDelegate(int i);

		private static ManualResetEvent eventt1 = new ManualResetEvent(false);
		private static ManualResetEvent eventt2 = new ManualResetEvent(false);
		
		public static void Main()
		{
			MethodDelegate deleg = new MethodDelegate(method);

			#region first way
			IAsyncResult asyncRes = deleg.BeginInvoke(4, null, null);
			IAsyncResult asyncRes2 = deleg.BeginInvoke(5, null, null);


			Console.WriteLine("do work");


			Console.WriteLine("when it need get the results");

			Console.WriteLine(deleg.EndInvoke(asyncRes2));
			Console.WriteLine(deleg.EndInvoke(asyncRes));
			#endregion

			#region second way
			AsyncCallback callback = new AsyncCallback(callbackMethod);
			object objectSuppliedState = new object();

			deleg.BeginInvoke(6, callback, objectSuppliedState);
			deleg.BeginInvoke(7, new AsyncCallback(callbackMethod2), new object[] { deleg, objectSuppliedState });

			eventt1.WaitOne();
			eventt2.WaitOne();
			#endregion
		}

		public static string method(int i)
		{
			return i.ToString();
		}

		// get results here
		public static void callbackMethod(IAsyncResult asyncResult)
		{
			MethodDelegate deleg =
				(MethodDelegate)((System.Runtime.Remoting.Messaging.AsyncResult)asyncResult).AsyncDelegate;

			Console.WriteLine(deleg.EndInvoke(asyncResult));
			eventt1.Set();
		}

		// get results here (a litle bit another way to get delegate for EndInoke)
		public static void callbackMethod2(IAsyncResult asyncResult)
		{
			object[] parameters = (object[])asyncResult.AsyncState;

			MethodDelegate deleg = (MethodDelegate)parameters[0];
			object objectSuppliedState = parameters[1];

			Console.WriteLine(deleg.EndInvoke(asyncResult));
			eventt2.Set();
		}
	}
}
