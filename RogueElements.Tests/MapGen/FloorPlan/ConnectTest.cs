// <copyright file="ConnectTest.cs" company="Audino">
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
    public class ConnectTest
    {
        [Test]
        public void GetPossibleExpansionsOne()
        {
            // two isolated rooms
            TestFloorPlan floorPlan = TestFloorPlan.InitFloorToContext(
                new Loc(22, 14),
                new Rect[] { new Rect(3, 3, 2, 2), new Rect(3, 9, 2, 2) },
                Array.Empty<Rect>(),
                Array.Empty<Tuple<char, char>>());

            var candList = new List<RoomHallIndex> { new RoomHallIndex(0, false) };
            var nodes = ConnectTestStep.GetPossibleExpansions(floorPlan, candList);

            Assert.That(nodes.Count, Is.EqualTo(1));
            Assert.That(nodes.GetSpawn(0).From, Is.EqualTo(new RoomHallIndex(0, false)));
            Assert.That(nodes.GetSpawn(0).To, Is.EqualTo(new RoomHallIndex(1, false)));
            Assert.That(nodes.GetSpawn(0).Connector, Is.EqualTo(new Rect(3, 5, 2, 4)));
        }

        [Test]
        public void GetPossibleExpansionsWeighted()
        {
            /* A F
               B E
               C-DG */
            TestFloorPlan floorPlan;
            {
                Rect[] rooms = new Rect[]
                {
                    new Rect(3, 1, 2, 2),
                    new Rect(3, 3, 2, 2),
                    new Rect(3, 5, 2, 3),
                    new Rect(7, 5, 2, 3),
                    new Rect(7, 3, 2, 2),
                    new Rect(7, 1, 2, 2),
                    new Rect(9, 6, 2, 2),
                };
                var links = new Tuple<char, char>[]
                {
                    Tuple.Create('A', 'B'),
                    Tuple.Create('B', 'C'),
                    Tuple.Create('C', 'a'),
                    Tuple.Create('a', 'D'),
                    Tuple.Create('D', 'E'),
                    Tuple.Create('E', 'F'),
                    Tuple.Create('D', 'G'),
                };
                floorPlan = TestFloorPlan.InitFloorToContext(
                    new Loc(22, 14),
                    rooms,
                    new Rect[] { new Rect(5, 6, 2, 2) },
                    links);
            }

            var candList = new List<RoomHallIndex>
            {
                new RoomHallIndex(0, false),
                new RoomHallIndex(1, false),
                new RoomHallIndex(2, false),
            };

            var nodes = ConnectTestStep.GetPossibleExpansions(floorPlan, candList);

            Assert.That(nodes.Count, Is.EqualTo(2));
            Assert.That(nodes.GetSpawnRate(0), Is.EqualTo(6));
            Assert.That(nodes.GetSpawn(0).From, Is.EqualTo(new RoomHallIndex(0, false)));
            Assert.That(nodes.GetSpawn(0).To, Is.EqualTo(new RoomHallIndex(5, false)));
            Assert.That(nodes.GetSpawn(0).Connector, Is.EqualTo(new Rect(5, 1, 2, 2)));
            Assert.That(nodes.GetSpawnRate(1), Is.EqualTo(4));
            Assert.That(nodes.GetSpawn(1).From, Is.EqualTo(new RoomHallIndex(1, false)));
            Assert.That(nodes.GetSpawn(1).To, Is.EqualTo(new RoomHallIndex(4, false)));
            Assert.That(nodes.GetSpawn(1).Connector, Is.EqualTo(new Rect(5, 3, 2, 2)));
        }

        [Test]
        public void GetPossibleExpansionsAll()
        {
            /*     A B



               C   D     E
                   |
               F   a-G   H


                   I J     */
            // go through and refuse all GetRoomToConnect calls
            // measure the GetRoomToConnect Calls
            TestFloorPlan floorPlan;
            {
                Rect[] rooms = new Rect[]
                {
                    new Rect(5, 1, 1, 2),
                    new Rect(7, 1, 2, 2),
                    new Rect(1, 5, 2, 1),
                    new Rect(5, 5, 2, 2),
                    new Rect(11, 5, 2, 1),
                    new Rect(1, 7, 2, 1),
                    new Rect(7, 7, 2, 2),
                    new Rect(11, 7, 2, 1),
                    new Rect(5, 11, 1, 2),
                    new Rect(7, 11, 1, 2),
                };
                floorPlan = TestFloorPlan.InitFloorToContext(
                    new Loc(22, 14),
                    rooms,
                    new Rect[] { new Rect(5, 7, 2, 2) },
                    new Tuple<char, char>[] { Tuple.Create('D', 'a'), Tuple.Create('a', 'G') });
            }

            var candList = new List<RoomHallIndex>
            {
                new RoomHallIndex(3, false),
                new RoomHallIndex(0, true),
                new RoomHallIndex(6, false),
            };

            var nodes = ConnectTestStep.GetPossibleExpansions(floorPlan, candList);

            Assert.That(nodes.Count, Is.EqualTo(8));
        }

        [Test]
        public void GetRoomToConnectNothingThere()
        {
            TestFloorPlan floorPlan = TestFloorPlan.InitFloorToContext(
                new Loc(22, 14),
                new Rect[] { new Rect(3, 3, 2, 2) },
                Array.Empty<Rect>(),
                Array.Empty<Tuple<char, char>>());

            var node = ConnectTestStep.GetRoomToConnect(floorPlan, new RoomHallIndex(0, false), Dir4.Down);

            Assert.That(node.HasValue, Is.False);
        }

        [Test]
        [TestCase(4, 7, Dir4.Down, 4, 6, 2, 1)]
        [TestCase(4, 9, Dir4.Down, 4, 6, 2, 3)]
        [TestCase(1, 4, Dir4.Left, 3, 4, 1, 2)]
        [TestCase(4, 1, Dir4.Up, 4, 3, 2, 1)]
        [TestCase(7, 4, Dir4.Right, 6, 4, 1, 2)]
        public void GetRoomToConnectBlocked(int blockX, int blockY, Dir4 dir, int rectX, int rectY, int rectW, int rectH)
        {
            TestFloorPlan floorPlan = TestFloorPlan.InitFloorToContext(
                new Loc(22, 14),
                new Rect[] { new Rect(4, 4, 2, 2), new Rect(blockX, blockY, 2, 2) },
                Array.Empty<Rect>(),
                Array.Empty<Tuple<char, char>>());

            var nodeResult = ConnectTestStep.GetRoomToConnect(floorPlan, new RoomHallIndex(0, false), dir);

            Assert.That(nodeResult.HasValue, Is.True);
            var node = nodeResult.Value;

            Assert.That(node.From, Is.EqualTo(new RoomHallIndex(0, false)));
            Assert.That(node.To, Is.EqualTo(new RoomHallIndex(1, false)));
            Assert.That(node.Connector, Is.EqualTo(new Rect(rectX, rectY, rectW, rectH)));
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void GetRoomToConnectBlockedMultiple(bool hall)
        {
            var rooms = new List<Rect>
            {
                new Rect(4, 4, 2, 2),
                new Rect(4, 10, 2, 2),
            };
            var halls = new List<Rect>();
            if (!hall)
                rooms.Add(new Rect(4, 7, 1, 2));
            else
                halls.Add(new Rect(4, 7, 1, 2));
            TestFloorPlan floorPlan = TestFloorPlan.InitFloorToContext(
                new Loc(22, 14),
                rooms.ToArray(),
                halls.ToArray(),
                Array.Empty<Tuple<char, char>>());

            var nodeResult = ConnectTestStep.GetRoomToConnect(floorPlan, new RoomHallIndex(0, false), Dir4.Down);

            Assert.That(nodeResult.HasValue, Is.True);
            var node = nodeResult.Value;

            Assert.That(node.From, Is.EqualTo(new RoomHallIndex(0, false)));
            if (!hall)
                Assert.That(node.To, Is.EqualTo(new RoomHallIndex(2, false)));
            else
                Assert.That(node.To, Is.EqualTo(new RoomHallIndex(0, true)));
            Assert.That(node.Connector, Is.EqualTo(new Rect(4, 6, 2, 1)));
        }

        [Test]
        [TestCase(false, true, 4, 2)]
        [TestCase(true, false, 5, 2)]
        [TestCase(true, true, 5, 1)]
        public void GetRoomToConnectRetracted(bool retractLeft, bool retractRight, int rectX, int rectW)
        {
            List<Rect> rooms = new List<Rect>
            {
                new Rect(4, 4, 3, 2),
                new Rect(4, 10, 2, 2),
            };
            if (retractLeft)
                rooms.Add(new Rect(2, 7, 2, 2));
            if (retractRight)
                rooms.Add(new Rect(7, 7, 2, 2));
            TestFloorPlan floorPlan = TestFloorPlan.InitFloorToContext(
                new Loc(22, 14),
                rooms.ToArray(),
                Array.Empty<Rect>(),
                Array.Empty<Tuple<char, char>>());

            var nodeResult = ConnectTestStep.GetRoomToConnect(floorPlan, new RoomHallIndex(0, false), Dir4.Down);

            Assert.That(nodeResult.HasValue, Is.True);
            var node = nodeResult.Value;

            Assert.That(node.From, Is.EqualTo(new RoomHallIndex(0, false)));
            Assert.That(node.To, Is.EqualTo(new RoomHallIndex(1, false)));
            Assert.That(node.Connector, Is.EqualTo(new Rect(rectX, 6, rectW, 4)));
        }

        [Test]
        public void GetRoomToConnectRetractedTooMuch()
        {
            TestFloorPlan floorPlan = TestFloorPlan.InitFloorToContext(
                new Loc(22, 14),
                new Rect[] { new Rect(4, 4, 2, 2), new Rect(4, 10, 2, 2), new Rect(2, 7, 2, 2), new Rect(6, 7, 2, 2) },
                Array.Empty<Rect>(),
                Array.Empty<Tuple<char, char>>());

            var nodeResult = ConnectTestStep.GetRoomToConnect(floorPlan, new RoomHallIndex(0, false), Dir4.Down);

            Assert.That(nodeResult.HasValue, Is.False);
        }

        [Test]
        [TestCase(-2, false)]
        [TestCase(-1, true)]
        [TestCase(0, true)]
        [TestCase(1, true)]
        [TestCase(2, false)]
        [TestCase(3, true)]
        [TestCase(4, true)]
        [TestCase(5, false)]
        [TestCase(6, false)]
        public void HasBorderOpening(int x, bool expected)
        {
            const Dir4 expandTo = Dir4.Up;
            Mock<IRoomGen> mockFrom = new Mock<IRoomGen>(MockBehavior.Strict);
            mockFrom.SetupGet(p => p.Draw).Returns(new Rect(0, 2, 6, 2));
            mockFrom.Setup(p => p.GetFulfillableBorder(expandTo, It.IsIn(0, 1, 4))).Returns(true);
            mockFrom.Setup(p => p.GetFulfillableBorder(expandTo, It.IsIn(2, 3, 5))).Returns(false);
            Rect rectTo = new Rect(x, 0, 2, 2);

            var mockHall = new Mock<IRandPicker<PermissiveRoomGen<IFloorPlanTestContext>>>(MockBehavior.Strict);

            var pathGen = new ConnectBranchStep<IFloorPlanTestContext>(mockHall.Object);
            bool hasOpening = ConnectTestStep.HasBorderOpening(mockFrom.Object, rectTo, expandTo);

            Assert.That(hasOpening, Is.EqualTo(expected));
        }

        private class ConnectTestStep : ConnectStep<IFloorPlanTestContext>
        {
            public ConnectTestStep(IRandPicker<PermissiveRoomGen<IFloorPlanTestContext>> genericHalls)
                : base(genericHalls)
            {
            }

            public static new bool HasBorderOpening(IRoomGen roomFrom, Rect rectTo, Dir4 expandTo) => ConnectStep<IFloorPlanTestContext>.HasBorderOpening(roomFrom, rectTo, expandTo);

            public static ListPathTraversalNode? GetRoomToConnect(TestFloorPlan floorPlan, RoomHallIndex chosenFrom, Dir4 dir) => ConnectStep<IFloorPlanTestContext>.GetRoomToConnect(floorPlan, chosenFrom, dir);

            public static SpawnList<ListPathTraversalNode> GetPossibleExpansions(TestFloorPlan floorPlan, List<RoomHallIndex> candList) => ConnectStep<IFloorPlanTestContext>.GetPossibleExpansions(floorPlan, candList);

            public override void ApplyToPath(IRandom rand, FloorPlan floorPlan)
            {
            }
        }
    }
}
