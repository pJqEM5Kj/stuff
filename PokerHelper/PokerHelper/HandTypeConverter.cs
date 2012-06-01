using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokerHelper
{
    public class HandTypeConverter
    {
        public static HandType ConvertToHandType(string value)
        {
            HandType ht;
            if (TryConvertToHandType(value, out ht))
            {
                return ht;
            }

            throw new InvalidOperationException();
        }

        public static bool TryConvertToHandType(string value, out HandType handType)
        {
            return Enum.TryParse(value, true, out handType);
        }

        public static string GetName(HandType value)
        {
            Utility.CheckEnumValue(value);
            return value.ToString().Replace("_", " ");
        }

        public static int GetHandValue(HandType value)
        {
            switch (value)
            {
                case HandType.Hight_card:
                    return HighCardHand.Val;

                case HandType.Pair:
                    return PairHand.Val;

                case HandType.Two_pair:
                    return TwoPairHand.Val;

                case HandType.Three_of_a_kind:
                    return ThreeOfAKindHand.Val;

                case HandType.Straight:
                    return StraightHand.Val;

                case HandType.Flush:
                    return FlushHand.Val;

                case HandType.Full_house:
                    return FullHouseHand.Val;

                case HandType.Four_of_a_kind:
                    return FourOfAKindHand.Val;

                case HandType.Straight_flush:
                    return StraightFlushHand.Val;

                default:
                    throw Utility.GetUnknownEnumValueException(value);
            }
        }

        internal static HandType GetHandType(int value)
        {
            HandType ht;
            if (TryGetHandType(value, out ht))
            {
                return ht;
            }

            throw new ArgumentException();
        }

        internal static bool TryGetHandType(int value, out HandType ht)
        {
            if (HighCardHand.Val == value)
            {
                ht = HandType.Hight_card;
                return true;
            }
            else if (PairHand.Val == value)
            {
                ht = HandType.Pair;
                return true;
            }
            else if (TwoPairHand.Val == value)
            {
                ht = HandType.Two_pair;
                return true;
            }
            else if (ThreeOfAKindHand.Val == value)
            {
                ht = HandType.Three_of_a_kind;
                return true;
            }
            else if (StraightHand.Val == value)
            {
                ht = HandType.Straight;
                return true;
            }
            else if (FlushHand.Val == value)
            {
                ht = HandType.Flush;
                return true;
            }
            else if (FullHouseHand.Val == value)
            {
                ht = HandType.Full_house;
                return true;
            }
            else if (FourOfAKindHand.Val == value)
            {
                ht = HandType.Four_of_a_kind;
                return true;
            }
            else if (StraightFlushHand.Val == value)
            {
                ht = HandType.Straight_flush;
                return true;
            }

            ht = (HandType)0;
            return false;
        }
    }
}
