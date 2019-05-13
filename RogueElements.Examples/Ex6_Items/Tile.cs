using System;
using System.Collections.Generic;
using RogueElements;

namespace RogueElements.Examples.Ex6_Items
{
    public class Tile : ITile
    {
        public int ID { get; set; }

        public Tile()
        {
            ID = 0;
        }
        public Tile(int id)
        {
            ID = id;
        }
        protected Tile(Tile other)
        {
            ID = other.ID;
        }
        public ITile Copy() { return new Tile(this); }

        public bool TileEquivalent(ITile other)
        {
            if (!(other is Tile tile))
                return false;
            return tile.ID == ID;
        }
    }
}
