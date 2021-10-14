// <copyright file="SetGridSpecialRoomTest.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;

namespace RogueElements.Tests
{
    // place on a floor with two normal rooms
    [TestFixture]
    public class SetGridSpecialRoomTest
    {
        [Test]
        [TestCase(0, 0)]
        [TestCase(1, 2)]
        [TestCase(2, 4)]
        public void PlaceRoom(int roll, int expectedChosen)
        {
            // verify rand is working
            // place on a floor where the first room is immutable
            // place on a floor where the first room is default
            string[] inGrid =
            {
                "0.0.0.0.0.0.0",
                ". . . . . . .",
                "0.A#B#C#D#E.0",
                ". . . . . . .",
                "0.0.0.0.0.0.0",
            };

            TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid);
            GridRoomPlan roomPlan = floorPlan.GetRoomPlan(1);
            roomPlan.Components.Set(new TestComponent());
            roomPlan = floorPlan.GetRoomPlan(3);
            roomPlan.RoomGen = new RoomGenDefault<IGridPathTestContext>();
            roomPlan.PreferHall = true;

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);

            // The roll for size
            testRand.Setup(p => p.Next(0, 0)).Returns(0);

            // The roll for choosing room index
            testRand.Setup(p => p.Next(3)).Returns(roll);

            var pathGen = new SetGridSpecialRoomStep<IGridPathTestContext>
            {
                Rooms = new PresetPicker<RoomGen<IGridPathTestContext>>(new RoomGenSquare<IGridPathTestContext>()),
            };
            pathGen.Filters.Add(new RoomFilterComponent(true, new TestComponent()));

            pathGen.ApplyToPath(testRand.Object, floorPlan);

            // check the rooms
            Assert.That(floorPlan.RoomCount, Is.EqualTo(5));
            for (int ii = 0; ii < 5; ii++)
            {
                if (ii == expectedChosen)
                    Assert.That(floorPlan.GetRoomPlan(ii).RoomGen, Is.TypeOf<RoomGenSquare<IGridPathTestContext>>());
                else
                    Assert.That(floorPlan.GetRoomPlan(ii).RoomGen, Is.Not.TypeOf<RoomGenSquare<IGridPathTestContext>>());
            }
        }

        [Test]
        public void PlaceRoomImpossible()
        {
            // verify rand is working
            // place on a floor where the first room is immutable
            // place on a floor where the first room is default
            string[] inGrid =
            {
                "0.0.0.0.0",
                ". . . . .",
                "0.A#B#C.0",
                ". . . . .",
                "0.0.0.0.0",
            };

            TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid);
            GridRoomPlan roomPlan = floorPlan.GetRoomPlan(0);
            roomPlan.Components.Set(new TestComponent());
            roomPlan = floorPlan.GetRoomPlan(1);
            roomPlan.Components.Set(new TestComponent());
            roomPlan = floorPlan.GetRoomPlan(2);
            roomPlan.Components.Set(new TestComponent());

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);

            // The roll for size
            testRand.Setup(p => p.Next(0, 0)).Returns(0);

            var pathGen = new SetGridSpecialRoomStep<IGridPathTestContext>
            {
                Rooms = new PresetPicker<RoomGen<IGridPathTestContext>>(new RoomGenSquare<IGridPathTestContext>()),
            };
            pathGen.Filters.Add(new RoomFilterComponent(true, new TestComponent()));

            pathGen.ApplyToPath(testRand.Object, floorPlan);

            // check the rooms
            Assert.That(floorPlan.RoomCount, Is.EqualTo(3));
            for (int ii = 0; ii < 3; ii++)
            {
                Assert.That(floorPlan.GetRoomPlan(ii).RoomGen, Is.TypeOf<TestGridRoomGen>());
            }
        }
    }
}
