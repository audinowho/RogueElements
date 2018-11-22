using System;
using System.Collections.Generic;
using RogueElements;

namespace RogueElements.Examples.Ex8_Custom
{

    public class Item
    {
        public int ID { get; set; }
        public Loc Loc { get; set; }

        public Item() { }
        public Item(int id) { ID = id; }
        public Item(int id, Loc loc) { ID = id; Loc = loc; }
    }
}
