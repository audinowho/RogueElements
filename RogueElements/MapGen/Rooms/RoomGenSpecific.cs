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
    /// Allows defining a precise tile layout for the room.
    /// Note: This class is not editor-friendly due to direct tile array manipulation.
    /// </summary>
    /// <typeparam name="T">The type of map context that supports tiled generation.</typeparam>
    [Serializable]
    public class RoomGenSpecific<T> : RoomGen<T>
        where T : ITiledGenContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RoomGenSpecific{T}"/> class.
        /// </summary>
        public RoomGenSpecific()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoomGenSpecific{T}"/> class with specified dimensions.
        /// </summary>
        /// <param name="width">The width of the room in tiles.</param>
        /// <param name="height">The height of the room in tiles.</param>
        public RoomGenSpecific(int width, int height)
        {
            this.Tiles = new ITile[width][];
            for (int xx = 0; xx < width; xx++)
                this.Tiles[xx] = new ITile[height];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoomGenSpecific{T}"/> class with specified dimensions and terrain.
        /// </summary>
        /// <param name="width">The width of the room in tiles.</param>
        /// <param name="height">The height of the room in tiles.</param>
        /// <param name="roomTerrain">The terrain type that represents walkable floor tiles.</param>
        public RoomGenSpecific(int width, int height, ITile roomTerrain)
            : this(width, height)
        {
            this.RoomTerrain = roomTerrain;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoomGenSpecific{T}"/> class as a copy of another.
        /// </summary>
        /// <param name="other">The instance to copy from.</param>
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

        /// <summary>
        /// Gets or sets the terrain type that represents walkable floor tiles.
        /// Used to determine which tiles are open for border connections.
        /// </summary>
        public ITile RoomTerrain { get; set; }

        /// <summary>
        /// Gets or sets the 2D array of tiles that define the room layout.
        /// </summary>
        public ITile[][] Tiles { get; set; }

        /// <inheritdoc/>
        public override RoomGen<T> Copy() => new RoomGenSpecific<T>(this);

        /// <inheritdoc/>
        public override Loc ProposeSize(IRandom rand)
        {
            return new Loc(this.Tiles.Length, this.Tiles[0].Length);
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public override string ToString()
        {
            return string.Format("{0}: {1}x{2}", this.GetType().GetFormattedTypeName(), this.Tiles.Length, this.Tiles[0].Length);
        }

        /// <inheritdoc/>
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
