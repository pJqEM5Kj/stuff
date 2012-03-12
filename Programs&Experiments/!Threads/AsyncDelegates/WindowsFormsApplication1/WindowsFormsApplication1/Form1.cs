using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
	public partial class Form1 : Form
	{
		delegate string MethodDelegate(int i);

		public Form1()
		{
			InitializeComponent();
		}


		private void Form1_Load(object sender, EventArgs e)
		{
			MethodDelegate deleg = new MethodDelegate(method);

			#region first way
			IAsyncResult asyncRes = deleg.BeginInvoke(4, null, null);
			IAsyncResult asyncRes2 = deleg.BeginInvoke(5, null, null);


			listBox1.Items.Add("do work");


			listBox1.Items.Add("when it need get the results");

			listBox1.Items.Add(deleg.EndInvoke(asyncRes2));
			listBox1.Items.Add(deleg.EndInvoke(asyncRes));
			#endregion

			#region second way
			AsyncCallback callback = new AsyncCallback(callbackMethod);
			object objectSuppliedState = new object();

			
			IAsyncResult qwerqwer = deleg.BeginInvoke(6, callback, objectSuppliedState);
			// qwerqwer.AsyncWaitHandle.WaitOne();
			deleg.BeginInvoke(7, new AsyncCallback(callbackMethod2), new object[] { deleg, objectSuppliedState });


			object objectSuppliedState2 = new object();
			AsyncOperation asyncOp = AsyncOperationManager.CreateOperation(objectSuppliedState2);
			System.Threading.SendOrPostCallback deleg2 = new System.Threading.SendOrPostCallback(methodForGettingResults);
			deleg.BeginInvoke(8, new AsyncCallback(callbackMethod3), new object[] { deleg, asyncOp, deleg2, objectSuppliedState });
			#endregion
		}

		private static string method(int i)
		{
			return i.ToString();
		}

		// get results here
		private void callbackMethod(IAsyncResult asyncResult)
		{
			MethodDelegate deleg =
				(MethodDelegate)((System.Runtime.Remoting.Messaging.AsyncResult)asyncResult).AsyncDelegate;

			// not thread safe
			listBox1.Items.Add(deleg.EndInvoke(asyncResult));
		}

		// get results here (a litle bit another way to get delegate for EndInoke)
		private void callbackMethod2(IAsyncResult asyncResult)
		{
			object[] parameters = (object[])asyncResult.AsyncState;

			MethodDelegate deleg = (MethodDelegate)parameters[0];
			object objectSuppliedState = parameters[1];

			// not thread safe
			listBox1.Items.Add(deleg.EndInvoke(asyncResult));
		}

		private void callbackMethod3(IAsyncResult asyncResult)
		{
			object[] parameters = (object[])asyncResult.AsyncState;

			MethodDelegate deleg = (MethodDelegate)parameters[0];
			AsyncOperation asyncOp = (AsyncOperation)parameters[1];
			System.Threading.SendOrPostCallback forPosting =
				(System.Threading.SendOrPostCallback)parameters[2];
			object objectSuppliedState = parameters[3];

			string res = deleg.EndInvoke(asyncResult);

			// thread safe
			asyncOp.PostOperationCompleted(forPosting, res);
		}

		private void methodForGettingResults(object str)
		{
			listBox1.Items.Add((string)str);
		}
	}
}
