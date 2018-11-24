using System;
using System.Collections.Generic;

namespace RogueElements
{
    [Serializable]
    public class RoomGenDefault<T> : PermissiveRoomGen<T> where T : ITiledGenContext
    {
        public RoomGenDefault() { }


        public override Loc ProposeSize(IRandom rand)
        {
            return new Loc(1);
        }

        public override void DrawOnMap(T map)
        {
            DrawMapDefault(map);
        }
    }

}
