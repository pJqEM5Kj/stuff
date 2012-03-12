using System;
using System.Collections.Generic;

using System.Windows;
using System.Windows.Controls;
using System.Media;
using System.Windows.Input;


namespace WindowsApplication1
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application application = new Application();
			
			MyWindow myWindow = new MyWindow();
			myWindow.Initialize();
			
			application.Run(myWindow);
		}
	}

	public class MyWindow : Window
	{
		public MyWindow() { }

		public void Initialize()
		{
			this.Title = "wow";
			this.ShowInTaskbar = false;
			
			this.Resources.Add("mybutton", new Label());
			this.AddChild(new Label());

			this.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(MyWindow_MouseLeftButtonDown);
		}

		public void MyWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (Resources.Contains("mybutton"))
			{
				Title = "Yes";
			}
			else
			{
				Title = "No";
			}

			this.Close();
		}
	}
}