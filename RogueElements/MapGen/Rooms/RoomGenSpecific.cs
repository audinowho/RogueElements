using System;
using System.Collections.Generic;

namespace RogueElements
{
    [Serializable]
    public class RoomGenSpecific<T> : RoomGen<T> where T : ITiledGenContext
    {
        public ITile RoomTerrain;
        public ITile[][] Tiles;
        public Dictionary<Dir4, bool[]> Borders;
        public bool FulfillAll;

        public RoomGenSpecific() { }
        public RoomGenSpecific(int width, int height)
        {
            Tiles = new ITile[width][];
            for (int xx = 0; xx < width; xx++)
                Tiles[xx] = new ITile[height];
            Borders = new Dictionary<Dir4, bool[]>();
            Borders[Dir4.Down] = new bool[width];
            Borders[Dir4.Up] = new bool[width];
            Borders[Dir4.Left] = new bool[height];
            Borders[Dir4.Right] = new bool[height];
        }
        public RoomGenSpecific(int width, int height, ITile roomTerrain, bool fulfillAll) : this(width, height)
        {
            RoomTerrain = roomTerrain;
            FulfillAll = fulfillAll;
        }
        protected RoomGenSpecific(RoomGenSpecific<T> other)
        {
            RoomTerrain = other.RoomTerrain;
            Tiles = new ITile[other.Tiles.Length][];
            for (int xx = 0; xx < other.Tiles.Length; xx++)
            {
                Tiles[xx] = new ITile[other.Tiles[0].Length];
                for (int yy = 0; yy < other.Tiles[0].Length; yy++)
                    Tiles[xx][yy] = other.Tiles[xx][yy].Copy();
            }
            Borders = new Dictionary<Dir4, bool[]>();
            foreach (Dir4 dir in DirExt.VALID_DIR4)
            {
                Borders[dir] = new bool[other.Borders[dir].Length];
                for (int jj = 0; jj < other.Borders[dir].Length; jj++)
                    Borders[dir][jj] = other.Borders[dir][jj];
            }
            FulfillAll = other.FulfillAll;
        }
        public override RoomGen<T> Copy() { return new RoomGenSpecific<T>(this); }

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
                foreach (Dir4 dir in DirExt.VALID_DIR4)
                {
                    for (int jj = 0; jj < fulfillableBorder[dir].Length; jj++)
                        fulfillableBorder[dir][jj] = true;
                }
            }
            else
            {
                for (int ii = 0; ii < draw.Width; ii++)
                {
                    fulfillableBorder[Dir4.Up][ii] = Tiles[ii][0].TileEquivalent(RoomTerrain) || Borders[Dir4.Up][ii];
                    fulfillableBorder[Dir4.Down][ii] = Tiles[ii][draw.Height - 1].TileEquivalent(RoomTerrain) || Borders[Dir4.Down][ii];
                }

                for (int ii = 0; ii < draw.Height; ii++)
                {
                    fulfillableBorder[Dir4.Left][ii] = Tiles[0][ii].TileEquivalent(RoomTerrain) || Borders[Dir4.Left][ii];
                    fulfillableBorder[Dir4.Right][ii] = Tiles[Draw.Width - 1][ii].TileEquivalent(RoomTerrain) || Borders[Dir4.Right][ii];
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
            FulfillRoomBorders(map, FulfillAll);
            SetRoomBorders(map);
        }
    }
}
