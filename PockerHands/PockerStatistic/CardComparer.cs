using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PockerStatistic
{
    public class CardComparer : IComparer<Card>, IEqualityComparer<Card>
    {
        //@!mov %4speed - removed all Code.Require checks

        public static readonly CardComparer Default = new CardComparer();


        public static int CompareCardsByValue(Card x, Card y)
        {
            //Code.RequireNotNull(x);
            //Code.RequireNotNull(y);

            return ((int)x.Value).CompareTo((int)y.Value);
        }

        public static int CompareCardsByValue_Old(Card x, Card y)
        {
            //Code.RequireNotNull(x);
            //Code.RequireNotNull(y);

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

        public static bool IsEqual(Card x, Card y)
        {
            return (x.Suit == y.Suit && x.Value == y.Value);
        }

        public static bool IsEqualByValue(Card x, Card y)
        {
            //Code.RequireNotNull(x);
            //Code.RequireNotNull(y);

            return ((int)x.Value == (int)y.Value);
        }


        #region IEqualityComparer<Card> Members

        public bool Equals(Card x, Card y)
        {
            return IsEqual(x, y);
        }

        public int GetHashCode(Card obj)
        {
            if (obj == null)
            {
                return 0;
            }

            return obj.Suit.GetHashCode() & obj.Value.GetHashCode();
        }

        #endregion


        #region IComparer<Card> Members

        public int Compare(Card x, Card y)
        {
            return CompareCardsByValue(x, y);
        }

        #endregion
    }
}
