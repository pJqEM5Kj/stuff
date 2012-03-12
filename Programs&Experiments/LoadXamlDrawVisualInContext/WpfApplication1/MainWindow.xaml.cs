using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Loaded += new RoutedEventHandler(MainWindow_Loaded);
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            AdornerLayer.GetAdornerLayer(b1).Add(new MyAdorner(b1));

            var cc = (ResourceDictionary)System.Windows.Markup.XamlReader.Load(new FileStream("D:\\1.xaml", FileMode.Open));
            var obj = (Style)cc["qqq"];

            b1.Style = obj;
        }
    }

    public class MyAdorner : Adorner
    {
        public MyAdorner(UIElement p)
            : base(p)
        {
            //
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            //base.OnRender(drawingContext);

            //drawingContext.DrawRectangle(Brushes.Red, null, new Rect(10, 10, 100, 100));

            var vb = new VisualBrush(new UserControl1());
            drawingContext.DrawRectangle(vb, null, new Rect(15, 15, 100, 100));
        }
    }
}
