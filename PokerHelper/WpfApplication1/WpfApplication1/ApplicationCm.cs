using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpfApplication1
{
    class ApplicationCm
    {
        private MainWindowPr MainWindow;


        internal void Start()
        {
            MainWindow = new MainWindowPr();
            MainWindow.Application = this;
            MainWindow.Start();
        }
    }
}
