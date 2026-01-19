// <copyright file="IRandom.cs" company="Audino">
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
    /// Defines a random number generator with repeatable seed support.
    /// </summary>
    public interface IRandom
    {
        /// <summary>
        /// Gets the seed value that the generator was initialized with.
        /// </summary>
        ulong FirstSeed { get; }

        /// <summary>
        /// Gets the next random 64-bit unsigned integer.
        /// </summary>
        /// <returns>A random 64-bit unsigned integer.</returns>
        ulong NextUInt64();

        /// <summary>
        /// Gets a non-negative random integer.
        /// </summary>
        /// <returns>A non-negative random integer.</returns>
        int Next();

        /// <summary>
        /// Gets a random integer within the specified range.
        /// </summary>
        /// <param name="minValue">The inclusive lower bound.</param>
        /// <param name="maxValue">The exclusive upper bound.</param>
        /// <returns>A random integer from minValue to maxValue - 1.</returns>
        int Next(int minValue, int maxValue);

        /// <summary>
        /// Gets a non-negative random integer less than the specified maximum.
        /// </summary>
        /// <param name="maxValue">The exclusive upper bound.</param>
        /// <returns>A random integer from 0 to maxValue - 1.</returns>
        int Next(int maxValue);

        /// <summary>
        /// Gets a random double between 0.0 and 1.0.
        /// </summary>
        /// <returns>A random double from 0.0 to 1.0.</returns>
        double NextDouble();
    }
}
