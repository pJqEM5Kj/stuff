using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokerHelper
{
    interface IHand: IComparable<IHand>
    {
        HandType HandType { get; }
        int Value { get; }
        Card[] Kickers { get; }
        double Card5ProbabilityPercent { get; }
        double Card7ProbabilityPercent { get; }
    }
}
