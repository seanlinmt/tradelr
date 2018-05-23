using System;

namespace tradelr.Crypto
{
    public sealed class RandomNumberGenerator : Random
    {
        private static readonly Random _global = new Random();
        [ThreadStatic]
        private static Random _localInstance;

        RandomNumberGenerator()
        {

        }

        public static Random Instance
        {
            get
            {
                Random inst = _localInstance;
                if (inst == null)
                {
                    int seed;
                    lock (_global) seed = _global.Next();
                    _localInstance = inst = new Random(seed);
                }
                return _localInstance;
            }
        }
    }

}
