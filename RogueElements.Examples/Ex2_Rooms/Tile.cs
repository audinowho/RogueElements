using System;
using System.Collections.Generic;
using RogueElements;

namespace RogueElements.Examples.Ex2_Rooms
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
    }
}
