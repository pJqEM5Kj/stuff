using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ConsoleApplication1
{
    class ParsedArgs
    {
        public ICard Card1;
        public ICard Card2;

        public static ParsedArgs ParseArgs(string[] args)
        {
            var cards = new List<ICard>();

            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];
                if (string.IsNullOrEmpty(arg))
                {
                    continue;
                }

                arg = arg.Trim();

                if (string.IsNullOrEmpty(arg))
                {
                    continue;
                }

                arg = arg.ToLower();

                CardSuit cardSuit;
                switch (arg[0])
                {
                    case 'c':
                        cardSuit = CardSuit.Clubs;
                        break;
                    case 'd':
                        cardSuit = CardSuit.Diamonds;
                        break;
                    case 'h':
                        cardSuit = CardSuit.Hearts;
                        break;
                    case 's':
                        cardSuit = CardSuit.Spades;
                        break;

                    default:
                        throw new InvalidOperationException();
                }

                arg = arg.Remove(0, 1);
                CardValue? cardValue = null;

                switch (arg)
                {
                    case "a":
                    case "ace":
                        cardValue = CardValue._Ace;
                        break;
                    case "k":
                    case "king":
                        cardValue = CardValue._King;
                        break;
                    case "q":
                    case "queen":
                        cardValue = CardValue._Queen;
                        break;
                    case "j":
                    case "jack":
                        cardValue = CardValue._Jack;
                        break;
                }

                if (cardValue == null)
                {
                    int ii = int.Parse(arg);
                    switch (ii)
                    {
                        case 2:
                            cardValue = CardValue._2;
                            break;
                        case 3:
                            cardValue = CardValue._3;
                            break;
                        case 4:
                            cardValue = CardValue._4;
                            break;
                        case 5:
                            cardValue = CardValue._5;
                            break;
                        case 6:
                            cardValue = CardValue._6;
                            break;
                        case 7:
                            cardValue = CardValue._7;
                            break;
                        case 8:
                            cardValue = CardValue._8;
                            break;
                        case 9:
                            cardValue = CardValue._9;
                            break;
                        case 10:
                            cardValue = CardValue._10;
                            break;

                        default:
                            throw new InvalidCastException();
                    }
                }

                cards.Add(new NormalCard(cardSuit, cardValue.Value));
            }

            var parsedArgs = new ParsedArgs();
            parsedArgs.Card1 = cards[0];
            parsedArgs.Card2 = cards[1];

            return parsedArgs;
        }
    }
}

