using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PockerStatistic
{
    class StatisticInternal
    {
        public Statistic.HandStatistic[] PlayerHandsStat;
        public Statistic.HandStatistic[] EnemyHandsStat;

        public void Init()
        {
            PlayerHandsStat = new Statistic.HandStatistic[10];
            InitHandTypes(PlayerHandsStat);

            EnemyHandsStat = new Statistic.HandStatistic[10];
            InitHandTypes(EnemyHandsStat);
        }

        private static void InitHandTypes(Statistic.HandStatistic[] handStatistics)
        {
            for (int hand_value = 0; hand_value < handStatistics.Length; hand_value++)
            {
                HandType hand_type;
                if (!HandTypeConverter.TryGetHandType(hand_value, out hand_type))
                {
                    handStatistics[hand_value].HandType = (HandType)int.MinValue;
                    continue;
                }
                else
                {
                    handStatistics[hand_value].HandType = hand_type;
                }
            }
        }
    }
}
