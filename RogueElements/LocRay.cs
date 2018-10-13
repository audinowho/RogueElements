using System;
using System.Collections.Generic;

namespace RogueElements
{
    [Serializable]
    public struct LocRay8
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

        public Loc Traverse(int dist)
        {
            return Loc + Dir.GetLoc() * dist;
        }
    }

    [Serializable]
    public struct LocRay4
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

        public Loc Traverse(int dist)
        {
            return Loc + Dir.GetLoc() * dist;
        }
    }
}
