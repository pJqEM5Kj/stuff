using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokerHelper
{
    public class ParamHelper
    {
        public static int GetParallelismLevel()
        {
            return (int)Math.Round(Environment.ProcessorCount * 2.5);
        }

        public static int GetSimulatedGameCount()
        {
            return 50000;
        }

        public static int GetEnemyPlayersCount()
        {
            return 1;
        }

        public static int GetCalculationTimeLimit()
        {
            return 1700; //ms
        }

        public static bool GetCalculationTimeLimitEnabled()
        {
            return false;
        }
    }
}
