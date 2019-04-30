using System;
using System.Collections.Generic;
using RogueElements;

namespace RogueElements.Examples.Ex6_Items
{
    public class StairsUp : Stairs
    {
    }
    public class StairsDown : Stairs
    {
    }
    public class Stairs : ISpawnable
    {
        public Loc Loc { get; set; }

        public Stairs() { }
        protected Stairs(Stairs other)
        {
            Loc = other.Loc;
        }
        public ISpawnable Copy() { return new Stairs(this); }
    }
}
