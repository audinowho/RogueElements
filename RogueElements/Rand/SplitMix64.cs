// <copyright file="SplitMix64.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
using System;
using System.Collections.Generic;
using System.Text;

namespace RogueElements
{
    public class SplitMix64
    {
        private ulong x;

        public SplitMix64(ulong seed)
        {
            this.x = seed;
        }

        public ulong Next()
        {
            ulong z = this.x += 0x9E3779B97F4A7C15;
            z = (z ^ (z >> 30)) * 0xBF58476D1CE4E5B9;
            z = (z ^ (z >> 27)) * 0x94D049BB133111EB;
            return z ^ (z >> 31);
        }
    }
}
