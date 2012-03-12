using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1
{
    interface IHand: IComparable<IHand>
    {
        double Value { get; }
        ICard[] Kickers { get; }
    }
}
