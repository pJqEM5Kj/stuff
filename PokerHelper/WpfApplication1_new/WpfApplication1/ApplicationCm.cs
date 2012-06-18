using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PokerHelper;

namespace WpfApplication1
{
    class ApplicationCm
    {
        private MainWindowPr MainWindow;


        public void Start()
        {
            MainWindow = new MainWindowPr();
            MainWindow.Application = this;
            MainWindow.Start();
        }
    }
}
