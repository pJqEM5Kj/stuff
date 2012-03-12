using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1
{
    struct Segment
    {
        public int StartIndx;
        public int Count;

        public Segment(int startIndx, int count)
        {
            StartIndx = startIndx;
            Count = count;
        }
    }
}
