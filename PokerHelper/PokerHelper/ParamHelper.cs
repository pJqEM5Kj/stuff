using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokerHelper
{
    public class ParamHelper
    {
        public static int GetParallelismLevel()
        {
            return (int)Math.Round(Environment.ProcessorCount * 2.5);
        }
    }
}
