// <copyright file="INoise.cs" company="Audino">
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
    /// Defines a noise function that generates deterministic random values based on position.
    /// </summary>
    public interface INoise
    {
        /// <summary>
        /// Gets the seed value that the noise function was initialized with.
        /// </summary>
        ulong FirstSeed { get; }

        /// <summary>
        /// Gets a non-negative random integer at the specified position.
        /// </summary>
        /// <param name="position">The position in the noise function.</param>
        /// <returns>A non-negative random integer.</returns>
        int GetInt(ulong position);

        /// <summary>
        /// Gets a non-negative random integer less than the specified maximum at the given position.
        /// </summary>
        /// <param name="position">The position in the noise function.</param>
        /// <param name="maxValue">The exclusive upper bound.</param>
        /// <returns>A random integer from 0 to maxValue - 1.</returns>
        int GetInt(ulong position, int maxValue);

        /// <summary>
        /// Gets a random integer within the specified range at the given position.
        /// </summary>
        /// <param name="position">The position in the noise function.</param>
        /// <param name="minValue">The inclusive lower bound.</param>
        /// <param name="maxValue">The exclusive upper bound.</param>
        /// <returns>A random integer from minValue to maxValue - 1.</returns>
        int GetInt(ulong position, int minValue, int maxValue);

        /// <summary>
        /// Gets a random 64-bit unsigned integer at the specified position.
        /// </summary>
        /// <param name="position">The position in the noise function.</param>
        /// <returns>A random 64-bit unsigned integer.</returns>
        ulong GetUInt64(ulong position);

        /// <summary>
        /// Gets a random 64-bit unsigned integer at the specified 2D coordinates.
        /// </summary>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        /// <returns>A random 64-bit unsigned integer.</returns>
        ulong Get2DUInt64(ulong x, ulong y);

        /// <summary>
        /// Gets a random double between 0.0 and 1.0 at the specified position.
        /// </summary>
        /// <param name="position">The position in the noise function.</param>
        /// <returns>A random double from 0.0 to 1.0.</returns>
        double GetDouble(ulong position);
    }
}
