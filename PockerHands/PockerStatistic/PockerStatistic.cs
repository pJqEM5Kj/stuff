//#define OneThreadForDBG

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ConsoleApplication1
{
    //@!mov
    // pockers usage
    // flop and turn stat

    public class PockerStatistic
    {
        private readonly object UpdateStat_SyncRoot = new object();
        private readonly object Rnd_SyncRoot = new object();

        private Random rnd = new Random(Guid.NewGuid().GetHashCode());


        private ICard[] GetPlayingDeck()
        {
            var cards = new List<ICard>();

            CardSuit[] cardSuits = Enum.GetValues(typeof(CardSuit))
                .Cast<CardSuit>()
                .ToArray();

            CardValue[] cardValues = Enum.GetValues(typeof(CardValue))
                .Cast<CardValue>()
                .ToArray();

            foreach (CardSuit cardSuit in cardSuits)
            {
                foreach (CardValue cardValue in cardValues)
                {
                    var card = new NormalCard(cardSuit, cardValue);
                    cards.Add(card);
                }
            }

            // no jockers yet
            return cards.ToArray();
        }

        private void Shuffle(ICard[] cards)
        {
            var rnds = new int[cards.Length];

            lock (Rnd_SyncRoot)
            {
                for (int i = cards.Length - 1; i > 0; i--)
                {
                    rnds[i] = rnd.Next(0, i + 1);
                }
            }

            for (int i = cards.Length - 1; i > 0; i--)
            {
                int rndIndx = rnds[i];

                if (rndIndx != i)
                {
                    ICard tmp = cards[i];
                    cards[i] = cards[rndIndx];
                    cards[rndIndx] = tmp;
                }
            }
        }

        private static void CheckParameters(ExperimentParameters param)
        {
            param.EnsureNotNull();

            param.PlayerCard1.EnsureNotNull();
            param.PlayerCard2.EnsureNotNull();

            if (param.GameNumber < 1)
            {
                throw new ArgumentException();
            }

            if (param.ParallelLevel < 1)
            {
                throw new ArgumentException();
            }

            switch (param.UseJockers)
            {
                case JockerUsage.None:
                case JockerUsage.One:
                case JockerUsage.Two:
                    break;
                default:
                    throw new ArgumentException();
            }

            if (param.EnemyPlayersCount < 1 || param.EnemyPlayersCount > 22)
            {
                throw new ArgumentException();
            }
        }

        public Statistic RunExperiment(ExperimentParameters param)
        {
            CheckParameters(param);

            if (param.UseJockers != JockerUsage.None)
            {
                //@!mov
                throw new NotImplementedException();
            }

            if (param.FlopAndTurnStatistic)
            {
                //@!mov
                throw new NotImplementedException();
            }

            ICard player_card1 = param.PlayerCard1;
            ICard player_card2 = param.PlayerCard2;
            int gameNumber = param.GameNumber;

            ICard[] cards = GetPlayingDeck();

            ICard[] cards_woPlayerCards = cards.Where(x => !PockerCardComparer.IsEqual(x, player_card1) && !PockerCardComparer.IsEqual(x, player_card2))
                .ToArray();

            var stat = new Statistic();
            stat.GameNumber = gameNumber;

            int parallelLevel = param.ParallelLevel;

#if OneThreadForDBG
            parallelLevel = 1;
#endif

            int enemyPlayersCount = param.EnemyPlayersCount;

            var semaphore = new SemaphoreSlim(parallelLevel);
            var countdown = new CountdownEvent(gameNumber);

            for (int i = 0; i < gameNumber; i++)
            {
                semaphore.Wait();

                Task.Factory.StartNew(
                    () =>
                    {
                        try
                        {
                            SimulateGame(player_card1, player_card2, cards_woPlayerCards, enemyPlayersCount, stat);
                        }
                        finally
                        {
                            semaphore.Release();
                            countdown.Signal();
                        }
                    });
            }

            countdown.Wait();
            countdown.Dispose();
            semaphore.Dispose();

            return stat;
        }

        private void SimulateGame(ICard player_card1, ICard player_card2, ICard[] cards_woPlayerCards, int enemyPlayersCount, Statistic stat)
        {
            var cards_shuffled = new ICard[cards_woPlayerCards.Length];
            Array.Copy(cards_woPlayerCards, cards_shuffled, cards_woPlayerCards.Length);
            Shuffle(cards_shuffled);

            var commonCards = new ICard[5];
            Array.Copy(cards_shuffled, 0, commonCards, 0, commonCards.Length);
            OrderCards(commonCards);

            ICard[] playerCards = AppendPreserveOrder(commonCards, player_card1, player_card2);
            IHand player_hand = GetStrongestHand(playerCards);

            bool draw = false;
            bool lose = false;

            int usedCardsCount = commonCards.Length;
            for (int enemyIndx = 0; enemyIndx < enemyPlayersCount; enemyIndx++)
            {
                ICard enemy_card1 = cards_shuffled[usedCardsCount];
                usedCardsCount++;
                ICard enemy_card2 = cards_shuffled[usedCardsCount];
                usedCardsCount++;

                ICard[] enemyCards = AppendPreserveOrder(commonCards, enemy_card1, enemy_card2);

                IHand enemy_hand = GetStrongestHand(enemyCards, player_hand);

                int res = (enemy_hand != null) ? player_hand.CompareTo(enemy_hand) : 1;

                if (res == 0)
                {
                    draw = true;
                }
                else if (res < 0)
                {
                    lose = true;
                    break;
                }
            }

            lock (UpdateStat_SyncRoot)
            {
                if (lose)
                {
                    stat.Lose++;
                }
                else if (draw)
                {
                    stat.Draw++;
                }
                else
                {
                    stat.Win++;
                }
            }
        }

        private IHand GetStrongestHand(ICard[] orderedCards, IHand hand = null)
        {
            if (hand != null && hand.Value > StraightFlushHand.Val)
            {
                return null;
            }

            StraightFlushHand straightFlushHand;
            if (StraightFlushHand.TryFind(orderedCards, out straightFlushHand))
            {
                return straightFlushHand;
            }

            if (hand != null && hand.Value > FourOfAKindHand.Val)
            {
                return null;
            }

            FourOfAKindHand fourOfAKindHand;
            if (FourOfAKindHand.TryFind(orderedCards, out fourOfAKindHand))
            {
                return fourOfAKindHand;
            }

            if (hand != null && hand.Value > FullHouseHand.Val)
            {
                return null;
            }

            FullHouseHand fullHouseHand;
            if (FullHouseHand.TryFind(orderedCards, out fullHouseHand))
            {
                return fullHouseHand;
            }

            if (hand != null && hand.Value > FlushHand.Val)
            {
                return null;
            }

            FlushHand flushHand;
            if (FlushHand.TryFind(orderedCards, out flushHand))
            {
                return flushHand;
            }

            if (hand != null && hand.Value > StraightHand.Val)
            {
                return null;
            }

            StraightHand straightHand;
            if (StraightHand.TryFind(orderedCards, out straightHand))
            {
                return straightHand;
            }

            if (hand != null && hand.Value > ThreeOfAKindHand.Val)
            {
                return null;
            }

            ThreeOfAKindHand threeOfAKindHand;
            if (ThreeOfAKindHand.TryFind(orderedCards, out threeOfAKindHand))
            {
                return threeOfAKindHand;
            }

            if (hand != null && hand.Value > TwoPairHand.Val)
            {
                return null;
            }

            TwoPairHand twoPairHand;
            if (TwoPairHand.TryFind(orderedCards, out twoPairHand))
            {
                return twoPairHand;
            }

            if (hand != null && hand.Value > PairHand.Val)
            {
                return null;
            }

            PairHand pairHand;
            if (PairHand.TryFind(orderedCards, out pairHand))
            {
                return pairHand;
            }

            if (hand != null && hand.Value > HighCardHand.Val)
            {
                return null;
            }

            HighCardHand highCardHand;
            if (HighCardHand.TryFind(orderedCards, out highCardHand))
            {
                return highCardHand;
            }

            throw new InvalidOperationException();
        }

        private static void OrderCards(ICard[] cards)
        {
            for (int i = 0; i < cards.Length - 1; i++)
            {
                ICard maxCard = cards[i];
                int maxCardIndx = i;

                for (int j = i + 1; j < cards.Length; j++)
                {
                    ICard card = cards[j];
                    if (PockerCardComparer.CompareCardsByValue(card, maxCard) > 0)
                    {
                        maxCard = card;
                        maxCardIndx = j;
                    }
                }

                if (maxCardIndx != i)
                {
                    ICard tmp = cards[i];
                    cards[i] = maxCard;
                    cards[maxCardIndx] = tmp;
                }
            }
        }

        private ICard[] AppendPreserveOrder(ICard[] commonCards, ICard card1, ICard card2)
        {
            var result = new ICard[commonCards.Length + 2];

            Array.Copy(commonCards, result, commonCards.Length);

            result[commonCards.Length] = card1;
            result[commonCards.Length + 1] = card2;

            for (int i = commonCards.Length; i < result.Length; i++)
            {
                int indx = i;
                while (indx > 0 && PockerCardComparer.CompareCardsByValue(result[indx - 1], result[indx]) < 0)
                {
                    ICard tmp = result[indx - 1];
                    result[indx - 1] = result[indx];
                    result[indx] = tmp;
                    indx--;
                }
            }

            return result;
        }
    }
}
