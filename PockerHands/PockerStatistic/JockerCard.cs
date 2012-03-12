using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1
{
    class JockerCard : ICard
    {
        public JockerType Type { get; private set; }

        public JockerCard(JockerType jockerType)
        {
            Type = jockerType;
        }

        public override string ToString()
        {
            switch (Type)
            {
                case JockerType.Black:
                    return "Jocker (b)";
                case JockerType.Color:
                    return "Jocker (c)";
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
