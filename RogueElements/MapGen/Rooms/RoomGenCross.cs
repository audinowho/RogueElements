// <copyright file="RoomGenCross.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    [Serializable]
    public class RoomGenCross<T> : RoomGen<T>
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

        public RoomGenCross()
        {
        }

        public RoomGenCross(RandRange majorWidth, RandRange majorHeight, RandRange minorHeight, RandRange minorWidth)
        {
            this.MajorWidth = majorWidth;
            this.MajorHeight = majorHeight;
            this.MinorWidth = minorWidth;
            this.MinorHeight = minorHeight;
        }

        protected RoomGenCross(RoomGenCross<T> other)
        {
            this.MajorWidth = other.MajorWidth;
            this.MajorHeight = other.MajorHeight;
            this.MinorWidth = other.MinorWidth;
            this.MinorHeight = other.MinorHeight;
        }

        public RandRange MajorWidth { get; set; }

        public RandRange MajorHeight { get; set; }

        public RandRange MinorHeight { get; set; }

        public RandRange MinorWidth { get; set; }

        protected int ChosenMinorWidth { get => this.chosenMinorWidth; set => this.chosenMinorWidth = value; }

        protected int ChosenMinorHeight { get => this.chosenMinorHeight; set => this.chosenMinorHeight = value; }

        protected int ChosenOffsetX { get => this.chosenOffsetX; set => this.chosenOffsetX = value; }

        protected int ChosenOffsetY { get => this.chosenOffsetY; set => this.chosenOffsetY = value; }

        public override RoomGen<T> Copy() => new RoomGenCross<T>(this);

        public override Loc ProposeSize(IRandom rand)
        {
            return new Loc(this.MajorWidth.Pick(rand), this.MajorHeight.Pick(rand));
        }

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
