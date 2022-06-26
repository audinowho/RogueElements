// <copyright file="FloorPlanTest.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Moq;
using NUnit.Framework;

namespace RogueElements.Tests
{
    [TestFixture]
    public class FloorPlanTest
    {
        [Test]
        [Ignore("TODO")]
        public void GetAll()
        {
            // init a layout and call the enumerator and make sure it has everything
            throw new NotImplementedException();
        }

        [Test]
        [Ignore("TODO")]
        public void GetHall()
        {
            // get existent hall
            // get nonexistent hall, should return null
            throw new NotImplementedException();
        }

        [Test]
        [Ignore("TODO")]
        public void GetRoom()
        {
            // get existent room
            // get nonexistent room, should return null
            throw new NotImplementedException();
        }

        [Test]
        [Ignore("TODO")]
        public void GetRoomGen()
        {
            // get existent room, hall
            // get nonexistent room, hall, should return null
            throw new NotImplementedException();
        }

        [Test]
        [Ignore("TODO")]
        public void GetRoomPlans()
        {
            // Test enumerator
            throw new NotImplementedException();
        }

        [Test]
        public void EraseRoomHallRoom()
        {
            // erase existent rooms in the middle of a list, checking for room consistency
            // and adjacency consistency
            TestFloorPlan floorPlan;
            {
                var links = new Tuple<char, char>[]
                {
                    Tuple.Create('A', 'B'),
                    Tuple.Create('B', 'C'),
                    Tuple.Create('C', 'A'),
                    Tuple.Create('A', 'a'),
                    Tuple.Create('B', 'b'),
                    Tuple.Create('C', 'c'),
                };
                floorPlan = TestFloorPlan.InitFloorToContext(
                    new Loc(22, 14),
                    new Rect[] { Rect.Empty, Rect.Empty, Rect.Empty },
                    new Rect[] { Rect.Empty, Rect.Empty, Rect.Empty },
                    links);
            }

            TestFloorPlan compareFloorPlan;
            {
                var links = new Tuple<char, char>[]
                {
                    Tuple.Create('B', 'A'),
                    Tuple.Create('A', 'a'),
                    Tuple.Create('B', 'c'),
                };
                compareFloorPlan = TestFloorPlan.InitFloorToContext(
                    new Loc(22, 14),
                    new Rect[] { Rect.Empty, Rect.Empty },
                    new Rect[] { Rect.Empty, Rect.Empty, Rect.Empty },
                    links);
            }

            ((TestFloorPlanGen)compareFloorPlan.GetRoom(1)).Identifier = 'C';

            floorPlan.EraseRoomHall(new RoomHallIndex(1, false));

            TestFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);
        }

