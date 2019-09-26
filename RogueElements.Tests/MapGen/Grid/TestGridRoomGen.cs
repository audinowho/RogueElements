// <copyright file="TestGridRoomGen.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RogueElements.Tests
{
    public class TestGridRoomGen : TestFloorRoomGen<IGridPathTestContext>
    {
        public TestGridRoomGen()
        {
        }

        public TestGridRoomGen(char id)
            : base(id)
        {
        }

        protected TestGridRoomGen(TestGridRoomGen other)
            : base(other)
        {
        }

        public override RoomGen<IGridPathTestContext> Copy() => new TestGridRoomGen(this);
    }
}