using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokerHelper
{
    class HandHelper
    {
        //@!mov %4speed - removed all Code.Require checks

        public static int CompareHands(IHand x, IHand y)
        {
            //Code.RequireNotNull(x);
            //Code.RequireNotNull(y);

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

        public static PlayerHandResult ComparePlayerHand(IHand player_hand, IHand enemy_hand)
        {
            //Code.RequireNotNull(player_hand);
            //Code.RequireNotNull(enemy_hand);

            int res = player_hand.CompareTo(enemy_hand);

            if (res < 0)
            {
                return PlayerHandResult.PlayerLose;
            }

            if (res == 0)
            {
                return PlayerHandResult.Draw;
            }

            return PlayerHandResult.PlayerWin;
        }

        public static int CompareManyCards(Card[] x, Card[] y)
        {
            //Code.RequireNotNull(x);
            //Code.RequireNotNull(y);

            int indx = 0;
            int length = Math.Min(x.Length, y.Length);

            while (true)
            {
                //@!mov %4speed
                //int compareRes = CardComparer.CompareCardsByValue(x[indx], y[indx]);
                int compareRes = ((int)x[indx].Value).CompareTo((int)y[indx].Value);
                if (compareRes != 0)
                {
                    return compareRes;
                }

                indx++;
                if (indx >= length)
                {
                    break;
                }
            }

            return 0;
        }

        public static int CompareManyCards_Old(IEnumerable<Card> x, IEnumerable<Card> y)
        {
            //Code.RequireNotNull(x);
            //Code.RequireNotNull(y);

            return Enumerable.Zip(x, y, (x_card, y_card) => CardComparer.CompareCardsByValue(x_card, y_card))
                .FirstOrDefault(compare_res => compare_res != 0);
        }

        public static Card[] GetKickers(Card[] cards, int kickerCount = 5, params Card[] exceptCards)
        {
            var kickers = new List<Card>(kickerCount);

            foreach (Card card in cards)
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
            int res = x.Value.CompareTo(y.Value);

            if (res != 0)
            {
                return res;
            }

            res = comparison((T)x, (T)y);
            if (res != 0)
            {
                return res;
            }

            if ((x.Kickers == null || x.Kickers.Length == 0) 
                && (y.Kickers == null || y.Kickers.Length == 0))
            {
                return res;
            }

            return CompareManyCards(x.Kickers, y.Kickers);
        }

        public static int CompareHandsFull_Old<T>(IHand x, IHand y, Comparison<T> comparison)
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

        public static string CardsToString(IEnumerable<Card> cards)
        {
            if (cards == null)
            {
                cards = new Card[0];
            }

            return string.Join(" ", cards.Select(x => x.ToString()).ToArray());
        }
    }
}
