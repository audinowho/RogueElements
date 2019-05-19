using System;
using System.Collections.Generic;
using RogueElements;

namespace RogueElements.Examples.Ex4_Stairs
{
    public class StairsUp : Stairs, IEntrance
    {
        public StairsUp() { }
        protected StairsUp(StairsUp other) : base(other) { }
        public override ISpawnable Copy() { return new StairsUp(this); }
    }
    public class StairsDown : Stairs, IExit
    {
        public StairsDown() { }
        protected StairsDown(StairsDown other) : base(other) { }
        public override ISpawnable Copy() { return new StairsDown(this); }
    }
    public abstract class Stairs : ISpawnable
    {
        public Loc Loc { get; set; }

        public Stairs() { }
        protected Stairs(Stairs other)
        {
            Loc = other.Loc;
        }
        public abstract ISpawnable Copy();
    }
}
