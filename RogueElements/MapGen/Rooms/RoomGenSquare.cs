// <copyright file="RoomGenSquare.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace RogueElements
{
    /// <summary>
    /// Generates a rectangular room with the specified width and height.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class RoomGenSquare<T> : PermissiveRoomGen<T>, ISizedRoomGen
        where T : ITiledGenContext
    {
        public RoomGenSquare()
        {
        }

        public RoomGenSquare(RandRange width, RandRange height)
        {
            this.Width = width;
            this.Height = height;
        }

        protected RoomGenSquare(RoomGenSquare<T> other)
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

        public override RoomGen<T> Copy() => new RoomGenSquare<T>(this);

        public override Loc ProposeSize(IRandom rand)
        {
            return new Loc(this.Width.Pick(rand), this.Height.Pick(rand));
        }

        public override void DrawOnMap(T map)
        {
            this.DrawMapDefault(map);
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}x{2}", this.GetType().Name, this.Width.ToString(), this.Height.ToString());
        }
    }
}
