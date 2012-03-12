using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1
{
    class PockerCardComparer : IComparer<ICard>
    {
        public static readonly PockerCardComparer Default = new PockerCardComparer();


        public int Compare(ICard x, ICard y)
        {
            return CompareCardsByValue(x, y);
        }

        public static int CompareCardsByValue(ICard x, ICard y)
        {
            x.EnsureNotNull();
            y.EnsureNotNull();

            if (x is JockerCard)
            {
                if (y is JockerCard)
                {
                    return 0;
                }

                var y_card = (NormalCard)y;

                if (y_card.Value == CardValue._Ace)
                {
                    return 0;
                }

                return 1;
            }
            else if (y is JockerCard)
            {
                var x_card = (NormalCard)x;

                if (x_card.Value == CardValue._Ace)
                {
                    return 0;
                }

                return -1;
            }
            else
            {
                var x_card = (NormalCard)x;
                var y_card = (NormalCard)y;

                int x_value = (int)x_card.Value;
                int y_value = (int)y_card.Value;

                if (x_value < y_value)
                {
                    return -1;
                }

                if (x_value > y_value)
                {
                    return 1;
                }

                return 0;
            }
        }

        public static int CompareCardsByValue(NormalCard x, NormalCard y)
        {
            x.EnsureNotNull();
            y.EnsureNotNull();

            int x_value = (int)x.Value;
            int y_value = (int)y.Value;

            if (x_value < y_value)
            {
                return -1;
            }

            if (x_value > y_value)
            {
                return 1;
            }

            return 0;
        }

        public static bool IsEqual(ICard x, ICard y)
        {
            if (x is JockerCard && y is JockerCard)
            {
                var x_card = (JockerCard)x;
                var y_card = (JockerCard)y;

                return x_card.Type == y_card.Type;
            }

            if (x is NormalCard && y is NormalCard)
            {
                var x_card = (NormalCard)x;
                var y_card = (NormalCard)y;

                return x_card.Suit == y_card.Suit && x_card.Value == y_card.Value;
            }

            return false;
        }

        public static bool IsEqualByValue(ICard x, ICard y)
        {
            return (CompareCardsByValue(x, y) == 0);
        }

        public static bool IsEqualByValue(NormalCard x, NormalCard y)
        {
            x.EnsureNotNull();
            y.EnsureNotNull();

            return (int)x.Value == (int)y.Value;
        }
    }
}
