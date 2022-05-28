// <copyright file="RoomGenBlocked.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Generates a rectangular room with the specified width and height, and with a rectangular block with specified width and height.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class RoomGenBlocked<T> : PermissiveRoomGen<T>, ISizedRoomGen
        where T : ITiledGenContext
    {
        public RoomGenBlocked()
        {
        }

        public RoomGenBlocked(ITile blockTerrain, RandRange width, RandRange height, RandRange blockWidth, RandRange blockHeight)
        {
            this.BlockTerrain = blockTerrain;
            this.Width = width;
            this.Height = height;
            this.BlockWidth = blockWidth;
            this.BlockHeight = blockHeight;
        }

        protected RoomGenBlocked(RoomGenBlocked<T> other)
        {
            this.BlockTerrain = other.BlockTerrain.Copy();
            this.Width = other.Width;
            this.Height = other.Height;
            this.BlockWidth = other.BlockWidth;
            this.BlockHeight = other.BlockHeight;
        }

        /// <summary>
        /// Width of the room.
        /// </summary>
        public RandRange Width { get; set; }

        /// <summary>
        /// Height of the room.
        /// </summary>
        public RandRange Height { get; set; }

        /// <summary>
        /// Width of the block.
        /// </summary>
        public RandRange BlockWidth { get; set; }

        /// <summary>
        /// Height of the block.
        /// </summary>
        public RandRange BlockHeight { get; set; }

        /// <summary>
        /// The terrain used for the block.
        /// </summary>
        public ITile BlockTerrain { get; set; }

        public override RoomGen<T> Copy() => new RoomGenBlocked<T>(this);

        public override Loc ProposeSize(IRandom rand)
        {
            return new Loc(this.Width.Pick(rand), this.Height.Pick(rand));
        }

        public override void DrawOnMap(T map)
        {
            for (int x = 0; x < this.Draw.Size.X; x++)
            {
                for (int y = 0; y < this.Draw.Size.Y; y++)
                    map.SetTile(new Loc(this.Draw.X + x, this.Draw.Y + y), map.RoomTerrain.Copy());
            }

            GenContextDebug.DebugProgress("Room Rect");

            Loc blockSize = new Loc(Math.Min(this.BlockWidth.Pick(map.Rand), this.Draw.Size.X - 2), Math.Min(this.BlockHeight.Pick(map.Rand), this.Draw.Size.Y - 2));
            Loc blockStart = new Loc(this.Draw.X + map.Rand.Next(1, this.Draw.Size.X - blockSize.X - 1), this.Draw.Y + map.Rand.Next(1, this.Draw.Size.Y - blockSize.Y - 1));
            for (int x = 0; x < blockSize.X; x++)
            {
                for (int y = 0; y < blockSize.Y; y++)
                    map.SetTile(new Loc(blockStart.X + x, blockStart.Y + y), this.BlockTerrain.Copy());
            }

            GenContextDebug.DebugProgress("Block Rect");

            // hall restrictions
            this.SetRoomBorders(map);
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}x{2}", this.GetType().Name, this.Width, this.Height);
        }
    }
}
