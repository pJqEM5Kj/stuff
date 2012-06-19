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
        internal bool IsDefaultStartSimulationKey(KeyEventArgs e)
        {
            return (e.Key == Key.Enter);
        }

        internal bool IsGlobalCancelKey(KeyEventArgs e)
        {
            return (e.Key == Key.Escape);
        }

        internal bool IsClearCardsInCardsInput(KeyEventArgs e)
        {
            bool shiftDown = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);

            if (shiftDown && (e.Key == Key.Delete || e.Key == Key.Back))
            {
                return true;
            }

            return false;
        }

        internal bool IsSetRandomCardsInCardsInput(KeyEventArgs e)
        {
            bool shiftDown = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);

            if (shiftDown && e.Key == Key.Enter)
            {
                return true;
            }

            return false;
        }

        internal bool IsExternalInput_StartSimulation(char ch)
        {
            return (char.ToLowerInvariant(ch) == 'q');
        }

        internal bool IsExternalInput_StopSimulation(char ch)
        {
            return (char.ToLowerInvariant(ch) == 'w');
        }

        internal bool IsExternalInput_GetCardsFromLogFile(char ch)
        {
            return (char.ToLowerInvariant(ch) == 'a');
        }
    }
}
