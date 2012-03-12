using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WindowsApplication1
{
	public partial class Form1 : Form
	{
		BackgroundWorker worker = new BackgroundWorker();
		Random rnd = new Random();
		
		public Form1()
		{
			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			//
		}

		private void Form1_Shown(object sender, EventArgs e)
		{
			//

		}

		private void button2_Click(object sender, EventArgs e)
		{
			Text = rnd.NextDouble().ToString();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
			worker.DoWork += new DoWorkEventHandler(worker_DoWork);
			worker.RunWorkerAsync();
		}

		void worker_DoWork(object sender, DoWorkEventArgs e)
		{
			BackgroundWorker bw = sender as BackgroundWorker;

			operation();
		}

		private void operation()
		{
			int j = Int16.MaxValue;
			for (int i = 0; i < Int16.MaxValue; i++)
			{
				j--;
				Text = j.ToString();
			}
			j = 5;
		}

		void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			button1.Enabled = false;
		}
	}
}