        [Test]
        public void EraseRoomHallHall()
        {
            // erase existent rooms in the middle of a list, checking for room consistency
            // and adjacency consistency
            TestFloorPlan floorPlan;
            {
                var links = new Tuple<char, char>[]
                {
                    Tuple.Create('a', 'b'),
                    Tuple.Create('b', 'c'),
                    Tuple.Create('c', 'a'),
                    Tuple.Create('A', 'a'),
                    Tuple.Create('B', 'b'),
                    Tuple.Create('C', 'c'),
                };
                floorPlan = TestFloorPlan.InitFloorToContext(
                    new Loc(22, 14),
                    new Rect[] { Rect.Empty, Rect.Empty, Rect.Empty },
                    new Rect[] { Rect.Empty, Rect.Empty, Rect.Empty },
                    links);
            }

            TestFloorPlan compareFloorPlan = TestFloorPlan.InitFloorToContext(
                new Loc(22, 14),
                new Rect[] { Rect.Empty, Rect.Empty, Rect.Empty },
                new Rect[] { Rect.Empty, Rect.Empty },
                new Tuple<char, char>[] { Tuple.Create('b', 'a'), Tuple.Create('A', 'a'), Tuple.Create('C', 'b') });
            ((TestFloorPlanGen)compareFloorPlan.GetRoomHall(new RoomHallIndex(1, true)).RoomGen).Identifier = 'c';

            floorPlan.EraseRoomHall(new RoomHallIndex(1, true));

            TestFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);
        }

        [Test]
        [Ignore("TODO")]
        public void EraseRoomHallException()
        {
            // erase nonexistent room
            // erase outof range
            throw new NotImplementedException();
        }

        [Test]
        public void AddRoomToEmptySpace()
        {
            // add to empty space
            TestFloorPlan floorPlan = TestFloorPlan.InitFloorToContext(
                new Loc(22, 14),
                Array.Empty<Rect>(),
                Array.Empty<Rect>(),
                Array.Empty<Tuple<char, char>>());

            TestFloorPlan compareFloorPlan = TestFloorPlan.InitFloorToContext(
                new Loc(22, 14),
                new Rect[] { new Rect(1, 1, 2, 3) },
                Array.Empty<Rect>(),
                Array.Empty<Tuple<char, char>>());

            var gen = new Mock<TestFloorPlanGen>(MockBehavior.Loose) { CallBase = true };
            gen.SetupGet(p => p.Draw).Returns(new Rect(1, 1, 2, 3));
            gen.Object.Identifier = 'A';
            floorPlan.AddRoom(gen.Object, new ComponentCollection());

            TestFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);
        }

        [Test]
        public void AddRoomToExisting()
        {
            // add with adjacents
            TestFloorPlan floorPlan = TestFloorPlan.InitFloorToContext(
                new Loc(22, 14),
                new Rect[] { new Rect(1, 1, 2, 3) },
                new Rect[] { new Rect(6, 1, 3, 3) },
                Array.Empty<Tuple<char, char>>());

            TestFloorPlan compareFloorPlan = TestFloorPlan.InitFloorToContext(
                new Loc(22, 14),
                new Rect[] { new Rect(1, 1, 2, 3), new Rect(3, 2, 3, 5) },
                new Rect[] { new Rect(6, 1, 3, 3) },
                new Tuple<char, char>[] { Tuple.Create('A', 'B'), Tuple.Create('a', 'B') });

            var gen = new Mock<TestFloorPlanGen>(MockBehavior.Loose) { CallBase = true };
            gen.SetupGet(p => p.Draw).Returns(new Rect(3, 2, 3, 5));
            gen.Object.Identifier = 'B';

            floorPlan.AddRoom(gen.Object, new ComponentCollection(), new RoomHallIndex(0, false), new RoomHallIndex(0, true));

            TestFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void AddRoomCollideExisting(bool hall)
        {
            // add on top of existing room?
            List<Rect> rooms = new List<Rect>();
            List<Rect> halls = new List<Rect>();
            if (!hall)
                rooms.Add(new Rect(1, 1, 2, 3));
            else
                halls.Add(new Rect(1, 1, 2, 3));
            TestFloorPlan floorPlan = TestFloorPlan.InitFloorToContext(
                new Loc(22, 14),
                rooms.ToArray(),
                halls.ToArray(),
                Array.Empty<Tuple<char, char>>());

            var gen = new Mock<TestFloorPlanGen>(MockBehavior.Loose) { CallBase = true };
            gen.SetupGet(p => p.Draw).Returns(new Rect(2, 2, 4, 4));
            gen.Object.Identifier = 'B';

            // check the rooms
            Assert.Throws<InvalidOperationException>(() => { floorPlan.AddRoom(gen.Object, new ComponentCollection()); });
        }

        [Test]
        [TestCase(-1, 0)]
        [TestCase(0, -1)]
        [TestCase(-1, -1)]
        [TestCase(22, 0)]
        [TestCase(0, 13)]
        public void AddRoomToOutOfBounds(int x, int y)
        {
            // attempt to touch out of bounds
            var floorPlan = new TestFloorPlan();
            floorPlan.InitSize(new Loc(22, 14));
            Mock<TestFloorPlanGen> gen = new Mock<TestFloorPlanGen>(MockBehavior.Loose);
            gen.SetupGet(p => p.Draw).Returns(new Rect(x, y, 2, 3));
            gen.Object.Identifier = 'A';

            // check the rooms
            Assert.Throws<InvalidOperationException>(() => { floorPlan.AddRoom(gen.Object, new ComponentCollection()); });
        }

        [Test]
        public void AddHall()
        {
            // add to empty space
            TestFloorPlan floorPlan = TestFloorPlan.InitFloorToContext(
                new Loc(22, 14),
                Array.Empty<Rect>(),
                Array.Empty<Rect>(),
                Array.Empty<Tuple<char, char>>());

            TestFloorPlan compareFloorPlan = TestFloorPlan.InitFloorToContext(
                new Loc(22, 14),
                Array.Empty<Rect>(),
                new Rect[] { new Rect(1, 1, 2, 3) },
                Array.Empty<Tuple<char, char>>());

            var gen = new Mock<TestFloorPlanGen>(MockBehavior.Loose) { CallBase = true };
            gen.SetupGet(p => p.Draw).Returns(new Rect(1, 1, 2, 3));
            gen.Object.Identifier = 'a';
            floorPlan.AddHall(gen.Object, new ComponentCollection());

            TestFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);
        }

        [Test]
        public void AddHallCollideExistingHall()
        {
            // add on top of existing hall, no consequences
            TestFloorPlan floorPlan = TestFloorPlan.InitFloorToContext(
                new Loc(22, 14),
                Array.Empty<Rect>(),
                new Rect[] { new Rect(1, 1, 2, 3) },
                Array.Empty<Tuple<char, char>>());

            TestFloorPlan compareFloorPlan = TestFloorPlan.InitFloorToContext(
                new Loc(22, 14),
                Array.Empty<Rect>(),
                new Rect[] { new Rect(1, 1, 2, 3), new Rect(2, 2, 4, 4) },
                Array.Empty<Tuple<char, char>>());

            var gen = new Mock<TestFloorPlanGen>(MockBehavior.Loose) { CallBase = true };
            gen.SetupGet(p => p.Draw).Returns(new Rect(2, 2, 4, 4));
            gen.Object.Identifier = 'b';

            floorPlan.AddHall(gen.Object, new ComponentCollection());

            TestFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);
        }

        [Test]
        public void AddHallCollideExistingRoom()
        {
            // add on top of existing room?
            TestFloorPlan floorPlan = TestFloorPlan.InitFloorToContext(
                new Loc(22, 14),
                new Rect[] { new Rect(1, 1, 2, 3) },
                Array.Empty<Rect>(),
                Array.Empty<Tuple<char, char>>());

            var gen = new Mock<TestFloorPlanGen>(MockBehavior.Loose) { CallBase = true };
            gen.SetupGet(p => p.Draw).Returns(new Rect(2, 2, 4, 4));
            gen.Object.Identifier = 'b';

            // check the rooms
            Assert.Throws<InvalidOperationException>(() => { floorPlan.AddHall(gen.Object, new ComponentCollection()); });
        }

        [Test]
        public void GetAdjacentRoomsNone()
        {
            // no adjacents
            // A B a
            TestFloorPlan floorPlan = TestFloorPlan.InitFloorToContext(
                new Loc(22, 14),
                new Rect[] { Rect.Empty, Rect.Empty },
                new Rect[] { Rect.Empty },
                Array.Empty<Tuple<char, char>>());

            List<int> adjacentRooms = floorPlan.GetAdjacentRooms(0);
            List<int> expectedRooms = new List<int>();
            Assert.That(adjacentRooms, Is.EqualTo(expectedRooms));
        }

        [Test]
        public void GetAdjacentRoomsRooms()
        {
            // rooms adjacent (and one room after)
            /* A-B-D
               |
               C     */
            TestFloorPlan floorPlan = TestFloorPlan.InitFloorToContext(
                new Loc(22, 14),
                new Rect[] { Rect.Empty, Rect.Empty, Rect.Empty },
                Array.Empty<Rect>(),
                new Tuple<char, char>[] { Tuple.Create('A', 'B'), Tuple.Create('A', 'C'), Tuple.Create('B', 'C') });

            List<int> adjacentRooms = floorPlan.GetAdjacentRooms(0);
            var expectedRooms = new List<int> { 1, 2 };
            Assert.That(adjacentRooms, Is.EqualTo(expectedRooms));
        }

        [Test]
        public void GetAdjacentRoomsOneHall()
        {
            // only hall adjacent
            // A-a-b
            TestFloorPlan floorPlan = TestFloorPlan.InitFloorToContext(
                new Loc(22, 14),
                new Rect[] { Rect.Empty },
                new Rect[] { Rect.Empty, Rect.Empty },
                new Tuple<char, char>[] { Tuple.Create('A', 'a'), Tuple.Create('a', 'b') });

            List<int> adjacentRooms = floorPlan.GetAdjacentRooms(0);
            List<int> expectedRooms = new List<int>();
            Assert.That(adjacentRooms, Is.EqualTo(expectedRooms));
        }

        [Test]
        public void GetAdjacentRoomsRoomsFromHall()
        {
            // only hall adjacent, leading to rooms
            /* A-a-b-B
                   |
                   C   */
            TestFloorPlan floorPlan = TestFloorPlan.InitFloorToContext(
                new Loc(22, 14),
                new Rect[] { Rect.Empty, Rect.Empty, Rect.Empty },
                new Rect[] { Rect.Empty, Rect.Empty },
                new Tuple<char, char>[] { Tuple.Create('A', 'a'), Tuple.Create('B', 'b'), Tuple.Create('C', 'b'), Tuple.Create('a', 'b') });

            List<int> adjacentRooms = floorPlan.GetAdjacentRooms(0);
            var expectedRooms = new List<int> { 1, 2 };
            Assert.That(adjacentRooms, Is.EqualTo(expectedRooms));
        }

        [Test]
        public void GetAdjacentRoomsRoomsFromHallCycle()
        {
            // loop of halls adjacent, leading to rooms
            /* A-a-b-B
                 | |
                 c-d-D
                 |
                 C     */
            TestFloorPlan floorPlan;
            {
                var links = new Tuple<char, char>[]
                {
                    Tuple.Create('A', 'a'),
                    Tuple.Create('B', 'b'),
                    Tuple.Create('C', 'c'),
                    Tuple.Create('D', 'd'),
                    Tuple.Create('a', 'b'),
                    Tuple.Create('a', 'c'),
                    Tuple.Create('d', 'd'),
                    Tuple.Create('b', 'd'),
                };
                floorPlan = TestFloorPlan.InitFloorToContext(
                    new Loc(22, 14),
                    new Rect[] { Rect.Empty, Rect.Empty, Rect.Empty, Rect.Empty },
                    new Rect[] { Rect.Empty, Rect.Empty, Rect.Empty, Rect.Empty },
                    links);
            }

            List<int> adjacentRooms = floorPlan.GetAdjacentRooms(0);
            List<int> expectedRooms = new List<int> { 1, 2, 3 };
            Assert.That(adjacentRooms, Is.EqualTo(expectedRooms));
        }

        [Test]
        public void GetDistanceNone()
        {
            // no adjacents
            // A-B C
            TestFloorPlan floorPlan = TestFloorPlan.InitFloorToContext(
                new Loc(22, 14),
                new Rect[] { Rect.Empty, Rect.Empty, Rect.Empty },
                Array.Empty<Rect>(),
                new Tuple<char, char>[] { Tuple.Create('A', 'B') });

            int distance = floorPlan.GetDistance(new RoomHallIndex(0, false), new RoomHallIndex(2, false));
            Assert.That(distance, Is.EqualTo(-1));
        }

        [Test]
        public void GetDistanceSame()
        {
            // same start and end
            // A-B
            TestFloorPlan floorPlan = TestFloorPlan.InitFloorToContext(
                new Loc(22, 14),
                new Rect[] { Rect.Empty, Rect.Empty },
                Array.Empty<Rect>(),
                new Tuple<char, char>[] { Tuple.Create('A', 'B') });

            int distance = floorPlan.GetDistance(new RoomHallIndex(0, false), new RoomHallIndex(0, false));
            Assert.That(distance, Is.EqualTo(0));
        }

        [Test]
        public void GetDistanceOneLine()
        {
            // only hall adjacent
            // A-a-B
            TestFloorPlan floorPlan = TestFloorPlan.InitFloorToContext(
                new Loc(22, 14),
                new Rect[] { Rect.Empty, Rect.Empty },
                new Rect[] { Rect.Empty },
                new Tuple<char, char>[] { Tuple.Create('A', 'a'), Tuple.Create('a', 'B') });

            int distance = floorPlan.GetDistance(new RoomHallIndex(0, false), new RoomHallIndex(1, false));
            Assert.That(distance, Is.EqualTo(2));
        }

        [Test]
        public void GetDistanceBranchRoute()
        {
            // only hall adjacent
            /* A-D-E
               | |
               B-C   */
            TestFloorPlan floorPlan;
            {
                var links = new Tuple<char, char>[]
                {
                    Tuple.Create('A', 'B'),
                    Tuple.Create('B', 'C'),
                    Tuple.Create('C', 'D'),
                    Tuple.Create('D', 'E'),
                    Tuple.Create('A', 'D'),
                };
                floorPlan = TestFloorPlan.InitFloorToContext(
                    new Loc(22, 14),
                    new Rect[] { Rect.Empty, Rect.Empty, Rect.Empty, Rect.Empty, Rect.Empty },
                    Array.Empty<Rect>(),
                    links);
            }

            int distance = floorPlan.GetDistance(new RoomHallIndex(0, false), new RoomHallIndex(4, false));
            Assert.That(distance, Is.EqualTo(2));
        }

        [Test]
        public void IsChokePointSingle()
        {
            // A
            TestFloorPlan floorPlan = TestFloorPlan.InitFloorToContext(
                new Loc(22, 14),
                new Rect[] { Rect.Empty },
                Array.Empty<Rect>(),
                Array.Empty<Tuple<char, char>>());

            bool result = floorPlan.IsChokePoint(new RoomHallIndex(0, false));
            Assert.That(result, Is.EqualTo(false));
        }

        [Test]
        public void IsChokePointIsolated()
        {
            // A-B C
            TestFloorPlan floorPlan = TestFloorPlan.InitFloorToContext(
                new Loc(22, 14),
                new Rect[] { Rect.Empty, Rect.Empty, Rect.Empty },
                Array.Empty<Rect>(),
                new Tuple<char, char>[] { Tuple.Create('A', 'B') });

            bool result = floorPlan.IsChokePoint(new RoomHallIndex(0, false));
            Assert.That(result, Is.EqualTo(false));
        }

        [Test]
        [TestCase(0, false, false)]
        [TestCase(0, true, true)]
        [TestCase(1, false, true)]
        [TestCase(2, false, false)]
        [TestCase(1, true, false)]
        public void IsChokePoint(int index, bool isHall, bool expected)
        {
            /* A-a-B-C
                   | |
                   D-b */
            TestFloorPlan floorPlan;
            {
                var links = new Tuple<char, char>[]
                {
                    Tuple.Create('A', 'a'),
                    Tuple.Create('a', 'B'),
                    Tuple.Create('B', 'C'),
                    Tuple.Create('B', 'D'),
                    Tuple.Create('C', 'b'),
                    Tuple.Create('D', 'b'),
                };
                floorPlan = TestFloorPlan.InitFloorToContext(
                    new Loc(22, 14),
                    new Rect[] { Rect.Empty, Rect.Empty, Rect.Empty, Rect.Empty },
                    new Rect[] { Rect.Empty, Rect.Empty },
                    links);
            }

            bool result = floorPlan.IsChokePoint(new RoomHallIndex(index, isHall));
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        [TestCase(Ignore = "TODO")]
        [Ignore("TODO")]
        [SuppressMessage("CodeCracker.CSharp.Usage", "CC0057:UnusedParameter", Justification = "TODO")]
        public void DrawOnMap(bool addRoom, bool addHall)
        {
            // verify that rooms follow border information of both rooms before and after
            throw new NotImplementedException();
        }

        [Test]
        [TestCase(0, false, 2, 2)] // start of room list; will affect all other rooms and halls
        [TestCase(2, false, 1, 2)] // mid of room list; will affect all rooms after and halls
        [TestCase(4, false, 0, 2)] // end of room list; will affect only halls
        [TestCase(0, true, 0, 2)] // repeat with halls
        [TestCase(2, true, 0, 1)]
        [TestCase(4, true, 0, 0)]
        public void TransferBorderToAdjacents(int index, bool isHall, int expectedRoom, int expectedHall)
        {
            List<Mock<IRoomGen>> roomGenTarget = new List<Mock<IRoomGen>>();
            List<Mock<IPermissiveRoomGen>> hallGenTarget = new List<Mock<IPermissiveRoomGen>>();

            var floorPlan = new TestFloorPlan();
            floorPlan.InitSize(new Loc(22, 14));

            for (int ii = 0; ii < 5; ii++)
            {
                Mock<IRoomGen> roomGen = new Mock<IRoomGen>(MockBehavior.Strict);
                if (!isHall && index == ii)
                {
                    roomGen.SetupGet(p => p.Draw).Returns(new Rect(new Loc(1, 5), new Loc(4, 2)));
                }
                else if (ii == 1 || ii == 3)
                {
                    roomGen.SetupGet(p => p.Draw).Returns(new Rect(new Loc(ii, 3), new Loc(2, 2)));
                    roomGen.Setup(p => p.AskBorderFromRoom(It.IsAny<Rect>(), It.IsAny<Func<Dir4, int, bool>>(), It.IsAny<Dir4>()));
                    roomGenTarget.Add(roomGen);
                }
                else
                {
                    roomGen.SetupGet(p => p.Draw).Returns(Rect.Empty);
                }

                var roomPlan = new FloorRoomPlan(roomGen.Object, new ComponentCollection());

                // only the room under test should get adjacents
                if (!isHall && index == ii)
                {
                    roomPlan.Adjacents.Add(new RoomHallIndex(1, false));
                    roomPlan.Adjacents.Add(new RoomHallIndex(3, false));
                    roomPlan.Adjacents.Add(new RoomHallIndex(1, true));
                    roomPlan.Adjacents.Add(new RoomHallIndex(3, true));
                }

                floorPlan.PublicRooms.Add(roomPlan);
            }

            for (int ii = 0; ii < 5; ii++)
            {
                Mock<IPermissiveRoomGen> roomGen = new Mock<IPermissiveRoomGen>(MockBehavior.Strict);
                roomGen.SetupGet(p => p.Draw).Returns(Rect.Empty);
                if (isHall && index == ii)
                {
                    roomGen.SetupGet(p => p.Draw).Returns(new Rect(new Loc(1, 5), new Loc(4, 2)));
                }
                else if (ii == 1 || ii == 3)
                {
                    roomGen.SetupGet(p => p.Draw).Returns(new Rect(new Loc(ii, 7), new Loc(2, 2)));
                    roomGen.Setup(p => p.AskBorderFromRoom(It.IsAny<Rect>(), It.IsAny<Func<Dir4, int, bool>>(), It.IsAny<Dir4>()));
                    hallGenTarget.Add(roomGen);
                }
                else
                {
                    roomGen.SetupGet(p => p.Draw).Returns(Rect.Empty);
                }

                var roomPlan = new FloorHallPlan(roomGen.Object, new ComponentCollection());

                if (isHall && index == ii)
                {
                    roomPlan.Adjacents.Add(new RoomHallIndex(1, false));
                    roomPlan.Adjacents.Add(new RoomHallIndex(3, false));
                    roomPlan.Adjacents.Add(new RoomHallIndex(1, true));
                    roomPlan.Adjacents.Add(new RoomHallIndex(3, true));
                }

                floorPlan.PublicHalls.Add(roomPlan);
            }

            IRoomGen from = floorPlan.GetRoomHall(new RoomHallIndex(index, isHall)).RoomGen;

            floorPlan.TransferBorderToAdjacents(new RoomHallIndex(index, isHall));

            for (int ii = 0; ii < roomGenTarget.Count; ii++)
            {
                if (ii >= roomGenTarget.Count - expectedRoom)
                    roomGenTarget[ii].Verify(p => p.AskBorderFromRoom(from.Draw, from.GetOpenedBorder, It.IsAny<Dir4>()), Times.Exactly(1));
                else
                    roomGenTarget[ii].Verify(p => p.AskBorderFromRoom(from.Draw, from.GetOpenedBorder, It.IsAny<Dir4>()), Times.Exactly(0));
            }

            for (int ii = 0; ii < hallGenTarget.Count; ii++)
            {
                if (ii >= hallGenTarget.Count - expectedHall)
                    hallGenTarget[ii].Verify(p => p.AskBorderFromRoom(from.Draw, from.GetOpenedBorder, It.IsAny<Dir4>()), Times.Exactly(1));
                else
                    hallGenTarget[ii].Verify(p => p.AskBorderFromRoom(from.Draw, from.GetOpenedBorder, It.IsAny<Dir4>()), Times.Exactly(0));
            }
        }

        [Test]
        [TestCase(0, 0, Dir4.None)] // none
        [TestCase(0, 1, Dir4.None)] // collided at bottom
        [TestCase(0, 2, Dir4.Down)] // joined at bottom
        [TestCase(0, 3, Dir4.None)] // not joined at bottom
        [TestCase(-2, 0, Dir4.Left)] // joined at left
        [TestCase(0, -2, Dir4.Up)] // joined at top
        [TestCase(2, 0, Dir4.Right)] // joined at right
        [TestCase(2, 2, Dir4.None)] // joined at right-down diagonal
        public void GetDirAdjacent(int dx, int dy, Dir4 expectedDir)
        {
            // transfers based on location; requires EXACT contact
            Mock<IRoomGen> mockFrom = new Mock<IRoomGen>(MockBehavior.Strict);
            mockFrom.SetupGet(p => p.Draw).Returns(new Rect(0, 0, 2, 2));
            Mock<IRoomGen> mockTo = new Mock<IRoomGen>(MockBehavior.Strict);
            mockTo.SetupGet(p => p.Draw).Returns(new Rect(dx, dy, 2, 2));

            var testFloorPlan = new TestFloorPlan();
            Dir4 dir = testFloorPlan.GetDirAdjacent(mockFrom.Object, mockTo.Object);
            Assert.That(dir, Is.EqualTo(expectedDir));
        }

        [Test]
        public void GetRectAdjacentError()
        {
            // transfers based on location; requires EXACT contact
            var testFloorPlan = new TestFloorPlan();
            Assert.Throws<ArgumentException>(() => { testFloorPlan.GetAdjacentRect(new Rect(0, 0, 2, 2), new Rect(0, 2, 2, 2), Dir4.None); });
        }

        // base cases
        [Test]
        [TestCase(0, 0, Dir4.Down, false)] // none
        [TestCase(0, 0, Dir4.Left, false)] // none
        [TestCase(0, 0, Dir4.Up, false)] // none
        [TestCase(0, 0, Dir4.Right, false)] // none
        [TestCase(0, 1, Dir4.Down, false)] // collided at bottom
        [TestCase(0, 1, Dir4.Left, false)] // collided at bottom
        [TestCase(0, 1, Dir4.Up, false)] // collided at bottom
        [TestCase(0, 1, Dir4.Right, false)] // collided at bottom
        [TestCase(0, 2, Dir4.Down, true)] // joined at bottom
        [TestCase(0, 2, Dir4.Left, false)] // joined at bottom, incorrect angle
        [TestCase(0, 2, Dir4.Up, false)] // joined at bottom, incorrect angle
        [TestCase(0, 2, Dir4.Right, false)] // joined at bottom, incorrect angle
        [TestCase(0, 3, Dir4.Down, false)] // not joined at bottom
        [TestCase(0, 3, Dir4.Left, false)] // not joined at bottom
        [TestCase(0, 3, Dir4.Up, false)] // not joined at bottom
        [TestCase(0, 3, Dir4.Right, false)] // not joined at bottom
        [TestCase(-2, 0, Dir4.Left, true)] // joined at left
        [TestCase(-2, 0, Dir4.Up, false)] // joined at left, incorrect angle
        [TestCase(-2, 0, Dir4.Right, false)] // joined at left, incorrect angle
        [TestCase(-2, 0, Dir4.Down, false)] // joined at left, incorrect angle
        [TestCase(0, -2, Dir4.Up, true)] // joined at top
        [TestCase(2, 0, Dir4.Right, true)] // joined at right
        [TestCase(2, 2, Dir4.Down, false)] // joined at right-down diagonal
        [TestCase(2, 2, Dir4.Left, false)] // joined at right-down diagonal
        [TestCase(2, 2, Dir4.Up, false)] // joined at right-down diagonal
        [TestCase(2, 2, Dir4.Right, false)] // joined at right-down diagonal
        public void GetRectAdjacent(int dx, int dy, Dir4 dir, bool result)
        {
            // transfers based on location; requires EXACT contact
            var testFloorPlan = new TestFloorPlan();
            Rect? rect = testFloorPlan.GetAdjacentRect(new Rect(0, 0, 2, 2), new Rect(dx, dy, 2, 2), dir);
            if (result)
                Assert.That(rect.Value, Is.EqualTo(new Rect(dx, dy, 2, 2)));
            else
                Assert.That(rect.HasValue, Is.EqualTo(false));
        }

        // wrapped
        [Test]
        [TestCase(0, 0, 10, 20, Dir4.Down, false, 0, 0)] // full overlap
        [TestCase(0, 0, 10, 20, Dir4.Left, false, 0, 0)] // full overlap
        [TestCase(0, 0, 10, 20, Dir4.Up, false, 0, 0)] // full overlap
        [TestCase(0, 0, 10, 20, Dir4.Right, false, 0, 0)] // full overlap
        [TestCase(0, 0, 8, 0, Dir4.Left, true, -2, 0)] // joined left exactly at border
        [TestCase(1, 0, 9, 0, Dir4.Left, true, -1, 0)] // joined left over border
        [TestCase(8, 0, 0, 0, Dir4.Right, true, 10, 0)] // joined right exactly at border
        [TestCase(9, 0, 1, 0, Dir4.Right, true, 11, 0)] // joined right over border
        [TestCase(4, 0, 2, 19, Dir4.Left, true, 2, -1)] // joined left over orth border
        [TestCase(0, 0, 0, 18, Dir4.Up, true, 0, -2)] // joined up exactly at border
        [TestCase(0, 1, 0, 19, Dir4.Up, true, 0, -1)] // joined up over border
        [TestCase(0, 0, 8, 20, Dir4.Left, true, -2, 0)] // joined left exactly at two borders
        [TestCase(1, 0, 9, 19, Dir4.Left, true, -1, -1)] // joined left over two border
        public void GetRectAdjacentWrap(int dx1, int dy1, int dx2, int dy2, Dir4 dir, bool result, int rx, int ry)
        {
            // transfers based on location; requires EXACT contact
            var testFloorPlan = new TestFloorPlan();
            testFloorPlan.InitRect(new Rect(0, 0, 10, 20), true);
            Rect? rect = testFloorPlan.GetAdjacentRect(new Rect(dx1, dy1, 2, 2), new Rect(dx2, dy2, 2, 2), dir);
            if (result)
                Assert.That(rect.Value, Is.EqualTo(new Rect(rx, ry, 2, 2)));
            else
                Assert.That(rect.HasValue, Is.EqualTo(false));

            // also test this with offset
        }

        [Test]
        [TestCase(0, 0, 10, 20, Dir4.Down, false, 0, 0)] // full overlap
        [TestCase(0, 0, 10, 20, Dir4.Left, false, 0, 0)] // full overlap
        [TestCase(0, 0, 10, 20, Dir4.Up, false, 0, 0)] // full overlap
        [TestCase(0, 0, 10, 20, Dir4.Right, false, 0, 0)] // full overlap
        [TestCase(0, 0, 8, 0, Dir4.Left, true, -2, 0)] // joined left exactly at border
        [TestCase(1, 0, 9, 0, Dir4.Left, true, -1, 0)] // joined left over border
        [TestCase(8, 0, 0, 0, Dir4.Right, true, 10, 0)] // joined right exactly at border
        [TestCase(9, 0, 1, 0, Dir4.Right, true, 11, 0)] // joined right over border
        [TestCase(4, 0, 2, 19, Dir4.Left, true, 2, -1)] // joined left over orth border
        [TestCase(0, 0, 0, 18, Dir4.Up, true, 0, -2)] // joined up exactly at border
        [TestCase(0, 1, 0, 19, Dir4.Up, true, 0, -1)] // joined up over border
        [TestCase(0, 0, 8, 20, Dir4.Left, true, -2, 0)] // joined left exactly at two borders
        [TestCase(1, 0, 9, 19, Dir4.Left, true, -1, -1)] // joined left over two border
        public void GetRectAdjacentWrapOffset(int dx1, int dy1, int dx2, int dy2, Dir4 dir, bool result, int rx, int ry)
        {
            // transfers based on location; requires EXACT contact
            Loc offset = new Loc(2, 3);
            var testFloorPlan = new TestFloorPlan();
            testFloorPlan.InitRect(new Rect(offset.X, offset.Y, 10, 20), true);
            Rect rect1 = new Rect(new Loc(dx1, dy1) + offset, new Loc(2));
            Rect rect2 = new Rect(new Loc(dx2, dy2) + offset, new Loc(2));
            Rect? rect = testFloorPlan.GetAdjacentRect(rect1, rect2, dir);
            if (result)
                Assert.That(rect.Value, Is.EqualTo(new Rect(new Loc(rx, ry) + offset, new Loc(2))));
            else
                Assert.That(rect.HasValue, Is.EqualTo(false));
        }

        [Test]
        [TestCase(false, false)] // no collision
        [TestCase(true, false)] // room collision
        [TestCase(false, true)] // hall collision
        public void CheckCollision(bool addRoom, bool addHall)
        {
            List<RoomHallIndex> collidesCompare = new List<RoomHallIndex>();
            List<Rect> rooms = new List<Rect>();
            if (addRoom)
            {
                rooms.Add(new Rect(1, 1, 2, 3));
                collidesCompare.Add(new RoomHallIndex(0, false));
            }

            List<Rect> halls = new List<Rect>();
            if (addHall)
            {
                halls.Add(new Rect(4, 1, 3, 2));
                collidesCompare.Add(new RoomHallIndex(0, true));
            }

            TestFloorPlan floorPlan = TestFloorPlan.InitFloorToContext(
                new Loc(22, 14),
                rooms.ToArray(),
                halls.ToArray(),
                Array.Empty<Tuple<char, char>>());

            List<RoomHallIndex> collides = floorPlan.CheckCollision(new Rect(2, 2, 4, 6));
            Assert.That(collidesCompare, Is.EqualTo(collides));
        }

        [Test]
        [TestCase(-3, 0)]
        [TestCase(-2, 1)]
        [TestCase(-1, 2)]
        [TestCase(0, 3)]
        [TestCase(1, 3)]
        [TestCase(2, 2)]
        [TestCase(3, 1)]
        [TestCase(4, 0)]
        public void GetBorderMatch(int x, int expectedMatch)
        {
            const Dir4 expandTo = Dir4.Up;
            Mock<IRoomGen> mockFrom = new Mock<IRoomGen>(MockBehavior.Strict);
            mockFrom.SetupGet(p => p.Draw).Returns(new Rect(0, 2, 4, 2));
            mockFrom.Setup(p => p.GetFulfillableBorder(expandTo, It.IsIn(0, 1, 2, 3))).Returns(true);
            Mock<IRoomGen> mockTo = new Mock<IRoomGen>(MockBehavior.Strict);
            mockTo.SetupGet(p => p.Draw).Returns(new Rect(0, 0, 3, 2));
            mockTo.Setup(p => p.GetFulfillableBorder(expandTo.Reverse(), It.IsIn(0, 1, 2))).Returns(true);

            int totalMatch = TestFloorPlan.GetBorderMatch(mockFrom.Object, mockTo.Object, new Loc(x, 0), expandTo);

            Assert.That(totalMatch, Is.EqualTo(expectedMatch));
        }

        [Test]
        [TestCase(-3, 0)]
        [TestCase(-2, 1)]
        [TestCase(-1, 0)]
        [TestCase(0, 2)]
        [TestCase(1, 0)]
        [TestCase(2, 1)]
        [TestCase(3, 0)]
        [TestCase(4, 0)]
        public void GetBorderMatchPatterned(int x, int expectedMatch)
        {
            const Dir4 expandTo = Dir4.Up;
            Mock<IRoomGen> mockFrom = new Mock<IRoomGen>(MockBehavior.Strict);
            mockFrom.SetupGet(p => p.Draw).Returns(new Rect(0, 2, 4, 2));
            mockFrom.Setup(p => p.GetFulfillableBorder(expandTo, It.IsIn(0, 2))).Returns(true);
            mockFrom.Setup(p => p.GetFulfillableBorder(expandTo, It.IsIn(1, 3))).Returns(false);
            Mock<IRoomGen> mockTo = new Mock<IRoomGen>(MockBehavior.Strict);
            mockTo.SetupGet(p => p.Draw).Returns(new Rect(0, 0, 3, 2));
            mockTo.Setup(p => p.GetFulfillableBorder(expandTo.Reverse(), It.IsIn(0, 2))).Returns(true);
            mockTo.Setup(p => p.GetFulfillableBorder(expandTo.Reverse(), It.IsIn(1))).Returns(false);

            int totalMatch = TestFloorPlan.GetBorderMatch(mockFrom.Object, mockTo.Object, new Loc(x, 0), expandTo);

            Assert.That(totalMatch, Is.EqualTo(expectedMatch));
        }

        [Test]
        [TestCase(Dir8.DownRight, Dir8.UpLeft, 0, 0, 4, 3)]
        [TestCase(Dir8.DownRight, Dir8.DownRight, 0, 0, 8, 6)]
        [TestCase(Dir8.None, Dir8.None, -2, -1, 4, 3)]
        [TestCase(Dir8.UpLeft, Dir8.UpLeft, -4, -3, 0, 0)]
        public void ResizeFloor(Dir8 expandDir, Dir8 anchorDir, int newX, int newY, int newRoomX, int newRoomY)
        {
            // add to empty space
            TestFloorPlan floorPlan = TestFloorPlan.InitFloorToContext(
                new Rect(Loc.Zero, new Loc(22, 14)),
                new Rect[] { new Rect(4, 3, 2, 3) },
                Array.Empty<Rect>(),
                Array.Empty<Tuple<char, char>>());

            TestFloorPlan compareFloorPlan = TestFloorPlan.InitFloorToContext(
                new Rect(new Loc(newX, newY), new Loc(26, 17)),
                new Rect[] { new Rect(newRoomX, newRoomY, 2, 3) },
                Array.Empty<Rect>(),
                Array.Empty<Tuple<char, char>>());

            floorPlan.Resize(new Loc(26, 17), expandDir, anchorDir);

            TestFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);
        }
    }
}