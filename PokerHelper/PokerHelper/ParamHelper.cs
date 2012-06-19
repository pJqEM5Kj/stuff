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
            if (Environment.ProcessorCount > 1)
            {
                return (int)Math.Round(Environment.ProcessorCount * 2.5);
            }

            return 1;
        }

        public static int GetSimulatedGameCount()
        {
            if (Environment.ProcessorCount > 1)
            {
                return 50000;
            }

            return 23000;
        }

        public static int GetEnemyPlayersCount()
        {
            return 1;
        }

        public static TimeSpan GetCalculationTimeLimit()
        {
            return TimeSpan.FromSeconds(1.7);
        }

        public static bool GetCalculationTimeLimitEnabled()
        {
            return false;
        }
    }
}
