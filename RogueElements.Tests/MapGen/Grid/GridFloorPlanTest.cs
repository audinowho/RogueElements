using System;
using System.Collections.Generic;
using NUnit.Framework;
using Moq;

namespace RogueElements.Tests
{
    [TestFixture]
    public class GridFloorPlanTest
    {
        //TODO: [Test]
        public void GetHall()
        {
            //get existent hall
            //get nonexistent hall, should return null
            throw new NotImplementedException();
        }

        //TODO: [Test]
        public void GetRoom()
        {
            //get existent room
            //get nonexistent room, should return null
            //get a big room
            throw new NotImplementedException();
        }

        //TODO: [Test]
        public void EraseRoom()
        {
            //erase existent room
            //erase nonexistent room (same deal)
            //erase a big room; the whole room should be gone
            throw new NotImplementedException();
        }

        //TODO: [Test]
        public void IsRoomOpen()
        {
            //erase existent room
            //erase nonexistent room (same deal)
            //erase a big room; the whole room should be gone
            throw new NotImplementedException();
        }

        [Test]
        public void AddRoomToEmptySpace()
        {
            //add to empty space
            string[] inGrid = { "0.0.0",
                                ". . .",
                                "0.0.0"};

            string[] outGrid ={ "0.A.0",
                                ". . .",
                                "0.0.0"};
            TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid);
            TestGridRoomGen gen = new TestGridRoomGen('A');
            floorPlan.AddRoom(new Rect(1, 0, 1, 1), gen);
            TestGridFloorPlan compareFloorPlan = TestGridFloorPlan.InitGridToContext(outGrid);

            TestGridFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);

        }

        [Test]
        public void AddBigRoomToEmptySpace()
        {
            //add big room to empty space
            string[] inGrid = { "0.0.0.0",
                                ". . . .",
                                "0.0.0.0",
                                ". . . .",
                                "0.0.0.0"};

            string[] outGrid ={ "0.0.0.0",
                                ". . . .",
                                "0.A.A.A",
                                ". . . .",
                                "0.A.A.A"};
            TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid);
            TestGridRoomGen gen = new TestGridRoomGen('A');
            floorPlan.AddRoom(new Rect(1, 1, 3, 2), gen);
            TestGridFloorPlan compareFloorPlan = TestGridFloorPlan.InitGridToContext(outGrid);

            TestGridFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);

        }

        [Test]
        public void AddCrossingRooms()
        {
            //add small room on big room
            string[] inGrid = { "0.0.0.0",
                                ". . . .",
                                "0.A.A.A",
                                ". . . .",
                                "0.A.A.A"};
            TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid);
            TestGridRoomGen gen = new TestGridRoomGen('B');
            Assert.Throws<InvalidOperationException>(() => { floorPlan.AddRoom(new Rect(1, 0, 2, 2), gen); });

        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void AddBigRoomToHall(int gridType)
        {
            //add small room on big room
            string[] inGrid = null;

            if (gridType == 0)
            {
                inGrid = new string[] { "0.0.0.0",
                                        ". . . .",
                                        "0.0#0.0",
                                        ". . . .",
                                        "0.0.0.0"};
            }
            else if (gridType == 1)
            {

                inGrid = new string[] { "0.0.0.0",
                                        ". . . .",
                                        "0.0.0.0",
                                        ". # . .",
                                        "0.0.0.0"};
            }
            else if (gridType == 2)
            {

                inGrid = new string[] { "0.0.0.0",
                                        ". . . .",
                                        "0.0.0.0",
                                        ". . # .",
                                        "0.0.0.0"};
            }
            else if (gridType == 3)
            {

                inGrid = new string[] { "0.0.0.0",
                                        ". . . .",
                                        "0.0.0.0",
                                        ". . . .",
                                        "0.0#0.0"};
            }

            TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid);
            TestGridRoomGen gen = new TestGridRoomGen('A');
            Assert.Throws<InvalidOperationException>(() => { floorPlan.AddRoom(new Rect(1, 1, 2, 2), gen); });
            
        }

        [Test]
        public void AddBigRoomToSurrounded()
        {
            //add room to a room/hall-ridden floor where halls just barely avoid the room
            string[] inGrid = { "A#B#C#D",
                                "# # # #",
                                "E#0.0#F",
                                "# . . #",
                                "G#0.0#H"};
            string[] outGrid ={ "A#B#C#D",
                                "# # # #",
                                "E#I.I#F",
                                "# . . #",
                                "G#I.I#H"};
            TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid);
            TestGridRoomGen gen = new TestGridRoomGen('I');
            floorPlan.AddRoom(new Rect(1, 1, 2, 2), gen);

            TestGridFloorPlan compareFloorPlan = TestGridFloorPlan.InitGridToContext(outGrid);

            TestGridFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);

        }

        [Test]
        [TestCase(-1, 0, 2, 2)]
        [TestCase(0, 0, 2, 4)]
        public void AddRoomOutOfRange(int x, int y, int w, int h)
        {
            //out of range
            string[] inGrid = { "0.0.0.0",
                                ". . . .",
                                "0.0.0.0",
                                ". . . .",
                                "0.0.0.0"};
            TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid);
            TestGridRoomGen gen = new TestGridRoomGen('A');
            Assert.Throws<ArgumentOutOfRangeException>(() => { floorPlan.AddRoom(new Rect(x, y, w, h), gen); });

        }



        [Test]
        [TestCase(Dir4.Down, 0)]
        [TestCase(Dir4.Left, 1)]
        [TestCase(Dir4.Up, 2)]
        [TestCase(Dir4.Right, 3)]
        [TestCase(Dir4.None, -1)]
        [TestCase((Dir4)4, -1)]
        public void SetHallInDirs(Dir4 dir, int expectedOut)
        {
            //valid/invalid dir
            string[] inGrid = { "0.0.0.0",
                                ". . . .",
                                "0.0.0.0",
                                ". . . .",
                                "0.0.0.0"};

            string[] outGrid = null;
            bool exception = false;
            if (expectedOut == 0)
            {
                outGrid = new string[]{ "0.0.0.0",
                                        ". . . .",
                                        "0.0.0.0",
                                        ". # . .",
                                        "0.0.0.0"};
            }
            else if (expectedOut == 1)
            {
                outGrid = new string[]{ "0.0.0.0",
                                        ". . . .",
                                        "0#0.0.0",
                                        ". . . .",
                                        "0.0.0.0"};
            }
            else if (expectedOut == 2)
            {
                outGrid = new string[]{ "0.0.0.0",
                                        ". # . .",
                                        "0.0.0.0",
                                        ". . . .",
                                        "0.0.0.0"};
            }
            else if (expectedOut == 3)
            {
                outGrid = new string[]{ "0.0.0.0",
                                        ". . . .",
                                        "0.0#0.0",
                                        ". . . .",
                                        "0.0.0.0"};
            }
            else
                exception = true;

            TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid);
            TestGridRoomGen gen = new TestGridRoomGen((char)0);
            if (exception)
            {
                Assert.Throws<ArgumentException>(() => { floorPlan.SetHall(new LocRay4(1, 1, dir), gen); });
                return;
            }
            else
                floorPlan.SetHall(new LocRay4(1, 1, dir), gen);
            TestGridFloorPlan compareFloorPlan = TestGridFloorPlan.InitGridToContext(outGrid);

            TestGridFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);

            //set existing hall to null
        }
        [Test]
        public void SetHallToNull()
        {
            //set existing hall to null
            string[] inGrid = { "0.0.0.0",
                                ". . . .",
                                "0.0#0.0",
                                ". . . .",
                                "0.0.0.0"};

            string[] outGrid ={ "0.0.0.0",
                                ". . . .",
                                "0.0.0.0",
                                ". . . .",
                                "0.0.0.0"};
            
            TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid);
            floorPlan.SetHall(new LocRay4(1, 1, Dir4.Right), null);
            TestGridFloorPlan compareFloorPlan = TestGridFloorPlan.InitGridToContext(outGrid);

            TestGridFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);
        }

        [Test]
        //valid adjacent rooms
        [TestCase(1, 1, 2, 1, 0)]
        [TestCase(1, 1, 1, 2, 1)]
        [TestCase(2, 1, 1, 1, 0)]
        [TestCase(1, 2, 1, 1, 1)]
        //non-adjacent rooms
        [TestCase(1, 1, 1, 1, -1)]
        [TestCase(1, 1, 3, 1, -1)]
        //diagonals
        [TestCase(1, 1, 2, 2, -1)]
        [TestCase(1, 1, 0, 2, -1)]
        public void SetConnectingHall(int x1, int y1, int x2, int y2, int expected)
        {
            string[] inGrid = { "0.0.0.0",
                                ". . . .",
                                "0.0.0.0",
                                ". . . .",
                                "0.0.0.0"};

            string[] outGrid = null;
            bool exception = false;
            if (expected == 0)
            {
                outGrid = new string[]{ "0.0.0.0",
                                        ". . . .",
                                        "0.0#0.0",
                                        ". . . .",
                                        "0.0.0.0"};
            }
            else if (expected == 1)
            {
                outGrid = new string[]{ "0.0.0.0",
                                        ". . . .",
                                        "0.0.0.0",
                                        ". # . .",
                                        "0.0.0.0"};
            }
            else
                exception = true;
            
            TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid);
            TestGridRoomGen gen = new TestGridRoomGen((char)0);
            if (exception)
            {
                Assert.Throws<ArgumentException>(() => { floorPlan.SetConnectingHall(new Loc(x1, y1), new Loc(x2, y2), gen); });
                return;
            }
            else
                floorPlan.SetConnectingHall(new Loc(x1, y1), new Loc(x2, y2), gen);
            TestGridFloorPlan compareFloorPlan = TestGridFloorPlan.InitGridToContext(outGrid);

            TestGridFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);
        }

        [Test]
        //normal size
        [TestCase(8, 5, 6, 4, 6, 4, 3, 2, 0, 0)]
        //normal size offset
        [TestCase(8, 5, 6, 4, 6, 4, 3, 2, 2, 1)]
        //exceeding size
        [TestCase(8, 5, 9, 6, 8, 5, 1, 1, 0, 0)]
        //exceeding size in multi-room size
        [TestCase(8, 11, 9, 6, 8, 6, 1, 6, 0, 4)]
        public void ChooseRoomBounds(int boundW, int boundH, int tryW, int tryH, int w, int h, int rMaxX, int rMaxY, int randX, int randY)
        {
            int boundX = 10;
            int boundY = 13;
            Mock<TestGridFloorPlan> floorPlan = new Mock<TestGridFloorPlan>() { CallBase = true };
            floorPlan.Object.InitSize(4, 4, 8, 5);
            floorPlan.Setup(p => p.GetCellBounds(new Rect())).Returns(new Rect(boundX, boundY, boundW, boundH));
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            Moq.Language.ISetupSequentialResult<int> seq = testRand.SetupSequence(p => p.Next(rMaxX));
            seq = seq.Returns(randX);
            if (rMaxX == rMaxY)
                seq = seq.Returns(randY);
            else
            {
                Moq.Language.ISetupSequentialResult<int> seq2 = testRand.SetupSequence(p => p.Next(rMaxY));
                seq2 = seq2.Returns(randY);
            }
            
            Mock<IRoomGen> mockRoom = new Mock<IRoomGen>(MockBehavior.Strict);
            mockRoom.Setup(p => p.ProposeSize(testRand.Object)).Returns(new Loc(tryW, tryH));
            mockRoom.Setup(p => p.PrepareSize(testRand.Object, new Loc(w, h)));
            int x = boundX + randX;
            int y = boundY + randY;
            mockRoom.Setup(p => p.SetLoc(new Loc(x, y)));

            //manually place the mock object instead of using AddRoom to copy it over
            floorPlan.Object.PublicArrayRooms.Add(new GridRoomPlan(new Rect(), mockRoom.Object));
            

            floorPlan.Object.ChooseRoomBounds(testRand.Object, 0);


            //verify all were called
            if (rMaxX == rMaxY)
                testRand.Verify(p => p.Next(rMaxX), Times.Exactly(2));
            else
            {
                testRand.Verify(p => p.Next(rMaxX), Times.Exactly(1));
                testRand.Verify(p => p.Next(rMaxY), Times.Exactly(1));
            }
            mockRoom.Verify(p => p.ProposeSize(It.IsAny<IRandom>()), Times.Exactly(1));
            mockRoom.Verify(p => p.PrepareSize(It.IsAny<IRandom>(), It.IsAny<Loc>()), Times.Exactly(1));
            mockRoom.Verify(p => p.SetLoc(It.IsAny<Loc>()), Times.Exactly(1));
        }

        [Test]
        public void ChooseHallBounds()
        {
            //verify the range is the OR of both rooms
            Mock<TestGridFloorPlan> floorPlan = new Mock<TestGridFloorPlan>() { CallBase = true };
            floorPlan.Object.InitSize(3, 4, 5, 5);
            Mock<IRandom> mockRand = new Mock<IRandom>(MockBehavior.Strict);
            Mock<IRoomGen> mockRoom1 = new Mock<IRoomGen>(MockBehavior.Strict);
            mockRoom1.SetupGet(p => p.Draw).Returns(new Rect(6,6,3,2));
            //manually place the mock object instead of using AddRoom to copy it over
            floorPlan.Object.PublicArrayRooms.Add(new GridRoomPlan(new Rect(1, 1, 1, 1), mockRoom1.Object));
            floorPlan.Object.PublicRooms[1][1] = 0;

            Mock<IRoomGen> mockRoom2 = new Mock<IRoomGen>(MockBehavior.Strict);
            mockRoom2.SetupGet(p => p.Draw).Returns(new Rect(8,14,3,2));
            //manually place the mock object instead of using AddRoom to copy it over
            floorPlan.Object.PublicArrayRooms.Add(new GridRoomPlan(new Rect(1, 1, 1, 1), mockRoom2.Object));
            floorPlan.Object.PublicRooms[1][2] = 1;

            Mock<IPermissiveRoomGen> mockHall = new Mock<IPermissiveRoomGen>(MockBehavior.Strict);
            mockHall.Setup(p => p.SetLoc(new Loc(6, 8)));
            mockHall.Setup(p => p.PrepareSize(mockRand.Object, new Loc(5,6)));
            floorPlan.Object.PublicVHalls[1][1].SetGen(mockHall.Object);
            
            floorPlan.Setup(p => p.GetHallTouchRange(mockRoom1.Object, Dir4.Down, 1)).Returns(new Range(6, 9));
            floorPlan.Setup(p => p.GetHallTouchRange(mockRoom2.Object, Dir4.Up, 1)).Returns(new Range(8, 11));

            floorPlan.Object.ChooseHallBounds(mockRand.Object, 1, 1, true);
            
            mockHall.Verify(p => p.SetLoc(It.IsAny<Loc>()), Times.Exactly(1));
            mockHall.Verify(p => p.PrepareSize(It.IsAny<IRandom>(), It.IsAny<Loc>()), Times.Exactly(1));
        }

        [Test]
        public void PlaceRoomsOnFloorRing()
        {
            //place a ring of rooms connected by halls
            string[] inGrid = { "A#B",
                                "# #",
                                "D#C"};
            TestGridFloorPlan gridPlan = TestGridFloorPlan.InitGridToContext(inGrid, 5, 5);
            for (int ii = 0; ii < gridPlan.RoomCount; ii++)
            {
                TestFloorPlanGen gen = new TestFloorPlanGen(((TestGridRoomGen)gridPlan.GetRoom(ii)).Identifier);
                gen.PrepareProposeSize(new Loc(5, 5));
                gridPlan.PublicArrayRooms[ii].RoomGen = gen;
            }
            gridPlan.PublicVHalls[0][0].SetGen(new TestFloorPlanGen('a'));
            gridPlan.PublicVHalls[1][0].SetGen(new TestFloorPlanGen('b'));
            gridPlan.PublicHHalls[0][0].SetGen(new TestFloorPlanGen('c'));
            gridPlan.PublicHHalls[0][1].SetGen(new TestFloorPlanGen('d'));

            TestFloorPlan compareFloorPlan = TestFloorPlan.InitFloorToContext(gridPlan.Size,
                new Rect[] { new Rect(0, 0, 5, 5), new Rect(6, 0, 5, 5),
                            new Rect(6, 6, 5, 5), new Rect(0, 6, 5, 5) },
                new Rect[] { new Rect(0, 5, 5, 1), new Rect(6, 5, 5, 1),
                            new Rect(5, 0, 1, 5), new Rect(5, 6, 1, 5) },
                new Tuple<char, char>[] { new Tuple<char, char>('A', 'a'), new Tuple<char, char>('a', 'D'),
                                        new Tuple<char, char>('B', 'b'), new Tuple<char, char>('b', 'C'),
                                        new Tuple<char, char>('A', 'c'), new Tuple<char, char>('c', 'B'),
                                        new Tuple<char, char>('D', 'd'), new Tuple<char, char>('d', 'C')});

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(It.IsAny<int>())).Returns(0);

            TestFloorPlan floorPlan = new TestFloorPlan();
            floorPlan.InitSize(gridPlan.Size);

            Mock<IFloorPlanTestContext> mockMap = new Mock<IFloorPlanTestContext>(MockBehavior.Strict);
            mockMap.SetupGet(p => p.Rand).Returns(testRand.Object);
            mockMap.SetupGet(p => p.RoomPlan).Returns(floorPlan);

            gridPlan.PlaceRoomsOnFloor(mockMap.Object);

            
            TestFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);
        }

        [Test]
        public void PlaceRoomsOnFloorDefault()
        {
            //place a line of rooms with one default
            string[] inGrid = { "A#B#C",
                                ". . .",
                                "0.0.0"};
            TestGridFloorPlan gridPlan = TestGridFloorPlan.InitGridToContext(inGrid, 5, 5);

            {
                TestFloorPlanGen gen = new TestFloorPlanGen('A');
                gen.PrepareProposeSize(new Loc(5, 5));
                gridPlan.PublicArrayRooms[0].RoomGen = gen;
            }
            {
                gridPlan.PublicArrayRooms[1].RoomGen = new RoomGenDefault<IFloorPlanTestContext>();
            }
            {
                TestFloorPlanGen gen = new TestFloorPlanGen('B');
                gen.PrepareProposeSize(new Loc(5, 5));
                gridPlan.PublicArrayRooms[2].RoomGen = gen;
            }
            gridPlan.PublicHHalls[0][0].SetGen(new TestFloorPlanGen('b'));
            gridPlan.PublicHHalls[1][0].SetGen(new TestFloorPlanGen('c'));

            TestFloorPlan compareFloorPlan = TestFloorPlan.InitFloorToContext(gridPlan.Size,
                new Rect[] { new Rect(0, 0, 5, 5), new Rect(12, 0, 5, 5) },
                new Rect[] { new Rect(6, 0, 1, 1), new Rect(5, 0, 1, 5), new Rect(7, 0, 5, 5) },
                new Tuple<char, char>[] { new Tuple<char, char>('A', 'b'), new Tuple<char, char>('b', 'a'),
                                        new Tuple<char, char>('a', 'c'), new Tuple<char, char>('c', 'B')});
            
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(It.IsAny<int>())).Returns(0);

            TestFloorPlan floorPlan = new TestFloorPlan();
            floorPlan.InitSize(gridPlan.Size);

            Mock<IFloorPlanTestContext> mockMap = new Mock<IFloorPlanTestContext>(MockBehavior.Strict);
            mockMap.SetupGet(p => p.Rand).Returns(testRand.Object);
            mockMap.SetupGet(p => p.RoomPlan).Returns(floorPlan);

            gridPlan.PlaceRoomsOnFloor(mockMap.Object);


            //check the rooms
            Assert.That(floorPlan.RoomCount, Is.EqualTo(compareFloorPlan.RoomCount));
            for (int ii = 0; ii < floorPlan.RoomCount; ii++)
            {
                FloorRoomPlan plan = floorPlan.PublicRooms[ii];
                FloorRoomPlan comparePlan = compareFloorPlan.PublicRooms[ii];
                Assert.That(plan.RoomGen, Is.EqualTo(comparePlan.RoomGen));
                Assert.That(plan.Adjacents, Is.EqualTo(comparePlan.Adjacents));
            }
            Assert.That(floorPlan.HallCount, Is.EqualTo(compareFloorPlan.HallCount));
            for (int ii = 0; ii < floorPlan.HallCount; ii++)
            {
                FloorHallPlan plan = floorPlan.PublicHalls[ii];
                FloorHallPlan comparePlan = compareFloorPlan.PublicHalls[ii];
                if (ii != 0)
                    Assert.That(plan.RoomGen, Is.EqualTo(comparePlan.RoomGen));
                else
                {
                    //special case for the default
                    Assert.That(plan.RoomGen, Is.TypeOf<RoomGenDefault<IFloorPlanTestContext>>());
                    Assert.That(plan.RoomGen.Draw, Is.EqualTo(comparePlan.RoomGen.Draw));
                }
                Assert.That(plan.Adjacents, Is.EqualTo(comparePlan.Adjacents));
            }
        }



        [Test]
        public void PlaceRoomsOnFloorIntrusiveHalls()
        {
            //place a ring of rooms connected by halls
            string[] inGrid = { "A.0",
                                ". .",
                                "A#B",
                                ". .",
                                "0.B"};
            TestGridFloorPlan gridPlan = TestGridFloorPlan.InitGridToContext(inGrid, 5, 5);
            for (int ii = 0; ii < gridPlan.RoomCount; ii++)
            {
                TestFloorPlanGen gen = new TestFloorPlanGen(((TestGridRoomGen)gridPlan.GetRoom(ii)).Identifier);
                gen.PrepareProposeSize(new Loc(2, 2));
                gridPlan.PublicArrayRooms[ii].RoomGen = gen;
            }
            gridPlan.PublicHHalls[0][1].SetGen(new TestFloorPlanGen('a'));

            TestFloorPlan compareFloorPlan = TestFloorPlan.InitFloorToContext(gridPlan.Size,
                new Rect[] { new Rect(0, 0, 2, 2), new Rect(9, 15, 2, 2) },
                new Rect[] { new Rect(2, 1, 4, 10), new Rect(6, 6, 3, 10) },
                new Tuple<char, char>[] { new Tuple<char, char>('A', 'a'), new Tuple<char, char>('a', 'b'), new Tuple<char, char>('b', 'B') });
            ((TestFloorPlanGen)compareFloorPlan.PublicHalls[1].Gen).Identifier = 'a';

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            Moq.Language.ISetupSequentialResult<int> seq = testRand.SetupSequence(p => p.Next(4));
            seq = seq.Returns(0);
            seq = seq.Returns(3);
            seq = testRand.SetupSequence(p => p.Next(10));
            seq = seq.Returns(0);
            seq = seq.Returns(9);

            TestFloorPlan floorPlan = new TestFloorPlan();
            floorPlan.InitSize(gridPlan.Size);

            Mock<IFloorPlanTestContext> mockMap = new Mock<IFloorPlanTestContext>(MockBehavior.Strict);
            mockMap.SetupGet(p => p.Rand).Returns(testRand.Object);
            mockMap.SetupGet(p => p.RoomPlan).Returns(floorPlan);

            gridPlan.PlaceRoomsOnFloor(mockMap.Object);

            TestFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);
        }


        [Test]
        public void PlaceRoomsOnFloorIntrusiveHallsAAonly()
        {
            //place a ring of rooms connected by halls
            string[] inGrid = { "A.0",
                                ". .",
                                "A#B",
                                ". .",
                                "0.B"};
            TestGridFloorPlan gridPlan = TestGridFloorPlan.InitGridToContext(inGrid, 5, 5);
            for (int ii = 0; ii < gridPlan.RoomCount; ii++)
            {
                TestFloorPlanGen gen = new TestFloorPlanGen(((TestGridRoomGen)gridPlan.GetRoom(ii)).Identifier);
                gen.PrepareProposeSize(new Loc(2, 2));
                gridPlan.PublicArrayRooms[ii].RoomGen = gen;
            }
            gridPlan.PublicHHalls[0][1].SetGen(new TestFloorPlanGen('a'));

            TestFloorPlan compareFloorPlan = TestFloorPlan.InitFloorToContext(gridPlan.Size,
                new Rect[] { new Rect(0, 0, 2, 2), new Rect(9, 6, 2, 2) },
                new Rect[] { new Rect(2, 1, 4, 7), new Rect(6, 6, 3, 2) },
                new Tuple<char, char>[] { new Tuple<char, char>('A', 'a'), new Tuple<char, char>('a', 'b'), new Tuple<char, char>('b', 'B') });
            ((TestFloorPlanGen)compareFloorPlan.PublicHalls[1].Gen).Identifier = 'a';

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            Moq.Language.ISetupSequentialResult<int> seq = testRand.SetupSequence(p => p.Next(4));
            seq = seq.Returns(0);
            seq = seq.Returns(3);
            seq = testRand.SetupSequence(p => p.Next(10));
            seq = seq.Returns(0);
            seq = seq.Returns(0);

            TestFloorPlan floorPlan = new TestFloorPlan();
            floorPlan.InitSize(gridPlan.Size);

            Mock<IFloorPlanTestContext> mockMap = new Mock<IFloorPlanTestContext>(MockBehavior.Strict);
            mockMap.SetupGet(p => p.Rand).Returns(testRand.Object);
            mockMap.SetupGet(p => p.RoomPlan).Returns(floorPlan);

            gridPlan.PlaceRoomsOnFloor(mockMap.Object);

            TestFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);
        }


        [Test]
        public void PlaceRoomsOnFloorIntrusiveHallsCloseB()
        {
            //place a ring of rooms connected by halls
            string[] inGrid = { "A.0",
                                ". .",
                                "A#B",
                                ". .",
                                "0.B"};
            TestGridFloorPlan gridPlan = TestGridFloorPlan.InitGridToContext(inGrid, 5, 5);
            for (int ii = 0; ii < gridPlan.RoomCount; ii++)
            {
                TestFloorPlanGen gen = new TestFloorPlanGen(((TestGridRoomGen)gridPlan.GetRoom(ii)).Identifier);
                gen.PrepareProposeSize(new Loc(2, 2));
                gridPlan.PublicArrayRooms[ii].RoomGen = gen;
            }
            gridPlan.PublicHHalls[0][1].SetGen(new TestFloorPlanGen('a'));

            TestFloorPlan compareFloorPlan = TestFloorPlan.InitFloorToContext(gridPlan.Size,
                new Rect[] { new Rect(0, 0, 2, 2), new Rect(6, 15, 2, 2) },
                new Rect[] { new Rect(2, 1, 3, 10), new Rect(5, 6, 1, 10) },
                new Tuple<char, char>[] { new Tuple<char, char>('A', 'a'), new Tuple<char, char>('a', 'b'), new Tuple<char, char>('b', 'B') });
            ((TestFloorPlanGen)compareFloorPlan.PublicHalls[1].Gen).Identifier = 'a';

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            Moq.Language.ISetupSequentialResult<int> seq = testRand.SetupSequence(p => p.Next(4));
            seq = seq.Returns(0);
            seq = seq.Returns(0);
            seq = testRand.SetupSequence(p => p.Next(10));
            seq = seq.Returns(0);
            seq = seq.Returns(9);

            TestFloorPlan floorPlan = new TestFloorPlan();
            floorPlan.InitSize(gridPlan.Size);

            Mock<IFloorPlanTestContext> mockMap = new Mock<IFloorPlanTestContext>(MockBehavior.Strict);
            mockMap.SetupGet(p => p.Rand).Returns(testRand.Object);
            mockMap.SetupGet(p => p.RoomPlan).Returns(floorPlan);

            gridPlan.PlaceRoomsOnFloor(mockMap.Object);

            TestFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);
        }

        [Test]
        public void GetAdjacentRoomsOneCellNoneNear()
        {
            //completely lone one-cell room
            string[] inGrid = { "0.0.0",
                                ". . .",
                                "0.A.0",
                                ". . .",
                                "0.0.0"};

            TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid);
            List<int> rooms = floorPlan.GetAdjacentRooms(0);
            List<int> compare = new List<int>();
            Assert.That(rooms, Is.EqualTo(compare));
        }

        [Test]
        public void GetAdjacentRoomsOneCellAllNear()
        {
            //completely filled one-cell room
            string[] inGrid = { "0.D.0",
                                ". # .",
                                "C#A#E",
                                ". # .",
                                "0.B.0"};

            TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid);
            List<int> rooms = floorPlan.GetAdjacentRooms(0);
            List<int> compare = new List<int>();
            compare.Add(3);
            compare.Add(1);
            compare.Add(2);
            compare.Add(4);
            Assert.That(rooms, Is.EqualTo(compare));
        }


        [Test]
        public void GetAdjacentRoomsOneCellAllNearDetached()
        {
            //completely filled for all but the one-cell room, detached
            string[] inGrid = { "G#D#H",
                                "# . #",
                                "C.A.E",
                                "# . #",
                                "F#B#I"};

            TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid);
            List<int> rooms = floorPlan.GetAdjacentRooms(0);
            List<int> compare = new List<int>();
            Assert.That(rooms, Is.EqualTo(compare));
        }


        [Test]
        public void GetAdjacentRoomsMultiCellAll()
        {
            //completely filled many-cell room
            string[] inGrid = { "0.G.H.I.0",
                                ". # # # .",
                                "E#A.A.A#J",
                                ". . . . .",
                                "F#A.A.A#K",
                                ". # # # .",
                                "0.B.C.D.0"};

            TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid);
            List<int> rooms = floorPlan.GetAdjacentRooms(0);
            List<int> compare = new List<int>();
            compare.Add(6);
            compare.Add(1);
            compare.Add(7);
            compare.Add(2);
            compare.Add(8);
            compare.Add(3);
            compare.Add(4);
            compare.Add(9);
            compare.Add(5);
            compare.Add(10);
            Assert.That(rooms, Is.EqualTo(compare));
        }


        [Test]
        public void GetAdjacentRoomsMultiCellRepeat()
        {
            //many cell room with double dip on a room
            string[] inGrid = { "0.C.0",
                                ". # .",
                                "B#A#D",
                                ". . .",
                                "B#A#D"};


            TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid);
            List<int> rooms = floorPlan.GetAdjacentRooms(0);
            List<int> compare = new List<int>();
            compare.Add(2);
            compare.Add(1);
            compare.Add(3);
            Assert.That(rooms, Is.EqualTo(compare));
        }

        //TODO: [Test]
        public void GetRoomNum()
        {
            throw new NotImplementedException();
        }

        //TODO: [Test]
        public void TestDisconnect()
        {
            throw new NotImplementedException();
        }

        //TODO: [Test]
        public void CheckAccessibility()
        {
            throw new NotImplementedException();
        }

        [Test]
        [TestCase(0, 0, 1, 1, 0, 0, 5, 3)]
        [TestCase(2, 1, 1, 1, 12, 4, 5, 3)]
        [TestCase(0, 0, 3, 2, 0, 0, 17, 7)]
        public void GetCellBounds(int cellX, int cellY, int cellW, int cellH, int x, int y, int w, int h)
        {
            TestGridFloorPlan floorPlan = new TestGridFloorPlan();
            floorPlan.InitSize(5, 5, 5, 3);
            Rect bounds = floorPlan.GetCellBounds(new Rect(cellX, cellY, cellW, cellH));
            Rect compareBounds = new Rect(x, y, w, h);
            Assert.That(bounds, Is.EqualTo(compareBounds));
        }

        [Test]
        [TestCase(Dir4.Down, 8, 12)]
        [TestCase(Dir4.Up, 8, 12)]
        [TestCase(Dir4.Left, 6, 8)]
        [TestCase(Dir4.Right, 6, 8)]
        public void GetHallTouchRange(Dir4 dir, int rangeMin, int rangeMax)
        {
            //a room that takes up one cell
            string[] inGrid = { "0.0.0.0",
                                ". . . .",
                                "0.A.0.0",
                                ". . . .",
                                "0.0.0.0"};

            TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid, 6, 4);
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            IRoomGen testGen = floorPlan.PublicArrayRooms[0].RoomGen;
            testGen.PrepareSize(testRand.Object, new Loc(4, 2));
            testGen.SetLoc(new Loc(8, 6));
            Range bounds = floorPlan.GetHallTouchRange(testGen, dir, 1);
            Range compareBounds = new Range(rangeMin, rangeMax);
            Assert.That(bounds, Is.EqualTo(compareBounds));
            
        }


        [Test]
        //a room that takes multiple cells but has area in the focused cell
        [TestCase(Dir4.Down, 0, 1, 6)]
        [TestCase(Dir4.Left, 0, 2, 4)]
        [TestCase(Dir4.Left, 1, 5, 9)]
        [TestCase(Dir4.Right, 2, 10, 12)]
        //a room that takes multiple cells and does not have area in the focused cell
        [TestCase(Dir4.Down, 1, 5, 6)]
        public void GetHallTouchRangeLargeRoom(Dir4 dir, int tier, int rangeMin, int rangeMax)
        {
            //a room that takes up one cell
            string[] inGrid = { "A.A.0.0",
                                ". . . .",
                                "A.A.0.0",
                                ". . . .",
                                "A.A.0.0"};

            TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid, 6, 4);
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            IRoomGen testGen = floorPlan.PublicArrayRooms[0].RoomGen;
            testGen.PrepareSize(testRand.Object, new Loc(5, 10));
            testGen.SetLoc(new Loc(1, 2));
            Range bounds = floorPlan.GetHallTouchRange(testGen, dir, tier);
            Range compareBounds = new Range(rangeMin, rangeMax);
            Assert.That(bounds, Is.EqualTo(compareBounds));
        }
    }


    public interface IGridPathTestContext : IRoomGridGenContext
    {

    }

    public class TestGridRoomGen : TestFloorRoomGen<IGridPathTestContext>
    {
        public TestGridRoomGen() { }
        public TestGridRoomGen(char id) : base(id) { }
    }

    public class TestGridFloorPlan : GridPlan
    {
        public List<GridRoomPlan> PublicArrayRooms { get { return arrayRooms; } }
        public int[][] PublicRooms { get { return rooms; } }
        public GridHallPlan[][] PublicVHalls { get { return vHalls; } }
        public GridHallPlan[][] PublicHHalls { get { return hHalls; } }
        
        public static void CompareFloorPlans(TestGridFloorPlan floorPlan, TestGridFloorPlan compareFloorPlan)
        {
            //check the rooms
            Assert.That(floorPlan.RoomCount, Is.EqualTo(compareFloorPlan.RoomCount));
            for (int ii = 0; ii < floorPlan.RoomCount; ii++)
            {
                GridRoomPlan plan = floorPlan.GetRoomPlan(ii);
                GridRoomPlan comparePlan = compareFloorPlan.GetRoomPlan(ii);
                Assert.That(plan.RoomGen, Is.EqualTo(comparePlan.RoomGen));
                Assert.That(plan.Bounds, Is.EqualTo(comparePlan.Bounds));
            }

            //check positions
            Assert.That(floorPlan.PublicRooms, Is.EqualTo(compareFloorPlan.PublicRooms));
            Assert.That(floorPlan.PublicVHalls.Length, Is.EqualTo(compareFloorPlan.PublicVHalls.Length));
            for (int xx = 0; xx < floorPlan.PublicVHalls.Length; xx++)
            {
                Assert.That(floorPlan.PublicVHalls[xx].Length, Is.EqualTo(compareFloorPlan.PublicVHalls[xx].Length));
                for (int yy = 0; yy < floorPlan.PublicVHalls[xx].Length; yy++)
                    Assert.That(floorPlan.PublicVHalls[xx][yy].Gens, Is.EqualTo(compareFloorPlan.PublicVHalls[xx][yy].Gens));
            }
            Assert.That(floorPlan.PublicHHalls.Length, Is.EqualTo(compareFloorPlan.PublicHHalls.Length));
            for (int xx = 0; xx < floorPlan.PublicHHalls.Length; xx++)
            {
                Assert.That(floorPlan.PublicHHalls[xx].Length, Is.EqualTo(compareFloorPlan.PublicHHalls[xx].Length));
                for (int yy = 0; yy < floorPlan.PublicVHalls[xx].Length; yy++)
                    Assert.That(floorPlan.PublicHHalls[xx][yy].Gens, Is.EqualTo(compareFloorPlan.PublicHHalls[xx][yy].Gens));
            }
        }


        public static TestGridFloorPlan InitGridToContext(string[] inGrid)
        {
            return InitGridToContext(inGrid, 0, 0);
        }

        public static TestGridFloorPlan InitGridToContext(string[] inGrid, int widthPerCell, int heightPerCell)
        {
            //transposes
            if (inGrid.Length % 2 == 0 || inGrid[0].Length % 2 == 0)
                throw new ArgumentException("Bad input grid!");
            TestGridFloorPlan floorPlan = new TestGridFloorPlan();
            floorPlan.InitSize(inGrid[0].Length / 2 + 1, inGrid.Length / 2 + 1, widthPerCell, heightPerCell);
            GridRoomPlan[] addedRooms = new GridRoomPlan[26];

            for (int xx = 0; xx < inGrid[0].Length; xx++)
            {
                for (int yy = 0; yy < inGrid.Length; yy++)
                {
                    char val = inGrid[yy][xx];
                    int x = xx / 2;
                    int y = yy / 2;
                    //rooms
                    if (xx % 2 == 0 && yy % 2 == 0)
                    {
                        if (val >= 'A' && val <= 'Z')
                        {
                            floorPlan.rooms[x][y] = val - 'A';
                            if (addedRooms[val - 'A'] == null)
                                addedRooms[val - 'A'] = new GridRoomPlan(new Rect(x,y,1,1), new TestGridRoomGen(val));
                            addedRooms[val - 'A'].Bounds = Rect.IncludeLoc(addedRooms[val - 'A'].Bounds, new Loc(x,y));
                        }
                        else if (val == '0')
                            floorPlan.rooms[x][y] = -1;
                        else
                            throw new ArgumentException(String.Format("Bad input grid val at room {0},{1}!", x, y));
                    }
                    else if (xx % 2 == 0 && yy % 2 == 1)
                    {
                        //vhalls
                        if (val == '#')
                            floorPlan.vHalls[x][y].SetGen(new TestGridRoomGen());
                        else if (val == '.')
                            floorPlan.vHalls[x][y].SetGen(null);
                        else
                            throw new ArgumentException(String.Format("Bad input grid val at vertical hall {0},{1}!", x, y));
                    }
                    else if (xx % 2 == 1 && yy % 2 == 0)
                    {
                        //hhalls
                        if (val == '#')
                            floorPlan.hHalls[x][y].SetGen(new TestGridRoomGen());
                        else if (val == '.')
                            floorPlan.hHalls[x][y].SetGen(null);
                        else
                            throw new ArgumentException(String.Format("Bad input grid val at horizontal hall {0},{1}!", x, y));
                    }
                    else if (xx % 2 == 1 && yy % 2 == 1)
                    {
                        //blank
                        if (val != ' ')
                            throw new ArgumentException(String.Format("Bad input grid val at blank zone!"));
                    }
                }
            }
            for (int ii = 0; ii < 26; ii++)
            {
                if (addedRooms[ii] != null)
                    floorPlan.arrayRooms.Add(addedRooms[ii]);
            }
            return floorPlan;
        }
    }
}