using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.General.Helper
{
    public class RNG
    {
        private readonly Random _rng = new Random(Environment.TickCount);
        private readonly int _min;
        private readonly int _max;

        public RNG(int min, int max)
        {
            _min = min;
            _max = max;
        }

        public int Next()
        {
            return _rng.Next(_min, _max);
        }
    }
}
