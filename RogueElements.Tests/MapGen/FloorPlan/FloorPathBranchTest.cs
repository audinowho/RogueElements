using System;
using System.Collections.Generic;
using NUnit.Framework;
using Moq;

namespace RogueElements.Tests
{
    [TestFixture]
    public class FloorPathBranchTest
    {
        [Test]
        public void PrepareRoomRestrained()
        {
            //confirm the room is properly downsized
            TestFloorPlan floorPlan = TestFloorPlan.InitFloorToContext(new Loc(8, 8),
                Array.Empty<Rect>(),
                Array.Empty<Rect>(),
                Array.Empty<Tuple<char, char>>());

            var testRand = new Mock<IRandom>(MockBehavior.Strict);

            var pathGen = new FloorPathBranch<IFloorPlanTestContext>();
            var roomGen = new TestFloorPlanGen('A') { ProposedSize = new Loc(20, 20) };
            var mockRooms = new Mock<IRandPicker<RoomGen<IFloorPlanTestContext>>>(MockBehavior.Strict);
            mockRooms.Setup(p => p.Pick(testRand.Object)).Returns(roomGen);
            pathGen.GenericRooms = mockRooms.Object;

            TestFloorPlanGen roomGenCompare = new TestFloorPlanGen('A');
            roomGenCompare.PrepareDraw(new Rect(-1, -1, 8, 8));

            RoomGen<IFloorPlanTestContext> chosenRoom = pathGen.PrepareRoom(testRand.Object, floorPlan, false);

            Assert.That(chosenRoom, Is.EqualTo(roomGenCompare));
        }

        [Test]
        public void ChooseRoomExpansionAlone()
        {
            //choose from a single room, add a room
            TestFloorPlan floorPlan = TestFloorPlan.InitFloorToContext(new Loc(22, 14),
                new Rect[] { new Rect(5, 5, 2, 2) },
                Array.Empty<Rect>(),
                Array.Empty<Tuple<char, char>>());


            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(1)).Returns(0);
            testRand.Setup(p => p.Next(100)).Returns(0);
            testRand.Setup(p => p.Next(64)).Returns(0);

            var pathGen = new Mock<FloorPathBranch<IFloorPlanTestContext>> { CallBase = true };
            pathGen.Object.HallPercent = 0;
            TestFloorPlanGen roomGen = new TestFloorPlanGen('A') { ProposedSize = new Loc(2, 2) };
            Mock<IRandPicker<RoomGen<IFloorPlanTestContext>>> mockRooms = new Mock<IRandPicker<RoomGen<IFloorPlanTestContext>>>(MockBehavior.Strict);
            mockRooms.Setup(p => p.Pick(testRand.Object)).Returns(roomGen);
            pathGen.Object.GenericRooms = mockRooms.Object;

            TestFloorPlanGen roomGenCompare = new TestFloorPlanGen('A');
            roomGenCompare.PrepareDraw(new Rect(4, 7, 2, 2));
            
            ListPathBranchExpansion expansion = pathGen.Object.ChooseRoomExpansion(testRand.Object, floorPlan, false);
            
            Assert.That(expansion.From, Is.EqualTo(new RoomHallIndex(0, false)));
            Assert.That(expansion.Room, Is.EqualTo(roomGenCompare));
            Assert.That(expansion.Hall, Is.EqualTo(null));
        }

        [Test]
        public void ChooseRoomExpansionAloneHall()
        {
            //choose from a single room, add a hall and a room
            TestFloorPlan floorPlan = TestFloorPlan.InitFloorToContext(new Loc(22, 14),
                new Rect[] { new Rect(5, 5, 2, 2) },
                Array.Empty<Rect>(),
                Array.Empty<Tuple<char, char>>());


            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(1)).Returns(0);
            testRand.Setup(p => p.Next(100)).Returns(0);
            testRand.Setup(p => p.Next(64)).Returns(4);
            testRand.Setup(p => p.Next(24)).Returns(4);

            var pathGen = new Mock<FloorPathBranch<IFloorPlanTestContext>> { CallBase = true };
            pathGen.Object.HallPercent = 100;

            TestFloorPlanGen hallGen = new TestFloorPlanGen('a') { ProposedSize = new Loc(2, 2) };
            Mock<IRandPicker<PermissiveRoomGen<IFloorPlanTestContext>>> mockHalls = new Mock<IRandPicker<PermissiveRoomGen<IFloorPlanTestContext>>>(MockBehavior.Strict);
            mockHalls.Setup(p => p.Pick(testRand.Object)).Returns(hallGen);
            pathGen.Object.GenericHalls = mockHalls.Object;

            TestFloorPlanGen roomGen = new TestFloorPlanGen('A') { ProposedSize = new Loc(2, 2) };
            Mock<IRandPicker<RoomGen<IFloorPlanTestContext>>> mockRooms = new Mock<IRandPicker<RoomGen<IFloorPlanTestContext>>>(MockBehavior.Strict);
            mockRooms.Setup(p => p.Pick(testRand.Object)).Returns(roomGen);
            pathGen.Object.GenericRooms = mockRooms.Object;


            TestFloorPlanGen hallGenCompare = new TestFloorPlanGen('a');
            hallGenCompare.PrepareDraw(new Rect(5, 7, 2, 2));
            TestFloorPlanGen roomGenCompare = new TestFloorPlanGen('A');
            roomGenCompare.PrepareDraw(new Rect(5, 9, 2, 2));

            ListPathBranchExpansion expansion = pathGen.Object.ChooseRoomExpansion(testRand.Object, floorPlan, false);

