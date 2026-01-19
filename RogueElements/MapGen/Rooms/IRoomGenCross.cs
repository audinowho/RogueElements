// <copyright file="IRoomGenCross.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Defines a room generator that creates cross-shaped rooms.
    /// The room is composed of two intersecting rectangles: one horizontal (major width x minor height)
    /// and one vertical (minor width x major height).
    /// </summary>
    public interface IRoomGenCross : IRoomGen
    {
        /// <summary>
        /// Gets or sets the range of possible widths for the horizontal rectangle.
        /// </summary>
        RandRange MajorWidth { get; set; }

        /// <summary>
        /// Gets or sets the range of possible heights for the vertical rectangle.
        /// </summary>
        RandRange MajorHeight { get; set; }

        /// <summary>
        /// Gets or sets the range of possible heights for the horizontal rectangle.
        /// </summary>
        RandRange MinorHeight { get; set; }

        /// <summary>
        /// Gets or sets the range of possible widths for the vertical rectangle.
        /// </summary>
        RandRange MinorWidth { get; set; }
    }

    /// <summary>
    /// Generates a room composed of two rectangles, one vertical and one horizontal.
    /// </summary>
    /// <typeparam name="T">The type of map context that supports tiled generation.</typeparam>
    [Serializable]
    public class RoomGenCross<T> : RoomGen<T>, IRoomGenCross
        where T : ITiledGenContext
    {
        [NonSerialized]
        private int chosenMinorWidth;

        [NonSerialized]
        private int chosenMinorHeight;

        [NonSerialized]
        private int chosenOffsetX;

        [NonSerialized]
        private int chosenOffsetY;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoomGenCross{T}"/> class.
        /// </summary>
        public RoomGenCross()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoomGenCross{T}"/> class with specified dimensions.
        /// </summary>
        /// <param name="majorWidth">The width range of the horizontal rectangle.</param>
        /// <param name="majorHeight">The height range of the vertical rectangle.</param>
        /// <param name="minorHeight">The height range of the horizontal rectangle.</param>
        /// <param name="minorWidth">The width range of the vertical rectangle.</param>
        public RoomGenCross(RandRange majorWidth, RandRange majorHeight, RandRange minorHeight, RandRange minorWidth)
        {
            this.MajorWidth = majorWidth;
            this.MajorHeight = majorHeight;
            this.MinorWidth = minorWidth;
            this.MinorHeight = minorHeight;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoomGenCross{T}"/> class as a copy of another.
        /// </summary>
        /// <param name="other">The instance to copy from.</param>
        protected RoomGenCross(RoomGenCross<T> other)
        {
            this.MajorWidth = other.MajorWidth;
            this.MajorHeight = other.MajorHeight;
            this.MinorWidth = other.MinorWidth;
            this.MinorHeight = other.MinorHeight;
        }

        /// <summary>
        /// Gets or sets the width of the horizontal rectangle.
        /// </summary>
        public RandRange MajorWidth { get; set; }

        /// <summary>
        /// Gets or sets the height of the horizontal rectangle.
        /// </summary>
        public RandRange MinorHeight { get; set; }

        /// <summary>
        /// Gets or sets the height of the vertical rectangle.
        /// </summary>
        public RandRange MajorHeight { get; set; }

        /// <summary>
        /// Gets or sets the width of the vertical rectangle.
        /// </summary>
        public RandRange MinorWidth { get; set; }

        /// <summary>
        /// Gets or sets the chosen width of the vertical rectangle after size preparation.
        /// </summary>
        protected int ChosenMinorWidth { get => this.chosenMinorWidth; set => this.chosenMinorWidth = value; }

        /// <summary>
        /// Gets or sets the chosen height of the horizontal rectangle after size preparation.
        /// </summary>
        protected int ChosenMinorHeight { get => this.chosenMinorHeight; set => this.chosenMinorHeight = value; }

        /// <summary>
        /// Gets or sets the X offset of the vertical rectangle within the bounding box.
        /// </summary>
        protected int ChosenOffsetX { get => this.chosenOffsetX; set => this.chosenOffsetX = value; }

        /// <summary>
        /// Gets or sets the Y offset of the horizontal rectangle within the bounding box.
        /// </summary>
        protected int ChosenOffsetY { get => this.chosenOffsetY; set => this.chosenOffsetY = value; }

        /// <inheritdoc/>
        public override RoomGen<T> Copy() => new RoomGenCross<T>(this);

        /// <inheritdoc/>
        public override Loc ProposeSize(IRandom rand)
        {
            return new Loc(this.MajorWidth.Pick(rand), this.MajorHeight.Pick(rand));
        }

        /// <inheritdoc/>
        public override void DrawOnMap(T map)
        {
            Loc size1 = new Loc(this.Draw.Width, this.ChosenMinorHeight);
            Loc size2 = new Loc(this.ChosenMinorWidth, this.Draw.Height);

            Loc start1 = new Loc(this.Draw.X, this.Draw.Y + this.ChosenOffsetY);
            Loc start2 = new Loc(this.Draw.X + this.ChosenOffsetX, this.Draw.Y);

            for (int x = 0; x < size1.X; x++)
            {
                for (int y = 0; y < size1.Y; y++)
                    map.SetTile(new Loc(start1.X + x, start1.Y + y), map.RoomTerrain.Copy());
            }

            GenContextDebug.DebugProgress("First Rect");
            for (int x = 0; x < size2.X; x++)
            {
                for (int y = 0; y < size2.Y; y++)
                    map.SetTile(new Loc(start2.X + x, start2.Y + y), map.RoomTerrain.Copy());
            }

            GenContextDebug.DebugProgress("Second Rect");

            // hall restrictions
            this.SetRoomBorders(map);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return string.Format("{0}: {1}x{2}+{3}x{4}", this.GetType().GetFormattedTypeName(), this.MajorWidth, this.MinorHeight, this.MinorWidth, this.MajorHeight);
        }

        /// <inheritdoc/>
        protected override void PrepareFulfillableBorders(IRandom rand)
        {
            this.ChosenMinorWidth = Math.Min(this.Draw.Width, this.MinorWidth.Pick(rand));
            this.ChosenMinorHeight = Math.Min(this.Draw.Height, this.MinorHeight.Pick(rand));

            this.ChosenOffsetX = rand.Next(this.Draw.Width - this.ChosenMinorWidth + 1);
            this.ChosenOffsetY = rand.Next(this.Draw.Height - this.ChosenMinorHeight + 1);

            for (int jj = this.ChosenOffsetX; jj < this.ChosenOffsetX + this.ChosenMinorWidth; jj++)
            {
                this.FulfillableBorder[Dir4.Up][jj] = true;
                this.FulfillableBorder[Dir4.Down][jj] = true;
            }

            for (int jj = this.ChosenOffsetY; jj < this.ChosenOffsetY + this.ChosenMinorHeight; jj++)
            {
                this.FulfillableBorder[Dir4.Left][jj] = true;
                this.FulfillableBorder[Dir4.Right][jj] = true;
            }
        }
    }
}
