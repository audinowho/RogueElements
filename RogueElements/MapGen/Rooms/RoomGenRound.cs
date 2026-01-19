// <copyright file="RoomGenRound.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace RogueElements
{
    /// <summary>
    /// Generates a rounded room. Square dimensions result in a circle, while rectangular dimensions result in capsules.
    /// </summary>
    /// <typeparam name="T">The type of map context that supports tiled generation.</typeparam>
    [Serializable]
    public class RoomGenRound<T> : RoomGen<T>, ISizedRoomGen
        where T : ITiledGenContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RoomGenRound{T}"/> class.
        /// </summary>
        public RoomGenRound()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoomGenRound{T}"/> class with specified dimensions.
        /// </summary>
        /// <param name="width">The range of possible widths for the room.</param>
        /// <param name="height">The range of possible heights for the room.</param>
        public RoomGenRound(RandRange width, RandRange height)
        {
            this.Width = width;
            this.Height = height;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoomGenRound{T}"/> class as a copy of another.
        /// </summary>
        /// <param name="other">The instance to copy from.</param>
        protected RoomGenRound(RoomGenRound<T> other)
        {
            this.Width = other.Width;
            this.Height = other.Height;
        }

        /// <summary>
        /// Gets or sets the range of possible widths for the room.
        /// </summary>
        public RandRange Width { get; set; }

        /// <summary>
        /// Gets or sets the range of possible heights for the room.
        /// </summary>
        public RandRange Height { get; set; }

        /// <inheritdoc/>
        public override RoomGen<T> Copy() => new RoomGenRound<T>(this);

        /// <inheritdoc/>
        public override Loc ProposeSize(IRandom rand)
        {
            return new Loc(this.Width.Pick(rand), this.Height.Pick(rand));
        }

        /// <inheritdoc/>
        public override void DrawOnMap(T map)
        {
            int diameter = Math.Min(this.Draw.Width, this.Draw.Height);

            for (int ii = 0; ii < this.Draw.Width; ii++)
            {
                for (int jj = 0; jj < this.Draw.Height; jj++)
                {
                    if (IsTileWithinRoom(ii, jj, diameter, this.Draw.Size))
                        map.SetTile(new Loc(this.Draw.X + ii, this.Draw.Y + jj), map.RoomTerrain.Copy());
                }
            }

            // hall restrictions
            this.SetRoomBorders(map);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return string.Format("{0}: {1}x{2}", this.GetType().GetFormattedTypeName(), this.Width.ToString(), this.Height.ToString());
        }

        /// <inheritdoc/>
        protected override void PrepareFulfillableBorders(IRandom rand)
        {
            int diameter = Math.Min(this.Draw.Width, this.Draw.Height);
            for (int jj = 0; jj < this.Draw.Width; jj++)
            {
                if (IsTileWithinRoom(jj, 0, diameter, this.Draw.Size))
                {
                    this.FulfillableBorder[Dir4.Up][jj] = true;
                    this.FulfillableBorder[Dir4.Down][jj] = true;
                }
            }

            for (int jj = 0; jj < this.Draw.Height; jj++)
            {
                if (IsTileWithinRoom(0, jj, diameter, this.Draw.Size))
                {
                    this.FulfillableBorder[Dir4.Left][jj] = true;
                    this.FulfillableBorder[Dir4.Right][jj] = true;
                }
            }
        }

        /// <summary>
        /// Determines whether a tile at the specified coordinates falls within the rounded room shape.
        /// </summary>
        /// <param name="baseX">The X coordinate of the tile relative to the room.</param>
        /// <param name="baseY">The Y coordinate of the tile relative to the room.</param>
        /// <param name="diameter">The diameter used for corner rounding.</param>
        /// <param name="size">The size of the room.</param>
        /// <returns>True if the tile is within the room; otherwise, false.</returns>
        private static bool IsTileWithinRoom(int baseX, int baseY, int diameter, Loc size)
        {
            Loc sizeX2 = size * 2;
            int x = (baseX * 2) + 1;
            int y = (baseY * 2) + 1;

            if (x < diameter)
            {
                int xdiff = diameter - x;
                if (y < diameter)
                {
                    int ydiff = diameter - y;
                    if ((xdiff * xdiff) + (ydiff * ydiff) < diameter * diameter)
                        return true;
                }
                else if (y > sizeX2.Y - diameter)
                {
                    int ydiff = y - (sizeX2.Y - diameter);
                    if ((xdiff * xdiff) + (ydiff * ydiff) < diameter * diameter)
                        return true;
                }
                else
                {
                    return true;
                }
            }
            else if (x > sizeX2.X - diameter)
            {
                int xdiff = x - (sizeX2.X - diameter);
                if (y < diameter)
                {
                    int ydiff = diameter - y;
                    if ((xdiff * xdiff) + (ydiff * ydiff) < diameter * diameter)
                        return true;
                }
                else if (y > sizeX2.Y - diameter)
                {
                    int ydiff = y - (sizeX2.Y - diameter);
                    if ((xdiff * xdiff) + (ydiff * ydiff) < diameter * diameter)
                        return true;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return true;
            }

            return false;
        }
    }
}
