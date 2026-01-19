// <copyright file="SplitMix64.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
using System;
using System.Collections.Generic;
using System.Text;

namespace RogueElements
{
    /// <summary>
    /// A splitmix64 random number generator used for initializing other RNG states.
    /// </summary>
    public class SplitMix64
    {
        private ulong x;

        /// <summary>
        /// Initializes a new instance of the <see cref="SplitMix64"/> class with a specified seed.
        /// </summary>
        /// <param name="seed">The seed value for the generator.</param>
        public SplitMix64(ulong seed)
        {
            this.x = seed;
        }

        /// <summary>
        /// Gets the next random 64-bit unsigned integer.
        /// </summary>
        /// <returns>A random 64-bit unsigned integer.</returns>
        public ulong Next()
        {
            ulong z = this.x += 0x9E3779B97F4A7C15;
            z = (z ^ (z >> 30)) * 0xBF58476D1CE4E5B9;
            z = (z ^ (z >> 27)) * 0x94D049BB133111EB;
            return z ^ (z >> 31);
        }
    }
}
