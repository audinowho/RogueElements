using System;

namespace RogueElements
{
    [Serializable]
    public class RoomGenSquare<T> : PermissiveRoomGen<T> where T : ITiledGenContext
    {

        public RandRange Width;
        public RandRange Height;

        public RoomGenSquare() { }

        public RoomGenSquare(RandRange width, RandRange height)
        {
            Width = width;
            Height = height;
        }

        public override Loc ProposeSize(IRandom rand)
        {
            return new Loc(Width.Pick(rand), Height.Pick(rand));
        }

        public override void DrawOnMap(T map)
        {
            DrawMapDefault(map);
        }
    }
}
