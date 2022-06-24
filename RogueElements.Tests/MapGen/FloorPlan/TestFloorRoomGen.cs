// <copyright file="TestFloorRoomGen.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace RogueElements.Tests
{
    public class TestFloorRoomGen<T> : PermissiveRoomGen<T>
        where T : ITiledGenContext
    {
        public TestFloorRoomGen()
        {
        }

        public TestFloorRoomGen(char id)
        {
            this.Identifier = id;
        }

        protected TestFloorRoomGen(TestFloorRoomGen<T> other)
        {
            this.Identifier = other.Identifier;
            this.ProposedSize = other.ProposedSize;
        }

        public char Identifier { get; set; }

        public Loc ProposedSize { get; set; }

        public override RoomGen<T> Copy() => new TestFloorRoomGen<T>(this);

        public override Loc ProposeSize(IRandom rand)
        {
            return this.ProposedSize;
        }

        public override void DrawOnMap(T map)
        {
            this.DrawMapDefault(map);
        }

        public void PrepareDraw(Rect rect)
        {
            this.Draw = rect;
            foreach (Dir4 dir in DirExt.VALID_DIR4)
            {
                this.OpenedBorder[dir] = new bool[this.Draw.GetBorderLength(dir)];
                this.FulfillableBorder[dir] = new bool[this.Draw.GetBorderLength(dir)];
                for (int jj = 0; jj < this.FulfillableBorder[dir].Length; jj++)
                    this.FulfillableBorder[dir][jj] = true;
            }
        }

        public void PrepareFulfillableBorder(Dir4 dir, bool[] fulfillable)
        {
            if (fulfillable.Length != this.FulfillableBorder[dir].Length)
                throw new ArgumentException("Incorrect border length.");
            for (int jj = 0; jj < fulfillable.Length; jj++)
                this.FulfillableBorder[dir][jj] = fulfillable[jj];
        }

        public override bool Equals(object obj)
        {
            if (obj is TestFloorRoomGen<T> roomGen)
                return roomGen.Identifier == this.Identifier && roomGen.Draw == this.Draw;
            return false;
        }

        public override int GetHashCode() => this.Identifier;
    }
}