using System;
using System.Collections.Generic;

namespace RogueElements
{
    [Serializable]
    public struct LocRay8 : IEquatable<LocRay8>
    {
        public Loc Loc;
        public Dir8 Dir;

        public LocRay8(Loc loc)
        {
            Loc = loc;
            Dir = Dir8.None;
        }
        public LocRay8(Dir8 dir)
        {
            Loc = new Loc();
            Dir = dir;
        }
        public LocRay8(Loc loc, Dir8 dir)
        {
            Loc = loc;
            Dir = dir;
        }
        public LocRay8(int x, int y, Dir8 dir)
        {
            Loc = new Loc(x, y);
            Dir = dir;
        }

        public Loc Traverse(int dist)
        {
            return Loc + Dir.GetLoc() * dist;
        }

        public bool Equals(LocRay8 other) => Loc == other.Loc && Dir == other.Dir;
        public override bool Equals(object obj) => (obj is LocRay8) && Equals((LocRay8)obj);
        public override int GetHashCode() => unchecked(971 + (Loc.GetHashCode() * 619) ^ (Dir.GetHashCode() * 491));

        public static bool operator ==(LocRay8 lhs, LocRay8 rhs) => lhs.Equals(rhs);
        public static bool operator !=(LocRay8 lhs, LocRay8 rhs) => !lhs.Equals(rhs);
    }

    [Serializable]
    public struct LocRay4 : IEquatable<LocRay4>
    {
        public Loc Loc;
        public Dir4 Dir;

        public LocRay4(Loc loc)
        {
            Loc = loc;
            Dir = Dir4.None;
        }
        public LocRay4(Dir4 dir)
        {
            Loc = new Loc();
            Dir = dir;
        }
        public LocRay4(Loc loc, Dir4 dir)
        {
            Loc = loc;
            Dir = dir;
        }
        public LocRay4(int x, int y, Dir4 dir)
        {
            Loc = new Loc(x, y);
            Dir = dir;
        }

        public Loc Traverse(int dist)
        {
            return Loc + Dir.GetLoc() * dist;
        }

        public bool Equals(LocRay4 other) => Loc == other.Loc && Dir == other.Dir;
        public override bool Equals(object obj) => (obj is LocRay4) && Equals((LocRay4)obj);
        public override int GetHashCode() => unchecked(571 + (Loc.GetHashCode() * 293) ^ (Dir.GetHashCode() * 827));

        public static bool operator ==(LocRay4 lhs, LocRay4 rhs) => lhs.Equals(rhs);
        public static bool operator !=(LocRay4 lhs, LocRay4 rhs) => !lhs.Equals(rhs);
    }
}
