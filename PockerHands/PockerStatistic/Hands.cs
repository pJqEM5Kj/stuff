using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1
{
    class HighCardHand : IHand
    {
        public const double Val = 1;
        public double Value { get { return Val; } }

        public ICard Card { get; set; }
        public ICard[] Kickers { get; set; }


        public int CompareTo(IHand other)
        {
            return HandHelper.CompareHandsFull<HighCardHand>(this, other,
                (x, y) => PockerCardComparer.CompareCardsByValue(x.Card, y.Card));
        }

        public static bool TryFind(ICard[] orderedCards, out HighCardHand hand)
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
        public const double Val = 2;
        public double Value { get { return Val; } }

        public ICard Card1 { get; set; }
        public ICard Card2 { get; set; }
        public ICard[] Kickers { get; set; }


        public int CompareTo(IHand other)
        {
            return HandHelper.CompareHandsFull<PairHand>(this, other,
                (x, y) => PockerCardComparer.CompareCardsByValue(x.Card1, y.Card1));
        }

        public static bool TryFind(ICard[] orderedCards, out PairHand hand)
        {
            for (int i = 1; i < orderedCards.Length; i++)
            {
                ICard prevCard = orderedCards[i - 1];
                ICard curCard = orderedCards[i];
                if (PockerCardComparer.IsEqualByValue(prevCard, curCard))
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
        public const double Val = 3;
        public double Value { get { return Val; } }

        public ICard Card1 { get; set; }
        public ICard Card2 { get; set; }

        public ICard Card3 { get; set; }
        public ICard Card4 { get; set; }

        public ICard[] Kickers { get; set; }


        public int CompareTo(IHand other)
        {
            return HandHelper.CompareHandsFull<TwoPairHand>(this, other,
                (x, y) =>
                {
                    return HandHelper.CompareManyCards(
                        new ICard[] { x.Card1, x.Card3, },
                        new ICard[] { y.Card1, y.Card3, });
                });
        }

        public static bool TryFind(ICard[] orderedCards, out TwoPairHand hand)
        {
            ICard _card1 = null;
            ICard _card2 = null;

            for (int i = 1; i < orderedCards.Length; i++)
            {
                ICard prevCard = orderedCards[i - 1];
                ICard curCard = orderedCards[i];
                if (PockerCardComparer.IsEqualByValue(prevCard, curCard))
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
        public const double Val = 4;
        public double Value { get { return Val; } }

        public ICard Card1 { get; set; }
        public ICard Card2 { get; set; }
        public ICard Card3 { get; set; }

        public ICard[] Kickers { get; set; }


        public int CompareTo(IHand other)
        {
            return HandHelper.CompareHandsFull<ThreeOfAKindHand>(this, other,
                (x, y) => PockerCardComparer.CompareCardsByValue(x.Card1, y.Card1));
        }

        public static bool TryFind(ICard[] orderedCards, out ThreeOfAKindHand hand)
        {
            int startIndx = 0;
            int count = 1;
            var sampleCard = (NormalCard)orderedCards[0];

            for (int i = 1; i < orderedCards.Length; i++)
            {
                var card = (NormalCard)orderedCards[i];
                if (PockerCardComparer.IsEqualByValue(sampleCard, card))
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

        public override string ToString()
        {
            return "Three Of A Kind: {0} {1} {2}      Kickers: {3}".FormatStr(Card1, Card2, Card3, HandHelper.CardsToString(Kickers));
        }
    }

    class StraightHand : IHand
    {
        public const double Val = 5;
        public double Value { get { return Val; } }

        public ICard Card1 { get; set; }
        public ICard Card2 { get; set; }
        public ICard Card3 { get; set; }
        public ICard Card4 { get; set; }
        public ICard Card5 { get; set; }

        public ICard[] Kickers { get; set; }


        public int CompareTo(IHand other)
        {
            return HandHelper.CompareHandsFull<StraightHand>(this, other,
                (x, y) => PockerCardComparer.CompareCardsByValue(x.Card1, y.Card1));
        }

        public static bool TryFind(ICard[] orderedCards, out StraightHand hand)
        {
            var straightCards = new List<NormalCard>(10);

            NormalCard sampleCard = (NormalCard)orderedCards[0];
            NormalCard firstAce = (sampleCard.Value == CardValue._Ace) ? sampleCard : null;

            straightCards.Add(sampleCard);

            for (int i = 1; i < orderedCards.Length; i++)
            {
                NormalCard card = (NormalCard)orderedCards[i];

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
        public const double Val = 6;
        public double Value { get { return Val; } }

        public ICard Card1 { get; set; }
        public ICard Card2 { get; set; }
        public ICard Card3 { get; set; }
        public ICard Card4 { get; set; }
        public ICard Card5 { get; set; }

        public ICard[] Kickers { get; set; }


        public int CompareTo(IHand other)
        {
            return HandHelper.CompareHandsFull<FlushHand>(this, other,
                (x, y) =>
                {
                    return HandHelper.CompareManyCards(
                        new ICard[] { x.Card1, x.Card2, x.Card3, x.Card4, x.Card5, },
                        new ICard[] { y.Card1, y.Card2, y.Card3, y.Card4, y.Card5, });
                });
        }

        public static bool TryFind(ICard[] orderedCards, out FlushHand hand)
        {
            var matrix = new int[4, 8];

            for (int i = 0; i < orderedCards.Length; i++)
            {
                var card = (NormalCard)orderedCards[i];

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
        public const double Val = 7;
        public double Value { get { return Val; } }

        public ICard Card1 { get; set; }
        public ICard Card2 { get; set; }
        public ICard Card3 { get; set; }
        public ICard Card4 { get; set; }
        public ICard Card5 { get; set; }

        public ICard[] Kickers { get; set; }


        public int CompareTo(IHand other)
        {
            return HandHelper.CompareHandsFull<FullHouseHand>(this, other,
                (x, y) =>
                {
                    return HandHelper.CompareManyCards(
                        new ICard[] { x.Card1, x.Card4, },
                        new ICard[] { y.Card1, y.Card4, });
                });
        }

        public static bool TryFind(ICard[] orderedCards, out FullHouseHand hand)
        {
            var cardGroups = new List<Segment>(10);

            int startIndx = 0;
            int count = 1;

            var sampleCard = (NormalCard)orderedCards[0];

            for (int i = 1; i < orderedCards.Length; i++)
            {
                var card = (NormalCard)orderedCards[i];

                if (PockerCardComparer.IsEqualByValue(sampleCard, card))
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
        public const double Val = 8;
        public double Value { get { return Val; } }

        public ICard Card1 { get; set; }
        public ICard Card2 { get; set; }
        public ICard Card3 { get; set; }
        public ICard Card4 { get; set; }

        public ICard[] Kickers { get; set; }


        public int CompareTo(IHand other)
        {
            return HandHelper.CompareHandsFull<FourOfAKindHand>(this, other,
                (x, y) => PockerCardComparer.CompareCardsByValue(x.Card1, y.Card1));
        }

        public static bool TryFind(ICard[] orderedCards, out FourOfAKindHand hand)
        {
            int startIndx = 0;
            int count = 1;
            var sampleCard = (NormalCard)orderedCards[0];

            for (int i = 1; i < orderedCards.Length; i++)
            {
                var card = (NormalCard)orderedCards[i];
                if (PockerCardComparer.IsEqualByValue(sampleCard, card))
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

        public override string ToString()
        {
            return "Four Of A Kind: {0} {1} {2} {3}      Kickers: {4}".FormatStr(Card1, Card2, Card3, Card4, HandHelper.CardsToString(Kickers));
        }
    }

    class StraightFlushHand : IHand
    {
        public const double Val = 9;
        public double Value { get { return Val; } }

        public ICard Card1 { get; set; }
        public ICard Card2 { get; set; }
        public ICard Card3 { get; set; }
        public ICard Card4 { get; set; }
        public ICard Card5 { get; set; }

        public bool Royal { get; private set; }

        public ICard[] Kickers { get; set; }


        public int CompareTo(IHand other)
        {
            return HandHelper.CompareHandsFull<StraightFlushHand>(this, other,
                (x, y) => PockerCardComparer.CompareCardsByValue(x.Card1, y.Card1));
        }

        public static bool TryFind(ICard[] orderedCards, out StraightFlushHand hand)
        {
            var matrix = new int[4, 8];

            for (int i = 0; i < orderedCards.Length; i++)
            {
                var card = (NormalCard)orderedCards[i];

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
                }
            }

            NormalCard[] flushCards = null;

            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                int count = matrix[i, 0];

                if (count >= 5)
                {
                    flushCards = new NormalCard[count];

                    for(int j = 1; j <= count; j++)
                    {
                        flushCards[j - 1] = (NormalCard)orderedCards[matrix[i, j]];
                    }

                    break;
                }
            }

            if (flushCards == null)
            {
                hand = null;
                return false;
            }

            var straightFlushCards = new List<NormalCard>(10);

            NormalCard sampleCard = flushCards[0];
            NormalCard firstAce = (sampleCard.Value == CardValue._Ace) ? sampleCard : null;
            straightFlushCards.Add(sampleCard);

            for (int i = 1; i < flushCards.Length; i++)
            {
                NormalCard card = flushCards[i];

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
                hand.Royal = PockerCardComparer.IsEqualByValue(hand.Card1, new NormalCard(CardSuit.Clubs, CardValue._Ace));
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
