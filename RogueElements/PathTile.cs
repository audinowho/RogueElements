using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RogueElements
{
    public class PathTile
    {

        public Loc Location;
        public bool Traversed;
        public int Cost;
        public double Heuristic;
        public PathTile BackReference;

        public PathTile(Loc location)
        {
            Location = location;
            Cost = -1;
        }
    }
}
