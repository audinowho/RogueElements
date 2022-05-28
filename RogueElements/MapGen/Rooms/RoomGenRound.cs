// <copyright file="RoomGenRound.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace RogueElements
{
    /// <summary>
    /// Generates a rounded room.  Square dimensions result in a circle, while rectangular dimensions result in capsules.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class RoomGenRound<T> : RoomGen<T>, ISizedRoomGen
        where T : ITiledGenContext
    {
        public RoomGenRound()
        {
        }

        public RoomGenRound(RandRange width, RandRange height)
        {
            this.Width = width;
            this.Height = height;
        }

        protected RoomGenRound(RoomGenRound<T> other)
        {
            this.Width = other.Width;
            this.Height = other.Height;
        }

        /// <summary>
        /// Width of the room.
        /// </summary>
        public RandRange Width { get; set; }

        /// <summary>
        /// Height of the room.
        /// </summary>
        public RandRange Height { get; set; }

        public override RoomGen<T> Copy() => new RoomGenRound<T>(this);

        public override Loc ProposeSize(IRandom rand)
        {
            return new Loc(this.Width.Pick(rand), this.Height.Pick(rand));
        }

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

        public override string ToString()
        {
            return string.Format("{0}: {1}x{2}", this.GetType().Name, this.Width.ToString(), this.Height.ToString());
        }

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

        private static bool IsTileWithinRoom(int baseX, int baseY, int diameter, Loc size)
        {
            Loc sizeX2 = size * 2;
            int x = (baseX * 2) + 1;
            int y = (baseY * 2) + 1;

            if (x < diameter)
            {
                if (y < diameter)
                {
                    if (((diameter - x) * (diameter - x)) + ((diameter - y) * (diameter - y)) < diameter * diameter)
                        return true;
                }
                else if (y > sizeX2.Y - diameter)
                {
                    if (((diameter - x) * (diameter - x)) + ((y - (sizeX2.Y - diameter)) * (y - (sizeX2.Y - diameter))) < diameter * diameter)
                        return true;
                }
                else
                {
                    return true;
                }
            }
            else if (x > sizeX2.X - diameter)
            {
                if (y < diameter)
                {
                    if (((x - (sizeX2.X - diameter)) * (x - (sizeX2.X - diameter))) + ((diameter - y) * (diameter - y)) < diameter * diameter)
                        return true;
                }
                else if (y > sizeX2.Y - diameter)
                {
                    if (((x - (sizeX2.X - diameter)) * (x - (sizeX2.X - diameter))) + ((y - (sizeX2.Y - diameter)) * (y - (sizeX2.Y - diameter))) < diameter * diameter)
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
