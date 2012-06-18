using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace PokerHelper
{
    public class CalculationParameters
    {
        public Card PlayerCard1;
        public Card PlayerCard2;

        public Card Flop1;
        public Card Flop2;
        public Card Flop3;
        public Card Turn;
        public Card River;

        public int GameNumber;
        public TimeSpan? TimeLimit;
        public int ParallelLevel;
        public int EnemyPlayersCount;

        public CancellationToken CancelToken;
        public int SimulatedGamesCount;
    }
}
