using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokerHelper
{
    public class Card : IEquatable<Card>
    {
        const bool shortStr = true;

        public CardSuit Suit { get; private set; }
        public CardValue Value { get; private set; }

        public bool IsValid
        {
            get
            {
                switch (Suit)
                {
                    case CardSuit.Spades:
                    case CardSuit.Hearts:
                    case CardSuit.Diamonds:
                    case CardSuit.Clubs:
                        break;

                    default:
                        return false;
                }

                switch (Value)
                {
                    case CardValue._2:
                    case CardValue._3:
                    case CardValue._4:
                    case CardValue._5:
                    case CardValue._6:
                    case CardValue._7:
                    case CardValue._8:
                    case CardValue._9:
                    case CardValue._10:
                    case CardValue._Jack:
                    case CardValue._Queen:
                    case CardValue._King:
                    case CardValue._Ace:
                        break;

                    default:
                        return false;
                }

                return true;
            }
        }

        public Card(CardSuit suit, CardValue value)
        {
            Suit = suit;
            Value = value;
        }

        public override string ToString()
        {
            string valueS = new string(Value.ToString().Skip(1).ToArray());
            string suitS = Suit.ToString();

            if (shortStr)
            {
                int i;
                if (!int.TryParse(valueS, out i))
                {
                    valueS = valueS[0].ToString();
                }

                suitS = suitS[0].ToString();
            }

            return "{0} ({1})".FormatStr(valueS, suitS);
        }

        #region IEquatable<Card> Members

        public bool Equals(Card other)
        {
            if (other == null)
            {
                return false;
            }

            return CardComparer.IsEqual(this, other);
        }

        #endregion
    }
}
