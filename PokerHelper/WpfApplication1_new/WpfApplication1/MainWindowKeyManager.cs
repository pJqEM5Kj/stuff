using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace WpfApplication1
{
    class MainWindowKeyManager
    {
        public bool IsDefaultStartSimulationKey(KeyEventArgs e)
        {
            return (e.Key == Key.Enter);
        }
    }
}
