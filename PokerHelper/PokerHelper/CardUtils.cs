using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokerHelper
{
    public static class CardUtils
    {
        public static string ConvertToString(Card card)
        {
            Code.RequireNotNull(card);
            Code.Require(card.IsValid);
            return ConvertToString(card.Value) + ConvertToString(card.Suit);
        }

        public static string ConvertToString(CardValue cardValue)
        {
            switch (cardValue)
            {
                case CardValue._2:
                    return "2";
                case CardValue._3:
                    return "3";
                case CardValue._4:
                    return "4";
                case CardValue._5:
                    return "5";
                case CardValue._6:
                    return "6";
                case CardValue._7:
                    return "7";
                case CardValue._8:
                    return "8";
                case CardValue._9:
                    return "9";
                case CardValue._10:
                    return "t";
                case CardValue._Jack:
                    return "j";
                case CardValue._Queen:
                    return "q";
                case CardValue._King:
                    return "k";
                case CardValue._Ace:
                    return "a";
            }

            throw Utility.GetUnknownEnumValueException(cardValue);
        }

        public static string ConvertToString(CardSuit cardSuit)
        {
            switch (cardSuit)
            {
                case CardSuit.Spades:
                    return "s";
                case CardSuit.Hearts:
                    return "h";
                case CardSuit.Diamonds:
                    return "d";
                case CardSuit.Clubs:
                    return "c";
            }

            throw Utility.GetUnknownEnumValueException(cardSuit);
        }

        public static Card ParseCard(string s, int indx = 0)
        {
            char value = char.ToLowerInvariant(s[indx]);
            char suit = char.ToLowerInvariant(s[indx + 1]);

            CardSuit cardSuit = ParseSuit(suit);
            CardValue cardValue = ParseValue(value);

            return new Card(cardSuit, cardValue);
        }

        public static CardValue ParseValue(char c)
        {
            if (!char.IsLower(c))
            {
                c = char.ToLowerInvariant(c);
            }

            switch (c)
            {
                case 'a':
                    return CardValue._Ace;
                case 'k':
                    return CardValue._King;
                case 'q':
                    return CardValue._Queen;
                case 'j':
                    return CardValue._Jack;
                case 't':
                    return CardValue._10;
                case '9':
                    return CardValue._9;
                case '8':
                    return CardValue._8;
                case '7':
                    return CardValue._7;
                case '6':
                    return CardValue._6;
                case '5':
                    return CardValue._5;
                case '4':
                    return CardValue._4;
                case '3':
                    return CardValue._3;
                case '2':
                    return CardValue._2;
            }

            throw new InvalidOperationException();
        }

        public static CardSuit ParseSuit(char c)
        {
            if (!char.IsLower(c))
            {
                c = char.ToLowerInvariant(c);
            }

            switch (c)
            {
                case 'c':
                    return CardSuit.Clubs;
                case 'd':
                    return CardSuit.Diamonds;
                case 'h':
                    return CardSuit.Hearts;
                case 's':
                    return CardSuit.Spades;
            }

            throw new InvalidOperationException();
        }
    }
}
