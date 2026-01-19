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
    /// <typeparam name="T">The type of map context that supports tiled generation.</typeparam>
    [Serializable]
    public class RoomGenSquare<T> : PermissiveRoomGen<T>, ISizedRoomGen
        where T : ITiledGenContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RoomGenSquare{T}"/> class.
        /// </summary>
        public RoomGenSquare()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoomGenSquare{T}"/> class with specified dimensions.
        /// </summary>
        /// <param name="width">The range of possible widths for the room.</param>
        /// <param name="height">The range of possible heights for the room.</param>
        public RoomGenSquare(RandRange width, RandRange height)
        {
            this.Width = width;
            this.Height = height;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoomGenSquare{T}"/> class as a copy of another.
        /// </summary>
        /// <param name="other">The instance to copy from.</param>
        protected RoomGenSquare(RoomGenSquare<T> other)
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
        public override RoomGen<T> Copy() => new RoomGenSquare<T>(this);

        /// <inheritdoc/>
        public override Loc ProposeSize(IRandom rand)
        {
            return new Loc(this.Width.Pick(rand), this.Height.Pick(rand));
        }

        /// <inheritdoc/>
        public override void DrawOnMap(T map)
        {
            this.DrawMapDefault(map);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return string.Format("{0}: {1}x{2}", this.GetType().GetFormattedTypeName(), this.Width.ToString(), this.Height.ToString());
        }
    }
}
