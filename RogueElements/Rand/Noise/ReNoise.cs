// <copyright file="ReNoise.cs" company="Audino">
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
    /// <remarks>
    /// This is a noise function based on murmur3 hash 128.
    /// </remarks>
    [Serializable]
    public class ReNoise : INoise
    {
        private const ulong C1 = 0x87c37b91114253d5;
        private const ulong C2 = 0x4cf5ad432745937f;

        public ReNoise()
            : this(unchecked((ulong)System.DateTime.Now.Ticks))
        {
        }

        public ReNoise(ulong seed)
        {
            this.FirstSeed = seed;
        }

        /// <summary>
        /// The seed value that the class was initialized with.
        /// </summary>
        public ulong FirstSeed { get; private set; }

        public ulong GetUInt64(ulong position)
        {
            byte[] data = BitConverter.GetBytes(position);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(data);
            return this.Hash(data)[0];
        }

        public ulong[] GetTwoUInt64(ulong position)
        {
            byte[] data = BitConverter.GetBytes(position);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(data);
            return this.Hash(data);
        }

        public ulong Get2DUInt64(ulong x, ulong y)
        {
            byte[] data1 = BitConverter.GetBytes(x);
            byte[] data2 = BitConverter.GetBytes(y);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(data1);
                Array.Reverse(data2);
            }

            byte[] data = new byte[sizeof(ulong) * 2];
            Array.Copy(data1, 0, data, 0, sizeof(ulong));
            Array.Copy(data2, 0, data, sizeof(ulong), sizeof(ulong));
            return this.Hash(data)[0];
        }

        public int GetInt(ulong position)
        {
            return (int)(this.GetUInt64(position) % int.MaxValue);
        }

        public int GetInt(ulong position, int maxValue)
        {
            if (maxValue < 0)
                throw new ArgumentOutOfRangeException(nameof(maxValue), $"{nameof(maxValue)} must be equal to or greater than zero");

            Contract.EndContractBlock();
            if (maxValue == 0)
                return 0;
            return (int)(this.GetUInt64(position) % (ulong)maxValue);
        }

        public int GetInt(ulong position, int minValue, int maxValue)
        {
            if (minValue > maxValue)
                throw new ArgumentOutOfRangeException(nameof(minValue), $"{nameof(minValue)} must be lower than or equal to {nameof(maxValue)}");

            Contract.EndContractBlock();

            long range = (long)maxValue - minValue;
            if (range == 0)
                return minValue;

            return (int)((long)(this.GetUInt64(position) % (ulong)range) + minValue);
        }

        public double GetDouble(ulong position)
        {
            return (double)this.GetUInt64(position) / ((double)ulong.MaxValue + 1);
        }

        private static ulong Rotl(ulong x, int k)
        {
            return (x << k) | (x >> (64 - k));
        }

        private static ulong FMix(ulong k)
        {
            k ^= k >> 33;
            k *= 0xff51afd7ed558ccd;
            k ^= k >> 33;
            k *= 0xc4ceb9fe1a85ec53;
            k ^= k >> 33;
            return k;
        }

        private static ulong ToUInt64(byte[] data, int start)
        {
            if (BitConverter.IsLittleEndian)
            {
                uint i1 = (uint)(data[start] | data[start + 1] << 8 | data[start + 2] << 16 | data[start + 3] << 24);
                ulong i2 = (ulong)(data[start + 4] | data[start + 5] << 8 | data[start + 6] << 16 | data[start + 7] << 24);
                return i1 | i2 << 32;
            }
            else
            {
                ulong i1 = (ulong)(data[start] << 24 | data[start + 1] << 16 | data[start + 2] << 8 | data[start + 3]);
                uint i2 = (uint)(data[start + 4] << 24 | data[start + 5] << 16 | data[start + 6] << 8 | data[start + 7]);
                return i2 | i1 << 32;
            }
        }

        private ulong[] Hash(byte[] data)
        {
            ulong h1 = this.FirstSeed;
            ulong h2 = this.FirstSeed;

            int curByte = 0;

            for (; curByte + 16 <= data.Length; curByte += 16)
            {
                ulong a1 = ToUInt64(data, curByte) * C1;
                a1 = Rotl(a1, 31) * C2;
                h1 ^= a1;
                h1 = (Rotl(h1, 27) + h2) * 5;
                h1 += 0x52dce729;

                ulong a2 = ToUInt64(data, curByte + 8) * C2;
                a2 = Rotl(a2, 33) * C1;
                h2 ^= a2;
                h2 = (Rotl(h2, 31) + h1) * 5;
                h2 += 0x38495ab5;
            }

            ulong k1 = 0;
            ulong k2 = 0;

#pragma warning disable CC0120 // Your Switch maybe include default clause
            switch (data.Length - curByte)
            {
                case 15: k2 ^= (ulong)data[curByte + 14] << 48; goto case 14;
                case 14: k2 ^= (ulong)data[curByte + 13] << 40; goto case 13;
                case 13: k2 ^= (ulong)data[curByte + 12] << 32; goto case 12;
                case 12: k2 ^= (ulong)data[curByte + 11] << 24; goto case 11;
                case 11: k2 ^= (ulong)data[curByte + 10] << 16; goto case 10;
                case 10: k2 ^= (ulong)data[curByte + 9] << 8; goto case 9;
                case 9: k2 ^= (ulong)data[curByte + 8] << 0; goto case 8;
                case 8: k1 ^= (ulong)data[curByte + 7] << 56; goto case 7;
                case 7: k1 ^= (ulong)data[curByte + 6] << 48; goto case 6;
                case 6: k1 ^= (ulong)data[curByte + 5] << 40; goto case 5;
                case 5: k1 ^= (ulong)data[curByte + 4] << 32; goto case 4;
                case 4: k1 ^= (ulong)data[curByte + 3] << 24; goto case 3;
                case 3: k1 ^= (ulong)data[curByte + 2] << 16; goto case 2;
                case 2: k1 ^= (ulong)data[curByte + 1] << 8; goto case 1;
                case 1: k1 ^= (ulong)data[curByte] << 0; break;
            }
#pragma warning restore CC0120 // Your Switch maybe include default clause

            k2 *= C2;
            k2 = Rotl(k2, 33);
            k2 *= C1;
            h2 ^= k2;

            k1 *= C1;
            k1 = Rotl(k1, 31);
            k1 *= C2;
            h1 ^= k1;

            h1 ^= (ulong)data.Length;
            h2 ^= (ulong)data.Length;

            h1 += h2;
            h2 += h1;

            h1 = FMix(h1);
            h2 = FMix(h2);

            h1 += h2;
            h2 += h1;

            return new ulong[2] { h1, h2 };
        }
    }
}
