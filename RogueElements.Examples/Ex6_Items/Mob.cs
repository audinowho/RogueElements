using System;
using System.Collections.Generic;
using RogueElements;

namespace RogueElements.Examples.Ex6_Items
{

    public class Mob : ISpawnable
    {
        public int ID { get; set; }
        public Loc Loc { get; set; }

        public Mob() { }
        public Mob(int id) { ID = id; }
        public Mob(int id, Loc loc) { ID = id; Loc = loc; }
        protected Mob(Mob other)
        {
            ID = other.ID;
            Loc = other.Loc;
        }
        public ISpawnable Copy() { return new Mob(this); }
    }
}
