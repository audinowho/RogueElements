using System;
using System.Collections.Generic;

namespace RogueElements
{
    [Serializable]
    public class RoomGenSpecific<T> : RoomGen<T> where T : ITiledGenContext
    {
        public ITile RoomTerrain;
        public ITile[][] Tiles;

        public RoomGenSpecific() { }
        public RoomGenSpecific(ITile roomTerrain) { RoomTerrain = roomTerrain; }

        public override Loc ProposeSize(IRandom rand)
        {
            return new Loc(Tiles.Length, Tiles[0].Length);
        }

        protected override void PrepareFulfillableBorders(IRandom rand)
        {
            //NOTE: Because the context is not passed in when preparing borders,
            //the tile ID representing an opening must be specified on this class instead.
            if (Draw.Width != Tiles.Length || Draw.Height != Tiles[0].Length)
            {
                for (int ii = 0; ii < 4; ii++)
                {
                    for (int jj = 0; jj < fulfillableBorder[ii].Length; jj++)
                        fulfillableBorder[ii][jj] = true;
                }
            }
            else
            {
                for (int ii = 0; ii < draw.Width; ii++)
                {
                    fulfillableBorder[(int)Dir4.Up][ii] = Tiles[ii][0].TileEquivalent(RoomTerrain);
                    fulfillableBorder[(int)Dir4.Down][ii] = Tiles[ii][draw.Height - 1].TileEquivalent(RoomTerrain);
                }

                for (int ii = 0; ii < draw.Height; ii++)
                {
                    fulfillableBorder[(int)Dir4.Left][ii] = Tiles[0][ii].TileEquivalent(RoomTerrain);
                    fulfillableBorder[(int)Dir4.Right][ii] = Tiles[Draw.Width - 1][ii].TileEquivalent(RoomTerrain);
                }
            }
        }

        public override void DrawOnMap(T map)
        {
            if (Draw.Width != Tiles.Length || Draw.Height != Tiles[0].Length)
            {
                DrawMapDefault(map);
                return;
            }
            for (int xx = 0; xx < Draw.Width; xx++)
            {
                for (int yy = 0; yy < Draw.Height; yy++)
                    map.SetTile(new Loc(Draw.X + xx, Draw.Y + yy), Tiles[xx][yy].Copy());
            }
            SetRoomBorders(map);
        }
    }
}
