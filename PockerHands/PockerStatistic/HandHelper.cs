using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1
{
    class HandHelper
    {
        public static int CompareHands(IHand x, IHand y)
        {
            x.EnsureNotNull();
            y.EnsureNotNull();

            if (x.Value < y.Value)
            {
                return -1;
            }

            if (x.Value > y.Value)
            {
                return 1;
            }

            return 0;
        }

        public static int CompareManyCards(IEnumerable<ICard> x, IEnumerable<ICard> y)
        {
            x.EnsureNotNull();
            y.EnsureNotNull();

            return Enumerable.Zip(x, y, (x_card, y_card) => PockerCardComparer.CompareCardsByValue(x_card, y_card))
                .FirstOrDefault(compare_res => compare_res != 0);
        }

        public static ICard[] GetKickers(IEnumerable<ICard> cards, int kickerCount = 5, params ICard[] exceptCards)
        {
            var kickers = new List<ICard>(kickerCount);

            foreach (ICard card in cards)
            {
                if (kickers.Count == kickerCount)
                {
                    break;
                }

                bool except = false;

                if (exceptCards != null)
                {
                    for (int i = 0; i < exceptCards.Length; i++)
                    {
                        if (object.ReferenceEquals(card, exceptCards[i]))
                        {
                            except = true;
                            break;
                        }
                    }
                }

                if (!except)
                {
                    kickers.Add(card);
                }
            }

            return kickers.ToArray();
        }

        public static int CompareHandsFull<T>(IHand x, IHand y, Comparison<T> comparison)
            where T : IHand
        {
            int res = 0;

            res = HandHelper.CompareHands(x, y);
            if (res != 0)
            {
                return res;
            }

            res = comparison((T)x, (T)y);
            if (res != 0)
            {
                return res;
            }

            if (x.Kickers.IsNullOrEmpty() && y.Kickers.IsNullOrEmpty())
            {
                return res;
            }

            return CompareManyCards(x.Kickers, y.Kickers);
        }

        public static string CardsToString(IEnumerable<ICard> cards)
        {
            if (cards == null)
            {
                cards = new ICard[0];
            }

            return string.Join(" ", cards.Select(x => x.ToString()).ToArray());
        }
    }
}
