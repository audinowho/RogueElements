// <copyright file="Range.cs" company="Audino">
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
    public struct Range : IEquatable<Range>
    {
        /// <summary>
        /// Start of the range (inclusive)
        /// </summary>
        public int Min;

        /// <summary>
        /// End of the range (exclusive)
        /// </summary>
        public int Max;

        public Range(int num)
        {
            this.Min = num;
            this.Max = num + 1;
        }

        public Range(int min, int max)
        {
            this.Min = min;
            this.Max = max;
        }

        public Range(Range other)
        {
            this.Min = other.Min;
            this.Max = other.Max;
        }

        public int Length => this.Max - this.Min;

        public static bool operator ==(Range lhs, Range rhs) => lhs.Equals(rhs);

        public static bool operator !=(Range lhs, Range rhs) => !lhs.Equals(rhs);

        public static Range operator +(Range lhs, int rhs) => lhs.Add(rhs);

        public static Range Intersect(Range range1, Range range2)
        {
            return new Range(Math.Max(range1.Min, range2.Min), Math.Min(range1.Max, range2.Max));
        }

        public static Range IncludeRange(Range range1, Range range2)
        {
            int min = Math.Min(range1.Min, range2.Min);
            int max = Math.Max(range1.Max, range2.Max);
            return new Range(min, max);
        }

        public bool Contains(int mid)
        {
            return this.Min <= mid && mid < this.Max;
        }

        public bool Contains(Range value)
        {
            return (this.Min <= value.Min) && (value.Max <= this.Max);
        }

        public override string ToString()
        {
            return $"({this.Min}, {this.Max}]";
        }

        public override bool Equals(object obj)
        {
            return (obj is Range) && this.Equals((Range)obj);
        }

        public bool Equals(Range other)
        {
            return this.Min == other.Min && this.Max == other.Max;
        }

        public override int GetHashCode()
        {
            return this.Min.GetHashCode() ^ this.Max.GetHashCode();
        }

        public Range Add(int value)
        {
            return new Range(this.Min + value, this.Max + value);
        }
    }
}
