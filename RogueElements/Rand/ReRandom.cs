using System;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Globalization;
using System.Diagnostics.Contracts;


namespace RogueElements
{
    /// <summary>
    /// A custom random class that holds on to its seed, for repeatability.
    /// </summary>
    [Serializable]
    public class ReRandom : IRandom
    {
        private ulong[] s;

        /// <summary>
        /// The seed value that the class was initialized with.
        /// </summary>
        public ulong FirstSeed { get; private set; }


        public ReRandom()
            : this(unchecked((ulong)System.DateTime.Now.Ticks))
        {
        }

        public ReRandom(ulong Seed)
        {
            FirstSeed = Seed;

            s = new ulong[2];
            s[0] = splitMix64(FirstSeed);
            s[1] = splitMix64(s[1]);
        }

        private static ulong splitMix64(ulong x)
        {
            ulong z = (x += 0x9E3779B97F4A7C15);
            z = (z ^ (z >> 30)) * 0xBF58476D1CE4E5B9;
            z = (z ^ (z >> 27)) * 0x94D049BB133111EB;
            return z ^ (z >> 31);
        }

        //xoroshiro128+
        /* This is the successor to xorshift128+. It is the fastest full-period
           generator passing BigCrush without systematic failures, but due to the
           relatively short period it is acceptable only for applications with a
           mild amount of parallelism; otherwise, use a xorshift1024* generator.

           Beside passing BigCrush, this generator passes the PractRand test suite
           up to (and included) 16TB, with the exception of binary rank tests,
           which fail due to the lowest bit being an LFSR; all other bits pass all
           tests. We suggest to use a sign test to extract a random Boolean value.
   
           Note that the generator uses a simulated rotate operation, which most C
           compilers will turn into a single instruction. In Java, you can use
           Long.rotateLeft(). In languages that do not make low-level rotation
           instructions accessible xorshift128+ could be faster.

           The state must be seeded so that it is not everywhere zero. If you have
           a 64-bit seed, we suggest to seed a splitmix64 generator and use its
           output to fill s. */

        private static ulong rotl(ulong x, int k)
        {
            return (x << k) | (x >> (64 - k));
        }

        public virtual ulong NextUInt64() {
            ulong s0 = s[0];
            ulong s1 = s[1];
            ulong result = s0 + s1;

            s1 ^= s0;
            s[0] = rotl(s0, 55) ^ s1 ^ (s1 << 14); // a, b
            s[1] = rotl(s1, 36); // c

            return (result >> 1 | result << 63);
        }


        ///* This is the jump function for the generator. It is equivalent
        //   to 2^64 calls to next(); it can be used to generate 2^64
        //   non-overlapping subsequences for parallel computations. */

        //void jump(void) {
        //    static const uint64_t JUMP[] = { 0xbeac0467eba5facb, 0xd86b048b86aa9922 };

        //    uint64_t s0 = 0;
        //    uint64_t s1 = 0;
        //    for(int i = 0; i < sizeof JUMP / sizeof *JUMP; i++)
        //        for(int b = 0; b < 64; b++) {
        //            if (JUMP[i] & 1ULL << b) {
        //                s0 ^= s[0];
        //                s1 ^= s[1];
        //            }
        //            next();
        //        }

        //    s[0] = s0;
        //    s[1] = s1;
        //}

        public virtual int Next()
        {
            return (int)(NextUInt64() % Int32.MaxValue);
        }

        public virtual int Next(int minValue, int maxValue)
        {
            if (minValue > maxValue)
            {
                throw new ArgumentOutOfRangeException("minValue", "minValue must be lower than or equal to maxValue");
            }
            Contract.EndContractBlock();

            long range = (long)maxValue - minValue;
            if (range == 0)
                return minValue;
            return (int)((long)(NextUInt64() % (ulong)range) + minValue);
        }


        public virtual int Next(int maxValue)
        {
            if (maxValue < 0)
            {
                throw new ArgumentOutOfRangeException("maxValue", "maxValue must be equal to or greater than zero");
            }
            Contract.EndContractBlock();
            if (maxValue == 0)
                return 0;
            return (int)(NextUInt64() % (ulong)maxValue);
        }


        public virtual double NextDouble()
        {
            return ((double)NextUInt64() / ((double)UInt64.MaxValue + 1));
        }
    }

}
