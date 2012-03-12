using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace WindowsWPFApplicationCarcass
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>

	public partial class Window1 : System.Windows.Window
	{
		Grid _content = new Grid();
		//Canvas _content = new Canvas();
		//DockPanel _content = new DockPanel();
		//StackPanel _content = new StackPanel();
		//VirtualizingStackPanel _content = new VirtualizingStackPanel();
		//WrapPanel _content = new WrapPanel();

		public Window1()
		{
			InitializeComponent();

			Content = _content;

			//-------
			this.Loaded += new RoutedEventHandler(Window1_Loaded);
			this.KeyDown += new KeyEventHandler(Window1_KeyDown);
		}

		void Window1_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Escape)
			{
				Close();
			}
		}

		//first method, start point of the programm
		void Window1_Loaded(object sender, RoutedEventArgs e)
		{
			//
		}

	}

}