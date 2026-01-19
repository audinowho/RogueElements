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
    /// <typeparam name="T">The type of map context that supports tiled generation.</typeparam>
    [Serializable]
    public class RoomGenBlocked<T> : PermissiveRoomGen<T>, ISizedRoomGen
        where T : ITiledGenContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RoomGenBlocked{T}"/> class.
        /// </summary>
        public RoomGenBlocked()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoomGenBlocked{T}"/> class with specified dimensions.
        /// </summary>
        /// <param name="blockTerrain">The terrain type to use for the blocking rectangle.</param>
        /// <param name="width">The range of possible widths for the room.</param>
        /// <param name="height">The range of possible heights for the room.</param>
        /// <param name="blockWidth">The range of possible widths for the block.</param>
        /// <param name="blockHeight">The range of possible heights for the block.</param>
        public RoomGenBlocked(ITile blockTerrain, RandRange width, RandRange height, RandRange blockWidth, RandRange blockHeight)
        {
            this.BlockTerrain = blockTerrain;
            this.Width = width;
            this.Height = height;
            this.BlockWidth = blockWidth;
            this.BlockHeight = blockHeight;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoomGenBlocked{T}"/> class as a copy of another.
        /// </summary>
        /// <param name="other">The instance to copy from.</param>
        protected RoomGenBlocked(RoomGenBlocked<T> other)
        {
            this.BlockTerrain = other.BlockTerrain.Copy();
            this.Width = other.Width;
            this.Height = other.Height;
            this.BlockWidth = other.BlockWidth;
            this.BlockHeight = other.BlockHeight;
        }

        /// <summary>
        /// Gets or sets the range of possible widths for the room.
        /// </summary>
        public RandRange Width { get; set; }

        /// <summary>
        /// Gets or sets the range of possible heights for the room.
        /// </summary>
        public RandRange Height { get; set; }

        /// <summary>
        /// Gets or sets the range of possible widths for the block.
        /// </summary>
        public RandRange BlockWidth { get; set; }

        /// <summary>
        /// Gets or sets the range of possible heights for the block.
        /// </summary>
        public RandRange BlockHeight { get; set; }

        /// <summary>
        /// Gets or sets the terrain used for the blocking rectangle in the center.
        /// </summary>
        public ITile BlockTerrain { get; set; }

        /// <inheritdoc/>
        public override RoomGen<T> Copy() => new RoomGenBlocked<T>(this);

        /// <inheritdoc/>
        public override Loc ProposeSize(IRandom rand)
        {
            return new Loc(this.Width.Pick(rand), this.Height.Pick(rand));
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public override string ToString()
        {
            return string.Format("{0}: {1}x{2}", this.GetType().GetFormattedTypeName(), this.Width, this.Height);
        }
    }
}
