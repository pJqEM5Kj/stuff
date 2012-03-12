using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            ParsedArgs parsedArgs = null;
            try
            {
                parsedArgs = ParsedArgs.ParseArgs(args);
            }
            catch //(Exception ex)
            {
                //throw;
            }

            new Program().Run(parsedArgs);
        }

        public void Run(ParsedArgs parsedArgs)
        {
            const int GameNumber = (int)(1.35E+5);

            ICard card1 = null;
            ICard card2 = null;

            if (parsedArgs != null && parsedArgs.Card1 != null && parsedArgs.Card2 != null)
            {
                card1 = parsedArgs.Card1;
                card2 = parsedArgs.Card2;
            }
            else
            {
                GetHardCodedCards(out card1, out card2);
            }

            //
            var param = new ExperimentParameters();
            param.PlayerCard1 = card1;
            param.PlayerCard2 = card2;
            param.GameNumber = GameNumber;
            param.EnemyPlayersCount = 1;

            var ps = new PockerStatistic();

            var sw = Stopwatch.StartNew();
            Statistic stat = ps.RunExperiment(param);
            sw.Stop();

            ShowResults(sw, stat);
        }

        private void GetHardCodedCards(out ICard card1, out ICard card2)
        {
            //1
            card1 = new NormalCard(CardSuit.Clubs, CardValue._Ace);
            card2 = new NormalCard(CardSuit.Clubs, CardValue._10);

            //2
            //card1 = new NormalCard(CardSuit.Diamonds, CardValue._Ace);
            //card2 = new NormalCard(CardSuit.Clubs, CardValue._Ace);

            //3
            //card1 = new NormalCard(CardSuit.Clubs, CardValue._2);
            //card2 = new NormalCard(CardSuit.Diamonds, CardValue._4);
        }

        private void ShowResults(Stopwatch sw, Statistic stat)
        {
            Console.WriteLine("Win percent = {0:0.####}".FormatStr(100 * stat.Win / (double)stat.GameNumber));
            Console.WriteLine("Draw percent = {0:0.####}".FormatStr(100 * stat.Draw / (double)stat.GameNumber));
            Console.WriteLine("Lose percent = {0:0.####}".FormatStr(100 * stat.Lose / (double)stat.GameNumber));
            Console.WriteLine("Game number = {0}".FormatStr(stat.GameNumber));
            Console.WriteLine("Time = {0}".FormatStr(sw.Elapsed));
        }
    }
}
