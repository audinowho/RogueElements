using System;
using System.Collections.Generic;
using RogueElements;

namespace RogueElements.Examples.Ex6_Items
{

    public class Item : ISpawnable
    {
        public int ID { get; set; }
        public Loc Loc { get; set; }

        public Item() { }
        public Item(int id) { ID = id; }
        public Item(int id, Loc loc) { ID = id; Loc = loc; }
        protected Item(Item other)
        {
            ID = other.ID;
            Loc = other.Loc;
        }
        public ISpawnable Copy() { return new Item(this); }

    }
}
