// <copyright file="GridFloorPlanTest.cs" company="Audino">
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
    public class GridFloorPlanTest
    {
        [Test]
        [Ignore("TODO")]
        public void GetAll()
        {
            // init a grid and call the enumerator and make sure it has everything
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
            // get a big room
            throw new NotImplementedException();
        }

        [Test]
        [Ignore("TODO")]
        public void EraseRoom()
        {
            // erase existent room
            // erase nonexistent room (same deal)
            // erase a big room; the whole room should be gone
            throw new NotImplementedException();
        }

        [Test]
        [Ignore("TODO")]
        public void IsRoomOpen()
        {
            // erase existent room
            // erase nonexistent room (same deal)
            // erase a big room; the whole room should be gone
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
        public void AddRoomToEmptySpace()
        {
            // add to empty space
            string[] inGrid =
            {
                "0.0.0",
                ". . .",
                "0.0.0",
            };

            string[] outGrid =
            {
                "0.A.0",
                ". . .",
                "0.0.0",
            };

            TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid);
            var gen = new TestGridRoomGen('A');
            floorPlan.AddRoom(new Rect(1, 0, 1, 1), gen, new ComponentCollection());
            TestGridFloorPlan compareFloorPlan = TestGridFloorPlan.InitGridToContext(outGrid);

            TestGridFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);
        }

        [Test]
        public void AddBigRoomToEmptySpace()
        {
            // add big room to empty space
            string[] inGrid =
            {
                "0.0.0.0",
                ". . . .",
                "0.0.0.0",
                ". . . .",
                "0.0.0.0",
            };

            string[] outGrid =
            {
                "0.0.0.0",
                ". . . .",
                "0.A.A.A",
                ". . . .",
                "0.A.A.A",
            };

            TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid);
            var gen = new TestGridRoomGen('A');
            floorPlan.AddRoom(new Rect(1, 1, 3, 2), gen, new ComponentCollection());
            TestGridFloorPlan compareFloorPlan = TestGridFloorPlan.InitGridToContext(outGrid);

            TestGridFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);
        }

        [Test]
        public void AddCrossingRooms()
        {
            // add small room on big room
            string[] inGrid =
            {
                "0.0.0.0",
                ". . . .",
                "0.A.A.A",
                ". . . .",
                "0.A.A.A",
            };

            TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid);
            var gen = new TestGridRoomGen('B');
            Assert.Throws<InvalidOperationException>(() => { floorPlan.AddRoom(new Rect(1, 0, 2, 2), gen, new ComponentCollection()); });
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void AddBigRoomToHall(int gridType)
        {
            // add small room on big room
            string[] inGrid = null;

            switch (gridType)
            {
                case 0:
                    inGrid = new string[]
                    {
                        "0.0.0.0",
                        ". . . .",
                        "0.0#0.0",
                        ". . . .",
                        "0.0.0.0",
                    };
                    break;
                case 1:
                    inGrid = new string[]
                    {
                        "0.0.0.0",
                        ". . . .",
                        "0.0.0.0",
                        ". # . .",
                        "0.0.0.0",
                    };
                    break;
                case 2:
                    inGrid = new string[]
                    {
                        "0.0.0.0",
                        ". . . .",
                        "0.0.0.0",
                        ". . # .",
                        "0.0.0.0",
                    };
                    break;
                case 3:
                    inGrid = new string[]
                    {
                        "0.0.0.0",
                        ". . . .",
                        "0.0.0.0",
                        ". . . .",
                        "0.0#0.0",
                    };
                    break;
                default:
                    throw new Exception("Unexpected Case");
            }

            TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid);
            var gen = new TestGridRoomGen('A');
            Assert.Throws<InvalidOperationException>(() => { floorPlan.AddRoom(new Rect(1, 1, 2, 2), gen, new ComponentCollection()); });
        }

        [Test]
        public void AddBigRoomToSurrounded()
        {
            // add room to a room/hall-ridden floor where halls just barely avoid the room
            string[] inGrid =
            {
                "A#B#C#D",
                "# # # #",
                "E#0.0#F",
                "# . . #",
                "G#0.0#H",
            };

            string[] outGrid =
            {
                "A#B#C#D",
                "# # # #",
                "E#I.I#F",
                "# . . #",
                "G#I.I#H",
            };

            TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid);
            var gen = new TestGridRoomGen('I');
            floorPlan.AddRoom(new Rect(1, 1, 2, 2), gen, new ComponentCollection());

            TestGridFloorPlan compareFloorPlan = TestGridFloorPlan.InitGridToContext(outGrid);

            TestGridFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);
        }

        [Test]
        [TestCase(-1, 0, 2, 2)]
        [TestCase(0, 0, 2, 4)]
        public void AddRoomOutOfRange(int x, int y, int w, int h)
        {
            // out of range
            string[] inGrid =
            {
                "0.0.0.0",
                ". . . .",
                "0.0.0.0",
                ". . . .",
                "0.0.0.0",
            };

            TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid);
            var gen = new TestGridRoomGen('A');
            Assert.Throws<ArgumentOutOfRangeException>(() => { floorPlan.AddRoom(new Rect(x, y, w, h), gen, new ComponentCollection()); });
        }

        [Test]
        [TestCase(0, 0, 2, 4)]
        [TestCase(0, 0, 5, 2)]
        public void AddRoomTooLargeWrapped(int x, int y, int w, int h)
        {
            // out of range
            string[] inGrid =
            {
                "0.0.0.0.",
                ". . . . ",
                "0.0.0.0.",
                ". . . . ",
                "0.0.0.0.",
                ". . . . ",
            };

            TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid);
            var gen = new TestGridRoomGen('A');
            Assert.Throws<ArgumentOutOfRangeException>(() => { floorPlan.AddRoom(new Rect(x, y, w, h), gen, new ComponentCollection()); });
        }

        [Test]
        [TestCase(0, 2, 0)]
        [TestCase(0, -1, 0)]
        [TestCase(3, 0, 1)]
        [TestCase(3, 2, 2)]
        [TestCase(2, 2, -1)]
        public void AddRoomOutOfRangeWrapped(int x, int y, int expectedOut)
        {
            // valid/invalid dir
            string[] inGrid =
            {
                "0.0.A.0.",
                ". . . . ",
                "0.0.0.0.",
                ". . . . ",
                "0.0.0.0.",
                ". . . . ",
            };

            string[] outGrid = null;
            bool exception = false;
            switch (expectedOut)
            {
                case 0:
                    outGrid = new string[]
                    {
                        "B.B.A.0.",
                        ". . . . ",
                        "0.0.0.0.",
                        ". . . . ",
                        "B.B.0.0.",
                        ". . . . ",
                    };
                    break;
                case 1:
                    outGrid = new string[]
                    {
                        "B.0.A.B.",
                        ". . . . ",
                        "B.0.0.B.",
                        ". . . . ",
                        "0.0.0.0.",
                        ". . . . ",
                    };
                    break;
                case 2:
                    outGrid = new string[]
                    {
                        "B.0.A.B.",
                        ". . . . ",
                        "0.0.0.0.",
                        ". . . . ",
                        "B.0.0.B.",
                        ". . . . ",
                    };
                    break;
                default:
                    exception = true;
                    break;
            }

            TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid);
            var gen = new TestGridRoomGen('B');

            if (exception)
            {
                Assert.Throws<InvalidOperationException>(() => { floorPlan.AddRoom(new Rect(x, y, 2, 2), gen, new ComponentCollection()); });
                return;
            }
            else
            {
                floorPlan.AddRoom(new Rect(x, y, 2, 2), gen, new ComponentCollection());
            }

            TestGridFloorPlan compareFloorPlan = TestGridFloorPlan.InitGridToContext(outGrid);

            TestGridFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);
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
            // valid/invalid dir
            string[] inGrid =
            {
                "0.0.0.0",
                ". . . .",
                "0.0.0.0",
                ". . . .",
                "0.0.0.0",
            };

            string[] outGrid = null;
            bool exception = false;
            switch (expectedOut)
            {
                case 0:
                    outGrid = new string[]
                    {
                        "0.0.0.0",
                        ". . . .",
                        "0.0.0.0",
                        ". # . .",
                        "0.0.0.0",
                    };
                    break;
                case 1:
                    outGrid = new string[]
                    {
                        "0.0.0.0",
                        ". . . .",
                        "0#0.0.0",
                        ". . . .",
                        "0.0.0.0",
                    };
                    break;
                case 2:
                    outGrid = new string[]
                    {
                        "0.0.0.0",
                        ". # . .",
                        "0.0.0.0",
                        ". . . .",
                        "0.0.0.0",
                    };
                    break;
                case 3:
                    outGrid = new string[]
                    {
                        "0.0.0.0",
                        ". . . .",
                        "0.0#0.0",
                        ". . . .",
                        "0.0.0.0",
                    };
                    break;
                default:
                    exception = true;
                    break;
            }

            TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid);
            var gen = new TestGridRoomGen((char)0);
            if (exception)
            {
                if (dir == Dir4.None)
                    Assert.Throws<ArgumentException>(() => { floorPlan.SetHall(new LocRay4(1, 1, dir), gen, new ComponentCollection()); });
                else
                    Assert.Throws<ArgumentOutOfRangeException>(() => { floorPlan.SetHall(new LocRay4(1, 1, dir), gen, new ComponentCollection()); });
                return;
            }
            else
            {
                floorPlan.SetHall(new LocRay4(1, 1, dir), gen, new ComponentCollection());
            }

            TestGridFloorPlan compareFloorPlan = TestGridFloorPlan.InitGridToContext(outGrid);

            TestGridFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);
        }

        [Test]
        [TestCase(2, 2, Dir4.Right, 0)]
        [TestCase(3, 2, Dir4.Left, 0)]
        [TestCase(3, 1, Dir4.Down, 1)]
        [TestCase(3, 2, Dir4.Up, 1)]
        [TestCase(2, 3, Dir4.Right, -1)]
        [TestCase(3, 3, Dir4.Left, -1)]
        [TestCase(4, 1, Dir4.Down, -1)]
        [TestCase(4, 2, Dir4.Up, -1)]
        [TestCase(3, 2, Dir4.Right, -1)]
        [TestCase(3, 2, Dir4.Down, -1)]
        public void SetHallOnBorder(int dx, int dy, Dir4 dir, int expectedOut)
        {
            // valid/invalid dir
            string[] inGrid =
            {
                "0.0.0.0",
                ". . . .",
                "0.0.0.0",
                ". . . .",
                "0.0.0.0",
            };

            string[] outGrid = null;
            bool exception = false;
            switch (expectedOut)
            {
                case 0:
                    outGrid = new string[]
                    {
                        "0.0.0.0",
                        ". . . .",
                        "0.0.0.0",
                        ". . . .",
                        "0.0.0#0",
                    };
                    break;
                case 1:
                    outGrid = new string[]
                    {
                        "0.0.0.0",
                        ". . . .",
                        "0.0.0.0",
                        ". . . #",
                        "0.0.0.0",
                    };
                    break;
                default:
                    exception = true;
                    break;
            }

            TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid);
            var gen = new TestGridRoomGen((char)0);
            if (exception)
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => { floorPlan.SetHall(new LocRay4(dx, dy, dir), gen, new ComponentCollection()); });
                return;
            }
            else
            {
                floorPlan.SetHall(new LocRay4(dx, dy, dir), gen, new ComponentCollection());
            }

            TestGridFloorPlan compareFloorPlan = TestGridFloorPlan.InitGridToContext(outGrid);

            TestGridFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);
        }

        [Test]
        [TestCase(2, 3, Dir4.Right, 0)]
        [TestCase(3, 3, Dir4.Left, 0)]
        [TestCase(4, 1, Dir4.Down, 1)]
        [TestCase(4, 2, Dir4.Up, 1)]
        [TestCase(3, 2, Dir4.Right, 2)]
        [TestCase(0, 2, Dir4.Left, 2)]
        [TestCase(3, 2, Dir4.Down, 3)]
        [TestCase(3, 0, Dir4.Up, 3)]
        public void SetHallOnBorderWrapped(int dx, int dy, Dir4 dir, int expectedOut)
        {
            // valid/invalid dir
            string[] inGrid =
            {
                "0.0.0.0.",
                ". . . . ",
                "0.0.0.0.",
                ". . . . ",
                "0.0.0.0.",
                ". . . . ",
            };

            string[] outGrid = null;
            bool exception = false;
            switch (expectedOut)
            {
                case 0:
                    outGrid = new string[]
                    {
                        "0.0.0#0.",
                        ". . . . ",
                        "0.0.0.0.",
                        ". . . . ",
                        "0.0.0.0.",
                        ". . . . ",
                    };
                    break;
                case 1:
                    outGrid = new string[]
                    {
                        "0.0.0.0.",
                        ". . . . ",
                        "0.0.0.0.",
                        "# . . . ",
                        "0.0.0.0.",
                        ". . . . ",
                    };
                    break;
                case 2:
                    outGrid = new string[]
                    {
                        "0.0.0.0.",
                        ". . . . ",
                        "0.0.0.0.",
                        ". . . . ",
                        "0.0.0.0#",
                        ". . . . ",
                    };
                    break;
                case 3:
                    outGrid = new string[]
                    {
                        "0.0.0.0.",
                        ". . . . ",
                        "0.0.0.0.",
                        ". . . . ",
                        "0.0.0.0.",
                        ". . . # ",
                    };
                    break;
                default:
                    exception = true;
                    break;
            }

            TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid);
            var gen = new TestGridRoomGen((char)0);
            if (exception)
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => { floorPlan.SetHall(new LocRay4(dx, dy, dir), gen, new ComponentCollection()); });
                return;
            }
            else
            {
                floorPlan.SetHall(new LocRay4(dx, dy, dir), gen, new ComponentCollection());
            }

            TestGridFloorPlan compareFloorPlan = TestGridFloorPlan.InitGridToContext(outGrid);

            TestGridFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);
        }

        [Test]
        public void SetHallToNull()
        {
            // set existing hall to null
            string[] inGrid =
            {
                "0.0.0.0",
                ". . . .",
                "0.0#0.0",
                ". . . .",
                "0.0.0.0",
            };

            string[] outGrid =
            {
                "0.0.0.0",
                ". . . .",
                "0.0.0.0",
                ". . . .",
                "0.0.0.0",
            };

            TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid);
            floorPlan.SetHall(new LocRay4(1, 1, Dir4.Right), null, new ComponentCollection());
            TestGridFloorPlan compareFloorPlan = TestGridFloorPlan.InitGridToContext(outGrid);

            TestGridFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);
        }

        [Test]

        // normal size
        [TestCase(8, 5, 6, 4, 6, 4, 3, 2, 0, 0)]

        // normal size offset
        [TestCase(8, 5, 6, 4, 6, 4, 3, 2, 2, 1)]

        // exceeding size
        [TestCase(8, 5, 9, 6, 8, 5, 1, 1, 0, 0)]

        // exceeding size in multi-room size
        [TestCase(8, 11, 9, 6, 8, 6, 1, 6, 0, 4)]
        public void ChooseRoomBounds(int boundW, int boundH, int tryW, int tryH, int w, int h, int rMaxX, int rMaxY, int randX, int randY)
        {
            const int boundX = 10;
            const int boundY = 13;
            Mock<TestGridFloorPlan> floorPlan = new Mock<TestGridFloorPlan> { CallBase = true };
            floorPlan.Object.InitSize(4, 4, 8, 5);
            floorPlan.Setup(p => p.GetCellBounds(Rect.Empty)).Returns(new Rect(boundX, boundY, boundW, boundH));
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            Moq.Language.ISetupSequentialResult<int> seq = testRand.SetupSequence(p => p.Next(rMaxX));
            seq = seq.Returns(randX);
            if (rMaxX == rMaxY)
            {
                seq = seq.Returns(randY);
            }
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

            // manually place the mock object instead of using AddRoom to copy it over
            floorPlan.Object.PublicArrayRooms.Add(new GridRoomPlan(Rect.Empty, mockRoom.Object, new ComponentCollection()));

            floorPlan.Object.ChooseRoomBounds(testRand.Object, 0);

            // verify all were called
            if (rMaxX == rMaxY)
            {
                testRand.Verify(p => p.Next(rMaxX), Times.Exactly(2));
            }
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
            // verify the range is the OR of both rooms
            Mock<TestGridFloorPlan> floorPlan = new Mock<TestGridFloorPlan> { CallBase = true };
            floorPlan.Object.InitSize(3, 4, 5, 5);
            Mock<IRandom> mockRand = new Mock<IRandom>(MockBehavior.Strict);
            Mock<IRoomGen> mockRoom1 = new Mock<IRoomGen>(MockBehavior.Strict);

            mockRoom1.SetupGet(p => p.Draw).Returns(new Rect(6, 6, 3, 2));

            // manually place the mock object instead of using AddRoom to copy it over
            floorPlan.Object.PublicArrayRooms.Add(new GridRoomPlan(new Rect(1, 1, 1, 1), mockRoom1.Object, new ComponentCollection()));
            floorPlan.Object.PublicRooms[1][1] = 0;

            Mock<IRoomGen> mockRoom2 = new Mock<IRoomGen>(MockBehavior.Strict);
            mockRoom2.SetupGet(p => p.Draw).Returns(new Rect(8, 14, 3, 2));

            // manually place the mock object instead of using AddRoom to copy it over
            floorPlan.Object.PublicArrayRooms.Add(new GridRoomPlan(new Rect(1, 1, 1, 1), mockRoom2.Object, new ComponentCollection()));
            floorPlan.Object.PublicRooms[1][2] = 1;

            Mock<IPermissiveRoomGen> mockHall = new Mock<IPermissiveRoomGen>(MockBehavior.Strict);
            mockHall.Setup(p => p.SetLoc(new Loc(6, 8)));
            mockHall.Setup(p => p.PrepareSize(mockRand.Object, new Loc(5, 6)));
            floorPlan.Object.PublicVHalls[1][1].SetHall(new GridHallPlan(mockHall.Object, new ComponentCollection()));

            floorPlan.Setup(p => p.GetHallTouchRange(new Rect(6, 6, 3, 2), mockRoom1.Object.GetFulfillableBorder, Dir4.Down, 1)).Returns(new IntRange(6, 9));
            floorPlan.Setup(p => p.GetHallTouchRange(new Rect(8, 14, 3, 2), mockRoom2.Object.GetFulfillableBorder, Dir4.Up, 1)).Returns(new IntRange(8, 11));

            floorPlan.Object.ChooseHallBounds(mockRand.Object, 1, 1, true);

            mockHall.Verify(p => p.SetLoc(It.IsAny<Loc>()), Times.Exactly(1));
            mockHall.Verify(p => p.PrepareSize(It.IsAny<IRandom>(), It.IsAny<Loc>()), Times.Exactly(1));
        }

        [Test]
        public void PlaceRoomsOnFloorBasic()
        {
            // place two rooms connected by a hall
            string[] inGrid =
            {
                "A#B",
                ". .",
                "0.0",
            };

            TestGridFloorPlan gridPlan = TestGridFloorPlan.InitGridToContext(inGrid, 5, 5);
            for (int ii = 0; ii < gridPlan.RoomCount; ii++)
            {
                var gen = new TestFloorPlanGen(((TestGridRoomGen)gridPlan.GetRoom(ii)).Identifier)
                {
                    ProposedSize = new Loc(5, 5),
                };
                gridPlan.PublicArrayRooms[ii].RoomGen = gen;
            }

            gridPlan.PublicHHalls[0][0].SetHall(new GridHallPlan(new TestFloorPlanGen('a'), new ComponentCollection()));

            TestFloorPlan compareFloorPlan;
            {
                var links = new Tuple<char, char>[]
                {
                    Tuple.Create('A', 'a'),
                    Tuple.Create('a', 'B'),
                };
                compareFloorPlan = TestFloorPlan.InitFloorToContext(
                    gridPlan.Size,
                    new Rect[] { new Rect(0, 0, 5, 5), new Rect(6, 0, 5, 5) },
                    new Rect[] { new Rect(5, 0, 1, 5) },
                    links);
            }

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(It.IsAny<int>())).Returns(0);

            var floorPlan = new TestFloorPlan();
            floorPlan.InitSize(gridPlan.Size, gridPlan.Wrap);

            Mock<IFloorPlanTestContext> mockMap = new Mock<IFloorPlanTestContext>(MockBehavior.Strict);
            mockMap.SetupGet(p => p.Rand).Returns(testRand.Object);
            mockMap.SetupGet(p => p.RoomPlan).Returns(floorPlan);

            gridPlan.PlaceRoomsOnFloor(mockMap.Object);

            TestFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);
        }

        [Test]
        public void PlaceRoomsOnFloorWrapped()
        {
            // place two rooms connected by a hall
            string[] inGrid =
            {
                "B.A#",
                ". . ",
                "0.0.",
                ". . ",
            };

            TestGridFloorPlan gridPlan = TestGridFloorPlan.InitGridToContext(inGrid, 5, 5);
            for (int ii = 0; ii < gridPlan.RoomCount; ii++)
            {
                var gen = new TestFloorPlanGen(((TestGridRoomGen)gridPlan.GetRoom(ii)).Identifier)
                {
                    ProposedSize = new Loc(5, 5),
                };
                gridPlan.PublicArrayRooms[ii].RoomGen = gen;
            }

            gridPlan.PublicHHalls[1][0].SetHall(new GridHallPlan(new TestFloorPlanGen('a'), new ComponentCollection()));

            TestFloorPlan compareFloorPlan;
            {
                var links = new Tuple<char, char>[]
                {
                    Tuple.Create('A', 'a'),
                    Tuple.Create('a', 'B'),
                };
                compareFloorPlan = TestFloorPlan.InitFloorToContext(
                    gridPlan.Size,
                    new Rect[] { new Rect(6, 0, 5, 5), new Rect(0, 0, 5, 5) },
                    new Rect[] { new Rect(11, 0, 1, 5) },
                    links);
            }

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(It.IsAny<int>())).Returns(0);

            var floorPlan = new TestFloorPlan();
            floorPlan.InitSize(gridPlan.Size, gridPlan.Wrap);

            Mock<IFloorPlanTestContext> mockMap = new Mock<IFloorPlanTestContext>(MockBehavior.Strict);
            mockMap.SetupGet(p => p.Rand).Returns(testRand.Object);
            mockMap.SetupGet(p => p.RoomPlan).Returns(floorPlan);

            gridPlan.PlaceRoomsOnFloor(mockMap.Object);

            TestFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);
        }

        [Test]
        public void PlaceRoomsOnFloorWrappedRing()
        {
            // place two rooms connected by a hall
            string[] inGrid =
            {
                "A#B#",
                ". . ",
                "0.0.",
                ". . ",
            };

            TestGridFloorPlan gridPlan = TestGridFloorPlan.InitGridToContext(inGrid, 5, 5);
            for (int ii = 0; ii < gridPlan.RoomCount; ii++)
            {
                var gen = new TestFloorPlanGen(((TestGridRoomGen)gridPlan.GetRoom(ii)).Identifier)
                {
                    ProposedSize = new Loc(5, 5),
                };
                gridPlan.PublicArrayRooms[ii].RoomGen = gen;
            }

            gridPlan.PublicHHalls[0][0].SetHall(new GridHallPlan(new TestFloorPlanGen('a'), new ComponentCollection()));
            gridPlan.PublicHHalls[1][0].SetHall(new GridHallPlan(new TestFloorPlanGen('b'), new ComponentCollection()));

            TestFloorPlan compareFloorPlan;
            {
                var links = new Tuple<char, char>[]
                {
                    Tuple.Create('A', 'a'),
                    Tuple.Create('a', 'B'),
                    Tuple.Create('B', 'b'),
                    Tuple.Create('b', 'A'),
                };
                compareFloorPlan = TestFloorPlan.InitFloorToContext(
                    gridPlan.Size,
                    new Rect[] { new Rect(0, 0, 5, 5), new Rect(6, 0, 5, 5) },
                    new Rect[] { new Rect(5, 0, 1, 5), new Rect(11, 0, 1, 5) },
                    links);
            }

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(It.IsAny<int>())).Returns(0);

            var floorPlan = new TestFloorPlan();
            floorPlan.InitSize(gridPlan.Size, gridPlan.Wrap);

            Mock<IFloorPlanTestContext> mockMap = new Mock<IFloorPlanTestContext>(MockBehavior.Strict);
            mockMap.SetupGet(p => p.Rand).Returns(testRand.Object);
            mockMap.SetupGet(p => p.RoomPlan).Returns(floorPlan);

            gridPlan.PlaceRoomsOnFloor(mockMap.Object);

            TestFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);
        }

        [Test]
        public void PlaceLargeRoomOnFloor()
        {
            // place two rooms connected by a hall
            string[] inGrid =
            {
                "A.A",
                ". .",
                "A.A",
            };

            TestGridFloorPlan gridPlan = TestGridFloorPlan.InitGridToContext(inGrid, 5, 5);
            for (int ii = 0; ii < gridPlan.RoomCount; ii++)
            {
                var gen = new TestFloorPlanGen(((TestGridRoomGen)gridPlan.GetRoom(ii)).Identifier)
                {
                    ProposedSize = new Loc(11, 11),
                };
                gridPlan.PublicArrayRooms[ii].RoomGen = gen;
            }

            TestFloorPlan compareFloorPlan;
            {
                var links = new Tuple<char, char>[]
                {
                };
                compareFloorPlan = TestFloorPlan.InitFloorToContext(
                    gridPlan.Size,
                    new Rect[] { new Rect(0, 0, 11, 11) },
                    new Rect[] { },
                    links);
            }

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(It.IsAny<int>())).Returns(0);

            var floorPlan = new TestFloorPlan();
            floorPlan.InitSize(gridPlan.Size, gridPlan.Wrap);

            Mock<IFloorPlanTestContext> mockMap = new Mock<IFloorPlanTestContext>(MockBehavior.Strict);
            mockMap.SetupGet(p => p.Rand).Returns(testRand.Object);
            mockMap.SetupGet(p => p.RoomPlan).Returns(floorPlan);

            gridPlan.PlaceRoomsOnFloor(mockMap.Object);

            TestFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);
        }

        [Test]
        public void PlaceLargeConnectedRoomOnFloor()
        {
            // place two rooms connected by a hall
            string[] inGrid =
            {
                "0.A",
                ". #",
                "B.B",
            };

            TestGridFloorPlan gridPlan = TestGridFloorPlan.InitGridToContext(inGrid, 5, 5);
            {
                TestFloorPlanGen gen = new TestFloorPlanGen('A')
                {
                    ProposedSize = new Loc(5, 5),
                };
                gridPlan.PublicArrayRooms[0].RoomGen = gen;
            }

            {
                TestFloorPlanGen gen = new TestFloorPlanGen('B')
                {
                    ProposedSize = new Loc(11, 5),
                };
                gridPlan.PublicArrayRooms[1].RoomGen = gen;
            }

            gridPlan.PublicVHalls[1][0].SetHall(new GridHallPlan(new TestFloorPlanGen('a'), new ComponentCollection()));

            TestFloorPlan compareFloorPlan;
            {
                var links = new Tuple<char, char>[]
                {
                    Tuple.Create('A', 'a'),
                    Tuple.Create('a', 'B'),
                };
                compareFloorPlan = TestFloorPlan.InitFloorToContext(
                    gridPlan.Size,
                    new Rect[] { new Rect(6, 0, 5, 5), new Rect(0, 6, 11, 5) },
                    new Rect[] { new Rect(6, 5, 5, 1) },
                    links);
            }

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(It.IsAny<int>())).Returns(0);

            var floorPlan = new TestFloorPlan();
            floorPlan.InitSize(gridPlan.Size, gridPlan.Wrap);

            Mock<IFloorPlanTestContext> mockMap = new Mock<IFloorPlanTestContext>(MockBehavior.Strict);
            mockMap.SetupGet(p => p.Rand).Returns(testRand.Object);
            mockMap.SetupGet(p => p.RoomPlan).Returns(floorPlan);

            gridPlan.PlaceRoomsOnFloor(mockMap.Object);

            TestFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);
        }

        [Test]
        public void PlaceLargeRoomOnFloorWrapped()
        {
            // place two rooms connected by a hall
            string[] inGrid =
            {
                "A#B.A.",
                ". . . ",
                "0.0.0.",
                ". . . ",
            };

            TestGridFloorPlan gridPlan = TestGridFloorPlan.InitGridToContext(inGrid, 5, 5);
            {
                TestFloorPlanGen gen = new TestFloorPlanGen('A')
                {
                    ProposedSize = new Loc(11, 5),
                };
                gridPlan.PublicArrayRooms[0].RoomGen = gen;
            }

            {
                TestFloorPlanGen gen = new TestFloorPlanGen('B')
                {
                    ProposedSize = new Loc(5, 5),
                };
                gridPlan.PublicArrayRooms[1].RoomGen = gen;
            }

            gridPlan.PublicHHalls[0][0].SetHall(new GridHallPlan(new TestFloorPlanGen('a'), new ComponentCollection()));

            TestFloorPlan compareFloorPlan;
            {
                var links = new Tuple<char, char>[]
                {
                    Tuple.Create('A', 'a'),
                    Tuple.Create('a', 'B'),
                };
                compareFloorPlan = TestFloorPlan.InitFloorToContext(
                    gridPlan.Size,
                    new Rect[] { new Rect(12, 0, 11, 5), new Rect(6, 0, 5, 5) },
                    new Rect[] { new Rect(23, 0, 1, 5) },
                    links);
            }

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(It.IsAny<int>())).Returns(0);

            var floorPlan = new TestFloorPlan();
            floorPlan.InitSize(gridPlan.Size, gridPlan.Wrap);

            Mock<IFloorPlanTestContext> mockMap = new Mock<IFloorPlanTestContext>(MockBehavior.Strict);
            mockMap.SetupGet(p => p.Rand).Returns(testRand.Object);
            mockMap.SetupGet(p => p.RoomPlan).Returns(floorPlan);

            gridPlan.PlaceRoomsOnFloor(mockMap.Object);

            TestFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);
        }

        [Test]
        public void PlaceLargeRoomOnFloorWrappedOrth()
        {
            // place two rooms connected by a hall
            string[] inGrid =
            {
                "A.0.A.",
                "# . . ",
                "B.0.0.",
                ". . . ",
            };

            TestGridFloorPlan gridPlan = TestGridFloorPlan.InitGridToContext(inGrid, 5, 5);
            {
                TestFloorPlanGen gen = new TestFloorPlanGen('A')
                {
                    ProposedSize = new Loc(11, 5),
                };
                gridPlan.PublicArrayRooms[0].RoomGen = gen;
            }

            {
                TestFloorPlanGen gen = new TestFloorPlanGen('B')
                {
                    ProposedSize = new Loc(5, 5),
                };
                gridPlan.PublicArrayRooms[1].RoomGen = gen;
            }

            gridPlan.PublicVHalls[0][0].SetHall(new GridHallPlan(new TestFloorPlanGen('a'), new ComponentCollection()));

            TestFloorPlan compareFloorPlan;
            {
                var links = new Tuple<char, char>[]
                {
                    Tuple.Create('A', 'a'),
                    Tuple.Create('a', 'B'),
                };
                compareFloorPlan = TestFloorPlan.InitFloorToContext(
                    gridPlan.Size,
                    new Rect[] { new Rect(12, 0, 11, 5), new Rect(0, 6, 5, 5) },
                    new Rect[] { new Rect(18, 5, 5, 1) },
                    links);
            }

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(It.IsAny<int>())).Returns(0);

            var floorPlan = new TestFloorPlan();
            floorPlan.InitSize(gridPlan.Size, gridPlan.Wrap);

            Mock<IFloorPlanTestContext> mockMap = new Mock<IFloorPlanTestContext>(MockBehavior.Strict);
            mockMap.SetupGet(p => p.Rand).Returns(testRand.Object);
            mockMap.SetupGet(p => p.RoomPlan).Returns(floorPlan);

            gridPlan.PlaceRoomsOnFloor(mockMap.Object);

            TestFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);
        }

        [Test]
        public void PlaceConnectedLargeRoomOnFloor()
        {
            // place two rooms connected by a hall
            string[] inGrid =
            {
                "A#B",
                ". .",
                "A#B",
            };

            TestGridFloorPlan gridPlan = TestGridFloorPlan.InitGridToContext(inGrid, 5, 5);
            for (int ii = 0; ii < gridPlan.RoomCount; ii++)
            {
                var gen = new TestFloorPlanGen(((TestGridRoomGen)gridPlan.GetRoom(ii)).Identifier)
                {
                    ProposedSize = new Loc(5, 11),
                };
                gridPlan.PublicArrayRooms[ii].RoomGen = gen;
            }

            gridPlan.PublicHHalls[0][0].SetHall(new GridHallPlan(new TestFloorPlanGen('a'), new ComponentCollection()));
            gridPlan.PublicHHalls[0][1].SetHall(new GridHallPlan(new TestFloorPlanGen('b'), new ComponentCollection()));

            TestFloorPlan compareFloorPlan;
            {
                var links = new Tuple<char, char>[]
                {
                    Tuple.Create('A', 'a'),
                    Tuple.Create('a', 'B'),
                    Tuple.Create('A', 'b'),
                    Tuple.Create('b', 'B'),
                };
                compareFloorPlan = TestFloorPlan.InitFloorToContext(
                    gridPlan.Size,
                    new Rect[] { new Rect(0, 0, 5, 11), new Rect(6, 0, 5, 11) },
                    new Rect[] { new Rect(5, 0, 1, 5), new Rect(5, 6, 1, 5) },
                    links);
            }

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(It.IsAny<int>())).Returns(0);

            var floorPlan = new TestFloorPlan();
            floorPlan.InitSize(gridPlan.Size, gridPlan.Wrap);

            Mock<IFloorPlanTestContext> mockMap = new Mock<IFloorPlanTestContext>(MockBehavior.Strict);
            mockMap.SetupGet(p => p.Rand).Returns(testRand.Object);
            mockMap.SetupGet(p => p.RoomPlan).Returns(floorPlan);

            gridPlan.PlaceRoomsOnFloor(mockMap.Object);

            TestFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);
        }

        [Test]
        public void PlaceConnectedLargeRoomOnFloorWrapped()
        {
            // place two rooms connected by a hall
            string[] inGrid =
            {
                "0.A#B.",
                ". . . ",
                "0.0.0.",
                ". . . ",
                "0.A#B.",
                ". . . ",
            };

            TestGridFloorPlan gridPlan = TestGridFloorPlan.InitGridToContext(inGrid, 5, 5);
            for (int ii = 0; ii < gridPlan.RoomCount; ii++)
            {
                var gen = new TestFloorPlanGen(((TestGridRoomGen)gridPlan.GetRoom(ii)).Identifier)
                {
                    ProposedSize = new Loc(5, 11),
                };
                gridPlan.PublicArrayRooms[ii].RoomGen = gen;
            }

            gridPlan.PublicHHalls[1][0].SetHall(new GridHallPlan(new TestFloorPlanGen('a'), new ComponentCollection()));
            gridPlan.PublicHHalls[1][2].SetHall(new GridHallPlan(new TestFloorPlanGen('b'), new ComponentCollection()));

            TestFloorPlan compareFloorPlan;
            {
                var links = new Tuple<char, char>[]
                {
                    Tuple.Create('A', 'a'),
                    Tuple.Create('a', 'B'),
                    Tuple.Create('A', 'b'),
                    Tuple.Create('b', 'B'),
                };
                compareFloorPlan = TestFloorPlan.InitFloorToContext(
                    gridPlan.Size,
                    new Rect[] { new Rect(6, 12, 5, 11), new Rect(12, 12, 5, 11) },
                    new Rect[] { new Rect(11, 18, 1, 5), new Rect(11, 12, 1, 5) },
                    links);
            }

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(It.IsAny<int>())).Returns(0);

            var floorPlan = new TestFloorPlan();
            floorPlan.InitSize(gridPlan.Size, gridPlan.Wrap);

            Mock<IFloorPlanTestContext> mockMap = new Mock<IFloorPlanTestContext>(MockBehavior.Strict);
            mockMap.SetupGet(p => p.Rand).Returns(testRand.Object);
            mockMap.SetupGet(p => p.RoomPlan).Returns(floorPlan);

            gridPlan.PlaceRoomsOnFloor(mockMap.Object);

            TestFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);
        }

        [Test]
        public void PlaceRoomsOnFloorRing()
        {
            // place a ring of rooms connected by halls
            string[] inGrid =
            {
                "A#B",
                "# #",
                "D#C",
            };

            TestGridFloorPlan gridPlan = TestGridFloorPlan.InitGridToContext(inGrid, 5, 5);
            for (int ii = 0; ii < gridPlan.RoomCount; ii++)
            {
                var gen = new TestFloorPlanGen(((TestGridRoomGen)gridPlan.GetRoom(ii)).Identifier)
                {
                    ProposedSize = new Loc(5, 5),
                };
                gridPlan.PublicArrayRooms[ii].RoomGen = gen;
            }

            gridPlan.PublicVHalls[0][0].SetHall(new GridHallPlan(new TestFloorPlanGen('a'), new ComponentCollection()));
            gridPlan.PublicVHalls[1][0].SetHall(new GridHallPlan(new TestFloorPlanGen('b'), new ComponentCollection()));
            gridPlan.PublicHHalls[0][0].SetHall(new GridHallPlan(new TestFloorPlanGen('c'), new ComponentCollection()));
            gridPlan.PublicHHalls[0][1].SetHall(new GridHallPlan(new TestFloorPlanGen('d'), new ComponentCollection()));

            TestFloorPlan compareFloorPlan;
            {
                var links = new Tuple<char, char>[]
                {
                    Tuple.Create('A', 'a'),
                    Tuple.Create('a', 'D'),
                    Tuple.Create('B', 'b'),
                    Tuple.Create('b', 'C'),
                    Tuple.Create('A', 'c'),
                    Tuple.Create('c', 'B'),
                    Tuple.Create('D', 'd'),
                    Tuple.Create('d', 'C'),
                };
                compareFloorPlan = TestFloorPlan.InitFloorToContext(
                    gridPlan.Size,
                    new Rect[] { new Rect(0, 0, 5, 5), new Rect(6, 0, 5, 5), new Rect(6, 6, 5, 5), new Rect(0, 6, 5, 5) },
                    new Rect[] { new Rect(0, 5, 5, 1), new Rect(6, 5, 5, 1), new Rect(5, 0, 1, 5), new Rect(5, 6, 1, 5) },
                    links);
            }

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(It.IsAny<int>())).Returns(0);

            var floorPlan = new TestFloorPlan();
            floorPlan.InitSize(gridPlan.Size, gridPlan.Wrap);

            Mock<IFloorPlanTestContext> mockMap = new Mock<IFloorPlanTestContext>(MockBehavior.Strict);
            mockMap.SetupGet(p => p.Rand).Returns(testRand.Object);
            mockMap.SetupGet(p => p.RoomPlan).Returns(floorPlan);

            gridPlan.PlaceRoomsOnFloor(mockMap.Object);

            TestFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);
        }

        [Test]
        public void PlaceRoomsOnFloorDefault()
        {
            // place a line of rooms with one default
            string[] inGrid =
            {
                "A#B#C",
                ". . .",
                "0.0.0",
            };

            TestGridFloorPlan gridPlan = TestGridFloorPlan.InitGridToContext(inGrid, 5, 5);
            {
                var gen = new TestFloorPlanGen('A') { ProposedSize = new Loc(5, 5) };
                gridPlan.PublicArrayRooms[0].RoomGen = gen;
            }

            {
                gridPlan.PublicArrayRooms[1].RoomGen = new RoomGenDefault<IFloorPlanTestContext>();
                gridPlan.PublicArrayRooms[1].PreferHall = true;
            }

            {
                var gen = new TestFloorPlanGen('B') { ProposedSize = new Loc(5, 5) };
                gridPlan.PublicArrayRooms[2].RoomGen = gen;
            }

            gridPlan.PublicHHalls[0][0].SetHall(new GridHallPlan(new TestFloorPlanGen('b'), new ComponentCollection()));
            gridPlan.PublicHHalls[1][0].SetHall(new GridHallPlan(new TestFloorPlanGen('c'), new ComponentCollection()));

            TestFloorPlan compareFloorPlan = TestFloorPlan.InitFloorToContext(
                gridPlan.Size,
                new Rect[] { new Rect(0, 0, 5, 5), new Rect(12, 0, 5, 5) },
                new Rect[] { new Rect(6, 0, 1, 1), new Rect(5, 0, 1, 5), new Rect(7, 0, 5, 5) },
                new Tuple<char, char>[] { Tuple.Create('A', 'b'), Tuple.Create('b', 'a'), Tuple.Create('a', 'c'), Tuple.Create('c', 'B') });

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(It.IsAny<int>())).Returns(0);

            var floorPlan = new TestFloorPlan();
            floorPlan.InitSize(gridPlan.Size, gridPlan.Wrap);

            Mock<IFloorPlanTestContext> mockMap = new Mock<IFloorPlanTestContext>(MockBehavior.Strict);
            mockMap.SetupGet(p => p.Rand).Returns(testRand.Object);
            mockMap.SetupGet(p => p.RoomPlan).Returns(floorPlan);

            gridPlan.PlaceRoomsOnFloor(mockMap.Object);

            // check the rooms
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
                {
                    Assert.That(plan.RoomGen, Is.EqualTo(comparePlan.RoomGen));
                }
                else
                {
                    // special case for the default
                    Assert.That(plan.RoomGen, Is.TypeOf<RoomGenDefault<IFloorPlanTestContext>>());
                    Assert.That(plan.RoomGen.Draw, Is.EqualTo(comparePlan.RoomGen.Draw));
                }

                Assert.That(plan.Adjacents, Is.EqualTo(comparePlan.Adjacents));
            }
        }

        [Test]
        public void PlaceRoomsOnFloorIntrusiveHalls()
        {
            // place two connected rooms that take up two spaces
            string[] inGrid =
            {
                "A.0",
                ". .",
                "A#B",
                ". .",
                "0.B",
            };

            TestGridFloorPlan gridPlan = TestGridFloorPlan.InitGridToContext(inGrid, 5, 5);
            for (int ii = 0; ii < gridPlan.RoomCount; ii++)
            {
                var gen = new TestFloorPlanGen(((TestGridRoomGen)gridPlan.GetRoom(ii)).Identifier)
                {
                    ProposedSize = new Loc(2, 2),
                };

                gridPlan.PublicArrayRooms[ii].RoomGen = gen;
            }

            gridPlan.PublicHHalls[0][1].SetHall(new GridHallPlan(new TestFloorPlanGen('a'), new ComponentCollection()));

            TestFloorPlan compareFloorPlan = TestFloorPlan.InitFloorToContext(
                gridPlan.Size,
                new Rect[] { new Rect(0, 0, 2, 2), new Rect(9, 15, 2, 2) },
                new Rect[] { new Rect(2, 1, 4, 10), new Rect(6, 6, 3, 10) },
                new Tuple<char, char>[] { Tuple.Create('A', 'a'), Tuple.Create('a', 'b'), Tuple.Create('b', 'B') });
            ((TestFloorPlanGen)compareFloorPlan.PublicHalls[1].RoomGen).Identifier = 'a';

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            Moq.Language.ISetupSequentialResult<int> seq = testRand.SetupSequence(p => p.Next(4));
            seq = seq.Returns(0);
            seq = seq.Returns(3);
            seq = testRand.SetupSequence(p => p.Next(10));
            seq = seq.Returns(0);
            seq = seq.Returns(9);

            var floorPlan = new TestFloorPlan();
            floorPlan.InitSize(gridPlan.Size, gridPlan.Wrap);

            Mock<IFloorPlanTestContext> mockMap = new Mock<IFloorPlanTestContext>(MockBehavior.Strict);
            mockMap.SetupGet(p => p.Rand).Returns(testRand.Object);
            mockMap.SetupGet(p => p.RoomPlan).Returns(floorPlan);

            gridPlan.PlaceRoomsOnFloor(mockMap.Object);

            TestFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);
        }

        [Test]
        public void PlaceRoomsOnFloorIntrusiveHallsAAonly()
        {
            // place two connected rooms that take up two spaces
            string[] inGrid =
            {
                "A.0",
                ". .",
                "A#B",
                ". .",
                "0.B",
            };

            TestGridFloorPlan gridPlan = TestGridFloorPlan.InitGridToContext(inGrid, 5, 5);
            for (int ii = 0; ii < gridPlan.RoomCount; ii++)
            {
                var gen = new TestFloorPlanGen(((TestGridRoomGen)gridPlan.GetRoom(ii)).Identifier)
                {
                    ProposedSize = new Loc(2, 2),
                };
                gridPlan.PublicArrayRooms[ii].RoomGen = gen;
            }

            gridPlan.PublicHHalls[0][1].SetHall(new GridHallPlan(new TestFloorPlanGen('a'), new ComponentCollection()));

            TestFloorPlan compareFloorPlan = TestFloorPlan.InitFloorToContext(
                gridPlan.Size,
                new Rect[] { new Rect(0, 0, 2, 2), new Rect(9, 6, 2, 2) },
                new Rect[] { new Rect(2, 1, 4, 7), new Rect(6, 6, 3, 2) },
                new Tuple<char, char>[] { Tuple.Create('A', 'a'), Tuple.Create('a', 'b'), Tuple.Create('b', 'B') });
            ((TestFloorPlanGen)compareFloorPlan.PublicHalls[1].RoomGen).Identifier = 'a';

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            Moq.Language.ISetupSequentialResult<int> seq = testRand.SetupSequence(p => p.Next(4));
            seq = seq.Returns(0);
            seq = seq.Returns(3);
            seq = testRand.SetupSequence(p => p.Next(10));
            seq = seq.Returns(0);
            seq = seq.Returns(0);

            var floorPlan = new TestFloorPlan();
            floorPlan.InitSize(gridPlan.Size, gridPlan.Wrap);

            Mock<IFloorPlanTestContext> mockMap = new Mock<IFloorPlanTestContext>(MockBehavior.Strict);
            mockMap.SetupGet(p => p.Rand).Returns(testRand.Object);
            mockMap.SetupGet(p => p.RoomPlan).Returns(floorPlan);

            gridPlan.PlaceRoomsOnFloor(mockMap.Object);

            TestFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);
        }

        [Test]
        public void PlaceRoomsOnFloorIntrusiveHallsCloseB()
        {
            // place a ring of rooms connected by halls
            string[] inGrid =
            {
                "A.0",
                ". .",
                "A#B",
                ". .",
                "0.B",
            };

            TestGridFloorPlan gridPlan = TestGridFloorPlan.InitGridToContext(inGrid, 5, 5);
            for (int ii = 0; ii < gridPlan.RoomCount; ii++)
            {
                var gen = new TestFloorPlanGen(((TestGridRoomGen)gridPlan.GetRoom(ii)).Identifier)
                {
                    ProposedSize = new Loc(2, 2),
                };
                gridPlan.PublicArrayRooms[ii].RoomGen = gen;
            }

            gridPlan.PublicHHalls[0][1].SetHall(new GridHallPlan(new TestFloorPlanGen('a'), new ComponentCollection()));

            TestFloorPlan compareFloorPlan = TestFloorPlan.InitFloorToContext(
                gridPlan.Size,
                new Rect[] { new Rect(0, 0, 2, 2), new Rect(6, 15, 2, 2) },
                new Rect[] { new Rect(2, 1, 3, 10), new Rect(5, 6, 1, 10) },
                new Tuple<char, char>[] { Tuple.Create('A', 'a'), Tuple.Create('a', 'b'), Tuple.Create('b', 'B') });
            ((TestFloorPlanGen)compareFloorPlan.PublicHalls[1].RoomGen).Identifier = 'a';

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            Moq.Language.ISetupSequentialResult<int> seq = testRand.SetupSequence(p => p.Next(4));
            seq = seq.Returns(0);
            seq = seq.Returns(0);
            seq = testRand.SetupSequence(p => p.Next(10));
            seq = seq.Returns(0);
            seq = seq.Returns(9);

            var floorPlan = new TestFloorPlan();
            floorPlan.InitSize(gridPlan.Size, gridPlan.Wrap);

            Mock<IFloorPlanTestContext> mockMap = new Mock<IFloorPlanTestContext>(MockBehavior.Strict);
            mockMap.SetupGet(p => p.Rand).Returns(testRand.Object);
            mockMap.SetupGet(p => p.RoomPlan).Returns(floorPlan);

            gridPlan.PlaceRoomsOnFloor(mockMap.Object);

            TestFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);
        }

        [Test]
        public void GetAdjacentRoomsOneCellNoneNear()
        {
            // completely lone one-cell room
            string[] inGrid =
            {
                "0.0.0",
                ". . .",
                "0.A.0",
                ". . .",
                "0.0.0",
            };

            TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid);
            List<int> rooms = floorPlan.GetAdjacentRooms(0);
            List<int> compare = new List<int>();
            Assert.That(rooms, Is.EqualTo(compare));
        }

        [Test]
        public void GetAdjacentRoomsOneCellAllNear()
        {
            // completely filled one-cell room
            string[] inGrid =
            {
                "0.D.0",
                ". # .",
                "C#A#E",
                ". # .",
                "0.B.0",
            };

            TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid);
            List<int> rooms = floorPlan.GetAdjacentRooms(0);
            List<int> compare = new List<int> { 3, 1, 2, 4 };
            Assert.That(rooms, Is.EqualTo(compare));
        }

        [Test]
        public void GetAdjacentRoomsOneCellAllNearDetached()
        {
            // completely filled for all but the one-cell room, detached
            string[] inGrid =
            {
                "G#D#H",
                "# . #",
                "C.A.E",
                "# . #",
                "F#B#I",
            };

            TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid);
            List<int> rooms = floorPlan.GetAdjacentRooms(0);
            List<int> compare = new List<int>();
            Assert.That(rooms, Is.EqualTo(compare));
        }

        [Test]
        public void GetAdjacentRoomsMultiCellAll()
        {
            // completely filled many-cell room
            string[] inGrid =
            {
                "0.G.H.I.0",
                ". # # # .",
                "E#A.A.A#J",
                ". . . . .",
                "F#A.A.A#K",
                ". # # # .",
                "0.B.C.D.0",
            };

            TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid);
            List<int> rooms = floorPlan.GetAdjacentRooms(0);
            List<int> compare = new List<int> { 6, 1, 7, 2, 8, 3, 4, 9, 5, 10 };
            Assert.That(rooms, Is.EqualTo(compare));
        }

        [Test]
        public void GetAdjacentRoomsMultiCellRepeat()
        {
            // many cell room with double dip on a room
            string[] inGrid =
            {
                "0.C.0",
                ". # .",
                "B#A#D",
                ". . .",
                "B#A#D",
            };

            TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid);
            List<int> rooms = floorPlan.GetAdjacentRooms(0);
            List<int> compare = new List<int> { 2, 1, 3 };
            Assert.That(rooms, Is.EqualTo(compare));
        }

        [Test]
        [Ignore("TODO")]
        public void GetRoomNum()
        {
            throw new NotImplementedException();
        }

        [Test]
        [Ignore("TODO")]
        public void TestDisconnect()
        {
            throw new NotImplementedException();
        }

        [Test]
        [Ignore("TODO")]
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
            var floorPlan = new TestGridFloorPlan();
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
            // a room that takes up one cell
            string[] inGrid =
            {
                "0.0.0.0",
                ". . . .",
                "0.A.0.0",
                ". . . .",
                "0.0.0.0",
            };

            TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid, 6, 4);
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            IRoomGen testGen = floorPlan.PublicArrayRooms[0].RoomGen;
            testGen.PrepareSize(testRand.Object, new Loc(4, 2));
            testGen.SetLoc(new Loc(8, 6));
            IntRange bounds = floorPlan.GetHallTouchRange(testGen.Draw, testGen.GetFulfillableBorder, dir, 1);
            IntRange compareBounds = new IntRange(rangeMin, rangeMax);
            Assert.That(bounds, Is.EqualTo(compareBounds));
        }

        [Test]

        // a room that takes multiple cells but has area in the focused cell
        [TestCase(Dir4.Down, 0, 1, 6)]
        [TestCase(Dir4.Left, 0, 2, 4)]
        [TestCase(Dir4.Left, 1, 5, 9)]
        [TestCase(Dir4.Right, 2, 10, 12)]

        // a room that takes multiple cells and does not have area in the focused cell
        [TestCase(Dir4.Down, 1, 5, 8)]
        public void GetHallTouchRangeLargeRoom(Dir4 dir, int tier, int rangeMin, int rangeMax)
        {
            // a room that takes up multiple cells
            string[] inGrid =
            {
                "A.A.0.0",
                ". . . .",
                "A.A.0.0",
                ". . . .",
                "A.A.0.0",
                ". . . .",
                "0.0.0.0",
            };

            TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid, 6, 4);
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            IRoomGen testGen = floorPlan.PublicArrayRooms[0].RoomGen;
            testGen.PrepareSize(testRand.Object, new Loc(5, 10));
            testGen.SetLoc(new Loc(1, 2));
            IntRange bounds = floorPlan.GetHallTouchRange(testGen.Draw, testGen.GetFulfillableBorder, dir, tier);
            IntRange compareBounds = new IntRange(rangeMin, rangeMax);
            Assert.That(bounds, Is.EqualTo(compareBounds));
        }

        // a room that takes up multiple cells and has fulfillable border in specific parts of the side
        // left only
        [TestCase(0, 2, 2, 4)]

        // left and right intersecting
        [TestCase(3, 6, 5, 6)]

        // right only (close to left)
        [TestCase(4, 5, 5, 7)]

        // right only (close to end)
        [TestCase(8, 9, 5, 11)]
        public void GetHallTouchRangeLargeRoomLeft(int fulfillStart, int fulfillEnd, int rangeMin, int rangeMax)
        {
            string[] inGrid =
            {
                "A.A.0.0",
                ". . . .",
                "A.A.0.0",
                ". . . .",
                "0.0.0.0",
            };
            int width = 10;
            bool[] fulfillable = new bool[width];
            for (int ii = fulfillStart; ii < fulfillEnd; ii++)
                fulfillable[ii] = true;

            TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid, 6, 4);
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            TestGridRoomGen testGen = (TestGridRoomGen)floorPlan.PublicArrayRooms[0].RoomGen;
            testGen.PrepareSize(testRand.Object, new Loc(width, 6));
            testGen.PrepareFulfillableBorder(Dir4.Down, fulfillable);
            testGen.SetLoc(new Loc(2, 1));
            IntRange bounds = floorPlan.GetHallTouchRange(testGen.Draw, testGen.GetFulfillableBorder, Dir4.Down, 0);
            IntRange compareBounds = new IntRange(rangeMin, rangeMax);
            Assert.That(bounds, Is.EqualTo(compareBounds));
        }

        [TestCase(1, 12, 14)]
        [TestCase(2, 13, 15)]
        public void GetHallTouchRangeLargeRoomBorder(int tier, int rangeMin, int rangeMax)
        {
            string[] inGrid =
            {
                "0.A.A.0",
                ". . . .",
                "0.A.A.0",
                ". . . .",
                "0.0.0.0",
            };
            int width = 13;
            int fulfill = 6;
            bool[] fulfillable = new bool[width];
            fulfillable[fulfill] = true;

            TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid, 6, 4);
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            TestGridRoomGen testGen = (TestGridRoomGen)floorPlan.PublicArrayRooms[0].RoomGen;
            testGen.PrepareSize(testRand.Object, new Loc(width, 6));
            testGen.PrepareFulfillableBorder(Dir4.Down, fulfillable);
            testGen.SetLoc(new Loc(7, 1));
            IntRange bounds = floorPlan.GetHallTouchRange(testGen.Draw, testGen.GetFulfillableBorder, Dir4.Down, tier);
            IntRange compareBounds = new IntRange(rangeMin, rangeMax);
            Assert.That(bounds, Is.EqualTo(compareBounds));
        }

        // a room that takes up multiple cells and has fulfillable border in specific parts of the side
        // left only
        [TestCase(0, 0, 2, 8)]

        // middle only
        [TestCase(5, 10, 7, 13)]

        // left and middle
        [TestCase(0, 9, 11, 12)]

        // right only
        [TestCase(15, 15, 12, 18)]

        // left and right, both close
        [TestCase(4, 11, 6, 8)]

        // left and right, left is closer
        [TestCase(4, 12, 6, 8)]

        // left and right, right is closer
        [TestCase(3, 11, 12, 14)]
        public void GetHallTouchRangeLargeRoomMiddle(int fulfill1, int fulfill2, int rangeMin, int rangeMax)
        {
            string[] inGrid =
            {
                "A.A.A.0",
                ". . . .",
                "A.A.A.0",
                ". . . .",
                "0.0.0.0",
            };
            int width = 16;
            bool[] fulfillable = new bool[width];
            fulfillable[fulfill1] = true;
            fulfillable[fulfill2] = true;

            TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid, 6, 4);
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            TestGridRoomGen testGen = (TestGridRoomGen)floorPlan.PublicArrayRooms[0].RoomGen;
            testGen.PrepareSize(testRand.Object, new Loc(width, 6));
            testGen.PrepareFulfillableBorder(Dir4.Down, fulfillable);
            testGen.SetLoc(new Loc(2, 1));
            IntRange bounds = floorPlan.GetHallTouchRange(testGen.Draw, testGen.GetFulfillableBorder, Dir4.Down, 1);
            IntRange compareBounds = new IntRange(rangeMin, rangeMax);
            Assert.That(bounds, Is.EqualTo(compareBounds));
        }
    }
}