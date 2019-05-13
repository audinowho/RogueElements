using System;
using System.Collections.Generic;
using NUnit.Framework;
using Moq;

namespace RogueElements.Tests
{
    [TestFixture]
    public class AddGridSpecialRoomTest
    {
        //place on a floor with two normal rooms

        [Test]
        [TestCase(0, 0)]
        [TestCase(1, 2)]
        [TestCase(2, 4)]
        public void PlaceRoom(int roll, int expectedChosen)
        {
            //verify rand is working
            //place on a floor where the first room is immutable
            //place on a floor where the first room is default
            string[] inGrid = { "0.0.0.0.0.0.0",
                                ". . . . . . .",
                                "0.A#B#C#D#E.0",
                                ". . . . . . .",
                                "0.0.0.0.0.0.0"};


            TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid);
            GridRoomPlan roomPlan = floorPlan.GetRoomPlan(1);
            roomPlan.Immutable = true;
            roomPlan = floorPlan.GetRoomPlan(3);
            roomPlan.RoomGen = new RoomGenDefault<IGridPathTestContext>();
            roomPlan.PreferHall = true;

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(3)).Returns(roll);

            var pathGen = new AddGridSpecialRoomStep<IGridPathTestContext>
            {
                Rooms = new PresetPicker<RoomGen<IGridPathTestContext>>(new RoomGenSquare<IGridPathTestContext>())
            };

            pathGen.ApplyToPath(testRand.Object, floorPlan);

            //check the rooms
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
            //verify rand is working
            //place on a floor where the first room is immutable
            //place on a floor where the first room is default
            string[] inGrid = { "0.0.0.0.0",
                                ". . . . .",
                                "0.A#B#C.0",
                                ". . . . .",
                                "0.0.0.0.0"};


            TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid);
            GridRoomPlan roomPlan = floorPlan.GetRoomPlan(0);
            roomPlan.Immutable = true;
            roomPlan = floorPlan.GetRoomPlan(1);
            roomPlan.Immutable = true;
            roomPlan = floorPlan.GetRoomPlan(2);
            roomPlan.Immutable = true;

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);

            var pathGen = new AddGridSpecialRoomStep<IGridPathTestContext>
            {
                Rooms = new PresetPicker<RoomGen<IGridPathTestContext>>(new RoomGenSquare<IGridPathTestContext>())
            };

            pathGen.ApplyToPath(testRand.Object, floorPlan);

            //check the rooms
            Assert.That(floorPlan.RoomCount, Is.EqualTo(3));
            for (int ii = 0; ii < 3; ii++)
            {
                Assert.That(floorPlan.GetRoomPlan(ii).RoomGen, Is.TypeOf<TestGridRoomGen>());
            }
        }
    }
}
