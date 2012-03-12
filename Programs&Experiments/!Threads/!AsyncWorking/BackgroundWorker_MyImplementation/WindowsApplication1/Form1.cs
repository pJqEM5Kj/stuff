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
			
			Loader load = new Loader();
			load.OnComplete += new LoaderCompletedEventHandler(handler);
			load.loadAsync(flist);
		}

		private void handler(object obj)
		{
			button1.Text = (string)obj;
		}
	}

	public delegate void LoaderCompletedEventHandler(object obj);
	public class Loader
	{
		public event LoaderCompletedEventHandler OnComplete;
		
		public void loadAsync(object obj)
		{
			AsyncOperation asyncOperation = AsyncOperationManager.CreateOperation(0);
			innerDelegateForWorking worker = new innerDelegateForWorking(loading);

			worker.BeginInvoke(obj, asyncOperation, null, null);
		}

		private delegate void innerDelegateForWorking(object obj, AsyncOperation asyncOperation);
		private void loading(object obj, AsyncOperation asyncOperation)
		{
			string[] flist = (string[])obj;

			//loading
			string res = "ok";

			asyncOperation.PostOperationCompleted(new SendOrPostCallback(OnComplete), res);
		}
	}
}