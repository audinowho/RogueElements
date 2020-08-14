// <copyright file="SetSpecialRoomTest.cs" company="Audino">
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
    public class SetSpecialRoomTest
    {
        [Test]
        [Ignore("TODO")]
        public void ApplyImmutable()
        {
            // place on a floor where the first room is immutable
            throw new NotImplementedException();
        }

        [Test]
        [Ignore("TODO")]
        public void ApplySmaller()
        {
            // place on a floor where the first room is smaller
            throw new NotImplementedException();
        }

        [Test]
        [Ignore("TODO")]
        public void ApplyMidSize()
        {
            // place on a floor where the first room is equal
            throw new NotImplementedException();
        }

        [Test]
        [Ignore("TODO")]
        public void ApplyLarger()
        {
            // place on a floor where the first room is larger
            throw new NotImplementedException();
        }

        [Test]
        [Ignore("TODO")]
        public void ApplyInHall()
        {
            // place on a floor where the first room is larger
            throw new NotImplementedException();
        }

        [Test]
        [Ignore("TODO")]
        public void ApplyWithComponent()
        {
            // check that components are placed as expected
            throw new NotImplementedException();
        }

        [Test]
        public void PlaceRoomOneAdjacent()
        {
            // the new room touches its only adjacent of the old room
            TestFloorPlan floorPlan = TestFloorPlan.InitFloorToContext(
                new Loc(22, 14),
                new Rect[] { new Rect(3, 3, 6, 6), new Rect(5, 1, 2, 2) },
                Array.Empty<Rect>(),
                new Tuple<char, char>[] { Tuple.Create('A', 'B') });
            TestFloorPlan compareFloorPlan = TestFloorPlan.InitFloorToContext(
                new Loc(22, 14),
                new Rect[] { new Rect(5, 1, 2, 2), new Rect(5, 3, 2, 2) },
                Array.Empty<Rect>(),
                new Tuple<char, char>[] { Tuple.Create('B', 'A') });
            ((TestFloorPlanGen)compareFloorPlan.GetRoom(0)).Identifier = 'B';
            ((TestFloorPlanGen)compareFloorPlan.GetRoom(1)).Identifier = 'C';

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);

            var gen = new TestFloorPlanGen('C');
            gen.PrepareDraw(new Rect(5, 3, 2, 2));

            var mockRooms = new Mock<IRandPicker<RoomGen<IFloorPlanTestContext>>>(MockBehavior.Strict);
            var mockHalls = new Mock<IRandPicker<PermissiveRoomGen<IFloorPlanTestContext>>>(MockBehavior.Strict);

            var pathGen = new SetSpecialRoomTestStep(mockRooms.Object, mockHalls.Object);
            pathGen.PlaceRoom(testRand.Object, floorPlan, gen, new RoomHallIndex(0, false));

            TestFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);
        }

        [Test]
        public void PlaceRoomMultiAdjacent()
        {
            // the new room touches adjacents of two sides of the old room
            TestFloorPlan floorPlan = TestFloorPlan.InitFloorToContext(
                new Loc(22, 14),
                new Rect[] { new Rect(3, 3, 6, 6), new Rect(3, 1, 2, 2), new Rect(1, 3, 2, 2) },
                Array.Empty<Rect>(),
                new Tuple<char, char>[] { Tuple.Create('A', 'B'), Tuple.Create('A', 'C') });
            TestFloorPlan compareFloorPlan = TestFloorPlan.InitFloorToContext(
                new Loc(22, 14),
                new Rect[] { new Rect(3, 1, 2, 2), new Rect(1, 3, 2, 2), new Rect(3, 3, 2, 2) },
                Array.Empty<Rect>(),
                new Tuple<char, char>[] { Tuple.Create('C', 'B'), Tuple.Create('C', 'A') });
            ((TestFloorPlanGen)compareFloorPlan.GetRoom(0)).Identifier = 'B';
            ((TestFloorPlanGen)compareFloorPlan.GetRoom(1)).Identifier = 'C';
            ((TestFloorPlanGen)compareFloorPlan.GetRoom(2)).Identifier = 'D';

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);

            var gen = new TestFloorPlanGen('D');
            gen.PrepareDraw(new Rect(3, 3, 2, 2));

            var mockRooms = new Mock<IRandPicker<RoomGen<IFloorPlanTestContext>>>(MockBehavior.Strict);
            var mockHalls = new Mock<IRandPicker<PermissiveRoomGen<IFloorPlanTestContext>>>(MockBehavior.Strict);

            var pathGen = new SetSpecialRoomTestStep(mockRooms.Object, mockHalls.Object);
            pathGen.PlaceRoom(testRand.Object, floorPlan, gen, new RoomHallIndex(0, false));

            TestFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);
        }

        [Test]
        public void PlaceRoomOneSupport()
        {
            // needs a supporting hall for one side
            TestFloorPlan floorPlan = TestFloorPlan.InitFloorToContext(
                new Loc(22, 14),
                new Rect[] { new Rect(3, 3, 6, 6), new Rect(5, 1, 2, 2), new Rect(5, 9, 2, 2) },
                Array.Empty<Rect>(),
                new Tuple<char, char>[] { Tuple.Create('A', 'B'), Tuple.Create('A', 'C') });
            TestFloorPlan compareFloorPlan = TestFloorPlan.InitFloorToContext(
                new Loc(22, 14),
                new Rect[] { new Rect(5, 1, 2, 2), new Rect(5, 9, 2, 2), new Rect(5, 3, 2, 2) },
                new Rect[] { new Rect(5, 5, 2, 4) },
                new Tuple<char, char>[] { Tuple.Create('C', 'A'), Tuple.Create('C', 'a'), Tuple.Create('a', 'B') });
            ((TestFloorPlanGen)compareFloorPlan.GetRoom(0)).Identifier = 'B';
            ((TestFloorPlanGen)compareFloorPlan.GetRoom(1)).Identifier = 'C';
            ((TestFloorPlanGen)compareFloorPlan.GetRoom(2)).Identifier = 'D';

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);

            var gen = new TestFloorPlanGen('D');
            gen.PrepareDraw(new Rect(5, 3, 2, 2));

            var mockRooms = new Mock<IRandPicker<RoomGen<IFloorPlanTestContext>>>(MockBehavior.Strict);

            Mock<IRandPicker<PermissiveRoomGen<IFloorPlanTestContext>>> mockHalls = new Mock<IRandPicker<PermissiveRoomGen<IFloorPlanTestContext>>>(MockBehavior.Strict);
            mockHalls.Setup(p => p.Pick(testRand.Object)).Returns(new TestFloorPlanGen('a'));

            var pathGen = new SetSpecialRoomTestStep(mockRooms.Object, mockHalls.Object);
            pathGen.PlaceRoom(testRand.Object, floorPlan, gen, new RoomHallIndex(0, false));

            TestFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);
        }

        [Test]
        public void PlaceRoomAllSupport()
        {
            // needs a supporting hall for all sides
            TestFloorPlan floorPlan = TestFloorPlan.InitFloorToContext(
                new Loc(22, 14),
                new Rect[] { new Rect(3, 3, 6, 6), new Rect(5, 9, 2, 2), new Rect(1, 5, 2, 2), new Rect(5, 1, 2, 2), new Rect(9, 5, 2, 2) },
                Array.Empty<Rect>(),
                new Tuple<char, char>[] { Tuple.Create('A', 'B'), Tuple.Create('A', 'C'), Tuple.Create('A', 'D'), Tuple.Create('A', 'E') });

            TestFloorPlan compareFloorPlan;
            {
                var links = new Tuple<char, char>[]
                {
                    Tuple.Create('E', 'a'),
                    Tuple.Create('E', 'b'),
                    Tuple.Create('E', 'c'),
                    Tuple.Create('E', 'd'),
                    Tuple.Create('a', 'A'),
                    Tuple.Create('b', 'B'),
                    Tuple.Create('c', 'C'),
                    Tuple.Create('d', 'D'),
                };
                compareFloorPlan = TestFloorPlan.InitFloorToContext(
                    new Loc(22, 14),
                    new Rect[] { new Rect(5, 9, 2, 2), new Rect(1, 5, 2, 2), new Rect(5, 1, 2, 2), new Rect(9, 5, 2, 2), new Rect(5, 5, 2, 2) },
                    new Rect[] { new Rect(5, 7, 2, 2), new Rect(3, 5, 2, 2), new Rect(5, 3, 2, 2), new Rect(7, 5, 2, 2) },
                    links);
            }

            ((TestFloorPlanGen)compareFloorPlan.GetRoom(0)).Identifier = 'B';
            ((TestFloorPlanGen)compareFloorPlan.GetRoom(1)).Identifier = 'C';
            ((TestFloorPlanGen)compareFloorPlan.GetRoom(2)).Identifier = 'D';
            ((TestFloorPlanGen)compareFloorPlan.GetRoom(3)).Identifier = 'E';
            ((TestFloorPlanGen)compareFloorPlan.GetRoom(4)).Identifier = 'F';

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);

            var gen = new TestFloorPlanGen('F');
            gen.PrepareDraw(new Rect(5, 5, 2, 2));

            var mockRooms = new Mock<IRandPicker<RoomGen<IFloorPlanTestContext>>>(MockBehavior.Strict);

            Mock<IRandPicker<PermissiveRoomGen<IFloorPlanTestContext>>> mockHalls = new Mock<IRandPicker<PermissiveRoomGen<IFloorPlanTestContext>>>(MockBehavior.Strict);
            Moq.Language.ISetupSequentialResult<PermissiveRoomGen<IFloorPlanTestContext>> hallSeq = mockHalls.SetupSequence(p => p.Pick(testRand.Object));
            hallSeq = hallSeq.Returns(new TestFloorPlanGen('a'));
            hallSeq = hallSeq.Returns(new TestFloorPlanGen('b'));
            hallSeq = hallSeq.Returns(new TestFloorPlanGen('c'));
            hallSeq = hallSeq.Returns(new TestFloorPlanGen('d'));

            var pathGen = new SetSpecialRoomTestStep(mockRooms.Object, mockHalls.Object);
            pathGen.PlaceRoom(testRand.Object, floorPlan, gen, new RoomHallIndex(0, false));

            TestFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);
        }

        [Test]

        // one for each direction
        [TestCase(5, 3, Dir4.Down, 5, 5, 2, 4)]
        [TestCase(7, 5, Dir4.Left, 3, 5, 4, 2)]
        [TestCase(5, 7, Dir4.Up, 5, 3, 2, 4)]
        [TestCase(3, 5, Dir4.Right, 5, 5, 4, 2)]

        // also test offsetting
        [TestCase(4, 3, Dir4.Down, 4, 5, 3, 4)]
        [TestCase(3, 3, Dir4.Down, 3, 5, 4, 4)]
        public void GetSupportRect(int x, int y, Dir4 dir, int expectX, int expectY, int expectW, int expectH)
        {
            // the adjacent tile lines up perfectly
            // 2x2 rooms here
            TestFloorPlan floorPlan = TestFloorPlan.InitFloorToContext(
                new Loc(22, 14),
                new Rect[] { new Rect(3, 3, 6, 6), new Rect(5, 9, 2, 2), new Rect(1, 5, 2, 2), new Rect(5, 1, 2, 2), new Rect(9, 5, 2, 2) },
                Array.Empty<Rect>(),
                new Tuple<char, char>[] { Tuple.Create('A', 'B'), Tuple.Create('A', 'C'), Tuple.Create('A', 'D'), Tuple.Create('A', 'E') });

            IRoomGen oldGen = floorPlan.GetRoom(0);

            Mock<IRoomGen> mockTo = new Mock<IRoomGen>(MockBehavior.Strict);
            mockTo.SetupGet(p => p.Draw).Returns(new Rect(x, y, 2, 2));

            var indexLookup = new Dictionary<Dir4, int> { { Dir4.Down, 1 }, { Dir4.Left, 2 }, { Dir4.Up, 3 }, { Dir4.Right, 4 } };
            var adjacentsInDir = new List<RoomHallIndex> { new RoomHallIndex(indexLookup[dir], false) };

            Rect rect = SetSpecialRoomTestStep.GetSupportRect(floorPlan, oldGen, mockTo.Object, dir, adjacentsInDir);

            Assert.That(rect, Is.EqualTo(new Rect(expectX, expectY, expectW, expectH)));
        }

        [Test]
        public void GetSupportRectOffset()
        {
            // the adjacent tile crosses past the border
            TestFloorPlan floorPlan = TestFloorPlan.InitFloorToContext(
                new Loc(22, 14),
                new Rect[] { new Rect(3, 3, 6, 6), new Rect(1, 9, 4, 2) },
                Array.Empty<Rect>(),
                new Tuple<char, char>[] { Tuple.Create('A', 'B') });

            IRoomGen oldGen = floorPlan.GetRoom(0);

            Mock<IRoomGen> mockTo = new Mock<IRoomGen>(MockBehavior.Strict);
            mockTo.SetupGet(p => p.Draw).Returns(new Rect(5, 3, 2, 2));

            List<RoomHallIndex> adjacentsInDir = new List<RoomHallIndex> { new RoomHallIndex(1, false) };

            Rect rect = SetSpecialRoomTestStep.GetSupportRect(floorPlan, oldGen, mockTo.Object, Dir4.Down, adjacentsInDir);

            Assert.That(rect, Is.EqualTo(new Rect(3, 5, 4, 4)));
        }

        [Test]
        public void GetSupportRectMultiple()
        {
            // the adjacent tile crosses past the border
            TestFloorPlan floorPlan = TestFloorPlan.InitFloorToContext(
                new Loc(22, 14),
                new Rect[] { new Rect(3, 3, 6, 6), new Rect(3, 9, 2, 2), new Rect(7, 9, 2, 2) },
                Array.Empty<Rect>(),
                new Tuple<char, char>[] { Tuple.Create('A', 'B'), Tuple.Create('A', 'C') });

            IRoomGen oldGen = floorPlan.GetRoom(0);

            Mock<IRoomGen> mockTo = new Mock<IRoomGen>(MockBehavior.Strict);
            mockTo.SetupGet(p => p.Draw).Returns(new Rect(5, 3, 2, 2));

            var adjacentsInDir = new List<RoomHallIndex>
            {
                new RoomHallIndex(1, false),
                new RoomHallIndex(2, false),
            };

            Rect rect = SetSpecialRoomTestStep.GetSupportRect(floorPlan, oldGen, mockTo.Object, Dir4.Down, adjacentsInDir);

            Assert.That(rect, Is.EqualTo(new Rect(3, 5, 6, 4)));
        }

        [Test]
        public void GetPossiblePlacements()
        {
            // verify all possible positions are checked
            Mock<IRoomGen> mockFrom = new Mock<IRoomGen>(MockBehavior.Strict);
            mockFrom.SetupGet(p => p.Draw).Returns(new Rect(3, 3, 5, 6));
            Mock<IRoomGen> mockTo = new Mock<IRoomGen>(MockBehavior.Strict);
            mockTo.SetupGet(p => p.Draw).Returns(new Rect(0, 0, 3, 3));

            var adjacentsByDir = new Dictionary<Dir4, List<IRoomGen>>();
            foreach (Dir4 dir in DirExt.VALID_DIR4)
                adjacentsByDir[dir] = new List<IRoomGen>();

            var mockRooms = new Mock<IRandPicker<RoomGen<IFloorPlanTestContext>>>(MockBehavior.Strict);
            var mockHalls = new Mock<IRandPicker<PermissiveRoomGen<IFloorPlanTestContext>>>(MockBehavior.Strict);

            var pathGen = new Mock<SetSpecialRoomTestStep>(mockRooms.Object, mockHalls.Object) { CallBase = true };
            pathGen.Setup(p => p.GetAllBorderMatch(adjacentsByDir, mockTo.Object, mockFrom.Object, It.IsAny<Loc>())).Returns(1);
            SpawnList<Loc> spawns = pathGen.Object.GetPossiblePlacements(adjacentsByDir, mockTo.Object, mockFrom.Object);

            Assert.That(spawns.Count, Is.EqualTo(10));
            Assert.That(spawns.GetSpawn(0), Is.EqualTo(new Loc(3, 3)));
            Assert.That(spawns.GetSpawn(1), Is.EqualTo(new Loc(3, 6)));
            Assert.That(spawns.GetSpawn(2), Is.EqualTo(new Loc(4, 3)));
            Assert.That(spawns.GetSpawn(3), Is.EqualTo(new Loc(4, 6)));
            Assert.That(spawns.GetSpawn(4), Is.EqualTo(new Loc(5, 3)));
            Assert.That(spawns.GetSpawn(5), Is.EqualTo(new Loc(5, 6)));
            Assert.That(spawns.GetSpawn(6), Is.EqualTo(new Loc(3, 4)));
            Assert.That(spawns.GetSpawn(7), Is.EqualTo(new Loc(5, 4)));
            Assert.That(spawns.GetSpawn(8), Is.EqualTo(new Loc(3, 5)));
            Assert.That(spawns.GetSpawn(9), Is.EqualTo(new Loc(5, 5)));
        }

        [Test]
        public void GetPossiblePlacementsNarrow()
        {
            // only vertical sliding allowed
            Mock<IRoomGen> mockFrom = new Mock<IRoomGen>(MockBehavior.Strict);
            mockFrom.SetupGet(p => p.Draw).Returns(new Rect(3, 3, 4, 4));
            Mock<IRoomGen> mockTo = new Mock<IRoomGen>(MockBehavior.Strict);
            mockTo.SetupGet(p => p.Draw).Returns(new Rect(0, 0, 4, 2));

            var adjacentsByDir = new Dictionary<Dir4, List<IRoomGen>>();
            foreach (Dir4 dir in DirExt.VALID_DIR4)
                adjacentsByDir[dir] = new List<IRoomGen>();

            var mockRooms = new Mock<IRandPicker<RoomGen<IFloorPlanTestContext>>>(MockBehavior.Strict);
            var mockHalls = new Mock<IRandPicker<PermissiveRoomGen<IFloorPlanTestContext>>>(MockBehavior.Strict);

            var pathGen = new Mock<SetSpecialRoomTestStep>(mockRooms.Object, mockHalls.Object) { CallBase = true };
            pathGen.Setup(p => p.GetAllBorderMatch(adjacentsByDir, mockTo.Object, mockFrom.Object, It.IsAny<Loc>())).Returns(1);
            SpawnList<Loc> spawns = pathGen.Object.GetPossiblePlacements(adjacentsByDir, mockTo.Object, mockFrom.Object);

            Assert.That(spawns.Count, Is.EqualTo(3));
            Assert.That(spawns.GetSpawn(0), Is.EqualTo(new Loc(3, 3)));
            Assert.That(spawns.GetSpawn(1), Is.EqualTo(new Loc(3, 5)));
            Assert.That(spawns.GetSpawn(2), Is.EqualTo(new Loc(3, 4)));
        }

        [Test]
        public void GetPossiblePlacementsSingle()
        {
            // no sliding allowed
            Mock<IRoomGen> mockFrom = new Mock<IRoomGen>(MockBehavior.Strict);
            mockFrom.SetupGet(p => p.Draw).Returns(new Rect(3, 3, 4, 4));
            Mock<IRoomGen> mockTo = new Mock<IRoomGen>(MockBehavior.Strict);
            mockTo.SetupGet(p => p.Draw).Returns(new Rect(0, 0, 4, 4));

            var adjacentsByDir = new Dictionary<Dir4, List<IRoomGen>>();
            foreach (Dir4 dir in DirExt.VALID_DIR4)
                adjacentsByDir[dir] = new List<IRoomGen>();

            var mockRooms = new Mock<IRandPicker<RoomGen<IFloorPlanTestContext>>>(MockBehavior.Strict);
            var mockHalls = new Mock<IRandPicker<PermissiveRoomGen<IFloorPlanTestContext>>>(MockBehavior.Strict);

            var pathGen = new Mock<SetSpecialRoomTestStep>(mockRooms.Object, mockHalls.Object) { CallBase = true };
            pathGen.Setup(p => p.GetAllBorderMatch(adjacentsByDir, mockTo.Object, mockFrom.Object, It.IsAny<Loc>())).Returns(1);
            SpawnList<Loc> spawns = pathGen.Object.GetPossiblePlacements(adjacentsByDir, mockTo.Object, mockFrom.Object);

            Assert.That(spawns.Count, Is.EqualTo(1));
            Assert.That(spawns.GetSpawn(0), Is.EqualTo(new Loc(3, 3)));
        }

        [Test]
        public void GetPossiblePlacementsNone()
        {
            // can't place anywhere, so resort to the middle
            // verify all possible positions are checked
            Mock<IRoomGen> mockFrom = new Mock<IRoomGen>(MockBehavior.Strict);
            mockFrom.SetupGet(p => p.Draw).Returns(new Rect(3, 3, 6, 6));
            Mock<IRoomGen> mockTo = new Mock<IRoomGen>(MockBehavior.Strict);
            mockTo.SetupGet(p => p.Draw).Returns(new Rect(0, 0, 3, 3));

            var adjacentsByDir = new Dictionary<Dir4, List<IRoomGen>>();
            foreach (Dir4 dir in DirExt.VALID_DIR4)
                adjacentsByDir[dir] = new List<IRoomGen>();

            var mockRooms = new Mock<IRandPicker<RoomGen<IFloorPlanTestContext>>>(MockBehavior.Strict);
            var mockHalls = new Mock<IRandPicker<PermissiveRoomGen<IFloorPlanTestContext>>>(MockBehavior.Strict);

            var pathGen = new Mock<SetSpecialRoomTestStep>(mockRooms.Object, mockHalls.Object) { CallBase = true };
            pathGen.Setup(p => p.GetAllBorderMatch(adjacentsByDir, mockTo.Object, mockFrom.Object, It.IsAny<Loc>())).Returns(0);
            SpawnList<Loc> spawns = pathGen.Object.GetPossiblePlacements(adjacentsByDir, mockTo.Object, mockFrom.Object);

            Assert.That(spawns.Count, Is.EqualTo(4));
            Assert.That(spawns.GetSpawn(0), Is.EqualTo(new Loc(4, 4)));
            Assert.That(spawns.GetSpawn(1), Is.EqualTo(new Loc(4, 5)));
            Assert.That(spawns.GetSpawn(2), Is.EqualTo(new Loc(5, 4)));
            Assert.That(spawns.GetSpawn(3), Is.EqualTo(new Loc(5, 5)));
        }

        [Test]
        [TestCase(2, 4)]
        [TestCase(4, 2)]
        [TestCase(3, 4)]
        [TestCase(4, 3)]
        [TestCase(3, 3)]
        public void GetPossiblePlacementsNoneNoRoom(int w, int h)
        {
            // can't place anywhere, can't resort to middle
            Mock<IRoomGen> mockFrom = new Mock<IRoomGen>(MockBehavior.Strict);
            mockFrom.SetupGet(p => p.Draw).Returns(new Rect(3, 3, 4, 4));
            Mock<IRoomGen> mockTo = new Mock<IRoomGen>(MockBehavior.Strict);
            mockTo.SetupGet(p => p.Draw).Returns(new Rect(0, 0, w, h));

            var adjacentsByDir = new Dictionary<Dir4, List<IRoomGen>>();
            foreach (Dir4 dir in DirExt.VALID_DIR4)
                adjacentsByDir[dir] = new List<IRoomGen>();

            var mockRooms = new Mock<IRandPicker<RoomGen<IFloorPlanTestContext>>>(MockBehavior.Strict);
            var mockHalls = new Mock<IRandPicker<PermissiveRoomGen<IFloorPlanTestContext>>>(MockBehavior.Strict);

            var pathGen = new Mock<SetSpecialRoomTestStep>(mockRooms.Object, mockHalls.Object) { CallBase = true };
            pathGen.Setup(p => p.GetAllBorderMatch(adjacentsByDir, mockTo.Object, mockFrom.Object, It.IsAny<Loc>())).Returns(0);
            SpawnList<Loc> spawns = pathGen.Object.GetPossiblePlacements(adjacentsByDir, mockTo.Object, mockFrom.Object);

            Assert.That(spawns.Count, Is.EqualTo(0));
        }

        [Test] // various combinations of collision

        // top
        [TestCase(3, 3, 2, 2, false, true, true, false)]
        [TestCase(4, 3, 2, 2, false, false, true, false)]
        [TestCase(5, 3, 2, 2, false, false, true, true)]

        // middle
        [TestCase(3, 4, 2, 2, false, true, false, false)]
        [TestCase(5, 4, 2, 2, false, false, false, true)]

        // bottom
        [TestCase(3, 5, 2, 2, true, true, false, false)]
        [TestCase(4, 5, 2, 2, true, false, false, false)]
        [TestCase(5, 5, 2, 2, true, false, false, true)]

        // horiz
        [TestCase(3, 4, 4, 2, false, true, false, true)]

        // vert
        [TestCase(4, 3, 2, 4, true, false, true, false)]

        // three
        [TestCase(3, 3, 4, 2, false, true, true, true)]

        // all
        [TestCase(3, 3, 4, 4, true, true, true, true)]
        public void GetAllBorderMatch(int x, int y, int w, int h, bool down, bool left, bool up, bool right)
        {
            // map consists of one adjacent each side
            Mock<IRoomGen> mockFrom = new Mock<IRoomGen>(MockBehavior.Strict);
            mockFrom.SetupGet(p => p.Draw).Returns(new Rect(3, 3, 4, 4));
            Mock<IRoomGen> mockTo = new Mock<IRoomGen>(MockBehavior.Strict);
            mockTo.SetupGet(p => p.Draw).Returns(new Rect(0, 0, w, h));

            var adjacentsByDir = new Dictionary<Dir4, List<IRoomGen>>();
            foreach (Dir4 dir in DirExt.VALID_DIR4)
                adjacentsByDir[dir] = new List<IRoomGen>();

            var mockRooms = new Mock<IRandPicker<RoomGen<IFloorPlanTestContext>>>(MockBehavior.Strict);
            var mockHalls = new Mock<IRandPicker<PermissiveRoomGen<IFloorPlanTestContext>>>(MockBehavior.Strict);

            var pathGen = new Mock<SetSpecialRoomTestStep>(mockRooms.Object, mockHalls.Object) { CallBase = true };
            pathGen.Setup(p => p.GetSideBorderMatch(mockTo.Object, adjacentsByDir, It.IsAny<Loc>(), It.IsAny<Dir4>(), It.IsAny<int>())).Returns(1);

            pathGen.Object.GetAllBorderMatch(adjacentsByDir, mockTo.Object, mockFrom.Object, new Loc(x, y));

            pathGen.Verify(p => p.GetSideBorderMatch(mockTo.Object, adjacentsByDir, new Loc(x, y), Dir4.Down, It.IsAny<int>()), Times.Exactly(down ? 1 : 0));
            pathGen.Verify(p => p.GetSideBorderMatch(mockTo.Object, adjacentsByDir, new Loc(x, y), Dir4.Left, It.IsAny<int>()), Times.Exactly(left ? 1 : 0));
            pathGen.Verify(p => p.GetSideBorderMatch(mockTo.Object, adjacentsByDir, new Loc(x, y), Dir4.Up, It.IsAny<int>()), Times.Exactly(up ? 1 : 0));
            pathGen.Verify(p => p.GetSideBorderMatch(mockTo.Object, adjacentsByDir, new Loc(x, y), Dir4.Right, It.IsAny<int>()), Times.Exactly(right ? 1 : 0));
        }

        [Test]
        public void GetAllBorderMatchMin()
        {
            // confirms that the result is based on the minimum adjacent match
            Mock<IRoomGen> mockFrom = new Mock<IRoomGen>(MockBehavior.Strict);
            mockFrom.SetupGet(p => p.Draw).Returns(new Rect(3, 3, 4, 4));
            Mock<IRoomGen> mockTo = new Mock<IRoomGen>(MockBehavior.Strict);
            mockTo.SetupGet(p => p.Draw).Returns(new Rect(0, 0, 3, 3));

            var adjacentsByDir = new Dictionary<Dir4, List<IRoomGen>>();
            foreach (Dir4 dir in DirExt.VALID_DIR4)
                adjacentsByDir[dir] = new List<IRoomGen>();

            var mockRooms = new Mock<IRandPicker<RoomGen<IFloorPlanTestContext>>>(MockBehavior.Strict);
            var mockHalls = new Mock<IRandPicker<PermissiveRoomGen<IFloorPlanTestContext>>>(MockBehavior.Strict);

            var pathGen = new Mock<SetSpecialRoomTestStep>(mockRooms.Object, mockHalls.Object) { CallBase = true };
            pathGen.Setup(p => p.GetSideBorderMatch(mockTo.Object, adjacentsByDir, It.IsAny<Loc>(), Dir4.Up, It.IsAny<int>())).Returns(3);
            pathGen.Setup(p => p.GetSideBorderMatch(mockTo.Object, adjacentsByDir, It.IsAny<Loc>(), Dir4.Left, It.IsAny<int>())).Returns(2);

            int match = pathGen.Object.GetAllBorderMatch(adjacentsByDir, mockTo.Object, mockFrom.Object, new Loc(3, 3));

            Assert.That(match, Is.EqualTo(2));
        }

        [Test]
        [Ignore("TODO")]
        public void GetSideBorderMatch()
        {
            // confirms that being unable to reach one of the adjacents
            // results in failure
            throw new NotImplementedException();
        }

        public class SetSpecialRoomTestStep : SetSpecialRoomStep<IFloorPlanTestContext>
        {
            public SetSpecialRoomTestStep(IRandPicker<RoomGen<IFloorPlanTestContext>> rooms, IRandPicker<PermissiveRoomGen<IFloorPlanTestContext>> halls)
                : base(rooms, halls)
            {
            }

            public static Rect GetSupportRect(TestFloorPlan floorPlan, IRoomGen oldGen, IRoomGen newGen, Dir4 dir, List<RoomHallIndex> adjacentsInDir) => SetSpecialRoomStep<IFloorPlanTestContext>.GetSupportRect(floorPlan, oldGen, newGen, dir, adjacentsInDir);
        }
    }
}
