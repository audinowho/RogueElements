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
        public int Min;
        public int Max;

        public RandRange(int num)
        {
            this.Min = num;
            this.Max = num;
        }

        public RandRange(int min, int max)
        {
            this.Min = min;
            this.Max = max;
        }

        public RandRange(RandRange other)
        {
            this.Min = other.Min;
            this.Max = other.Max;
        }

        public static RandRange Empty => new RandRange(0);

        public bool ChangesState => false;

        public bool CanPick => this.Min <= this.Max;

        public static bool operator ==(RandRange lhs, RandRange rhs) => lhs.Equals(rhs);

        public static bool operator !=(RandRange lhs, RandRange rhs) => !lhs.Equals(rhs);

        public IRandPicker<int> CopyState() => new RandRange(this);

        public IEnumerable<int> EnumerateOutcomes()
        {
            yield return this.Min;
            for (int ii = this.Min + 1; ii < this.Max; ii++)
                yield return ii;
        }

        public int Pick(IRandom rand) => rand.Next(this.Min, this.Max);

        public bool Equals(RandRange other) => this.Min == other.Min && this.Max == other.Max;

        public override bool Equals(object obj) => (obj is RandRange) && this.Equals((RandRange)obj);

        public override int GetHashCode() => unchecked(191 + (this.Min.GetHashCode() * 313) ^ (this.Max.GetHashCode() * 739));

        public override string ToString()
        {
            if (this.Min + 1 >= this.Max)
                return this.Min.ToString();
            else
                return string.Format("{0}-{1}", this.Min, this.Max);
        }
    }
}
