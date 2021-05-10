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
    [Serializable]
    public struct IntRange : IEquatable<IntRange>
    {
        /// <summary>
        /// Start of the range (inclusive)
        /// </summary>
        public int Min;

        /// <summary>
        /// End of the range (exclusive)
        /// </summary>
        public int Max;

        public IntRange(int num)
        {
            this.Min = num;
            this.Max = num + 1;
        }

        public IntRange(int min, int max)
        {
            this.Min = min;
            this.Max = max;
        }

        public IntRange(IntRange other)
        {
            this.Min = other.Min;
            this.Max = other.Max;
        }

        public int Length => this.Max - this.Min;

        public static bool operator ==(IntRange lhs, IntRange rhs) => lhs.Equals(rhs);

        public static bool operator !=(IntRange lhs, IntRange rhs) => !lhs.Equals(rhs);

        public static IntRange operator +(IntRange lhs, int rhs) => lhs.Add(rhs);

        public static IntRange Intersect(IntRange range1, IntRange range2)
        {
            return new IntRange(Math.Max(range1.Min, range2.Min), Math.Min(range1.Max, range2.Max));
        }

        public static IntRange IncludeRange(IntRange range1, IntRange range2)
        {
            int min = Math.Min(range1.Min, range2.Min);
            int max = Math.Max(range1.Max, range2.Max);
            return new IntRange(min, max);
        }

        public bool Contains(int mid)
        {
            return this.Min <= mid && mid < this.Max;
        }

        public bool Contains(IntRange value)
        {
            return (this.Min <= value.Min) && (value.Max <= this.Max);
        }

        public override string ToString()
        {
            if (this.Min + 1 == this.Max)
                return this.Min.ToString();
            else
                return $"[{this.Min}, {this.Max})";
        }

        public override bool Equals(object obj)
        {
            return (obj is IntRange) && this.Equals((IntRange)obj);
        }

        public bool Equals(IntRange other)
        {
            return this.Min == other.Min && this.Max == other.Max;
        }

        public override int GetHashCode()
        {
            return this.Min.GetHashCode() ^ this.Max.GetHashCode();
        }

        public IntRange Add(int value)
        {
            return new IntRange(this.Min + value, this.Max + value);
        }
    }
}
