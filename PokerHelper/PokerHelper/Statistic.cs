using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokerHelper
{
    public class Statistic
    {
        public int Win;
        public int Draw;
        public int Lose;
        public int GameNumber;
        public HandStatistic[] PlayerHandsStat;
        public HandStatistic[] EnemyHandsStat;

        public struct HandStatistic
        {
            public HandType HandType;
            public int Win;
            public int Lose;
            public int Draw;
        }
    }
}
