// <copyright file="RoomGenBump.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Generates a rectangular room with the specified width and height, and with the tiles at the perimeter randomly blocked.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class RoomGenBump<T> : PermissiveRoomGen<T>, ISizedRoomGen
        where T : ITiledGenContext
    {
        public RoomGenBump()
        {
        }

        public RoomGenBump(RandRange width, RandRange height, RandRange bumpPercent)
        {
            this.Width = width;
            this.Height = height;
            this.BumpPercent = bumpPercent;
        }

        protected RoomGenBump(RoomGenBump<T> other)
        {
            this.Width = other.Width;
            this.Height = other.Height;
            this.BumpPercent = other.BumpPercent;
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
        /// Chance of a block tile at the room's perimeter.
        /// </summary>
        public RandRange BumpPercent { get; set; }

        public override RoomGen<T> Copy() => new RoomGenBump<T>(this);

        public override Loc ProposeSize(IRandom rand)
        {
            return new Loc(this.Width.Pick(rand), this.Height.Pick(rand));
        }

        public override void DrawOnMap(T map)
        {
            // add peturbations
            if (this.Draw.Size.X > 2 && this.Draw.Size.Y > 2)
            {
                int tlTotal = (this.Draw.Size.X - 1) / 2 * this.BumpPercent.Pick(map.Rand) / 100;
                int blTotal = (this.Draw.Size.X - 1) / 2 * this.BumpPercent.Pick(map.Rand) / 100;
                int trTotal = (this.Draw.Size.X - 1) / 2 * this.BumpPercent.Pick(map.Rand) / 100;
                int brTotal = (this.Draw.Size.X - 1) / 2 * this.BumpPercent.Pick(map.Rand) / 100;

                int ltTotal = (this.Draw.Size.Y - 1) / 2 * this.BumpPercent.Pick(map.Rand) / 100;
                int rtTotal = (this.Draw.Size.Y - 1) / 2 * this.BumpPercent.Pick(map.Rand) / 100;
                int lbTotal = (this.Draw.Size.Y - 1) / 2 * this.BumpPercent.Pick(map.Rand) / 100;
                int rbTotal = (this.Draw.Size.Y - 1) / 2 * this.BumpPercent.Pick(map.Rand) / 100;

                for (int x = 0; x < this.Draw.Size.X; x++)
                {
                    for (int y = 0; y < this.Draw.Size.Y; y++)
                    {
                        if ((y != 0 || (x >= tlTotal && x < this.Draw.Size.X - trTotal)) &&
                            (y != this.Draw.Size.Y - 1 || (x >= blTotal && x < this.Draw.Size.X - brTotal)) &&
                            (x != 0 || (y >= ltTotal && y < this.Draw.Size.Y - lbTotal)) &&
                            (x != this.Draw.Size.X - 1 || (y >= rtTotal && y < this.Draw.Size.Y - rbTotal)))
                            map.SetTile(new Loc(this.Draw.X + x, this.Draw.Y + y), map.RoomTerrain.Copy());
                    }
                }
            }
            else
            {
                for (int x = 0; x < this.Draw.Size.X; x++)
                {
                    for (int y = 0; y < this.Draw.Size.Y; y++)
                        map.SetTile(new Loc(this.Draw.X + x, this.Draw.Y + y), map.RoomTerrain.Copy());
                }
            }

            // fulfill existing borders since this room doesn't cover the entire square
            this.FulfillRoomBorders(map, false);

            // hall restrictions
            this.SetRoomBorders(map);
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}x{2}", this.GetType().Name, this.Width, this.Height);
        }
    }
}
