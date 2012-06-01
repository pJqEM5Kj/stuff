using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokerHelper
{
    static class Code
    {
        public static void Require(bool condition)
        {
            if (!condition)
            {
                throw new InvalidOperationException();
            }
        }

        public static void RequireNotNull(object obj, string name = null)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(name);
            }
        }
    }
}
