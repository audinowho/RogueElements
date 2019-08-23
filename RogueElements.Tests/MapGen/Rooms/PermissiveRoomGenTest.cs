// <copyright file="PermissiveRoomGenTest.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;

namespace RogueElements.Tests
{
    [TestFixture]
    public class PermissiveRoomGenTest
    {
        [Test]
        public void PrepareSize()
        {
            // verify all fulfillableborders set to true
            // as well as bordertofulfill set to correct sizes
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            var roomGen = new TestPermissiveRoomGen<ITiledGenContext>();
            roomGen.PrepareSize(testRand.Object, new Loc(2, 3));

            var expectedFulfillable = new Dictionary<Dir4, bool[]>
            {
                [Dir4.Down] = new bool[] { true, true },
                [Dir4.Left] = new bool[] { true, true, true },
                [Dir4.Up] = new bool[] { true, true },
                [Dir4.Right] = new bool[] { true, true, true },
            };

            var expectedToFulfill = new Dictionary<Dir4, bool[]>
            {
                [Dir4.Down] = new bool[] { false, false },
                [Dir4.Left] = new bool[] { false, false, false },
                [Dir4.Up] = new bool[] { false, false },
                [Dir4.Right] = new bool[] { false, false, false },
            };

            Assert.That(roomGen.PublicFulfillableBorder, Is.EqualTo(expectedFulfillable));
            Assert.That(roomGen.PublicBorderToFulfill, Is.EqualTo(expectedToFulfill));
        }

        public class TestPermissiveRoomGen<T> : PermissiveRoomGen<T>
            where T : ITiledGenContext
        {
            public Dictionary<Dir4, bool[]> PublicFulfillableBorder => this.FulfillableBorder;

            public Dictionary<Dir4, bool[]> PublicOpenedBorder => this.OpenedBorder;

            public Dictionary<Dir4, bool[]> PublicBorderToFulfill => this.BorderToFulfill;

            public override RoomGen<T> Copy() => new TestPermissiveRoomGen<T>();

            public override Loc ProposeSize(IRandom rand) => Loc.Zero;

            public override void DrawOnMap(T map)
            {
            }

            public void PublicPrepareFulfillableBorders(IRandom rand)
            {
                this.PrepareFulfillableBorders(rand);
            }
        }
    }
}
