using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1
{
    public class NormalCard : ICard
    {
        const bool shortStr = true;

        public CardSuit Suit { get; private set; }
        public CardValue Value { get; private set; }

        public NormalCard(CardSuit suit, CardValue value)
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
    }
}
