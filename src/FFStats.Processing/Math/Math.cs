using System;
using System.Collections.Generic;
using static System.Math;
using System.Text;

namespace FFStats.Processing
{
    static class Math
    {
        public static bool FuzzyCompareEqual(double val1, double val2)
        {
            return Abs(val1 - val2) < 1e-7;
        }
    }
}
