// <copyright file="AddRoomTest.cs" company="Audino">
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
    public class AddRoomTest
    {
        [Test]
        [Ignore("TODO")]
        public void AddConnected()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void AddDisconnectedEarlyBorder()
        {
            // confirm the top left corner can be used
            TestFloorPlan floorPlan;
            {
                floorPlan = TestFloorPlan.InitFloorToContext(
                    new Loc(20, 20),
                    new Rect[] { },
                    new Rect[] { },
                    new Tuple<char, char>[] { });
            }

            TestFloorPlan compareFloorPlan;
            {
                compareFloorPlan = TestFloorPlan.InitFloorToContext(
                    new Loc(20, 20),
                    new Rect[] { new Rect(0, 0, 4, 4) },
                    new Rect[] { },
                    new Tuple<char, char>[] { });
            }

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(1, 1)).Returns(1);
            testRand.Setup(p => p.Next(0, 17)).Returns(0);

            var roomGen = new TestFloorPlanGen('A') { ProposedSize = new Loc(4, 4) };
            var mockRooms = new Mock<IRandPicker<RoomGen<IFloorPlanTestContext>>>(MockBehavior.Strict);
            mockRooms.Setup(p => p.Pick(testRand.Object)).Returns(roomGen);

            var pathGen = new AddDisconnectedRoomsStep<IFloorPlanTestContext>(mockRooms.Object) { Amount = new RandRange(1) };
            pathGen.ApplyToPath(testRand.Object, floorPlan);

            TestFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);
        }

        [Test]
        public void AddDisconnectedLateBorder()
        {
            // confirm bottom right corner can be used
            TestFloorPlan floorPlan;
            {
                floorPlan = TestFloorPlan.InitFloorToContext(
                    new Loc(20, 20),
                    new Rect[] { },
                    new Rect[] { },
                    new Tuple<char, char>[] { });
            }

            TestFloorPlan compareFloorPlan;
            {
                compareFloorPlan = TestFloorPlan.InitFloorToContext(
                    new Loc(20, 20),
                    new Rect[] { new Rect(16, 16, 4, 4) },
                    new Rect[] { },
                    new Tuple<char, char>[] { });
            }

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(1, 1)).Returns(1);
            testRand.Setup(p => p.Next(0, 17)).Returns(16);

            var roomGen = new TestFloorPlanGen('A') { ProposedSize = new Loc(4, 4) };
            var mockRooms = new Mock<IRandPicker<RoomGen<IFloorPlanTestContext>>>(MockBehavior.Strict);
            mockRooms.Setup(p => p.Pick(testRand.Object)).Returns(roomGen);

            var pathGen = new AddDisconnectedRoomsStep<IFloorPlanTestContext>(mockRooms.Object) { Amount = new RandRange(1) };
            pathGen.ApplyToPath(testRand.Object, floorPlan);

            TestFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);
        }

        [Test]
        public void AddDisconnectedNoCollide()
        {
            // fail via collision
            TestFloorPlan floorPlan;
            {
                floorPlan = TestFloorPlan.InitFloorToContext(
                    new Loc(20, 20),
                    new Rect[] { new Rect(4, 4, 16, 16) },
                    new Rect[] { },
                    new Tuple<char, char>[] { });
            }

            TestFloorPlan compareFloorPlan;
            {
                compareFloorPlan = TestFloorPlan.InitFloorToContext(
                    new Loc(20, 20),
                    new Rect[] { new Rect(4, 4, 16, 16), new Rect(0, 0, 3, 3) },
                    new Rect[] { },
                    new Tuple<char, char>[] { });
            }

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(1, 1)).Returns(1);
            testRand.Setup(p => p.Next(0, 18)).Returns(0);

            var roomGen = new TestFloorPlanGen('B') { ProposedSize = new Loc(3, 3) };
            var mockRooms = new Mock<IRandPicker<RoomGen<IFloorPlanTestContext>>>(MockBehavior.Strict);
            mockRooms.Setup(p => p.Pick(testRand.Object)).Returns(roomGen);

            var pathGen = new AddDisconnectedRoomsStep<IFloorPlanTestContext>(mockRooms.Object) { Amount = new RandRange(1) };
            pathGen.ApplyToPath(testRand.Object, floorPlan);

            TestFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);
        }

        [Test]
        public void AddDisconnectedCollide()
        {
            // fail via collision
            TestFloorPlan floorPlan;
            {
                floorPlan = TestFloorPlan.InitFloorToContext(
                    new Loc(20, 20),
                    new Rect[] { new Rect(4, 4, 16, 16) },
                    new Rect[] { },
                    new Tuple<char, char>[] { });
            }

            TestFloorPlan compareFloorPlan;
            {
                compareFloorPlan = TestFloorPlan.InitFloorToContext(
                    new Loc(20, 20),
                    new Rect[] { new Rect(4, 4, 16, 16) },
                    new Rect[] { },
                    new Tuple<char, char>[] { });
            }

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(1, 1)).Returns(1);
            testRand.Setup(p => p.Next(0, 17)).Returns(0);

            var roomGen = new TestFloorPlanGen('B') { ProposedSize = new Loc(4, 4) };
            var mockRooms = new Mock<IRandPicker<RoomGen<IFloorPlanTestContext>>>(MockBehavior.Strict);
            mockRooms.Setup(p => p.Pick(testRand.Object)).Returns(roomGen);

            var pathGen = new AddDisconnectedRoomsStep<IFloorPlanTestContext>(mockRooms.Object) { Amount = new RandRange(1) };
            pathGen.ApplyToPath(testRand.Object, floorPlan);

            TestFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);
        }
    }
}
