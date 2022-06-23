// <copyright file="RoomGenSpecific.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Generates a room with specific tiles and borders.
    /// EDITOR UNFRIENDLY
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class RoomGenSpecific<T> : RoomGen<T>
        where T : ITiledGenContext
    {
        public RoomGenSpecific()
        {
        }

        public RoomGenSpecific(int width, int height)
        {
            this.Tiles = new ITile[width][];
            for (int xx = 0; xx < width; xx++)
                this.Tiles[xx] = new ITile[height];
        }

        public RoomGenSpecific(int width, int height, ITile roomTerrain)
            : this(width, height)
        {
            this.RoomTerrain = roomTerrain;
        }

        protected RoomGenSpecific(RoomGenSpecific<T> other)
        {
            this.RoomTerrain = other.RoomTerrain;
            this.Tiles = new ITile[other.Tiles.Length][];
            for (int xx = 0; xx < other.Tiles.Length; xx++)
            {
                this.Tiles[xx] = new ITile[other.Tiles[0].Length];
                for (int yy = 0; yy < other.Tiles[0].Length; yy++)
                    this.Tiles[xx][yy] = other.Tiles[xx][yy].Copy();
            }
        }

        public ITile RoomTerrain { get; set; }

        public ITile[][] Tiles { get; set; }

        public override RoomGen<T> Copy() => new RoomGenSpecific<T>(this);

        public override Loc ProposeSize(IRandom rand)
        {
            return new Loc(this.Tiles.Length, this.Tiles[0].Length);
        }

        public override void DrawOnMap(T map)
        {
            if (this.Draw.Width != this.Tiles.Length || this.Draw.Height != this.Tiles[0].Length)
            {
                this.DrawMapDefault(map);
                return;
            }

            for (int xx = 0; xx < this.Draw.Width; xx++)
            {
                for (int yy = 0; yy < this.Draw.Height; yy++)
                    map.SetTile(new Loc(this.Draw.X + xx, this.Draw.Y + yy), this.Tiles[xx][yy].Copy());
            }

            this.SetRoomBorders(map);
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}x{2}", this.GetType().Name, this.Tiles.Length, this.Tiles[0].Length);
        }

        protected override void PrepareFulfillableBorders(IRandom rand)
        {
            // NOTE: Because the context is not passed in when preparing borders,
            // the tile ID representing an opening must be specified on this class instead.
            if (this.Draw.Width != this.Tiles.Length || this.Draw.Height != this.Tiles[0].Length)
            {
                foreach (Dir4 dir in DirExt.VALID_DIR4)
                {
                    for (int jj = 0; jj < this.FulfillableBorder[dir].Length; jj++)
                        this.FulfillableBorder[dir][jj] = true;
                }
            }
            else
            {
                for (int ii = 0; ii < this.Draw.Width; ii++)
                {
                    this.FulfillableBorder[Dir4.Up][ii] = this.RoomTerrain.TileEquivalent(this.Tiles[ii][0]);
                    this.FulfillableBorder[Dir4.Down][ii] = this.RoomTerrain.TileEquivalent(this.Tiles[ii][this.Draw.Height - 1]);
                }

                for (int ii = 0; ii < this.Draw.Height; ii++)
                {
                    this.FulfillableBorder[Dir4.Left][ii] = this.RoomTerrain.TileEquivalent(this.Tiles[0][ii]);
                    this.FulfillableBorder[Dir4.Right][ii] = this.RoomTerrain.TileEquivalent(this.Tiles[this.Draw.Width - 1][ii]);
                }
            }
        }
    }
}
