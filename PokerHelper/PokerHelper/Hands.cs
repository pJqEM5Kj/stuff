using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokerHelper
{
    class HighCardHand : IHand
    {
        public const double Card5ProbPercent = 50.1;
        public const double Card7ProbPercent = 17.4;

        public const int Val = 1;
        public const HandType HType = HandType.Hight_card;

        public int Value { get { return Val; } }
        public HandType HandType { get { return HType; } }
        public double Card5ProbabilityPercent { get { return Card5ProbPercent; } }
        public double Card7ProbabilityPercent { get { return Card7ProbPercent; } }

        public Card Card { get; set; }
        public Card[] Kickers { get; set; }


        public int CompareTo(IHand other)
        {
            return HandHelper.CompareHandsFull<HighCardHand>(this, other,
                (x, y) => CardComparer.CompareCardsByValue(x.Card, y.Card));
        }

        public static bool TryFind(Card[] orderedCards, out HighCardHand hand)
        {
            hand = new HighCardHand();
            hand.Card = orderedCards[0];
            hand.Kickers = HandHelper.GetKickers(orderedCards, 4, hand.Card);
            return true;
        }

        public override string ToString()
        {
            return "Hight: {0}      Kickers: {1}".FormatStr(Card, HandHelper.CardsToString(Kickers));
        }
    }

    class PairHand : IHand
    {
        public const double Card5ProbPercent = 42.3;
        public const double Card7ProbPercent = 43.8;

        public const int Val = 2;
        public const HandType HType = HandType.Pair;

        public int Value { get { return Val; } }
        public HandType HandType { get { return HType; } }
        public double Card5ProbabilityPercent { get { return Card5ProbPercent; } }
        public double Card7ProbabilityPercent { get { return Card7ProbPercent; } }

        public Card Card1 { get; set; }
        public Card Card2 { get; set; }
        public Card[] Kickers { get; set; }


        public int CompareTo(IHand other)
        {
            return HandHelper.CompareHandsFull<PairHand>(this, other,
                (x, y) => CardComparer.CompareCardsByValue(x.Card1, y.Card1));
        }

        public static bool TryFind(Card[] orderedCards, out PairHand hand)
        {
            for (int i = 1; i < orderedCards.Length; i++)
            {
                Card prevCard = orderedCards[i - 1];
                Card curCard = orderedCards[i];
                if (CardComparer.IsEqualByValue(prevCard, curCard))
                {
                    hand = new PairHand()
                    {
                        Card1 = prevCard,
                        Card2 = curCard,
                    };
                    hand.Kickers = HandHelper.GetKickers(orderedCards, 3, hand.Card1, hand.Card2);
                    return true;
                }
            }

            hand = null;
            return false;
        }

        public static bool TryFind(Card[] orderedCards, bool[] cards_equality_by_value_helper, out PairHand hand)
        {
            for (int i = 1; i < orderedCards.Length; i++)
            {
                Card prevCard = orderedCards[i - 1];
                Card curCard = orderedCards[i];
                if (cards_equality_by_value_helper[i - 1])
                {
                    hand = new PairHand()
                    {
                        Card1 = prevCard,
                        Card2 = curCard,
                    };
                    hand.Kickers = HandHelper.GetKickers(orderedCards, 3, hand.Card1, hand.Card2);
                    return true;
                }
            }

            hand = null;
            return false;
        }

        public override string ToString()
        {
            return "Pair: {0} {1}      Kickers: {2}".FormatStr(Card1, Card2, HandHelper.CardsToString(Kickers));
        }
    }

    class TwoPairHand : IHand
    {
        public const double Card5ProbPercent = 4.75;
        public const double Card7ProbPercent = 23.5;

        public const int Val = 3;
        public const HandType HType = HandType.Two_pair;

        public int Value { get { return Val; } }
        public HandType HandType { get { return HType; } }
        public double Card5ProbabilityPercent { get { return Card5ProbPercent; } }
        public double Card7ProbabilityPercent { get { return Card7ProbPercent; } }

        public Card Card1 { get; set; }
        public Card Card2 { get; set; }

        public Card Card3 { get; set; }
        public Card Card4 { get; set; }

        public Card[] Kickers { get; set; }


        public int CompareTo(IHand other)
        {
            return HandHelper.CompareHandsFull<TwoPairHand>(this, other,
                (x, y) =>
                {
                    return HandHelper.CompareManyCards(
                        new Card[] { x.Card1, x.Card3, },
                        new Card[] { y.Card1, y.Card3, });
                });
        }

        public static bool TryFind(Card[] orderedCards, out TwoPairHand hand)
        {
            Card _card1 = null;
            Card _card2 = null;

            for (int i = 1; i < orderedCards.Length; i++)
            {
                Card prevCard = orderedCards[i - 1];
                Card curCard = orderedCards[i];
                if (CardComparer.IsEqualByValue(prevCard, curCard))
                {
                    if (_card1 == null && _card2 == null)
                    {
                        _card1 = prevCard;
                        _card2 = curCard;
                        i++;
                        continue;
                    }
                    else
                    {
                        hand = new TwoPairHand()
                        {
                            Card1 = _card1,
                            Card2 = _card2,
                            Card3 = prevCard,
                            Card4 = curCard,
                        };
                        hand.Kickers = HandHelper.GetKickers(orderedCards, 1, hand.Card1, hand.Card2, hand.Card3, hand.Card4);
                        return true;
                    }
                }
            }

            hand = null;
            return false;
        }

        public static bool TryFind(Card[] orderedCards, bool[] cards_equality_by_value_helper, out TwoPairHand hand)
        {
            Card _card1 = null;
            Card _card2 = null;

            for (int i = 1; i < orderedCards.Length; i++)
            {
                Card prevCard = orderedCards[i - 1];
                Card curCard = orderedCards[i];
                if (cards_equality_by_value_helper[i - 1])
                {
                    if (_card1 == null && _card2 == null)
                    {
                        _card1 = prevCard;
                        _card2 = curCard;
                        i++;
                        continue;
                    }
                    else
                    {
                        hand = new TwoPairHand()
                        {
                            Card1 = _card1,
                            Card2 = _card2,
                            Card3 = prevCard,
                            Card4 = curCard,
                        };
                        hand.Kickers = HandHelper.GetKickers(orderedCards, 1, hand.Card1, hand.Card2, hand.Card3, hand.Card4);
                        return true;
                    }
                }
            }

            hand = null;
            return false;
        }

        public override string ToString()
        {
            return "Two Pair: {0} {1} - {2} {3}      Kickers: {4}".FormatStr(Card1, Card2, Card3, Card4, HandHelper.CardsToString(Kickers));
        }
    }

    class ThreeOfAKindHand : IHand
    {
        public const double Card5ProbPercent = 2.11;
        public const double Card7ProbPercent = 4.83;

        public const int Val = 4;
        public const HandType HType = HandType.Three_of_a_kind;

        public int Value { get { return Val; } }
        public HandType HandType { get { return HType; } }
        public double Card5ProbabilityPercent { get { return Card5ProbPercent; } }
        public double Card7ProbabilityPercent { get { return Card7ProbPercent; } }

        public Card Card1 { get; set; }
        public Card Card2 { get; set; }
        public Card Card3 { get; set; }

        public Card[] Kickers { get; set; }


        public int CompareTo(IHand other)
        {
            return HandHelper.CompareHandsFull<ThreeOfAKindHand>(this, other,
                (x, y) => CardComparer.CompareCardsByValue(x.Card1, y.Card1));
        }

        public static bool TryFind(Card[] orderedCards, out ThreeOfAKindHand hand)
        {
            int startIndx = 0;
            int count = 1;
            Card sampleCard = orderedCards[0];

            for (int i = 1; i < orderedCards.Length; i++)
            {
                Card card = orderedCards[i];
                if (CardComparer.IsEqualByValue(sampleCard, card))
                {
                    count++;
                    if (count >= 3)
                    {
                        break;
                    }
                }
                else
                {
                    startIndx = i;
                    count = 1;
                    sampleCard = card;
                }
            }

            if (count < 3)
            {
                hand = null;
                return false;
            }

            hand = new ThreeOfAKindHand()
            {
                Card1 = orderedCards[startIndx],
                Card2 = orderedCards[startIndx + 1],
                Card3 = orderedCards[startIndx + 2],
            };
            hand.Kickers = HandHelper.GetKickers(orderedCards, 2, hand.Card1, hand.Card2, hand.Card3);
            return true;
        }

        public static bool TryFind(Card[] orderedCards, bool[] cards_equality_by_value_helper, out ThreeOfAKindHand hand)
        {
            int startIndx = 0;
            int count = 1;

            for (int i = 1; i < orderedCards.Length; i++)
            {
                if (cards_equality_by_value_helper[i - 1])
                {
                    count++;
                    if (count >= 3)
                    {
                        break;
                    }
                }
                else
                {
                    startIndx = i;
                    count = 1;
                }
            }

            if (count < 3)
            {
                hand = null;
                return false;
            }

            hand = new ThreeOfAKindHand()
            {
                Card1 = orderedCards[startIndx],
                Card2 = orderedCards[startIndx + 1],
                Card3 = orderedCards[startIndx + 2],
            };
            hand.Kickers = HandHelper.GetKickers(orderedCards, 2, hand.Card1, hand.Card2, hand.Card3);
            return true;
        }

        public override string ToString()
        {
            return "Three Of A Kind: {0} {1} {2}      Kickers: {3}".FormatStr(Card1, Card2, Card3, HandHelper.CardsToString(Kickers));
        }
    }

    class StraightHand : IHand
    {
        public const double Card5ProbPercent = 0.392;
        public const double Card7ProbPercent = 4.62;

        public const int Val = 5;
        public const HandType HType = HandType.Straight;

        public int Value { get { return Val; } }
        public HandType HandType { get { return HType; } }
        public double Card5ProbabilityPercent { get { return Card5ProbPercent; } }
        public double Card7ProbabilityPercent { get { return Card7ProbPercent; } }

        public Card Card1 { get; set; }
        public Card Card2 { get; set; }
        public Card Card3 { get; set; }
        public Card Card4 { get; set; }
        public Card Card5 { get; set; }

        public Card[] Kickers { get; set; }


        public int CompareTo(IHand other)
        {
            return HandHelper.CompareHandsFull<StraightHand>(this, other,
                (x, y) => CardComparer.CompareCardsByValue(x.Card1, y.Card1));
        }

        public static bool TryFind(Card[] orderedCards, out StraightHand hand)
        {
            var straightCards = new List<Card>(10);

            Card sampleCard = orderedCards[0];
            Card firstAce = (sampleCard.Value == CardValue._Ace) ? sampleCard : null;

            straightCards.Add(sampleCard);

            for (int i = 1; i < orderedCards.Length; i++)
            {
                Card card = orderedCards[i];

                int diff = (int)sampleCard.Value - (int)card.Value;

                if (diff == 1)
                {
                    straightCards.Add(card);
                    if (straightCards.Count >= 5)
                    {
                        break;
                    }
                    sampleCard = card;

                    // special case when ace value treat as 1
                    if (sampleCard.Value == CardValue._2 && straightCards.Count == 4 && firstAce != null)
                    {
                        straightCards.Add(firstAce);
                        break;
                    }
                }
                else if (diff == 0)
                {
                    // nop
                }
                else
                {
                    straightCards.Clear();
                    straightCards.Add(card);
                    sampleCard = card;
                }
            }

            if (straightCards.Count >= 5)
            {
                hand = new StraightHand()
                {
                    Card1 = straightCards[0],
                    Card2 = straightCards[1],
                    Card3 = straightCards[2],
                    Card4 = straightCards[3],
                    Card5 = straightCards[4],
                };
                return true;
            }

            hand = null;
            return false;
        }

        public override string ToString()
        {
            return "Straight: {0} {1} {2} {3} {4}".FormatStr(Card1, Card2, Card3, Card4, Card5);
        }
    }

    class FlushHand : IHand
    {
        public const double Card5ProbPercent = 0.197;
        public const double Card7ProbPercent = 3.03;

        public const int Val = 6;
        public const HandType HType = HandType.Flush;

        public int Value { get { return Val; } }
        public HandType HandType { get { return HType; } }
        public double Card5ProbabilityPercent { get { return Card5ProbPercent; } }
        public double Card7ProbabilityPercent { get { return Card7ProbPercent; } }

        public Card Card1 { get; set; }
        public Card Card2 { get; set; }
        public Card Card3 { get; set; }
        public Card Card4 { get; set; }
        public Card Card5 { get; set; }

        public Card[] Kickers { get; set; }


        public int CompareTo(IHand other)
        {
            return HandHelper.CompareHandsFull<FlushHand>(this, other,
                (x, y) =>
                {
                    return HandHelper.CompareManyCards(
                        new Card[] { x.Card1, x.Card2, x.Card3, x.Card4, x.Card5, },
                        new Card[] { y.Card1, y.Card2, y.Card3, y.Card4, y.Card5, });
                });
        }

        public static bool TryFind(Card[] orderedCards, out FlushHand hand)
        {
            var matrix = new int[4, 8];

            for (int i = 0; i < orderedCards.Length; i++)
            {
                Card card = orderedCards[i];

                switch (card.Suit)
                {
                    case CardSuit.Spades:
                        int spadesCount = matrix[0, 0];
                        spadesCount++;
                        matrix[0, spadesCount] = i;
                        matrix[0, 0] = spadesCount;
                        break;

                    case CardSuit.Hearts:
                        int heartsCount = matrix[1, 0];
                        heartsCount++;
                        matrix[1, heartsCount] = i;
                        matrix[1, 0] = heartsCount;
                        break;

                    case CardSuit.Diamonds:
                        int diamondsCount = matrix[2, 0];
                        matrix[2, diamondsCount] = i;
                        diamondsCount++;
                        matrix[2, 0] = diamondsCount;
                        break;

                    case CardSuit.Clubs:
                        int clubsCount = matrix[3, 0];
                        clubsCount++;
                        matrix[3, clubsCount] = i;
                        matrix[3, 0] = clubsCount;
                        break;

                    default:
                        throw Utility.GetUnknownEnumValueException(card.Suit);
                }
            }

            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                if (matrix[i, 0] >= 5)
                {
                    hand = new FlushHand()
                    {
                        Card1 = orderedCards[matrix[i, 1]],
                        Card2 = orderedCards[matrix[i, 2]],
                        Card3 = orderedCards[matrix[i, 3]],
                        Card4 = orderedCards[matrix[i, 4]],
                        Card5 = orderedCards[matrix[i, 5]],
                    };
                    return true;
                }
            }

            hand = null;
            return false;
        }

        public override string ToString()
        {
            return "Flush: {0} {1} {2} {3} {4}".FormatStr(Card1, Card2, Card3, Card4, Card5);
        }
    }

    class FullHouseHand : IHand
    {
        public const double Card5ProbPercent = 0.144;
        public const double Card7ProbPercent = 2.6;

        public const int Val = 7;
        public const HandType HType = HandType.Full_house;

        public int Value { get { return Val; } }
        public HandType HandType { get { return HType; } }
        public double Card5ProbabilityPercent { get { return Card5ProbPercent; } }
        public double Card7ProbabilityPercent { get { return Card7ProbPercent; } }

        public Card Card1 { get; set; }
        public Card Card2 { get; set; }
        public Card Card3 { get; set; }
        public Card Card4 { get; set; }
        public Card Card5 { get; set; }

        public Card[] Kickers { get; set; }


        public int CompareTo(IHand other)
        {
            return HandHelper.CompareHandsFull<FullHouseHand>(this, other,
                (x, y) =>
                {
                    return HandHelper.CompareManyCards(
                        new Card[] { x.Card1, x.Card4, },
                        new Card[] { y.Card1, y.Card4, });
                });
        }

        public static bool TryFind(Card[] orderedCards, out FullHouseHand hand)
        {
            var cardGroups = new List<Segment>(10);

            int startIndx = 0;
            int count = 1;

            Card sampleCard = orderedCards[0];

            for (int i = 1; i < orderedCards.Length; i++)
            {
                Card card = orderedCards[i];

                if (CardComparer.IsEqualByValue(sampleCard, card))
                {
                    count++;
                    continue;
                }
                else
                {
                    if (count > 1)
                    {
                        cardGroups.Add(new Segment(startIndx, count));
                    }

                    startIndx = i;
                    count = 1;
                    sampleCard = card;
                }
            }

            if (count > 1)
            {
                cardGroups.Add(new Segment(startIndx, count));
            }

            Segment? triple = null;
            Segment? pair = null;
            foreach (Segment cardGroup in cardGroups)
            {
                if (triple != null && pair != null)
                {
                    break;
                }

                if (triple == null && cardGroup.Count > 2)
                {
                    triple = cardGroup;
                    continue;
                }

                if (pair == null && cardGroup.Count > 1)
                {
                    pair = cardGroup;
                    continue;
                }
            }

            if (triple != null && pair != null)
            {
                Segment _triple = triple.Value;
                Segment _pair = pair.Value;

                hand = new FullHouseHand()
                {
                    Card1 = orderedCards[_triple.StartIndx],
                    Card2 = orderedCards[_triple.StartIndx + 1],
                    Card3 = orderedCards[_triple.StartIndx + 2],
                    Card4 = orderedCards[_pair.StartIndx],
                    Card5 = orderedCards[_pair.StartIndx + 1],
                };
                return true;
            }

            hand = null;
            return false;
        }

        public static bool TryFind(Card[] orderedCards, bool[] cards_equality_by_value_helper, out FullHouseHand hand)
        {
            var cardGroups = new List<Segment>(10);

            int startIndx = 0;
            int count = 1;

            for (int i = 1; i < orderedCards.Length; i++)
            {
                if (cards_equality_by_value_helper[i - 1])
                {
                    count++;
                    continue;
                }
                else
                {
                    if (count > 1)
                    {
                        cardGroups.Add(new Segment(startIndx, count));
                    }

                    startIndx = i;
                    count = 1;
                }
            }

            if (count > 1)
            {
                cardGroups.Add(new Segment(startIndx, count));
            }

            Segment? triple = null;
            Segment? pair = null;
            foreach (Segment cardGroup in cardGroups)
            {
                if (triple != null && pair != null)
                {
                    break;
                }

                if (triple == null && cardGroup.Count > 2)
                {
                    triple = cardGroup;
                    continue;
                }

                if (pair == null && cardGroup.Count > 1)
                {
                    pair = cardGroup;
                    continue;
                }
            }

            if (triple != null && pair != null)
            {
                Segment _triple = triple.Value;
                Segment _pair = pair.Value;

                hand = new FullHouseHand()
                {
                    Card1 = orderedCards[_triple.StartIndx],
                    Card2 = orderedCards[_triple.StartIndx + 1],
                    Card3 = orderedCards[_triple.StartIndx + 2],
                    Card4 = orderedCards[_pair.StartIndx],
                    Card5 = orderedCards[_pair.StartIndx + 1],
                };
                return true;
            }

            hand = null;
            return false;
        }

        public override string ToString()
        {
            return "FullHouse: {0} {1} {2} - {3} {4}".FormatStr(Card1, Card2, Card3, Card4, Card5);
        }
    }

    class FourOfAKindHand : IHand
    {
        public const double Card5ProbPercent = 0.024;
        public const double Card7ProbPercent = 0.168;

        public const int Val = 8;
        public const HandType HType = HandType.Four_of_a_kind;

        public int Value { get { return Val; } }
        public HandType HandType { get { return HType; } }
        public double Card5ProbabilityPercent { get { return Card5ProbPercent; } }
        public double Card7ProbabilityPercent { get { return Card7ProbPercent; } }

        public Card Card1 { get; set; }
        public Card Card2 { get; set; }
        public Card Card3 { get; set; }
        public Card Card4 { get; set; }

        public Card[] Kickers { get; set; }


        public int CompareTo(IHand other)
        {
            return HandHelper.CompareHandsFull<FourOfAKindHand>(this, other,
                (x, y) => CardComparer.CompareCardsByValue(x.Card1, y.Card1));
        }

        public static bool TryFind(Card[] orderedCards, out FourOfAKindHand hand)
        {
            int startIndx = 0;
            int count = 1;
            Card sampleCard = orderedCards[0];

            for (int i = 1; i < orderedCards.Length; i++)
            {
                Card card = orderedCards[i];
                if (CardComparer.IsEqualByValue(sampleCard, card))
                {
                    count++;
                    if (count >= 4)
                    {
                        break;
                    }
                }
                else
                {
                    startIndx = i;
                    count = 1;
                    sampleCard = card;
                }
            }

            if (count < 4)
            {
                hand = null;
                return false;
            }

            hand = new FourOfAKindHand()
            {
                Card1 = orderedCards[startIndx],
                Card2 = orderedCards[startIndx + 1],
                Card3 = orderedCards[startIndx + 2],
                Card4 = orderedCards[startIndx + 3],
            };
            hand.Kickers = HandHelper.GetKickers(orderedCards, 1, hand.Card1, hand.Card2, hand.Card3, hand.Card4);
            return true;
        }

        public static bool TryFind(Card[] orderedCards, bool[] cards_equality_by_value_helper, out FourOfAKindHand hand)
        {
            int startIndx = 0;
            int count = 1;

            for (int i = 1; i < orderedCards.Length; i++)
            {
                if (cards_equality_by_value_helper[i - 1])
                {
                    count++;
                    if (count >= 4)
                    {
                        break;
                    }
                }
                else
                {
                    startIndx = i;
                    count = 1;
                }
            }

            if (count < 4)
            {
                hand = null;
                return false;
            }

            hand = new FourOfAKindHand()
            {
                Card1 = orderedCards[startIndx],
                Card2 = orderedCards[startIndx + 1],
                Card3 = orderedCards[startIndx + 2],
                Card4 = orderedCards[startIndx + 3],
            };
            hand.Kickers = HandHelper.GetKickers(orderedCards, 1, hand.Card1, hand.Card2, hand.Card3, hand.Card4);
            return true;
        }

        public override string ToString()
        {
            return "Four Of A Kind: {0} {1} {2} {3}      Kickers: {4}".FormatStr(Card1, Card2, Card3, Card4, HandHelper.CardsToString(Kickers));
        }
    }

    class StraightFlushHand : IHand
    {
        public const double Card5ProbPercent = 0.00139; // 0.000154 - royal
        public const double Card7ProbPercent = 0.0279; // 0.0032 - royal

        public const int Val = 9;
        public const HandType HType = HandType.Straight_flush;

        public int Value { get { return Val; } }
        public HandType HandType { get { return HType; } }
        public double Card5ProbabilityPercent { get { return Card5ProbPercent; } }
        public double Card7ProbabilityPercent { get { return Card7ProbPercent; } }

        public Card Card1 { get; set; }
        public Card Card2 { get; set; }
        public Card Card3 { get; set; }
        public Card Card4 { get; set; }
        public Card Card5 { get; set; }

        public bool Royal { get; private set; }

        public Card[] Kickers { get; set; }


        public int CompareTo(IHand other)
        {
            return HandHelper.CompareHandsFull<StraightFlushHand>(this, other,
                (x, y) => CardComparer.CompareCardsByValue(x.Card1, y.Card1));
        }

        public static bool TryFind(Card[] orderedCards, out StraightFlushHand hand)
        {
            var matrix = new int[4, 8];

            for (int i = 0; i < orderedCards.Length; i++)
            {
                Card card = orderedCards[i];

                switch (card.Suit)
                {
                    case CardSuit.Spades:
                        int spadesCount = matrix[0, 0];
                        spadesCount++;
                        matrix[0, spadesCount] = i;
                        matrix[0, 0] = spadesCount;
                        break;

                    case CardSuit.Hearts:
                        int heartsCount = matrix[1, 0];
                        heartsCount++;
                        matrix[1, heartsCount] = i;
                        matrix[1, 0] = heartsCount;
                        break;

                    case CardSuit.Diamonds:
                        int diamondsCount = matrix[2, 0];
                        matrix[2, diamondsCount] = i;
                        diamondsCount++;
                        matrix[2, 0] = diamondsCount;
                        break;

                    case CardSuit.Clubs:
                        int clubsCount = matrix[3, 0];
                        clubsCount++;
                        matrix[3, clubsCount] = i;
                        matrix[3, 0] = clubsCount;
                        break;

                    default:
                        throw Utility.GetUnknownEnumValueException(card.Suit);
                }
            }

            Card[] flushCards = null;

            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                int count = matrix[i, 0];

                if (count >= 5)
                {
                    flushCards = new Card[count];

                    for (int j = 1; j <= count; j++)
                    {
                        flushCards[j - 1] = orderedCards[matrix[i, j]];
                    }

                    break;
                }
            }

            if (flushCards == null)
            {
                hand = null;
                return false;
            }

            var straightFlushCards = new List<Card>(10);

            Card sampleCard = flushCards[0];
            Card firstAce = (sampleCard.Value == CardValue._Ace) ? sampleCard : null;
            straightFlushCards.Add(sampleCard);

            for (int i = 1; i < flushCards.Length; i++)
            {
                Card card = flushCards[i];

                int diff = (int)sampleCard.Value - (int)card.Value;

                if (diff == 1)
                {
                    straightFlushCards.Add(card);
                    if (straightFlushCards.Count >= 5)
                    {
                        break;
                    }
                    sampleCard = card;

                    // special case when ace value treat as 1
                    if (card.Value == CardValue._2 && straightFlushCards.Count == 4 && firstAce != null)
                    {
                        straightFlushCards.Add(firstAce);
                        break;
                    }
                }
                else
                {
                    sampleCard = card;

                    straightFlushCards.Clear();
                    straightFlushCards.Add(sampleCard);
                }
            }

            if (straightFlushCards.Count >= 5)
            {
                hand = new StraightFlushHand()
                {
                    Card1 = straightFlushCards[0],
                    Card2 = straightFlushCards[1],
                    Card3 = straightFlushCards[2],
                    Card4 = straightFlushCards[3],
                    Card5 = straightFlushCards[4],
                };
                hand.Royal = CardComparer.IsEqualByValue(hand.Card1, new Card(CardSuit.Clubs, CardValue._Ace));
                return true;
            }

            hand = null;
            return false;
        }

        public override string ToString()
        {
            if (Royal)
            {
                return "Royal Flush: {0} {1} {2} {3} {4}".FormatStr(Card1, Card2, Card3, Card4, Card5);
            }
            else
            {
                return "Straight Flush: {0} {1} {2} {3} {4}".FormatStr(Card1, Card2, Card3, Card4, Card5);
            }
        }
    }
}
