﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PockerStatistic
{
    public class ParamHelper
    {
        public static int GetParallelLevel()
        {
            return (int)Math.Round(Environment.ProcessorCount * 2.5);
        }
    }
}
