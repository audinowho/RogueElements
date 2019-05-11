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
            Min = num;
            Max = num + 1;
        }

        public Range(int min, int max)
        {
            Min = min;
            Max = max;
        }

        public Range(Range other)
        {
            Min = other.Min;
            Max = other.Max;
        }

        public bool Contains(int mid)
        {
            return (Min <= mid && mid < Max);
        }

        public bool Contains(Range value)
        {
            return (Min <= value.Min) && (value.Max <= Max);
        }

        public int Length
        {
            get { return Max - Min; }
        }

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

        public override string ToString()
        {
            return $"({Min}, {Max}]";
        }

        public override bool Equals(object obj)
        {
            return (obj is Range) && Equals((Range)obj);
        }

        public bool Equals(Range other)
        {
            return (Min == other.Min && Max == other.Max);
        }

        public override int GetHashCode()
        {
            return Min.GetHashCode() ^ Max.GetHashCode();
        }


        public Range Add(int value)
        {
            return new Range(Min + value, Max + value);
        }

        public static bool operator ==(Range lhs, Range rhs) => lhs.Equals(rhs);
        public static bool operator !=(Range lhs, Range rhs) => !lhs.Equals(rhs);
        public static Range operator +(Range lhs, int rhs) => lhs.Add(rhs);
    }
}
