// <copyright file="IntRange.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RogueElements
{
    /// <summary>
    /// Represents a range of integers with inclusive minimum and exclusive maximum.
    /// </summary>
    [Serializable]
    public struct IntRange : IEquatable<IntRange>
    {
        /// <summary>
        /// Start of the range (inclusive).
        /// </summary>
        public int Min;

        /// <summary>
        /// End of the range (exclusive).
        /// </summary>
        public int Max;

        /// <summary>
        /// Initializes a new instance of the <see cref="IntRange"/> struct containing a single value.
        /// </summary>
        /// <param name="num">The single value in the range.</param>
        public IntRange(int num)
        {
            this.Min = num;
            this.Max = num + 1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IntRange"/> struct with specified bounds.
        /// </summary>
        /// <param name="min">The inclusive minimum.</param>
        /// <param name="max">The exclusive maximum.</param>
        public IntRange(int min, int max)
        {
            this.Min = min;
            this.Max = max;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IntRange"/> struct by copying another range.
        /// </summary>
        /// <param name="other">The range to copy.</param>
        public IntRange(IntRange other)
        {
            this.Min = other.Min;
            this.Max = other.Max;
        }

        /// <summary>
        /// Gets the length of the range (Max - Min).
        /// </summary>
        public int Length => this.Max - this.Min;

        public static bool operator ==(IntRange lhs, IntRange rhs) => lhs.Equals(rhs);

        public static bool operator !=(IntRange lhs, IntRange rhs) => !lhs.Equals(rhs);

        public static IntRange operator +(IntRange lhs, int rhs) => lhs.Add(rhs);

        /// <summary>
        /// Returns the intersection of two ranges.
        /// </summary>
        /// <param name="range1">The first range.</param>
        /// <param name="range2">The second range.</param>
        /// <returns>A range representing the intersection.</returns>
        public static IntRange Intersect(IntRange range1, IntRange range2)
        {
            return new IntRange(Math.Max(range1.Min, range2.Min), Math.Min(range1.Max, range2.Max));
        }

        /// <summary>
        /// Returns a range that spans both input ranges.
        /// </summary>
        /// <param name="range1">The first range.</param>
        /// <param name="range2">The second range.</param>
        /// <returns>A range that includes both input ranges.</returns>
        public static IntRange IncludeRange(IntRange range1, IntRange range2)
        {
            int min = Math.Min(range1.Min, range2.Min);
            int max = Math.Max(range1.Max, range2.Max);
            return new IntRange(min, max);
        }

        /// <summary>
        /// Determines whether a value is within this range.
        /// </summary>
        /// <param name="mid">The value to check.</param>
        /// <returns><c>true</c> if the value is within the range; otherwise <c>false</c>.</returns>
        public bool Contains(int mid)
        {
            return this.Min <= mid && mid < this.Max;
        }

        /// <summary>
        /// Determines whether another range is entirely contained within this range.
        /// </summary>
        /// <param name="value">The range to check.</param>
        /// <returns><c>true</c> if the range is entirely contained; otherwise <c>false</c>.</returns>
        public bool Contains(IntRange value)
        {
            return (this.Min <= value.Min) && (value.Max <= this.Max);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            if (this.Min + 1 == this.Max)
                return this.Min.ToString();
            else
                return $"[{this.Min}, {this.Max})";
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return (obj is IntRange) && this.Equals((IntRange)obj);
        }

        /// <inheritdoc/>
        public bool Equals(IntRange other)
        {
            return this.Min == other.Min && this.Max == other.Max;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return this.Min.GetHashCode() ^ this.Max.GetHashCode();
        }

        /// <summary>
        /// Returns a new range offset by the specified value.
        /// </summary>
        /// <param name="value">The offset to apply to both Min and Max.</param>
        /// <returns>A new offset range.</returns>
        public IntRange Add(int value)
        {
            return new IntRange(this.Min + value, this.Max + value);
        }
    }
}
