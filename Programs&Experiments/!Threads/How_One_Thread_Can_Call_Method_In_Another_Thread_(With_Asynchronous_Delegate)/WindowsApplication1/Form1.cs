using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace WindowsApplication1
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private void Form1_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				e.Effect = DragDropEffects.Copy;
			}
			else
			{
				e.Effect = DragDropEffects.None;
			}
		}

		private void Form1_DragDrop(object sender, DragEventArgs e)
		{
			string[] flist = (string[])e.Data.GetData(DataFormats.FileDrop);
			
			DelegateForAnotherThreadMethod delegateForAnotherThreadMethod =
				new DelegateForAnotherThreadMethod(anotherThreadMethod);

			delegateForAnotherThreadMethod.BeginInvoke(
				AsyncOperationManager.CreateOperation(0), null, null);

			// continue working
		}

		private void mainThreadMethod(object obj)
		{
			button1.Text = (string)obj;
		}

		private delegate void DelegateForAnotherThreadMethod(object obj);
		private void anotherThreadMethod(object obj)
		{
			// it work in another thread
			AsyncOperation asyncOperation = (AsyncOperation)obj;

			asyncOperation.Post(new SendOrPostCallback(mainThreadMethod), "i'm another thread and i call u");

			// continue working
		}
	}
}