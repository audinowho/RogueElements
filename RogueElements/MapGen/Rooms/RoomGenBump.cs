using System;
using System.Collections.Generic;

namespace RogueElements
{
    [Serializable]
    public class RoomGenBump<T> : PermissiveRoomGen<T> where T : ITiledGenContext
    {

        public RandRange Width;
        public RandRange Height;

        public RandRange BumpPercent;

        public RoomGenBump() { }

        public RoomGenBump(RandRange width, RandRange height, RandRange bumpPercent)
        {
            Width = width;
            Height = height;
            BumpPercent = bumpPercent;
        }
        
        public override Loc ProposeSize(IRandom rand)
        {
            return new Loc(Width.Pick(rand), Height.Pick(rand));
        }

        public override void DrawOnMap(T map)
        {
            //add peturbations
            if (Draw.Size.X > 2 && Draw.Size.Y > 2)
            {
                int tlTotal = (Draw.Size.X-1) / 2 * BumpPercent.Pick(map.Rand) / 100;
                int blTotal = (Draw.Size.X-1) / 2 * BumpPercent.Pick(map.Rand) / 100;
                int trTotal = (Draw.Size.X-1) / 2 * BumpPercent.Pick(map.Rand) / 100;
                int brTotal = (Draw.Size.X-1) / 2 * BumpPercent.Pick(map.Rand) / 100;

                int ltTotal = (Draw.Size.Y-1) / 2 * BumpPercent.Pick(map.Rand) / 100;
                int rtTotal = (Draw.Size.Y-1) / 2 * BumpPercent.Pick(map.Rand) / 100;
                int lbTotal = (Draw.Size.Y-1) / 2 * BumpPercent.Pick(map.Rand) / 100;
                int rbTotal = (Draw.Size.Y-1) / 2 * BumpPercent.Pick(map.Rand) / 100;


                for (int x = 0; x < Draw.Size.X; x++)
                {
                    for (int y = 0; y < Draw.Size.Y; y++)
                    {
                        if ((y == 0) && (x < tlTotal || x >= Draw.Size.X - trTotal) ||
                            (y == Draw.Size.Y - 1) && (x < blTotal || x >= Draw.Size.X - brTotal) ||
                            (x == 0) && (y < ltTotal || y >= Draw.Size.Y - lbTotal) ||
                            (x == Draw.Size.X - 1) && (y < rtTotal || y >= Draw.Size.Y - rbTotal))
                        {

                        }
                        else
                            map.Tiles[Draw.X + x][Draw.Y + y] = map.RoomTerrain.Copy();
                    }
                }
                
            }
            else
            {
                for (int x = 0; x < Draw.Size.X; x++)
                {
                    for (int y = 0; y < Draw.Size.Y; y++)
                        map.Tiles[Draw.X + x][Draw.Y + y] = map.RoomTerrain.Copy();
                }
            }

            //fulfill existing borders since this room doesn't cover the entire square
            FulfillRoomBorders(map);
            //hall restrictions
            SetRoomBorders(map);
        }
    }
}