            Assert.That(expansion.From, Is.EqualTo(new RoomHallIndex(0, false)));
            Assert.That(expansion.Room, Is.EqualTo(roomGenCompare));
            Assert.That(expansion.Hall, Is.EqualTo(hallGenCompare));
        }


        [Test]
        public void ChooseRoomExpansionAloneBranch()
        {
            //choose from a single room
            TestFloorPlan floorPlan = TestFloorPlan.InitFloorToContext(new Loc(22, 14),
                new Rect[] { new Rect(3, 3, 2, 2) },
                Array.Empty<Rect>(),
                Array.Empty<Tuple<char, char>>());


            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);

            FloorPathBranch<IFloorPlanTestContext> pathGen = new FloorPathBranch<IFloorPlanTestContext>();
            
            ListPathBranchExpansion expansion = pathGen.ChooseRoomExpansion(testRand.Object, floorPlan, true);

            Assert.That(expansion, Is.EqualTo(null));

        }

        [Test]
        public void ChooseRoomExpansionNoMoreTries()
        {
            //choose from a single room
            TestFloorPlan floorPlan = TestFloorPlan.InitFloorToContext(new Loc(2, 2),
                new Rect[] { new Rect(1, 1, 2, 2) },
                Array.Empty<Rect>(),
                Array.Empty<Tuple<char, char>>());

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(1)).Returns(0);
            testRand.Setup(p => p.Next(100)).Returns(0);

            var pathGen = new Mock<FloorPathBranch<IFloorPlanTestContext>> { CallBase = true };
            pathGen.Object.HallPercent = 0;
            TestFloorPlanGen roomGen = new TestFloorPlanGen('A') { ProposedSize = new Loc(2, 2) };
            Mock<IRandPicker<RoomGen<IFloorPlanTestContext>>> mockRooms = new Mock<IRandPicker<RoomGen<IFloorPlanTestContext>>>(MockBehavior.Strict);
            mockRooms.Setup(p => p.Pick(testRand.Object)).Returns(roomGen);
            pathGen.Object.GenericRooms = mockRooms.Object;
            
            ListPathBranchExpansion expansion = pathGen.Object.ChooseRoomExpansion(testRand.Object, floorPlan, false);

            Assert.That(expansion, Is.EqualTo(null));
        }


        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void GetPossibleExpansionsAlone(bool branch)
        {
            TestFloorPlan floorPlan = TestFloorPlan.InitFloorToContext(new Loc(22, 14),
                new Rect[] { new Rect() },
                Array.Empty<Rect>(),
                Array.Empty<Tuple<char, char>>());


            FloorPathBranch<IFloorPlanTestContext> pathGen = new FloorPathBranch<IFloorPlanTestContext>();
            List<RoomHallIndex> roomsFrom = pathGen.GetPossibleExpansions(floorPlan, branch);
            List<RoomHallIndex> compare = new List<RoomHallIndex>();
            if (!branch)
                compare.Add(new RoomHallIndex(0, false));

            Assert.That(roomsFrom, Is.EqualTo(compare));
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void GetPossibleExpansionsDouble(bool branch)
        {
            TestFloorPlan floorPlan = TestFloorPlan.InitFloorToContext(new Loc(22, 14),
                new Rect[] { new Rect(), new Rect() },
                Array.Empty<Rect>(),
                new Tuple<char, char>[] { new Tuple<char, char>('A', 'B') });


            FloorPathBranch<IFloorPlanTestContext> pathGen = new FloorPathBranch<IFloorPlanTestContext>();
            List<RoomHallIndex> roomsFrom = pathGen.GetPossibleExpansions(floorPlan, branch);
            List<RoomHallIndex> compare = new List<RoomHallIndex>();
            if (!branch)
            {
                compare.Add(new RoomHallIndex(0, false));
                compare.Add(new RoomHallIndex(1, false));
            }

            Assert.That(roomsFrom, Is.EqualTo(compare));
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void GetPossibleExpansionsTriple(bool branch)
        {
            TestFloorPlan floorPlan = TestFloorPlan.InitFloorToContext(new Loc(22, 14),
                new Rect[] { new Rect(), new Rect(), new Rect() },
                Array.Empty<Rect>(),
                new Tuple<char, char>[] { new Tuple<char, char>('A', 'B'), new Tuple<char, char>('B', 'C') });


            FloorPathBranch<IFloorPlanTestContext> pathGen = new FloorPathBranch<IFloorPlanTestContext>();
            List<RoomHallIndex> roomsFrom = pathGen.GetPossibleExpansions(floorPlan, branch);
            List<RoomHallIndex> compare = new List<RoomHallIndex>();
            if (!branch)
            {
                compare.Add(new RoomHallIndex(0, false));
                compare.Add(new RoomHallIndex(2, false));
            }
            else
                compare.Add(new RoomHallIndex(1, false));

            Assert.That(roomsFrom, Is.EqualTo(compare));
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void GetPossibleExpansionsTripleMidHall(bool branch)
        {
            TestFloorPlan floorPlan = TestFloorPlan.InitFloorToContext(new Loc(22, 14),
                new Rect[] { new Rect(), new Rect() },
                new Rect[] { new Rect() },
                new Tuple<char, char>[] { new Tuple<char, char>('A', 'a'), new Tuple<char, char>('a', 'B') });


            FloorPathBranch<IFloorPlanTestContext> pathGen = new FloorPathBranch<IFloorPlanTestContext>();
            List<RoomHallIndex> roomsFrom = pathGen.GetPossibleExpansions(floorPlan, branch);
            List<RoomHallIndex> compare = new List<RoomHallIndex>();
            if (!branch)
            {
                compare.Add(new RoomHallIndex(0, false));
                compare.Add(new RoomHallIndex(1, false));
            }
            else
                compare.Add(new RoomHallIndex(0, true));

            Assert.That(roomsFrom, Is.EqualTo(compare));
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void GetPossibleExpansionsTripleEdgeHall(bool branch)
        {
            TestFloorPlan floorPlan = TestFloorPlan.InitFloorToContext(new Loc(22, 14),
                new Rect[] { new Rect() },
                new Rect[] { new Rect(), new Rect() },
                new Tuple<char, char>[] { new Tuple<char, char>('A', 'a'), new Tuple<char, char>('A', 'b') });


            FloorPathBranch<IFloorPlanTestContext> pathGen = new FloorPathBranch<IFloorPlanTestContext>();
            List<RoomHallIndex> roomsFrom = pathGen.GetPossibleExpansions(floorPlan, branch);
            List<RoomHallIndex> compare = new List<RoomHallIndex>();
            if (!branch)
            {
                compare.Add(new RoomHallIndex(0, true));
                compare.Add(new RoomHallIndex(1, true));
            }
            else
                compare.Add(new RoomHallIndex(0, false));

            Assert.That(roomsFrom, Is.EqualTo(compare));
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void GetPossibleExpansionsT(bool branch)
        {
            TestFloorPlan floorPlan = TestFloorPlan.InitFloorToContext(new Loc(22, 14),
                new Rect[] { new Rect(), new Rect(), new Rect(), new Rect() },
                Array.Empty<Rect>(),
                new Tuple<char, char>[] { new Tuple<char, char>('A', 'B'), new Tuple<char, char>('A', 'C'), new Tuple<char, char>('A', 'D') });


            FloorPathBranch<IFloorPlanTestContext> pathGen = new FloorPathBranch<IFloorPlanTestContext>();
            List<RoomHallIndex> roomsFrom = pathGen.GetPossibleExpansions(floorPlan, branch);
            List<RoomHallIndex> compare = new List<RoomHallIndex>();
            if (!branch)
            {
                compare.Add(new RoomHallIndex(1, false));
                compare.Add(new RoomHallIndex(2, false));
                compare.Add(new RoomHallIndex(3, false));
            }
            else
                compare.Add(new RoomHallIndex(0, false));

            Assert.That(roomsFrom, Is.EqualTo(compare));
        }


        [Test]
        [TestCase(Dir4.Down)]
        [TestCase(Dir4.Left)]
        [TestCase(Dir4.Up)]
        [TestCase(Dir4.Right)]
        public void AddLegalPlacementsNoCollision(Dir4 expandTo)
        {
            TestFloorPlan floorPlan = TestFloorPlan.InitFloorToContext(new Loc(22, 14),
                new Rect[] { new Rect(5, 5, 3, 3) },
                Array.Empty<Rect>(),
                Array.Empty<Tuple<char, char>>());
            
            TestFloorPlanGen gen = new TestFloorPlanGen('B');
            gen.PrepareDraw(new Rect(0, 0, 3, 2));
            
            FloorPathBranch<IFloorPlanTestContext> pathGen = new FloorPathBranch<IFloorPlanTestContext>();

            SpawnList<Loc> possiblePlacements = new SpawnList<Loc>();

            pathGen.AddLegalPlacements(possiblePlacements, floorPlan, new RoomHallIndex(0, false), floorPlan.GetRoom(0), gen, expandTo);

            if (expandTo == Dir4.Up)
            {
                Assert.That(possiblePlacements.GetSpawn(0), Is.EqualTo(new Loc(3, 3)));
                Assert.That(possiblePlacements.GetSpawnRate(0), Is.EqualTo(9));
                Assert.That(possiblePlacements.GetSpawn(1), Is.EqualTo(new Loc(4, 3)));
                Assert.That(possiblePlacements.GetSpawnRate(1), Is.EqualTo(18));
                Assert.That(possiblePlacements.GetSpawn(2), Is.EqualTo(new Loc(5, 3)));
                Assert.That(possiblePlacements.GetSpawnRate(2), Is.EqualTo(27));
                Assert.That(possiblePlacements.GetSpawn(3), Is.EqualTo(new Loc(6, 3)));
                Assert.That(possiblePlacements.GetSpawnRate(3), Is.EqualTo(18));
                Assert.That(possiblePlacements.GetSpawn(4), Is.EqualTo(new Loc(7, 3)));
                Assert.That(possiblePlacements.GetSpawnRate(4), Is.EqualTo(9));
                Assert.That(possiblePlacements.Count, Is.EqualTo(5));
            }
            else if (expandTo == Dir4.Down)
            {
                Assert.That(possiblePlacements.GetSpawn(0), Is.EqualTo(new Loc(3, 8)));
                Assert.That(possiblePlacements.GetSpawnRate(0), Is.EqualTo(9));
                Assert.That(possiblePlacements.GetSpawn(1), Is.EqualTo(new Loc(4, 8)));
                Assert.That(possiblePlacements.GetSpawnRate(1), Is.EqualTo(18));
                Assert.That(possiblePlacements.GetSpawn(2), Is.EqualTo(new Loc(5, 8)));
                Assert.That(possiblePlacements.GetSpawnRate(2), Is.EqualTo(27));
                Assert.That(possiblePlacements.GetSpawn(3), Is.EqualTo(new Loc(6, 8)));
                Assert.That(possiblePlacements.GetSpawnRate(3), Is.EqualTo(18));
                Assert.That(possiblePlacements.GetSpawn(4), Is.EqualTo(new Loc(7, 8)));
                Assert.That(possiblePlacements.GetSpawnRate(4), Is.EqualTo(9));
                Assert.That(possiblePlacements.Count, Is.EqualTo(5));
            }
            else if (expandTo == Dir4.Left)
            {
                Assert.That(possiblePlacements.GetSpawn(0), Is.EqualTo(new Loc(2, 4)));
                Assert.That(possiblePlacements.GetSpawnRate(0), Is.EqualTo(6));
                Assert.That(possiblePlacements.GetSpawn(1), Is.EqualTo(new Loc(2, 5)));
                Assert.That(possiblePlacements.GetSpawnRate(1), Is.EqualTo(12));
                Assert.That(possiblePlacements.GetSpawn(2), Is.EqualTo(new Loc(2, 6)));
                Assert.That(possiblePlacements.GetSpawnRate(2), Is.EqualTo(12));
                Assert.That(possiblePlacements.GetSpawn(3), Is.EqualTo(new Loc(2, 7)));
                Assert.That(possiblePlacements.GetSpawnRate(3), Is.EqualTo(6));
                Assert.That(possiblePlacements.Count, Is.EqualTo(4));
            }
            else if (expandTo == Dir4.Right)
            {
                Assert.That(possiblePlacements.GetSpawn(0), Is.EqualTo(new Loc(8, 4)));
                Assert.That(possiblePlacements.GetSpawnRate(0), Is.EqualTo(6));
                Assert.That(possiblePlacements.GetSpawn(1), Is.EqualTo(new Loc(8, 5)));
                Assert.That(possiblePlacements.GetSpawnRate(1), Is.EqualTo(12));
                Assert.That(possiblePlacements.GetSpawn(2), Is.EqualTo(new Loc(8, 6)));
                Assert.That(possiblePlacements.GetSpawnRate(2), Is.EqualTo(12));
                Assert.That(possiblePlacements.GetSpawn(3), Is.EqualTo(new Loc(8, 7)));
                Assert.That(possiblePlacements.GetSpawnRate(3), Is.EqualTo(6));
                Assert.That(possiblePlacements.Count, Is.EqualTo(4));
            }
        }


        [Test]
        [TestCase(Dir4.Left, false)]
        [TestCase(Dir4.Left, true)]
        [TestCase(Dir4.Up, false)]
        [TestCase(Dir4.Up, true)]
        public void AddLegalPlacementsCollision(Dir4 expandTo, bool isHall)
        {
            //          DDD CC
            //       BB DDD CC
            //       BBAAA
            //       BBAAA
            //         AAA

            List<Rect> rooms = new List<Rect>();
            List<Rect> halls = new List<Rect>();
            rooms.Add(new Rect(5, 5, 3, 3));
            if (!isHall)
            {
                rooms.Add(new Rect(3, 4, 2, 3));
                rooms.Add(new Rect(10, 3, 2, 2));
            }
            else
            {
                halls.Add(new Rect(3, 4, 2, 3));
                halls.Add(new Rect(10, 3, 2, 2));
            }
            TestFloorPlan floorPlan = TestFloorPlan.InitFloorToContext(new Loc(22, 14),
                rooms.ToArray(), halls.ToArray(),
                Array.Empty<Tuple<char, char>>());

            TestFloorPlanGen gen = new TestFloorPlanGen('B');
            gen.PrepareDraw(new Rect(0, 0, 3, 2));

            FloorPathBranch<IFloorPlanTestContext> pathGen = new FloorPathBranch<IFloorPlanTestContext>();

            SpawnList<Loc> possiblePlacements = new SpawnList<Loc>();

            pathGen.AddLegalPlacements(possiblePlacements, floorPlan, new RoomHallIndex(0, false), floorPlan.GetRoom(0), gen, expandTo);

            if (expandTo == Dir4.Up)
            {
                Assert.That(possiblePlacements.GetSpawn(0), Is.EqualTo(new Loc(6, 3)));
                Assert.That(possiblePlacements.GetSpawnRate(0), Is.EqualTo(18));
                Assert.That(possiblePlacements.Count, Is.EqualTo(1));
            }
            else if (expandTo == Dir4.Left)
            {
                Assert.That(possiblePlacements.Count, Is.EqualTo(0));
            }
        }


        [Test]
        public void AddLegalPlacementsCornerCollision()
        {
            //tests to verify new rooms aren't touched from corners
            //+-------
            //|AABB
            //|AABB
            //|  CC
            //|  CC
            TestFloorPlan floorPlan = TestFloorPlan.InitFloorToContext(new Loc(22, 14),
                new Rect[] { new Rect(1, 1, 2, 2), new Rect(3, 1, 2, 2) },
                Array.Empty<Rect>(),
                Array.Empty<Tuple<char, char>>());

            TestFloorPlanGen gen = new TestFloorPlanGen('C');
            gen.PrepareDraw(new Rect(0, 0, 2, 2));

            FloorPathBranch<IFloorPlanTestContext> pathGen = new FloorPathBranch<IFloorPlanTestContext>();

            SpawnList<Loc> possiblePlacements = new SpawnList<Loc>();

            pathGen.AddLegalPlacements(possiblePlacements, floorPlan, new RoomHallIndex(1, false), floorPlan.GetRoom(1), gen, Dir4.Down);

            Assert.That(possiblePlacements.GetSpawn(0), Is.EqualTo(new Loc(4, 3)));
            Assert.That(possiblePlacements.GetSpawnRate(0), Is.EqualTo(4));
            Assert.That(possiblePlacements.Count, Is.EqualTo(1));
        }


        [Test]
        public void AddLegalPlacementsBackCollision()
        {
            //tests to verify new rooms aren't touched from behind
            //+-------
            //|AA BB
            //|AA BB
            //|CCCC
            //|CCCC
            TestFloorPlan floorPlan = TestFloorPlan.InitFloorToContext(new Loc(22, 14),
                new Rect[] { new Rect(1, 1, 2, 2), new Rect(4, 1, 2, 2) },
                Array.Empty<Rect>(),
                Array.Empty<Tuple<char, char>>());

            TestFloorPlanGen gen = new TestFloorPlanGen('C');
            gen.PrepareDraw(new Rect(0, 0, 4, 2));

            FloorPathBranch<IFloorPlanTestContext> pathGen = new FloorPathBranch<IFloorPlanTestContext>();

            SpawnList<Loc> possiblePlacements = new SpawnList<Loc>();

            pathGen.AddLegalPlacements(possiblePlacements, floorPlan, new RoomHallIndex(1, false), floorPlan.GetRoom(1), gen, Dir4.Down);

            Assert.That(possiblePlacements.GetSpawn(0), Is.EqualTo(new Loc(4, 3)));
            Assert.That(possiblePlacements.GetSpawnRate(0), Is.EqualTo(16));
            Assert.That(possiblePlacements.GetSpawn(1), Is.EqualTo(new Loc(5, 3)));
            Assert.That(possiblePlacements.GetSpawnRate(1), Is.EqualTo(8));
            Assert.That(possiblePlacements.Count, Is.EqualTo(2));
        }

        [Test]
        [TestCase(2, 2, 7, 7, Dir4.Down, 0)]
        [TestCase(2, 2, 7, 7, Dir4.Left, 0)]
        [TestCase(2, 2, 7, 7, Dir4.Up, 0)]
        [TestCase(2, 2, 7, 7, Dir4.Right, 0)]
        [TestCase(1, 1, 7, 7, Dir4.Down, 4)]
        [TestCase(3, 3, 7, 7, Dir4.Left, 4)]
        [TestCase(3, 3, 7, 7, Dir4.Up, 4)]
        [TestCase(1, 1, 7, 7, Dir4.Right, 4)]
        public void AddLegalPlacementsBorderCollision(int x, int y, int w, int h, Dir4 expandTo, int possible)
        {
            TestFloorPlan floorPlan = TestFloorPlan.InitFloorToContext(new Loc(w, h),
                new Rect[] { new Rect(x, y, 3, 3) },
                Array.Empty<Rect>(),
                Array.Empty<Tuple<char, char>>());

            TestFloorPlanGen gen = new TestFloorPlanGen('B');
            gen.PrepareDraw(new Rect(0, 0, 3, 3));

            FloorPathBranch<IFloorPlanTestContext> pathGen = new FloorPathBranch<IFloorPlanTestContext>();

            SpawnList<Loc> possiblePlacements = new SpawnList<Loc>();

            pathGen.AddLegalPlacements(possiblePlacements, floorPlan, new RoomHallIndex(0, false), floorPlan.GetRoom(0), gen, expandTo);

            Assert.That(possiblePlacements.Count, Is.EqualTo(possible));
        }

        [Test]
        public void CreatePath0Percent()
        {
            TestFloorPlan floorPlan = TestFloorPlan.InitFloorToContext(new Loc(14, 10),
                Array.Empty<Rect>(),
                Array.Empty<Rect>(),
                Array.Empty<Tuple<char, char>>());

            TestFloorPlan compareFloorPlan = TestFloorPlan.InitFloorToContext(new Loc(14, 10),
                new Rect[] { new Rect(2, 3, 4, 5) },
                Array.Empty<Rect>(),
                Array.Empty<Tuple<char, char>>());

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(0, 0)).Returns(0);
            testRand.Setup(p => p.Next(0, 11)).Returns(2);
            testRand.Setup(p => p.Next(0, 6)).Returns(3);

            var pathGen = new Mock<FloorPathBranch<IFloorPlanTestContext>> { CallBase = true };
            pathGen.Object.FillPercent = new RandRange(0);
            pathGen.Object.HallPercent = 0;
            pathGen.Object.BranchRatio = new RandRange(0);
            pathGen.Object.NoForcedBranches = false;

            TestFloorPlanGen roomGen = new TestFloorPlanGen('A');
            roomGen.PrepareDraw(new Rect(0, 0, 4, 5));
            pathGen.Setup(p => p.PrepareRoom(testRand.Object, floorPlan, false)).Returns(roomGen);
            

            pathGen.Object.ApplyToPath(testRand.Object, floorPlan);

            TestFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);

            testRand.Verify(p => p.Next(0, 11), Times.Exactly(1));
            testRand.Verify(p => p.Next(0, 6), Times.Exactly(1));
            pathGen.Verify(p => p.PrepareRoom(testRand.Object, floorPlan, false), Times.Exactly(1));
        }

        [Test]
        public void CreatePath100Percent()
        {
            TestFloorPlan floorPlan = TestFloorPlan.InitFloorToContext(new Loc(20, 4),
                Array.Empty<Rect>(),
                Array.Empty<Rect>(),
                Array.Empty<Tuple<char, char>>());

            TestFloorPlan compareFloorPlan = TestFloorPlan.InitFloorToContext(new Loc(20, 4),
                new Rect[] { new Rect(0, 0, 6, 4), new Rect(7, 0, 6, 4), new Rect(14, 0, 6, 4) },
                new Rect[] { new Rect(6, 0, 1, 4), new Rect(13, 0, 1, 4) },
                new Tuple<char, char>[] { new Tuple<char, char>('A', 'a'), new Tuple<char, char>('a', 'B'), new Tuple<char, char>('B', 'b'), new Tuple<char, char>('b', 'C') });

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(0, 0)).Returns(0);
            testRand.Setup(p => p.Next(100, 100)).Returns(100);
            testRand.Setup(p => p.Next(0, 15)).Returns(0);
            testRand.Setup(p => p.Next(0, 1)).Returns(0);

            var pathGen = new Mock<FloorPathBranch<IFloorPlanTestContext>> { CallBase = true };
            pathGen.Object.FillPercent = new RandRange(100);
            pathGen.Object.HallPercent = 50;
            pathGen.Object.BranchRatio = new RandRange(0);
            pathGen.Object.NoForcedBranches = false;

            TestFloorPlanGen roomGen = new TestFloorPlanGen('A');
            roomGen.PrepareDraw(new Rect(0, 0, 6, 4));
            pathGen.Setup(p => p.PrepareRoom(testRand.Object, floorPlan, false)).Returns(roomGen);

            Moq.Language.ISetupSequentialResult<ListPathBranchExpansion> pathSeq = pathGen.SetupSequence(p => p.ChooseRoomExpansion(testRand.Object, floorPlan, false));
            {
                TestFloorPlanGen addedGen = new TestFloorPlanGen('B');
                addedGen.PrepareDraw(new Rect(7, 0, 6, 4));
                TestFloorPlanGen addedHall = new TestFloorPlanGen('a');
                addedHall.PrepareDraw(new Rect(6, 0, 1, 4));
                pathSeq = pathSeq.Returns(new ListPathBranchExpansion(new RoomHallIndex(0, false), addedGen, addedHall));
            }
            {
                TestFloorPlanGen addedGen = new TestFloorPlanGen('C');
                addedGen.PrepareDraw(new Rect(14, 0, 6, 4));
                TestFloorPlanGen addedHall = new TestFloorPlanGen('b');
                addedHall.PrepareDraw(new Rect(13, 0, 1, 4));
                pathSeq = pathSeq.Returns(new ListPathBranchExpansion(new RoomHallIndex(1, false), addedGen, addedHall));
            }
            

            pathGen.Object.ApplyToPath(testRand.Object, floorPlan);

            TestFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);

            testRand.Verify(p => p.Next(0, 15), Times.Exactly(1));
            testRand.Verify(p => p.Next(0, 1), Times.Exactly(1));
            pathGen.Verify(p => p.ChooseRoomExpansion(testRand.Object, floorPlan, false), Times.Exactly(2));
            pathGen.Verify(p => p.PrepareRoom(testRand.Object, floorPlan, false), Times.Exactly(1));
        }

        [Test]
        public void CreatePath50Percent()
        {
            TestFloorPlan floorPlan = TestFloorPlan.InitFloorToContext(new Loc(20, 12),
                Array.Empty<Rect>(),
                Array.Empty<Rect>(),
                Array.Empty<Tuple<char, char>>());

            TestFloorPlan compareFloorPlan = TestFloorPlan.InitFloorToContext(new Loc(20, 12),
                new Rect[] { new Rect(0, 0, 6, 4), new Rect(7, 0, 6, 4), new Rect(14, 0, 6, 4), new Rect(14, 5, 6, 4), new Rect(4, 5, 6, 4) },
                new Rect[] { new Rect(6, 0, 1, 4), new Rect(13, 0, 1, 4), new Rect(15, 4, 4, 1), new Rect(10, 5, 4, 4) },
                new Tuple<char, char>[] { new Tuple<char, char>('A', 'a'), new Tuple<char, char>('a', 'B'), new Tuple<char, char>('B', 'b'), new Tuple<char, char>('b', 'C'),
                    new Tuple<char, char>('C', 'c'), new Tuple<char, char>('c', 'D'), new Tuple<char, char>('D', 'd'), new Tuple<char, char>('d', 'E') });

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(0, 0)).Returns(0);
            testRand.Setup(p => p.Next(50, 50)).Returns(50);
            testRand.Setup(p => p.Next(0, 15)).Returns(0);
            testRand.Setup(p => p.Next(0, 9)).Returns(0);

            var pathGen = new Mock<FloorPathBranch<IFloorPlanTestContext>> { CallBase = true };
            pathGen.Object.FillPercent = new RandRange(50);
            pathGen.Object.HallPercent = 50;
            pathGen.Object.BranchRatio = new RandRange(0);
            pathGen.Object.NoForcedBranches = false;

            TestFloorPlanGen roomGen = new TestFloorPlanGen('A');
            roomGen.PrepareDraw(new Rect(0, 0, 6, 4));
            pathGen.Setup(p => p.PrepareRoom(testRand.Object, floorPlan, false)).Returns(roomGen);

            Moq.Language.ISetupSequentialResult<ListPathBranchExpansion> pathSeq = pathGen.SetupSequence(p => p.ChooseRoomExpansion(testRand.Object, floorPlan, false));
            {
                TestFloorPlanGen addedGen = new TestFloorPlanGen('B');
                addedGen.PrepareDraw(new Rect(7, 0, 6, 4));
                TestFloorPlanGen addedHall = new TestFloorPlanGen('a');
                addedHall.PrepareDraw(new Rect(6, 0, 1, 4));
                pathSeq = pathSeq.Returns(new ListPathBranchExpansion(new RoomHallIndex(0, false), addedGen, addedHall));
            }
            {
                TestFloorPlanGen addedGen = new TestFloorPlanGen('C');
                addedGen.PrepareDraw(new Rect(14, 0, 6, 4));
                TestFloorPlanGen addedHall = new TestFloorPlanGen('b');
                addedHall.PrepareDraw(new Rect(13, 0, 1, 4));
                pathSeq = pathSeq.Returns(new ListPathBranchExpansion(new RoomHallIndex(1, false), addedGen, addedHall));
            }
            {
                TestFloorPlanGen addedGen = new TestFloorPlanGen('D');
                addedGen.PrepareDraw(new Rect(14, 5, 6, 4));
                TestFloorPlanGen addedHall = new TestFloorPlanGen('c');
                addedHall.PrepareDraw(new Rect(15, 4, 4, 1));
                pathSeq = pathSeq.Returns(new ListPathBranchExpansion(new RoomHallIndex(2, false), addedGen, addedHall));
            }
            {
                TestFloorPlanGen addedGen = new TestFloorPlanGen('E');
                addedGen.PrepareDraw(new Rect(4, 5, 6, 4));
                TestFloorPlanGen addedHall = new TestFloorPlanGen('d');
                addedHall.PrepareDraw(new Rect(10, 5, 4, 4));
                pathSeq = pathSeq.Returns(new ListPathBranchExpansion(new RoomHallIndex(3, false), addedGen, addedHall));
            }


            pathGen.Object.ApplyToPath(testRand.Object, floorPlan);

            TestFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);

            testRand.Verify(p => p.Next(0, 15), Times.Exactly(1));
            testRand.Verify(p => p.Next(0, 9), Times.Exactly(1));
            pathGen.Verify(p => p.ChooseRoomExpansion(testRand.Object, floorPlan, false), Times.Exactly(4));
            pathGen.Verify(p => p.PrepareRoom(testRand.Object, floorPlan, false), Times.Exactly(1));
        }

        [Test]
        public void CreatePath75PercentNoFit()
        {
            //a situation in which a no-branching path
            //is forced to branch to make the room quota
            TestFloorPlan floorPlan = TestFloorPlan.InitFloorToContext(new Loc(20, 9),
                Array.Empty<Rect>(),
                Array.Empty<Rect>(),
                Array.Empty<Tuple<char, char>>());

            TestFloorPlan compareFloorPlan = TestFloorPlan.InitFloorToContext(new Loc(20, 9),
                new Rect[] { new Rect(0, 0, 6, 4), new Rect(7, 0, 6, 4), new Rect(7, 5, 6, 4), new Rect(0, 5, 6, 4), new Rect(14, 0, 6, 4) },
                new Rect[] { new Rect(6, 0, 1, 4), new Rect(8, 4, 4, 1), new Rect(6, 5, 1, 4), new Rect(13, 0, 1, 4) },
                new Tuple<char, char>[] { new Tuple<char, char>('A', 'a'), new Tuple<char, char>('a', 'B'), new Tuple<char, char>('B', 'b'), new Tuple<char, char>('b', 'C'),
                    new Tuple<char, char>('C', 'c'), new Tuple<char, char>('c', 'D'), new Tuple<char, char>('B', 'd'), new Tuple<char, char>('d', 'E') });

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(0, 0)).Returns(0);
            testRand.Setup(p => p.Next(75, 75)).Returns(75);
            testRand.Setup(p => p.Next(0, 15)).Returns(0);
            testRand.Setup(p => p.Next(0, 6)).Returns(0);

            var pathGen = new Mock<FloorPathBranch<IFloorPlanTestContext>> { CallBase = true };
            pathGen.Object.FillPercent = new RandRange(75);
            pathGen.Object.HallPercent = 50;
            pathGen.Object.BranchRatio = new RandRange(0);
            pathGen.Object.NoForcedBranches = false;

            TestFloorPlanGen roomGen = new TestFloorPlanGen('A');
            roomGen.PrepareDraw(new Rect(0, 0, 6, 4));
            pathGen.Setup(p => p.PrepareRoom(testRand.Object, floorPlan, false)).Returns(roomGen);

            Moq.Language.ISetupSequentialResult<ListPathBranchExpansion> pathSeq = pathGen.SetupSequence(p => p.ChooseRoomExpansion(testRand.Object, floorPlan, false));
            {
                TestFloorPlanGen addedGen = new TestFloorPlanGen('B');
                addedGen.PrepareDraw(new Rect(7, 0, 6, 4));
                TestFloorPlanGen addedHall = new TestFloorPlanGen('a');
                addedHall.PrepareDraw(new Rect(6, 0, 1, 4));
                pathSeq = pathSeq.Returns(new ListPathBranchExpansion(new RoomHallIndex(0, false), addedGen, addedHall));
            }
            {
                TestFloorPlanGen addedGen = new TestFloorPlanGen('C');
                addedGen.PrepareDraw(new Rect(7, 5, 6, 4));
                TestFloorPlanGen addedHall = new TestFloorPlanGen('b');
                addedHall.PrepareDraw(new Rect(8, 4, 4, 1));
                pathSeq = pathSeq.Returns(new ListPathBranchExpansion(new RoomHallIndex(1, false), addedGen, addedHall));
            }
            {
                TestFloorPlanGen addedGen = new TestFloorPlanGen('D');
                addedGen.PrepareDraw(new Rect(0, 5, 6, 4));
                TestFloorPlanGen addedHall = new TestFloorPlanGen('c');
                addedHall.PrepareDraw(new Rect(6, 5, 1, 4));
                pathSeq = pathSeq.Returns(new ListPathBranchExpansion(new RoomHallIndex(2, false), addedGen, addedHall));
            }
            {
                TestFloorPlanGen addedGen = new TestFloorPlanGen('D');
                addedGen.PrepareDraw(new Rect(0, 5, 6, 4));
                TestFloorPlanGen addedHall = new TestFloorPlanGen('c');
                addedHall.PrepareDraw(new Rect(6, 5, 1, 4));
                pathSeq = pathSeq.Returns(null);
            }
            pathSeq = pathGen.SetupSequence(p => p.ChooseRoomExpansion(testRand.Object, floorPlan, true));
            {
                TestFloorPlanGen addedGen = new TestFloorPlanGen('E');
                addedGen.PrepareDraw(new Rect(14, 0, 6, 4));
                TestFloorPlanGen addedHall = new TestFloorPlanGen('d');
                addedHall.PrepareDraw(new Rect(13, 0, 1, 4));
                pathSeq = pathSeq.Returns(new ListPathBranchExpansion(new RoomHallIndex(1, false), addedGen, addedHall));
            }


            pathGen.Object.ApplyToPath(testRand.Object, floorPlan);

            TestFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);

            testRand.Verify(p => p.Next(0, 15), Times.Exactly(1));
            testRand.Verify(p => p.Next(0, 6), Times.Exactly(1));
            pathGen.Verify(p => p.ChooseRoomExpansion(testRand.Object, floorPlan, false), Times.Exactly(4));
            pathGen.Verify(p => p.ChooseRoomExpansion(testRand.Object, floorPlan, true), Times.Exactly(1));
            pathGen.Verify(p => p.PrepareRoom(testRand.Object, floorPlan, false), Times.Exactly(1));
        }

        [Test]
        public void CreatePath75PercentNoFitCannotBranch()
        {
            //cannot make branch quota after ten tries
            TestFloorPlan floorPlan = TestFloorPlan.InitFloorToContext(new Loc(20, 9),
                Array.Empty<Rect>(),
                Array.Empty<Rect>(),
                Array.Empty<Tuple<char, char>>());

            TestFloorPlan compareFloorPlan = TestFloorPlan.InitFloorToContext(new Loc(20, 9),
                new Rect[] { new Rect(0, 0, 6, 4), new Rect(7, 0, 6, 4), new Rect(7, 5, 6, 4), new Rect(0, 5, 6, 4) },
                new Rect[] { new Rect(6, 0, 1, 4), new Rect(8, 4, 4, 1), new Rect(6, 5, 1, 4) },
                new Tuple<char, char>[] { new Tuple<char, char>('A', 'a'), new Tuple<char, char>('a', 'B'), new Tuple<char, char>('B', 'b'), new Tuple<char, char>('b', 'C'),
                    new Tuple<char, char>('C', 'c'), new Tuple<char, char>('c', 'D') });

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(0, 0)).Returns(0);
            testRand.Setup(p => p.Next(75, 75)).Returns(75);
            testRand.Setup(p => p.Next(0, 15)).Returns(0);
            testRand.Setup(p => p.Next(0, 6)).Returns(0);

            var pathGen = new Mock<FloorPathBranch<IFloorPlanTestContext>> { CallBase = true };
            pathGen.Object.FillPercent = new RandRange(75);
            pathGen.Object.HallPercent = 50;
            pathGen.Object.BranchRatio = new RandRange(0);
            pathGen.Object.NoForcedBranches = true;

            TestFloorPlanGen roomGen = new TestFloorPlanGen('A');
            roomGen.PrepareDraw(new Rect(0, 0, 6, 4));
            pathGen.Setup(p => p.PrepareRoom(testRand.Object, floorPlan, false)).Returns(roomGen);

            Moq.Language.ISetupSequentialResult<ListPathBranchExpansion> pathSeq = pathGen.SetupSequence(p => p.ChooseRoomExpansion(testRand.Object, floorPlan, false));
            for (int ii = 0; ii < 10; ii++)
            {
                {
                    TestFloorPlanGen addedGen = new TestFloorPlanGen('B');
                    addedGen.PrepareDraw(new Rect(7, 0, 6, 4));
                    TestFloorPlanGen addedHall = new TestFloorPlanGen('a');
                    addedHall.PrepareDraw(new Rect(6, 0, 1, 4));
                    pathSeq = pathSeq.Returns(new ListPathBranchExpansion(new RoomHallIndex(0, false), addedGen, addedHall));
                }
                {
                    TestFloorPlanGen addedGen = new TestFloorPlanGen('C');
                    addedGen.PrepareDraw(new Rect(7, 5, 6, 4));
                    TestFloorPlanGen addedHall = new TestFloorPlanGen('b');
                    addedHall.PrepareDraw(new Rect(8, 4, 4, 1));
                    pathSeq = pathSeq.Returns(new ListPathBranchExpansion(new RoomHallIndex(1, false), addedGen, addedHall));
                }
                {
                    TestFloorPlanGen addedGen = new TestFloorPlanGen('D');
                    addedGen.PrepareDraw(new Rect(0, 5, 6, 4));
                    TestFloorPlanGen addedHall = new TestFloorPlanGen('c');
                    addedHall.PrepareDraw(new Rect(6, 5, 1, 4));
                    pathSeq = pathSeq.Returns(new ListPathBranchExpansion(new RoomHallIndex(2, false), addedGen, addedHall));
                }
                {
                    TestFloorPlanGen addedGen = new TestFloorPlanGen('D');
                    addedGen.PrepareDraw(new Rect(0, 5, 6, 4));
                    TestFloorPlanGen addedHall = new TestFloorPlanGen('c');
                    addedHall.PrepareDraw(new Rect(6, 5, 1, 4));
                    pathSeq = pathSeq.Returns(null);
                }
            }


            pathGen.Object.ApplyToPath(testRand.Object, floorPlan);

            TestFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);

            testRand.Verify(p => p.Next(0, 15), Times.Exactly(10));
            testRand.Verify(p => p.Next(0, 6), Times.Exactly(10));
            pathGen.Verify(p => p.ChooseRoomExpansion(testRand.Object, floorPlan, false), Times.Exactly(40));
            pathGen.Verify(p => p.ChooseRoomExpansion(testRand.Object, floorPlan, true), Times.Exactly(0));
            pathGen.Verify(p => p.PrepareRoom(testRand.Object, floorPlan, false), Times.Exactly(10));
        }

        [Test]
        public void CreatePath50PercentBranch()
        {
            //A-B-C-D-F-G-I-J
            //  | | |
            //  E H K
            TestFloorPlan floorPlan = TestFloorPlan.InitFloorToContext(new Loc(32, 8),
                Array.Empty<Rect>(),
                Array.Empty<Rect>(),
                Array.Empty<Tuple<char, char>>());

            TestFloorPlan compareFloorPlan = TestFloorPlan.InitFloorToContext(new Loc(32, 8),
                new Rect[] { new Rect(0, 0, 4, 4), new Rect(4, 0, 4, 4), new Rect(8, 0, 4, 4), new Rect(12, 0, 4, 4),
                    new Rect(5, 4, 2, 4),
                    new Rect(16, 0, 4, 4), new Rect(20, 0, 4, 4),
                    new Rect(9, 4, 2, 4),
                    new Rect(24, 0, 4, 4), new Rect(28, 0, 4, 4),
                    new Rect(13, 4, 2, 4) },
                Array.Empty<Rect>(),
                new Tuple<char, char>[] { new Tuple<char, char>('A', 'B'), new Tuple<char, char>('B', 'C'), new Tuple<char, char>('C', 'D'),
                    new Tuple<char, char>('B', 'E'),
                    new Tuple<char, char>('D', 'F'), new Tuple<char, char>('F', 'G'),
                    new Tuple<char, char>('C', 'H'),
                    new Tuple<char, char>('G', 'I'), new Tuple<char, char>('I', 'J'),
                    new Tuple<char, char>('D', 'K')});

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(58, 58)).Returns(58);
            testRand.Setup(p => p.Next(50, 50)).Returns(50);
            testRand.Setup(p => p.Next(0, 29)).Returns(0);
            testRand.Setup(p => p.Next(0, 5)).Returns(0);

            var pathGen = new Mock<FloorPathBranch<IFloorPlanTestContext>> { CallBase = true };
            pathGen.Object.FillPercent = new RandRange(58);
            pathGen.Object.HallPercent = 50;
            pathGen.Object.BranchRatio = new RandRange(50);
            pathGen.Object.NoForcedBranches = false;

            TestFloorPlanGen roomGen = new TestFloorPlanGen('A');
            roomGen.PrepareDraw(new Rect(0, 0, 4, 4));
            pathGen.Setup(p => p.PrepareRoom(testRand.Object, floorPlan, false)).Returns(roomGen);

            Moq.Language.ISetupSequentialResult<ListPathBranchExpansion> pathSeq = pathGen.SetupSequence(p => p.ChooseRoomExpansion(testRand.Object, floorPlan, false));
            {
                TestFloorPlanGen addedGen = new TestFloorPlanGen('B');
                addedGen.PrepareDraw(new Rect(4, 0, 4, 4));
                pathSeq = pathSeq.Returns(new ListPathBranchExpansion(new RoomHallIndex(0, false), addedGen, null));
            }
            {
                TestFloorPlanGen addedGen = new TestFloorPlanGen('C');
                addedGen.PrepareDraw(new Rect(8, 0, 4, 4));
                pathSeq = pathSeq.Returns(new ListPathBranchExpansion(new RoomHallIndex(1, false), addedGen, null));
            }
            {
                TestFloorPlanGen addedGen = new TestFloorPlanGen('D');
                addedGen.PrepareDraw(new Rect(12, 0, 4, 4));
                pathSeq = pathSeq.Returns(new ListPathBranchExpansion(new RoomHallIndex(2, false), addedGen, null));
            }
            {
                TestFloorPlanGen addedGen = new TestFloorPlanGen('F');
                addedGen.PrepareDraw(new Rect(16, 0, 4, 4));
                pathSeq = pathSeq.Returns(new ListPathBranchExpansion(new RoomHallIndex(3, false), addedGen, null));
            }
            {
                TestFloorPlanGen addedGen = new TestFloorPlanGen('G');
                addedGen.PrepareDraw(new Rect(20, 0, 4, 4));
                pathSeq = pathSeq.Returns(new ListPathBranchExpansion(new RoomHallIndex(5, false), addedGen, null));
            }
            {
                TestFloorPlanGen addedGen = new TestFloorPlanGen('I');
                addedGen.PrepareDraw(new Rect(24, 0, 4, 4));
                pathSeq = pathSeq.Returns(new ListPathBranchExpansion(new RoomHallIndex(6, false), addedGen, null));
            }
            {
                TestFloorPlanGen addedGen = new TestFloorPlanGen('J');
                addedGen.PrepareDraw(new Rect(28, 0, 4, 4));
                pathSeq = pathSeq.Returns(new ListPathBranchExpansion(new RoomHallIndex(8, false), addedGen, null));
            }
            pathSeq = pathGen.SetupSequence(p => p.ChooseRoomExpansion(testRand.Object, floorPlan, true));
            {
                TestFloorPlanGen addedGen = new TestFloorPlanGen('E');
                addedGen.PrepareDraw(new Rect(5, 4, 2, 4));
                pathSeq = pathSeq.Returns(new ListPathBranchExpansion(new RoomHallIndex(1, false), addedGen, null));
            }
            {
                TestFloorPlanGen addedGen = new TestFloorPlanGen('H');
                addedGen.PrepareDraw(new Rect(9, 4, 2, 4));
                pathSeq = pathSeq.Returns(new ListPathBranchExpansion(new RoomHallIndex(2, false), addedGen, null));
            }
            {
                TestFloorPlanGen addedGen = new TestFloorPlanGen('K');
                addedGen.PrepareDraw(new Rect(13, 4, 2, 4));
                pathSeq = pathSeq.Returns(new ListPathBranchExpansion(new RoomHallIndex(3, false), addedGen, null));
            }

            pathGen.Object.ApplyToPath(testRand.Object, floorPlan);

            TestFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);

            testRand.Verify(p => p.Next(0, 29), Times.Exactly(1));
            testRand.Verify(p => p.Next(0, 5), Times.Exactly(1));
            pathGen.Verify(p => p.ChooseRoomExpansion(testRand.Object, floorPlan, false), Times.Exactly(7));
            pathGen.Verify(p => p.ChooseRoomExpansion(testRand.Object, floorPlan, true), Times.Exactly(3));
            pathGen.Verify(p => p.PrepareRoom(testRand.Object, floorPlan, false), Times.Exactly(1));
        }

        [Test]
        public void CreatePath50PercentBranchUsingHalls()
        {
            //A-B-C-D-F-H
            //  | | |
            //  a b c
            //  | | |
            //  E G I
            TestFloorPlan floorPlan = TestFloorPlan.InitFloorToContext(new Loc(24, 12),
                Array.Empty<Rect>(),
                Array.Empty<Rect>(),
                Array.Empty<Tuple<char, char>>());

            TestFloorPlan compareFloorPlan = TestFloorPlan.InitFloorToContext(new Loc(24, 12),
                new Rect[] { new Rect(0, 0, 4, 4), new Rect(4, 0, 4, 4), new Rect(8, 0, 4, 4), new Rect(12, 0, 4, 4),
                    new Rect(4, 8, 4, 4),
                    new Rect(16, 0, 4, 4),
                    new Rect(8, 8, 4, 4),
                    new Rect(20, 0, 4, 4),
                    new Rect(12, 8, 4, 4) },
                new Rect[] { new Rect(5, 4, 2, 4), new Rect(9, 4, 2, 4), new Rect(13, 4, 2, 4) },
                new Tuple<char, char>[] { new Tuple<char, char>('A', 'B'), new Tuple<char, char>('B', 'C'), new Tuple<char, char>('C', 'D'),
                    new Tuple<char, char>('B', 'a'), new Tuple<char, char>('a', 'E'),
                    new Tuple<char, char>('D', 'F'),
                    new Tuple<char, char>('C', 'b'), new Tuple<char, char>('b', 'G'),
                    new Tuple<char, char>('F', 'H'),
                    new Tuple<char, char>('D', 'c'), new Tuple<char, char>('c', 'I') });

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(55, 55)).Returns(55);
            testRand.Setup(p => p.Next(50, 50)).Returns(50);
            testRand.Setup(p => p.Next(0, 21)).Returns(0);
            testRand.Setup(p => p.Next(0, 9)).Returns(0);

            var pathGen = new Mock<FloorPathBranch<IFloorPlanTestContext>> { CallBase = true };
            pathGen.Object.FillPercent = new RandRange(55);
            pathGen.Object.HallPercent = 50;
            pathGen.Object.BranchRatio = new RandRange(50);
            pathGen.Object.NoForcedBranches = false;

            TestFloorPlanGen roomGen = new TestFloorPlanGen('A');
            roomGen.PrepareDraw(new Rect(0, 0, 4, 4));
            pathGen.Setup(p => p.PrepareRoom(testRand.Object, floorPlan, false)).Returns(roomGen);

            Moq.Language.ISetupSequentialResult<ListPathBranchExpansion> pathSeq = pathGen.SetupSequence(p => p.ChooseRoomExpansion(testRand.Object, floorPlan, false));
            {
                TestFloorPlanGen addedGen = new TestFloorPlanGen('B');
                addedGen.PrepareDraw(new Rect(4, 0, 4, 4));
                pathSeq = pathSeq.Returns(new ListPathBranchExpansion(new RoomHallIndex(0, false), addedGen, null));
            }
            {
                TestFloorPlanGen addedGen = new TestFloorPlanGen('C');
                addedGen.PrepareDraw(new Rect(8, 0, 4, 4));
                pathSeq = pathSeq.Returns(new ListPathBranchExpansion(new RoomHallIndex(1, false), addedGen, null));
            }
            {
                TestFloorPlanGen addedGen = new TestFloorPlanGen('D');
                addedGen.PrepareDraw(new Rect(12, 0, 4, 4));
                pathSeq = pathSeq.Returns(new ListPathBranchExpansion(new RoomHallIndex(2, false), addedGen, null));
            }
            {
                TestFloorPlanGen addedGen = new TestFloorPlanGen('F');
                addedGen.PrepareDraw(new Rect(16, 0, 4, 4));
                pathSeq = pathSeq.Returns(new ListPathBranchExpansion(new RoomHallIndex(3, false), addedGen, null));
            }
            {
                TestFloorPlanGen addedGen = new TestFloorPlanGen('H');
                addedGen.PrepareDraw(new Rect(20, 0, 4, 4));
                pathSeq = pathSeq.Returns(new ListPathBranchExpansion(new RoomHallIndex(5, false), addedGen, null));
            }
            
            pathSeq = pathGen.SetupSequence(p => p.ChooseRoomExpansion(testRand.Object, floorPlan, true));
            {
                TestFloorPlanGen addedGen = new TestFloorPlanGen('E');
                addedGen.PrepareDraw(new Rect(4, 8, 4, 4));
                TestFloorPlanGen addedHall = new TestFloorPlanGen('a');
                addedHall.PrepareDraw(new Rect(5, 4, 2, 4));
                pathSeq = pathSeq.Returns(new ListPathBranchExpansion(new RoomHallIndex(1, false), addedGen, addedHall));
            }
            {
                TestFloorPlanGen addedGen = new TestFloorPlanGen('G');
                addedGen.PrepareDraw(new Rect(8, 8, 4, 4));
                TestFloorPlanGen addedHall = new TestFloorPlanGen('b');
                addedHall.PrepareDraw(new Rect(9, 4, 2, 4));
                pathSeq = pathSeq.Returns(new ListPathBranchExpansion(new RoomHallIndex(2, false), addedGen, addedHall));
            }
            {
                TestFloorPlanGen addedGen = new TestFloorPlanGen('I');
                addedGen.PrepareDraw(new Rect(12, 8, 4, 4));
                TestFloorPlanGen addedHall = new TestFloorPlanGen('c');
                addedHall.PrepareDraw(new Rect(13, 4, 2, 4));
                pathSeq = pathSeq.Returns(new ListPathBranchExpansion(new RoomHallIndex(3, false), addedGen, addedHall));
            }


            pathGen.Object.ApplyToPath(testRand.Object, floorPlan);

            TestFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);

            testRand.Verify(p => p.Next(0, 21), Times.Exactly(1));
            testRand.Verify(p => p.Next(0, 9), Times.Exactly(1));
            pathGen.Verify(p => p.ChooseRoomExpansion(testRand.Object, floorPlan, false), Times.Exactly(5));
            pathGen.Verify(p => p.ChooseRoomExpansion(testRand.Object, floorPlan, true), Times.Exactly(3));
            pathGen.Verify(p => p.PrepareRoom(testRand.Object, floorPlan, false), Times.Exactly(1));
        }

        [Test]
        public void CreatePath100PercentBranch()
        {
            //A-B-C-E-G
            //  | | |
            //  D F H
            TestFloorPlan floorPlan = TestFloorPlan.InitFloorToContext(new Loc(20, 8),
                Array.Empty<Rect>(),
                Array.Empty<Rect>(),
                Array.Empty<Tuple<char, char>>());

            TestFloorPlan compareFloorPlan = TestFloorPlan.InitFloorToContext(new Loc(20, 8),
                new Rect[] { new Rect(0, 0, 4, 4), new Rect(4, 0, 4, 4), new Rect(8, 0, 4, 4),
                    new Rect(5, 4, 2, 4),
                    new Rect(12, 0, 4, 4),
                    new Rect(9, 4, 2, 4),
                    new Rect(16, 0, 4, 4),
                    new Rect(13, 4, 2, 4) },
                Array.Empty<Rect>(),
                new Tuple<char, char>[] { new Tuple<char, char>('A', 'B'), new Tuple<char, char>('B', 'C'),
                    new Tuple<char, char>('B', 'D'),
                    new Tuple<char, char>('C', 'E'),
                    new Tuple<char, char>('C', 'F'),
                    new Tuple<char, char>('E', 'G'),
                    new Tuple<char, char>('E', 'H')});

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(65, 65)).Returns(65);
            testRand.Setup(p => p.Next(100, 100)).Returns(100);
            testRand.Setup(p => p.Next(0, 17)).Returns(0);
            testRand.Setup(p => p.Next(0, 5)).Returns(0);

            var pathGen = new Mock<FloorPathBranch<IFloorPlanTestContext>> { CallBase = true };
            pathGen.Object.FillPercent = new RandRange(65);
            pathGen.Object.HallPercent = 50;
            pathGen.Object.BranchRatio = new RandRange(100);
            pathGen.Object.NoForcedBranches = false;

            TestFloorPlanGen roomGen = new TestFloorPlanGen('A');
            roomGen.PrepareDraw(new Rect(0, 0, 4, 4));
            pathGen.Setup(p => p.PrepareRoom(testRand.Object, floorPlan, false)).Returns(roomGen);

            Moq.Language.ISetupSequentialResult<ListPathBranchExpansion> pathSeq = pathGen.SetupSequence(p => p.ChooseRoomExpansion(testRand.Object, floorPlan, false));
            {
                TestFloorPlanGen addedGen = new TestFloorPlanGen('B');
                addedGen.PrepareDraw(new Rect(4, 0, 4, 4));
                pathSeq = pathSeq.Returns(new ListPathBranchExpansion(new RoomHallIndex(0, false), addedGen, null));
            }
            {
                TestFloorPlanGen addedGen = new TestFloorPlanGen('C');
                addedGen.PrepareDraw(new Rect(8, 0, 4, 4));
                pathSeq = pathSeq.Returns(new ListPathBranchExpansion(new RoomHallIndex(1, false), addedGen, null));
            }
            {
                TestFloorPlanGen addedGen = new TestFloorPlanGen('E');
                addedGen.PrepareDraw(new Rect(12, 0, 4, 4));
                pathSeq = pathSeq.Returns(new ListPathBranchExpansion(new RoomHallIndex(2, false), addedGen, null));
            }
            {
                TestFloorPlanGen addedGen = new TestFloorPlanGen('G');
                addedGen.PrepareDraw(new Rect(16, 0, 4, 4));
                pathSeq = pathSeq.Returns(new ListPathBranchExpansion(new RoomHallIndex(4, false), addedGen, null));
            }

            pathSeq = pathGen.SetupSequence(p => p.ChooseRoomExpansion(testRand.Object, floorPlan, true));
            {
                TestFloorPlanGen addedGen = new TestFloorPlanGen('D');
                addedGen.PrepareDraw(new Rect(5, 4, 2, 4));
                pathSeq = pathSeq.Returns(new ListPathBranchExpansion(new RoomHallIndex(1, false), addedGen, null));
            }
            {
                TestFloorPlanGen addedGen = new TestFloorPlanGen('F');
                addedGen.PrepareDraw(new Rect(9, 4, 2, 4));
                pathSeq = pathSeq.Returns(new ListPathBranchExpansion(new RoomHallIndex(2, false), addedGen, null));
            }
            {
                TestFloorPlanGen addedGen = new TestFloorPlanGen('H');
                addedGen.PrepareDraw(new Rect(13, 4, 2, 4));
                pathSeq = pathSeq.Returns(new ListPathBranchExpansion(new RoomHallIndex(4, false), addedGen, null));
            }

            pathGen.Object.ApplyToPath(testRand.Object, floorPlan);

            TestFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);

            testRand.Verify(p => p.Next(0, 17), Times.Exactly(1));
            testRand.Verify(p => p.Next(0, 5), Times.Exactly(1));
            pathGen.Verify(p => p.ChooseRoomExpansion(testRand.Object, floorPlan, false), Times.Exactly(4));
            pathGen.Verify(p => p.ChooseRoomExpansion(testRand.Object, floorPlan, true), Times.Exactly(3));
            pathGen.Verify(p => p.PrepareRoom(testRand.Object, floorPlan, false), Times.Exactly(1));
        }

        [Test]
        public void CreatePath50PercentBranchExtend()
        {
            //to confirm that newly made branches also count as terminals
            //A-B-C-D-G-J
            //  | | |
            //  E H K
            //  | | |
            //  F I L
            TestFloorPlan floorPlan = TestFloorPlan.InitFloorToContext(new Loc(24, 12),
                Array.Empty<Rect>(),
                Array.Empty<Rect>(),
                Array.Empty<Tuple<char, char>>());

            TestFloorPlan compareFloorPlan = TestFloorPlan.InitFloorToContext(new Loc(24, 12),
                new Rect[] { new Rect(0, 0, 4, 4), new Rect(4, 0, 4, 4), new Rect(8, 0, 4, 4), new Rect(12, 0, 4, 4),
                    new Rect(5, 4, 2, 4), new Rect(4, 8, 4, 4),
                    new Rect(16, 0, 4, 4),
                    new Rect(9, 4, 2, 4), new Rect(8, 8, 4, 4),
                    new Rect(20, 0, 4, 4),
                    new Rect(13, 4, 2, 4), new Rect(12, 8, 4, 4) },
                Array.Empty<Rect>(),
                new Tuple<char, char>[] { new Tuple<char, char>('A', 'B'), new Tuple<char, char>('B', 'C'), new Tuple<char, char>('C', 'D'),
                    new Tuple<char, char>('B', 'E'), new Tuple<char, char>('E', 'F'),
                    new Tuple<char, char>('D', 'G'),
                    new Tuple<char, char>('C', 'H'), new Tuple<char, char>('H', 'I'),
                    new Tuple<char, char>('G', 'J'),
                    new Tuple<char, char>('D', 'K'), new Tuple<char, char>('K', 'L') });

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(55, 55)).Returns(55);
            testRand.Setup(p => p.Next(50, 50)).Returns(50);
            testRand.Setup(p => p.Next(0, 21)).Returns(0);
            testRand.Setup(p => p.Next(0, 9)).Returns(0);

            var pathGen = new Mock<FloorPathBranch<IFloorPlanTestContext>> { CallBase = true };
            pathGen.Object.FillPercent = new RandRange(55);
            pathGen.Object.HallPercent = 50;
            pathGen.Object.BranchRatio = new RandRange(50);
            pathGen.Object.NoForcedBranches = false;

            TestFloorPlanGen roomGen = new TestFloorPlanGen('A');
            roomGen.PrepareDraw(new Rect(0, 0, 4, 4));
            pathGen.Setup(p => p.PrepareRoom(testRand.Object, floorPlan, false)).Returns(roomGen);

            Moq.Language.ISetupSequentialResult<ListPathBranchExpansion> pathSeq = pathGen.SetupSequence(p => p.ChooseRoomExpansion(testRand.Object, floorPlan, false));
            {
                TestFloorPlanGen addedGen = new TestFloorPlanGen('B');
                addedGen.PrepareDraw(new Rect(4, 0, 4, 4));
                pathSeq = pathSeq.Returns(new ListPathBranchExpansion(new RoomHallIndex(0, false), addedGen, null));
            }
            {
                TestFloorPlanGen addedGen = new TestFloorPlanGen('C');
                addedGen.PrepareDraw(new Rect(8, 0, 4, 4));
                pathSeq = pathSeq.Returns(new ListPathBranchExpansion(new RoomHallIndex(1, false), addedGen, null));
            }
            {
                TestFloorPlanGen addedGen = new TestFloorPlanGen('D');
                addedGen.PrepareDraw(new Rect(12, 0, 4, 4));
                pathSeq = pathSeq.Returns(new ListPathBranchExpansion(new RoomHallIndex(2, false), addedGen, null));
            }
            {
                TestFloorPlanGen addedGen = new TestFloorPlanGen('F');
                addedGen.PrepareDraw(new Rect(4, 8, 4, 4));
                pathSeq = pathSeq.Returns(new ListPathBranchExpansion(new RoomHallIndex(4, false), addedGen, null));
            }
            {
                TestFloorPlanGen addedGen = new TestFloorPlanGen('G');
                addedGen.PrepareDraw(new Rect(16, 0, 4, 4));
                pathSeq = pathSeq.Returns(new ListPathBranchExpansion(new RoomHallIndex(3, false), addedGen, null));
            }
            {
                TestFloorPlanGen addedGen = new TestFloorPlanGen('I');
                addedGen.PrepareDraw(new Rect(8, 8, 4, 4));
                pathSeq = pathSeq.Returns(new ListPathBranchExpansion(new RoomHallIndex(7, false), addedGen, null));
            }
            {
                TestFloorPlanGen addedGen = new TestFloorPlanGen('J');
                addedGen.PrepareDraw(new Rect(20, 0, 4, 4));
                pathSeq = pathSeq.Returns(new ListPathBranchExpansion(new RoomHallIndex(6, false), addedGen, null));
            }
            {
                TestFloorPlanGen addedGen = new TestFloorPlanGen('L');
                addedGen.PrepareDraw(new Rect(12, 8, 4, 4));
                pathSeq = pathSeq.Returns(new ListPathBranchExpansion(new RoomHallIndex(10, false), addedGen, null));
            }

            pathSeq = pathGen.SetupSequence(p => p.ChooseRoomExpansion(testRand.Object, floorPlan, true));
            {
                TestFloorPlanGen addedGen = new TestFloorPlanGen('E');
                addedGen.PrepareDraw(new Rect(5, 4, 2, 4));
                pathSeq = pathSeq.Returns(new ListPathBranchExpansion(new RoomHallIndex(1, false), addedGen, null));
            }
            {
                TestFloorPlanGen addedGen = new TestFloorPlanGen('H');
                addedGen.PrepareDraw(new Rect(9, 4, 2, 4));
                pathSeq = pathSeq.Returns(new ListPathBranchExpansion(new RoomHallIndex(2, false), addedGen, null));
            }
            {
                TestFloorPlanGen addedGen = new TestFloorPlanGen('K');
                addedGen.PrepareDraw(new Rect(13, 4, 2, 4));
                pathSeq = pathSeq.Returns(new ListPathBranchExpansion(new RoomHallIndex(3, false), addedGen, null));
            }


            pathGen.Object.ApplyToPath(testRand.Object, floorPlan);

            //check the rooms
            TestFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);

            testRand.Verify(p => p.Next(0, 21), Times.Exactly(1));
            testRand.Verify(p => p.Next(0, 9), Times.Exactly(1));
            pathGen.Verify(p => p.ChooseRoomExpansion(testRand.Object, floorPlan, false), Times.Exactly(8));
            pathGen.Verify(p => p.ChooseRoomExpansion(testRand.Object, floorPlan, true), Times.Exactly(3));
            pathGen.Verify(p => p.PrepareRoom(testRand.Object, floorPlan, false), Times.Exactly(1));
        }

        [Test]
        public void CreatePath100PercentBranchFromBranch()
        {
            //to confirm that newly made branches, if containing halls, are also eligible
            //A-B-C
            //  |
            //  a-b-E
            //  | |
            //  D F
            TestFloorPlan floorPlan = TestFloorPlan.InitFloorToContext(new Loc(17, 12),
                Array.Empty<Rect>(),
                Array.Empty<Rect>(),
                Array.Empty<Tuple<char, char>>());

            TestFloorPlan compareFloorPlan = TestFloorPlan.InitFloorToContext(new Loc(17, 12),
                new Rect[] { new Rect(0, 0, 4, 4), new Rect(4, 0, 4, 4), new Rect(8, 0, 4, 4),
                    new Rect(4, 8, 4, 4),
                    new Rect(13, 4, 4, 4),
                    new Rect(9, 7, 2, 5) },
                new Rect[] { new Rect(5, 4, 2, 4), new Rect(7, 5, 6, 2) },
                new Tuple<char, char>[] { new Tuple<char, char>('A', 'B'), new Tuple<char, char>('B', 'C'),
                    new Tuple<char, char>('B', 'a'), new Tuple<char, char>('a', 'D'),
                    new Tuple<char, char>('a', 'b'), new Tuple<char, char>('b', 'E'),
                    new Tuple<char, char>('b', 'F') });

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(50, 50)).Returns(50);
            testRand.Setup(p => p.Next(100, 100)).Returns(100);
            testRand.Setup(p => p.Next(0, 14)).Returns(0);
            testRand.Setup(p => p.Next(0, 9)).Returns(0);

            var pathGen = new Mock<FloorPathBranch<IFloorPlanTestContext>> { CallBase = true };
            pathGen.CallBase = true;
            pathGen.Object.FillPercent = new RandRange(50);
            pathGen.Object.HallPercent = 50;
            pathGen.Object.BranchRatio = new RandRange(100);
            pathGen.Object.NoForcedBranches = false;

            TestFloorPlanGen roomGen = new TestFloorPlanGen('A');
            roomGen.PrepareDraw(new Rect(0, 0, 4, 4));
            pathGen.Setup(p => p.PrepareRoom(testRand.Object, floorPlan, false)).Returns(roomGen);

            Moq.Language.ISetupSequentialResult<ListPathBranchExpansion> pathSeq = pathGen.SetupSequence(p => p.ChooseRoomExpansion(testRand.Object, floorPlan, false));
            {
                TestFloorPlanGen addedGen = new TestFloorPlanGen('B');
                addedGen.PrepareDraw(new Rect(4, 0, 4, 4));
                pathSeq = pathSeq.Returns(new ListPathBranchExpansion(new RoomHallIndex(0, false), addedGen, null));
            }
            {
                TestFloorPlanGen addedGen = new TestFloorPlanGen('C');
                addedGen.PrepareDraw(new Rect(8, 0, 4, 4));
                pathSeq = pathSeq.Returns(new ListPathBranchExpansion(new RoomHallIndex(1, false), addedGen, null));
            }

            pathSeq = pathGen.SetupSequence(p => p.ChooseRoomExpansion(testRand.Object, floorPlan, true));
            {
                TestFloorPlanGen addedGen = new TestFloorPlanGen('D');
                addedGen.PrepareDraw(new Rect(4, 8, 4, 4));
                TestFloorPlanGen addedHall = new TestFloorPlanGen('a');
                addedHall.PrepareDraw(new Rect(5, 4, 2, 4));
                pathSeq = pathSeq.Returns(new ListPathBranchExpansion(new RoomHallIndex(1, false), addedGen, addedHall));
            }
            {
                TestFloorPlanGen addedGen = new TestFloorPlanGen('E');
                addedGen.PrepareDraw(new Rect(13, 4, 4, 4));
                TestFloorPlanGen addedHall = new TestFloorPlanGen('b');
                addedHall.PrepareDraw(new Rect(7, 5, 6, 2));
                pathSeq = pathSeq.Returns(new ListPathBranchExpansion(new RoomHallIndex(0, true), addedGen, addedHall));
            }
            {
                TestFloorPlanGen addedGen = new TestFloorPlanGen('F');
                addedGen.PrepareDraw(new Rect(9, 7, 2, 5));
                pathSeq = pathSeq.Returns(new ListPathBranchExpansion(new RoomHallIndex(1, true), addedGen, null));
            }


            pathGen.Object.ApplyToPath(testRand.Object, floorPlan);

            TestFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);

            testRand.Verify(p => p.Next(0, 14), Times.Exactly(1));
            testRand.Verify(p => p.Next(0, 9), Times.Exactly(1));
            pathGen.Verify(p => p.ChooseRoomExpansion(testRand.Object, floorPlan, false), Times.Exactly(2));
            pathGen.Verify(p => p.ChooseRoomExpansion(testRand.Object, floorPlan, true), Times.Exactly(3));
            pathGen.Verify(p => p.PrepareRoom(testRand.Object, floorPlan, false), Times.Exactly(1));
        }

        [Test]
        public void CreatePath100PercentBranchWithHalls()
        {
            //A-a-B-b-D
            //  | | |
            //  C E F
            TestFloorPlan floorPlan = TestFloorPlan.InitFloorToContext(new Loc(20, 8),
                Array.Empty<Rect>(),
                Array.Empty<Rect>(),
                Array.Empty<Tuple<char, char>>());

            TestFloorPlan compareFloorPlan = TestFloorPlan.InitFloorToContext(new Loc(20, 8),
                new Rect[] { new Rect(0, 0, 4, 4), new Rect(8, 0, 4, 4),
                    new Rect(5, 4, 2, 4),
                    new Rect(16, 0, 4, 4),
                    new Rect(9, 4, 2, 4),
                    new Rect(13, 4, 2, 4)},
                new Rect[] { new Rect(4, 0, 4, 4), new Rect(12, 0, 4, 4) },
                new Tuple<char, char>[] { new Tuple<char, char>('A', 'a'), new Tuple<char, char>('a', 'B'),
                    new Tuple<char, char>('a', 'C'),
                    new Tuple<char, char>('B', 'b'),
                    new Tuple<char, char>('b', 'D'),
                    new Tuple<char, char>('B', 'E'),
                    new Tuple<char, char>('b', 'F')});

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(65, 65)).Returns(65);
            testRand.Setup(p => p.Next(100, 100)).Returns(100);
            testRand.Setup(p => p.Next(0, 17)).Returns(0);
            testRand.Setup(p => p.Next(0, 5)).Returns(0);

            var pathGen = new Mock<FloorPathBranch<IFloorPlanTestContext>> { CallBase = true };
            pathGen.CallBase = true;
            pathGen.Object.FillPercent = new RandRange(65);
            pathGen.Object.HallPercent = 50;
            pathGen.Object.BranchRatio = new RandRange(100);
            pathGen.Object.NoForcedBranches = false;

            TestFloorPlanGen roomGen = new TestFloorPlanGen('A');
            roomGen.PrepareDraw(new Rect(0, 0, 4, 4));
            pathGen.Setup(p => p.PrepareRoom(testRand.Object, floorPlan, false)).Returns(roomGen);

            Moq.Language.ISetupSequentialResult<ListPathBranchExpansion> pathSeq = pathGen.SetupSequence(p => p.ChooseRoomExpansion(testRand.Object, floorPlan, false));
            {
                TestFloorPlanGen addedGen = new TestFloorPlanGen('B');
                addedGen.PrepareDraw(new Rect(8, 0, 4, 4));
                TestFloorPlanGen addedHall = new TestFloorPlanGen('a');
                addedHall.PrepareDraw(new Rect(4, 0, 4, 4));
                pathSeq = pathSeq.Returns(new ListPathBranchExpansion(new RoomHallIndex(0, false), addedGen, addedHall));
            }
            {
                TestFloorPlanGen addedGen = new TestFloorPlanGen('D');
                addedGen.PrepareDraw(new Rect(16, 0, 4, 4));
                TestFloorPlanGen addedHall = new TestFloorPlanGen('b');
                addedHall.PrepareDraw(new Rect(12, 0, 4, 4));
                pathSeq = pathSeq.Returns(new ListPathBranchExpansion(new RoomHallIndex(1, false), addedGen, addedHall));
            }

            pathSeq = pathGen.SetupSequence(p => p.ChooseRoomExpansion(testRand.Object, floorPlan, true));
            {
                TestFloorPlanGen addedGen = new TestFloorPlanGen('C');
                addedGen.PrepareDraw(new Rect(5, 4, 2, 4));
                pathSeq = pathSeq.Returns(new ListPathBranchExpansion(new RoomHallIndex(0, true), addedGen, null));
            }
            {
                TestFloorPlanGen addedGen = new TestFloorPlanGen('E');
                addedGen.PrepareDraw(new Rect(9, 4, 2, 4));
                pathSeq = pathSeq.Returns(new ListPathBranchExpansion(new RoomHallIndex(1, false), addedGen, null));
            }
            {
                TestFloorPlanGen addedGen = new TestFloorPlanGen('F');
                addedGen.PrepareDraw(new Rect(13, 4, 2, 4));
                pathSeq = pathSeq.Returns(new ListPathBranchExpansion(new RoomHallIndex(1, true), addedGen, null));
            }

            pathGen.Object.ApplyToPath(testRand.Object, floorPlan);

            TestFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);

            testRand.Verify(p => p.Next(0, 17), Times.Exactly(1));
            testRand.Verify(p => p.Next(0, 5), Times.Exactly(1));
            pathGen.Verify(p => p.ChooseRoomExpansion(testRand.Object, floorPlan, false), Times.Exactly(2));
            pathGen.Verify(p => p.ChooseRoomExpansion(testRand.Object, floorPlan, true), Times.Exactly(3));
            pathGen.Verify(p => p.PrepareRoom(testRand.Object, floorPlan, false), Times.Exactly(1));
        }

        [Test]
        public void CreatePath400PercentBranch()
        {
            //and this is to go even further beyond
            //   D E I J N O
            //   \ / \ / \ /
            //A---B---C---H---M
            //   / \ / \ / \
            //   F G K L P Q
            TestFloorPlan floorPlan = TestFloorPlan.InitFloorToContext(new Loc(30, 4),
                Array.Empty<Rect>(),
                Array.Empty<Rect>(),
                Array.Empty<Tuple<char, char>>());

            TestFloorPlan compareFloorPlan = TestFloorPlan.InitFloorToContext(new Loc(30, 4),
                new Rect[] { new Rect(0, 1, 6, 2), new Rect(6, 1, 6, 2), new Rect(12, 1, 6, 2),
                    new Rect(7, 0, 1, 1), new Rect(9, 0, 1, 1), new Rect(7, 3, 1, 1), new Rect(9, 3, 1, 1),
                    new Rect(18, 1, 6, 2),
                    new Rect(13, 0, 1, 1), new Rect(15, 0, 1, 1), new Rect(13, 3, 1, 1), new Rect(15, 3, 1, 1),
                    new Rect(24, 1, 6, 2),
                    new Rect(19, 0, 1, 1), new Rect(21, 0, 1, 1), new Rect(19, 3, 1, 1), new Rect(21, 3, 1, 1) },
                Array.Empty<Rect>(),
                new Tuple<char, char>[] { new Tuple<char, char>('A', 'B'), new Tuple<char, char>('B', 'C'),
                    new Tuple<char, char>('B', 'D'), new Tuple<char, char>('B', 'E'), new Tuple<char, char>('B', 'F'), new Tuple<char, char>('B', 'G'),
                    new Tuple<char, char>('C', 'H'),
                    new Tuple<char, char>('C', 'I'), new Tuple<char, char>('C', 'J'), new Tuple<char, char>('C', 'K'), new Tuple<char, char>('C', 'L'),
                    new Tuple<char, char>('H', 'M'),
                    new Tuple<char, char>('H', 'N'), new Tuple<char, char>('H', 'O'), new Tuple<char, char>('H', 'P'), new Tuple<char, char>('H', 'Q'),});

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(60, 60)).Returns(60);
            testRand.Setup(p => p.Next(400, 400)).Returns(400);
            testRand.Setup(p => p.Next(0, 25)).Returns(0);
            testRand.Setup(p => p.Next(0, 3)).Returns(1);

            var pathGen = new Mock<FloorPathBranch<IFloorPlanTestContext>> { CallBase = true };
            pathGen.CallBase = true;
            pathGen.Object.FillPercent = new RandRange(60);
            pathGen.Object.HallPercent = 50;
            pathGen.Object.BranchRatio = new RandRange(400);
            pathGen.Object.NoForcedBranches = false;

            TestFloorPlanGen roomGen = new TestFloorPlanGen('A');
            roomGen.PrepareDraw(new Rect(0, 0, 6, 2));
            pathGen.Setup(p => p.PrepareRoom(testRand.Object, floorPlan, false)).Returns(roomGen);

            Moq.Language.ISetupSequentialResult<ListPathBranchExpansion> pathSeq = pathGen.SetupSequence(p => p.ChooseRoomExpansion(testRand.Object, floorPlan, false));
            {
                TestFloorPlanGen addedGen = new TestFloorPlanGen('B');
                addedGen.PrepareDraw(new Rect(6, 1, 6, 2));
                pathSeq = pathSeq.Returns(new ListPathBranchExpansion(new RoomHallIndex(0, false), addedGen, null));
            }
            {
                TestFloorPlanGen addedGen = new TestFloorPlanGen('C');
                addedGen.PrepareDraw(new Rect(12, 1, 6, 2));
                pathSeq = pathSeq.Returns(new ListPathBranchExpansion(new RoomHallIndex(1, false), addedGen, null));
            }
            {
                TestFloorPlanGen addedGen = new TestFloorPlanGen('H');
                addedGen.PrepareDraw(new Rect(18, 1, 6, 2));
                pathSeq = pathSeq.Returns(new ListPathBranchExpansion(new RoomHallIndex(2, false), addedGen, null));
            }
            {
                TestFloorPlanGen addedGen = new TestFloorPlanGen('M');
                addedGen.PrepareDraw(new Rect(24, 1, 6, 2));
                pathSeq = pathSeq.Returns(new ListPathBranchExpansion(new RoomHallIndex(7, false), addedGen, null));
            }

            pathSeq = pathGen.SetupSequence(p => p.ChooseRoomExpansion(testRand.Object, floorPlan, true));
            {
                TestFloorPlanGen addedGen = new TestFloorPlanGen('D');
                addedGen.PrepareDraw(new Rect(7, 0, 1, 1));
                pathSeq = pathSeq.Returns(new ListPathBranchExpansion(new RoomHallIndex(1, false), addedGen, null));
            }
            {
                TestFloorPlanGen addedGen = new TestFloorPlanGen('E');
                addedGen.PrepareDraw(new Rect(9, 0, 1, 1));
                pathSeq = pathSeq.Returns(new ListPathBranchExpansion(new RoomHallIndex(1, false), addedGen, null));
            }
            {
                TestFloorPlanGen addedGen = new TestFloorPlanGen('F');
                addedGen.PrepareDraw(new Rect(7, 3, 1, 1));
                pathSeq = pathSeq.Returns(new ListPathBranchExpansion(new RoomHallIndex(1, false), addedGen, null));
            }
            {
                TestFloorPlanGen addedGen = new TestFloorPlanGen('G');
                addedGen.PrepareDraw(new Rect(9, 3, 1, 1));
                pathSeq = pathSeq.Returns(new ListPathBranchExpansion(new RoomHallIndex(1, false), addedGen, null));
            }
            {
                TestFloorPlanGen addedGen = new TestFloorPlanGen('I');
                addedGen.PrepareDraw(new Rect(13, 0, 1, 1));
                pathSeq = pathSeq.Returns(new ListPathBranchExpansion(new RoomHallIndex(2, false), addedGen, null));
            }
            {
                TestFloorPlanGen addedGen = new TestFloorPlanGen('J');
                addedGen.PrepareDraw(new Rect(15, 0, 1, 1));
                pathSeq = pathSeq.Returns(new ListPathBranchExpansion(new RoomHallIndex(2, false), addedGen, null));
            }
            {
                TestFloorPlanGen addedGen = new TestFloorPlanGen('K');
                addedGen.PrepareDraw(new Rect(13, 3, 1, 1));
                pathSeq = pathSeq.Returns(new ListPathBranchExpansion(new RoomHallIndex(2, false), addedGen, null));
            }
            {
                TestFloorPlanGen addedGen = new TestFloorPlanGen('L');
                addedGen.PrepareDraw(new Rect(15, 3, 1, 1));
                pathSeq = pathSeq.Returns(new ListPathBranchExpansion(new RoomHallIndex(2, false), addedGen, null));
            }
            {
                TestFloorPlanGen addedGen = new TestFloorPlanGen('N');
                addedGen.PrepareDraw(new Rect(19, 0, 1, 1));
                pathSeq = pathSeq.Returns(new ListPathBranchExpansion(new RoomHallIndex(7, false), addedGen, null));
            }
            {
                TestFloorPlanGen addedGen = new TestFloorPlanGen('O');
                addedGen.PrepareDraw(new Rect(21, 0, 1, 1));
                pathSeq = pathSeq.Returns(new ListPathBranchExpansion(new RoomHallIndex(7, false), addedGen, null));
            }
            {
                TestFloorPlanGen addedGen = new TestFloorPlanGen('P');
                addedGen.PrepareDraw(new Rect(19, 3, 1, 1));
                pathSeq = pathSeq.Returns(new ListPathBranchExpansion(new RoomHallIndex(7, false), addedGen, null));
            }
            {
                TestFloorPlanGen addedGen = new TestFloorPlanGen('Q');
                addedGen.PrepareDraw(new Rect(21, 3, 1, 1));
                pathSeq = pathSeq.Returns(new ListPathBranchExpansion(new RoomHallIndex(7, false), addedGen, null));
            }

            pathGen.Object.ApplyToPath(testRand.Object, floorPlan);

            TestFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);

            testRand.Verify(p => p.Next(0, 25), Times.Exactly(1));
            testRand.Verify(p => p.Next(0, 3), Times.Exactly(1));
            pathGen.Verify(p => p.ChooseRoomExpansion(testRand.Object, floorPlan, false), Times.Exactly(4));
            pathGen.Verify(p => p.ChooseRoomExpansion(testRand.Object, floorPlan, true), Times.Exactly(12));
            pathGen.Verify(p => p.PrepareRoom(testRand.Object, floorPlan, false), Times.Exactly(1));
        }
    }
}
