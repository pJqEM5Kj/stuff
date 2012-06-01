using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PokerHelper;

namespace ConsoleApplication1
{
    class StatisticBuilder1
    {
        public int GameCount = 10000;
        public int ParallellLevel = 2;
        public int MaxEnemyCount = 5;

        public string FileNamePattern = @"D:\2cards_vs_enemy_{0}.txt";
        public string FileNamePattern2 = @"D:\2cards_vs_enemy_{0}_rel.txt";

        public void BuildStatistic()
        {
            CardValue[] card_values = Enum.GetValues(typeof(CardValue)).OfType<CardValue>().ToArray();

            Func<Card, Card, int, int, int, double> func =
                (Card card1, Card card2, int gameNumber, int parallelLevel, int enemyCount) =>
                {
                    var parameters = new ExperimentParameters();

                    parameters.PlayerCard1 = card1;
                    parameters.PlayerCard2 = card2;

                    parameters.GameNumber = gameNumber;
                    parameters.ParallelLevel = parallelLevel;
                    parameters.EnemyPlayersCount = enemyCount;

                    var psc = new PokerStatisticCalc();
                    Statistic stat = psc.RunExperiment(parameters);
                    return Math.Round(stat.Win * 100d / stat.GameNumber, 2);
                };

            for (int enemy = 1; enemy <= MaxEnemyCount; enemy++)
            {
                double min = int.MaxValue;
                double max = int.MinValue;

                var table = new List<KeyValuePair<CardValue, List<Tuple<CardValue, double, double>>>>();

                for (int i = 0; i < card_values.Length; i++)
                {
                    var line = new List<Tuple<CardValue, double, double>>();

                    for (int j = 0; j < card_values.Length; j++)
                    {
                        double d1 = func(
                            new Card(CardSuit.Clubs, card_values[i]),
                            new Card(CardSuit.Hearts, card_values[j]),
                            GameCount,
                            ParallellLevel,
                            enemy);

                        min = Math.Min(min, d1);
                        max = Math.Max(max, d1);

                        double d2 = -1;
                        if (i != j)
                        {
                            d2 = func(
                                new Card(CardSuit.Clubs, card_values[i]),
                                new Card(CardSuit.Clubs, card_values[j]),
                                GameCount,
                                ParallellLevel,
                                enemy);

                            min = Math.Min(min, d2);
                            max = Math.Max(max, d2);
                        }

                        line.Add(Tuple.Create(card_values[j], d1, d2));
                    }

                    table.Add(new KeyValuePair<CardValue, List<Tuple<CardValue, double, double>>>(card_values[i], line));
                }

                var make_rel = new Func<double, double, double, double>(
                    (double mmin, double mmax, double val) =>
                    {
                        double range = mmax - mmin;
                        double d1 = (val - mmin) / range; // [0;1]
                        return d1;
                        //double rel = mmax / mmin;
                        //return 1 + d1 * rel;
                    });

                var lines = new List<List<string>>();
                var lines2 = new List<List<string>>();
                foreach (var item in table)
                {
                    var nodes = new List<string>();
                    var nodes2 = new List<string>();
                    foreach (var item2 in item.Value)
                    {
                        nodes.Add("{0}{1} - {2} ({3})".FormatStr(
                            CardUtils.ConvertToString(item.Key),
                            CardUtils.ConvertToString(item2.Item1),
                            item2.Item2,
                            (item2.Item3 == -1 ? "-" : item2.Item3.ToString())));

                        nodes2.Add("{0}{1} - {2:0.####} ({3})".FormatStr(
                            CardUtils.ConvertToString(item.Key),
                            CardUtils.ConvertToString(item2.Item1),
                            make_rel(min, max, item2.Item2),
                            (item2.Item3 == -1 ? "-" : make_rel(min, max, item2.Item3).ToString("0.####"))));
                    }

                    lines.Add(nodes);
                    lines2.Add(nodes2);
                }

                int maxLength = lines.Select(x => x.Max(y => y.Length)).Max();
                int maxLength2 = lines2.Select(x => x.Max(y => y.Length)).Max();

                for (int i = 0; i < lines.Count; i++)
                {
                    for (int j = 0; j < lines[i].Count; j++)
                    {
                        lines[i][j] = lines[i][j].PadRight(maxLength);
                        lines2[i][j] = lines2[i][j].PadRight(maxLength2);
                    }
                }

                string s = string.Join(Environment.NewLine, lines.Select(l => string.Join(" | ", l)));
                File.WriteAllText(FileNamePattern.FormatStr(enemy), s);

                string s2 = string.Join(Environment.NewLine, lines2.Select(l => string.Join(" | ", l)));
                File.WriteAllText(FileNamePattern2.FormatStr(enemy), s2);
            }
        }
    }
}
