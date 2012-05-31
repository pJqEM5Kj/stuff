//#define OneThreadForDBG

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace PockerStatistic
{
    public class PockerStatisticCalc
    {
        private readonly object UpdateStat_SyncRoot = new object();
        private readonly object Rnd_SyncRoot = new object();

        private Random rnd = new Random(Guid.NewGuid().GetHashCode());
        private int SimulatedGamesCount;


        public Statistic RunExperiment(ExperimentParameters param)
        {
            CheckParameters(param);

            var sw = Stopwatch.StartNew();

            SimulatedGamesCount = 0;

            Card player_card1 = param.PlayerCard1;
            Card player_card2 = param.PlayerCard2;

            Card[] open_cards = GetOpenCards(param);
            ValidateInputCards(player_card1, player_card2, open_cards);

            Card[] cards = GetAllCards();

            Card[] free_cards = GetFreeCards(cards, player_card1, player_card2, open_cards);

            var stat = new StatisticInternal();
            stat.Init();

            int parallelLevel = param.ParallelLevel;

#if OneThreadForDBG
            parallelLevel = 1;
#endif

            int enemyPlayersCount = param.EnemyPlayersCount;
            int gameNumber = param.GameNumber;

            var semaphore = new SemaphoreSlim(parallelLevel);
            var countdown = new CountdownEvent(gameNumber);

            for (int i = 0; i < gameNumber; i++)
            {
                param.CancelToken.ThrowIfCancellationRequested();

                if (param.TimeLimit.HasValue
                    && sw.Elapsed > param.TimeLimit.Value)
                {
                    countdown.Signal(gameNumber - i);
                    break;
                }

                semaphore.Wait();
                SimulatedGamesCount++;

                Task.Factory.StartNew(
                    () =>
                    {
                        try
                        {
                            SimulateGame(player_card1, player_card2, open_cards, free_cards, enemyPlayersCount, stat, param);
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

            Statistic public_stat = PreparePublicStatistic(stat);

            return public_stat;
        }


        private Card[] GetAllCards()
        {
            var cards = new List<Card>();

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
                    var card = new Card(cardSuit, cardValue);
                    cards.Add(card);
                }
            }

            return cards.ToArray();
        }

        private void Shuffle(Card[] cards)
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
                    Card tmp = cards[i];
                    cards[i] = cards[rndIndx];
                    cards[rndIndx] = tmp;
                }
            }
        }

        private static void CheckParameters(ExperimentParameters param)
        {
            Code.RequireNotNull(param);
            Code.RequireNotNull(param.PlayerCard1);
            Code.RequireNotNull(param.PlayerCard2);

            Code.Require(param.GameNumber > 0);
            Code.Require(param.ParallelLevel > 0);
            Code.Require(param.EnemyPlayersCount > 0 && param.EnemyPlayersCount <= 22);
            Code.Require(param.TimeLimit == null || param.TimeLimit.Value > TimeSpan.Zero);
        }

        private void SimulateGame(Card player_card1, Card player_card2, Card[] open_cards, Card[] free_cards, int enemyPlayersCount, StatisticInternal stat, ExperimentParameters param)
        {
            var common_cards = new Card[5];
            Array.Copy(open_cards, common_cards, open_cards.Length);

            var cards_shuffled = new Card[free_cards.Length];
            Array.Copy(free_cards, cards_shuffled, free_cards.Length);
            Shuffle(cards_shuffled);

            if (open_cards.Length < 5)
            {
                int destinationIndex = open_cards.Length;
                int number_of_elements_to_copy = common_cards.Length - open_cards.Length;

                Array.Copy(cards_shuffled, 0, common_cards, destinationIndex, number_of_elements_to_copy);
            }

            OrderCards(common_cards);

            Card[] player_cards = AppendPreserveOrder(common_cards, player_card1, player_card2);
            bool[] player_cards_equality_by_value_helper = GetEqualityCardsByValueHelper(player_cards);
            IHand player_hand = GetStrongestHand(player_cards, player_cards_equality_by_value_helper, null);

            IHand enemy_stat_hand = null;
            PlayerHandResult player_stat_hand_result = PlayerHandResult.PlayerWin;
            PlayerHandResult player_hand_final_result = PlayerHandResult.PlayerWin;

            int used_cards_count = common_cards.Length;
            for (int enemyIndx = 0; enemyIndx < enemyPlayersCount; enemyIndx++)
            {
                Card enemy_card1 = cards_shuffled[used_cards_count];
                used_cards_count++;
                Card enemy_card2 = cards_shuffled[used_cards_count];
                used_cards_count++;

                Card[] enemy_cards = AppendPreserveOrder(common_cards, enemy_card1, enemy_card2);
                bool[] enemy_cards_equality_by_value_helper = GetEqualityCardsByValueHelper(enemy_cards);
                IHand enemy_hand = GetStrongestHand(enemy_cards, enemy_cards_equality_by_value_helper, (enemyIndx > 0 ? player_hand : null));

                if (enemyIndx == 0)
                {
                    enemy_stat_hand = enemy_hand;
                }

                PlayerHandResult player_hand_result = (enemy_hand != null) ?
                    HandHelper.ComparePlayerHand(player_hand, enemy_hand) : PlayerHandResult.PlayerWin;

                switch (player_hand_result)
                {
                    case PlayerHandResult.PlayerWin:
                        // nop
                        break;

                    case PlayerHandResult.Draw:
                        player_hand_final_result = PlayerHandResult.Draw;
                        if (enemyIndx == 0)
                        {
                            player_stat_hand_result = PlayerHandResult.Draw;
                        }
                        break;

                    case PlayerHandResult.PlayerLose:
                        player_hand_final_result = PlayerHandResult.PlayerLose;
                        if (enemyIndx == 0)
                        {
                            player_stat_hand_result = PlayerHandResult.PlayerLose;
                        }
                        break;

                    default:
                        throw Utility.GetUnknownEnumValueException(player_hand_result);
                }

                if (player_hand_final_result == PlayerHandResult.PlayerLose)
                {
                    break;
                }
            }

            UpdateStatistic(stat, player_hand, player_hand_final_result, enemy_stat_hand, player_stat_hand_result);
            Interlocked.Increment(ref param.SimulatedGamesCount);
        }

        private IHand GetStrongestHand(Card[] orderedCards, bool[] cards_equality_by_value_helper, IHand hand = null)
        {
            int? hand_value = (hand != null) ? hand.Value : (int?)null;

            if (hand_value.HasValue && hand_value.Value > StraightFlushHand.Val)
            {
                return null;
            }

            StraightFlushHand straightFlushHand;
            if (StraightFlushHand.TryFind(orderedCards, out straightFlushHand))
            {
                return straightFlushHand;
            }

            if (hand_value.HasValue && hand_value.Value > FourOfAKindHand.Val)
            {
                return null;
            }

            FourOfAKindHand fourOfAKindHand;
            if (FourOfAKindHand.TryFind(orderedCards, cards_equality_by_value_helper, out fourOfAKindHand))
            {
                return fourOfAKindHand;
            }

            if (hand_value.HasValue && hand_value.Value > FullHouseHand.Val)
            {
                return null;
            }

            FullHouseHand fullHouseHand;
            if (FullHouseHand.TryFind(orderedCards, cards_equality_by_value_helper, out fullHouseHand))
            {
                return fullHouseHand;
            }

            if (hand_value.HasValue && hand_value.Value > FlushHand.Val)
            {
                return null;
            }

            FlushHand flushHand;
            if (FlushHand.TryFind(orderedCards, out flushHand))
            {
                return flushHand;
            }

            if (hand_value.HasValue && hand_value.Value > StraightHand.Val)
            {
                return null;
            }

            StraightHand straightHand;
            if (StraightHand.TryFind(orderedCards, out straightHand))
            {
                return straightHand;
            }

            if (hand_value.HasValue && hand_value.Value > ThreeOfAKindHand.Val)
            {
                return null;
            }

            ThreeOfAKindHand threeOfAKindHand;
            if (ThreeOfAKindHand.TryFind(orderedCards, cards_equality_by_value_helper, out threeOfAKindHand))
            {
                return threeOfAKindHand;
            }

            if (hand_value.HasValue && hand_value.Value > TwoPairHand.Val)
            {
                return null;
            }

            TwoPairHand twoPairHand;
            if (TwoPairHand.TryFind(orderedCards, cards_equality_by_value_helper, out twoPairHand))
            {
                return twoPairHand;
            }

            if (hand_value.HasValue && hand_value.Value > PairHand.Val)
            {
                return null;
            }

            PairHand pairHand;
            if (PairHand.TryFind(orderedCards, cards_equality_by_value_helper, out pairHand))
            {
                return pairHand;
            }

            if (hand_value.HasValue && hand_value.Value > HighCardHand.Val)
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

        private IHand GetStrongestHand_Old(Card[] orderedCards, IHand hand = null)
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

        private static void OrderCards(Card[] cards)
        {
            Array.Sort(cards.Select(x => (-1)*(int)x.Value).ToArray(), cards);
        }

        private static void OrderCards_Old(Card[] cards)
        {
            for (int i = 0; i < cards.Length - 1; i++)
            {
                Card maxCard = cards[i];
                int maxCardIndx = i;

                for (int j = i + 1; j < cards.Length; j++)
                {
                    Card card = cards[j];
                    if (CardComparer.CompareCardsByValue(card, maxCard) > 0)
                    {
                        maxCard = card;
                        maxCardIndx = j;
                    }
                }

                if (maxCardIndx != i)
                {
                    Card tmp = cards[i];
                    cards[i] = maxCard;
                    cards[maxCardIndx] = tmp;
                }
            }
        }

        private Card[] AppendPreserveOrder(Card[] commonCards, Card card1, Card card2)
        {
            var result = new Card[commonCards.Length + 2];

            Array.Copy(commonCards, result, commonCards.Length);

            result[commonCards.Length] = card1;
            result[commonCards.Length + 1] = card2;

            for (int i = commonCards.Length; i < result.Length; i++)
            {
                int indx = i;
                while (indx > 0 && CardComparer.CompareCardsByValue(result[indx - 1], result[indx]) < 0)
                {
                    Card tmp = result[indx - 1];
                    result[indx - 1] = result[indx];
                    result[indx] = tmp;
                    indx--;
                }
            }

            return result;
        }

        private Card[] GetOpenCards(ExperimentParameters ex_params)
        {
            var boardCards = new Card[]
            {
                ex_params.Flop1,
                ex_params.Flop2,
                ex_params.Flop3,
                ex_params.Turn,
                ex_params.River,
            };

            return boardCards.Where(x => x != null).ToArray();
        }

        private Card[] GetFreeCards(Card[] cards, Card player_card1, Card player_card2, Card[] open_cards)
        {
            var free_cards = new List<Card>(cards.Length);

            foreach (Card card in cards)
            {
                if (CardComparer.IsEqual(card, player_card1)
                    || CardComparer.IsEqual(card, player_card2))
                {
                    continue;
                }

                if (open_cards.Any((Card open_card) => CardComparer.IsEqual(card, open_card)))
                {
                    continue;
                }

                free_cards.Add(card);
            }

            return free_cards.ToArray();
        }

        private Statistic PreparePublicStatistic(StatisticInternal stat)
        {
            var public_stat = new Statistic();
            public_stat.GameNumber = SimulatedGamesCount;

            var player_hands_stat = new List<Statistic.HandStatistic>();

            for (int hand_value = 0; hand_value < stat.PlayerHandsStat.Length; hand_value++)
            {
                HandType player_hand_type;
                if (!HandTypeConverter.TryGetHandType(hand_value, out player_hand_type))
                {
                    continue;
                }

                player_hands_stat.Add(stat.PlayerHandsStat[hand_value]);
            }
            public_stat.PlayerHandsStat = player_hands_stat.ToArray();

            public_stat.Win = public_stat.PlayerHandsStat.Sum(x => x.Win);
            public_stat.Draw = public_stat.PlayerHandsStat.Sum(x => x.Draw);
            public_stat.Lose = public_stat.PlayerHandsStat.Sum(x => x.Lose);

            var enemy_hands_stat = new List<Statistic.HandStatistic>();
            for (int hand_value = 0; hand_value < stat.EnemyHandsStat.Length; hand_value++)
            {
                HandType enemy_hand_type;
                if (!HandTypeConverter.TryGetHandType(hand_value, out enemy_hand_type))
                {
                    continue;
                }

                enemy_hands_stat.Add(stat.EnemyHandsStat[hand_value]);
            }
            public_stat.EnemyHandsStat = enemy_hands_stat.ToArray();

            return public_stat;
        }

        private void ValidateInputCards(Card player_card1, Card player_card2, Card[] open_cards)
        {
            var cards = new Card[open_cards.Length + 2];
            cards[0] = player_card1;
            cards[1] = player_card2;
            Array.Copy(open_cards, 0, cards, 2, open_cards.Length);

            var hs = new HashSet<Card>(CardComparer.Default);
            foreach (Card card in cards)
            {
                if (card == null)
                {
                    continue;
                }

                if (!card.IsValid)
                {
                    throw new InvalidOperationException();
                }

                if (!hs.Add(card))
                {
                    throw new InvalidOperationException();
                }
            }
        }

        private void UpdateStatistic(StatisticInternal stat, IHand player_hand, PlayerHandResult player_hand_final_result,
            IHand enemy_stat_hand, PlayerHandResult player_stat_hand_result)
        {
            lock (UpdateStat_SyncRoot)
            {
                Statistic.HandStatistic hs;

                hs = stat.PlayerHandsStat[player_hand.Value];
                switch (player_hand_final_result)
                {
                    case PlayerHandResult.PlayerWin:
                        hs.Win++;
                        break;
                    case PlayerHandResult.Draw:
                        hs.Draw++;
                        break;
                    case PlayerHandResult.PlayerLose:
                        hs.Lose++;
                        break;
                    default:
                        throw Utility.GetUnknownEnumValueException(player_hand_final_result);
                }
                stat.PlayerHandsStat[player_hand.Value] = hs;

                hs = stat.EnemyHandsStat[enemy_stat_hand.Value];
                switch (player_stat_hand_result)
                {
                    case PlayerHandResult.PlayerWin:
                        hs.Lose++;
                        break;
                    case PlayerHandResult.Draw:
                        hs.Draw++;
                        break;
                    case PlayerHandResult.PlayerLose:
                        hs.Win++;
                        break;
                    default:
                        throw Utility.GetUnknownEnumValueException(player_stat_hand_result);
                }
                stat.EnemyHandsStat[enemy_stat_hand.Value] = hs;
            }
        }

        private bool[] GetEqualityCardsByValueHelper(Card[] cards)
        {
            var eq = new List<bool>();
            for (int i = 1; i < cards.Length; i++)
            {
                //@!mov %4speed
                //eq.Add(CardComparer.IsEqualByValue(cards[i - 1], cards[i]));
                eq.Add((int)cards[i-1].Value == (int)cards[i].Value);
            }
            return eq.ToArray();
        }
    }
}
