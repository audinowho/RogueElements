using System;
using System.Collections;
using System.Collections.Generic;

namespace RogueElements
{
    [Serializable]
    public struct RandRange : IRandPicker<int>, IEquatable<RandRange>
    {
        public int Min;
        public int Max;
        public bool ChangesState => false;
        public bool CanPick => Min <= Max;

        public RandRange(int num) { Min = num; Max = num; }
        public RandRange(int min, int max) { Min = min; Max = max; }
        public RandRange(RandRange other)
        {
            Min = other.Min;
            Max = other.Max;
        }
        public IRandPicker<int> CopyState() => new RandRange(this);

        public IEnumerator<int> GetEnumerator()
        {
            yield return Min;
            for (int ii = Min + 1; ii < Max; ii++)
                yield return ii;
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public int Pick(IRandom rand) => rand.Next(Min, Max);

        public bool Equals(RandRange other) => Min == other.Min && Max == other.Max;
        public override bool Equals(object obj) => (obj is RandRange) && Equals((RandRange)obj);

        public override int GetHashCode() => unchecked(191 + (Min.GetHashCode() * 313) ^ (Max.GetHashCode() * 739));

        public static bool operator ==(RandRange lhs, RandRange rhs) => lhs.Equals(rhs);
        public static bool operator !=(RandRange lhs, RandRange rhs) => !lhs.Equals(rhs);
    }
}
