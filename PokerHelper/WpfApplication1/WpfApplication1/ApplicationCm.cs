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


        internal void Start()
        {
            MainWindow = new MainWindowPr();
            MainWindow.Application = this;
            MainWindow.Start();
        }

        internal Statistic CalculatePokerStatistic(CalculationParameters calculationParameters)
        {
            var psc = new PokerStatisticCalc();
            Statistic statistic = psc.RunExperiment(calculationParameters);
            return statistic;
        }
    }
}
