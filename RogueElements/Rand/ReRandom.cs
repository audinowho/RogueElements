// <copyright file="ReRandom.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Runtime;
using System.Runtime.CompilerServices;

namespace RogueElements
{
    /// <summary>
    /// A custom random class that holds on to its seed, for repeatability.
    /// </summary>
    [Serializable]
    public class ReRandom : IRandom
    {
        private readonly ulong[] s;

        public ReRandom()
            : this(unchecked((ulong)System.DateTime.Now.Ticks))
        {
        }

        public ReRandom(ulong seed)
        {
            this.FirstSeed = seed;

            this.s = new ulong[2];
            this.s[0] = SplitMix64(this.FirstSeed);
            this.s[1] = SplitMix64(this.s[1]);
        }

        /// <summary>
        /// The seed value that the class was initialized with.
        /// </summary>
        public ulong FirstSeed { get; private set; }

        // xoroshiro128+
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

        public virtual ulong NextUInt64()
        {
            ulong s0 = this.s[0];
            ulong s1 = this.s[1];
            ulong result = s0 + s1;

            s1 ^= s0;
            this.s[0] = Rotl(s0, 55) ^ s1 ^ (s1 << 14); // a, b
            this.s[1] = Rotl(s1, 36); // c

            return result >> 1 | result << 63;
        }

        public virtual int Next()
        {
            return (int)(this.NextUInt64() % int.MaxValue);
        }

        public virtual int Next(int minValue, int maxValue)
        {
            if (minValue > maxValue)
                throw new ArgumentOutOfRangeException(nameof(minValue), $"{nameof(minValue)} must be lower than or equal to {nameof(maxValue)}");

            Contract.EndContractBlock();

            long range = (long)maxValue - minValue;
            if (range == 0)
                return minValue;
            return (int)((long)(this.NextUInt64() % (ulong)range) + minValue);
        }

        public virtual int Next(int maxValue)
        {
            if (maxValue < 0)
                throw new ArgumentOutOfRangeException(nameof(maxValue), $"{nameof(maxValue)} must be equal to or greater than zero");

            Contract.EndContractBlock();
            if (maxValue == 0)
                return 0;
            return (int)(this.NextUInt64() % (ulong)maxValue);
        }

        public virtual double NextDouble()
        {
            return (double)this.NextUInt64() / ((double)ulong.MaxValue + 1);
        }

        private static ulong SplitMix64(ulong x)
        {
            ulong z = x += 0x9E3779B97F4A7C15;
            z = (z ^ (z >> 30)) * 0xBF58476D1CE4E5B9;
            z = (z ^ (z >> 27)) * 0x94D049BB133111EB;
            return z ^ (z >> 31);
        }

        private static ulong Rotl(ulong x, int k)
        {
            return (x << k) | (x >> (64 - k));
        }
    }
}
