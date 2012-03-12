using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1
{
    public class ExperimentParameters
    {
        public ICard PlayerCard1;
        public ICard PlayerCard2;
        public int GameNumber;
        public int ParallelLevel = 30;
        public JockerUsage UseJockers = JockerUsage.None;
        public int EnemyPlayersCount = 1;
        public bool FlopAndTurnStatistic = false;
    }
}
