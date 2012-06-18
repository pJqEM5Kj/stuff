using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for RenderWaiting.xaml
    /// </summary>
    public partial class RenderWaiting : UserControl
    {
        private Storyboard storyboard;


        public RenderWaiting()
        {
            InitializeComponent();

            Loaded += RenderWaiting_Loaded;
            IsVisibleChanged += RenderWaiting_IsVisibleChanged;
        }

        private void RenderWaiting_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (storyboard == null)
            {
                return;
            }

            if (IsVisible)
            {
                storyboard.SetValue(Storyboard.TargetNameProperty, "animationTarget");
                storyboard.Begin(animationTarget);
            }
            else if (!IsVisible)
            {
                storyboard.ClearValue(Storyboard.TargetNameProperty);
            }
        }

        private void RenderWaiting_Loaded(object sender, RoutedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(this))
            {
                return;
            }

            storyboard = Resources["storyboard"] as Storyboard;
            if (storyboard != null)
            {
                storyboard.SetValue(Storyboard.TargetNameProperty, "animationTarget");
                storyboard.Begin(animationTarget);
            }
        }
    }
}
