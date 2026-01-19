// <copyright file="RandRange.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Selects an integer in a predefined range.
    /// </summary>
    [Serializable]
    public struct RandRange : IRandPicker<int>, IEquatable<RandRange>
    {
        /// <summary>
        /// The minimum value (inclusive).
        /// </summary>
        public int Min;

        /// <summary>
        /// The maximum value (exclusive).
        /// </summary>
        public int Max;

        /// <summary>
        /// Initializes a new instance of the <see cref="RandRange"/> struct with a single value.
        /// </summary>
        /// <param name="num">The exact value to return.</param>
        public RandRange(int num)
        {
            this.Min = num;
            this.Max = num;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RandRange"/> struct with a range.
        /// </summary>
        /// <param name="min">The minimum value (inclusive).</param>
        /// <param name="max">The maximum value (exclusive).</param>
        public RandRange(int min, int max)
        {
            this.Min = min;
            this.Max = max;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RandRange"/> struct by copying another.
        /// </summary>
        /// <param name="other">The instance to copy.</param>
        public RandRange(RandRange other)
        {
            this.Min = other.Min;
            this.Max = other.Max;
        }

        /// <summary>
        /// Gets an empty range representing zero.
        /// </summary>
        public static RandRange Empty => new RandRange(0);

        /// <inheritdoc/>
        public bool ChangesState => false;

        /// <inheritdoc/>
        public bool CanPick => this.Min <= this.Max;

        /// <summary>
        /// Determines whether two ranges are equal.
        /// </summary>
        /// <param name="lhs">The left-hand side range.</param>
        /// <param name="rhs">The right-hand side range.</param>
        /// <returns>True if the ranges are equal.</returns>
        public static bool operator ==(RandRange lhs, RandRange rhs) => lhs.Equals(rhs);

        /// <summary>
        /// Determines whether two ranges are not equal.
        /// </summary>
        /// <param name="lhs">The left-hand side range.</param>
        /// <param name="rhs">The right-hand side range.</param>
        /// <returns>True if the ranges are not equal.</returns>
        public static bool operator !=(RandRange lhs, RandRange rhs) => !lhs.Equals(rhs);

        /// <inheritdoc/>
        public IRandPicker<int> CopyState() => new RandRange(this);

        /// <inheritdoc/>
        public IEnumerable<int> EnumerateOutcomes()
        {
            yield return this.Min;
            for (int ii = this.Min + 1; ii < this.Max; ii++)
                yield return ii;
        }

        /// <inheritdoc/>
        public int Pick(IRandom rand) => rand.Next(this.Min, this.Max);

        /// <inheritdoc/>
        public bool Equals(RandRange other) => this.Min == other.Min && this.Max == other.Max;

        /// <inheritdoc/>
        public override bool Equals(object obj) => (obj is RandRange) && this.Equals((RandRange)obj);

        /// <inheritdoc/>
        public override int GetHashCode() => unchecked(191 + (this.Min.GetHashCode() * 313) ^ (this.Max.GetHashCode() * 739));

        /// <inheritdoc/>
        public override string ToString()
        {
            if (this.Min + 1 >= this.Max)
                return this.Min.ToString();
            else
                return string.Format("{0}-{1}", this.Min, this.Max);
        }
    }
}
