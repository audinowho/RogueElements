// <copyright file="GridPathBranchTest.cs" company="Audino">
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
    public class GridPathBranchTest
    {
        [Test]
        [Ignore("TODO")]
        public void PlacePathWithComponent()
        {
            throw new NotImplementedException();
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void GetPossibleExpansionsAlone(bool branch)
        {
            string[] inGrid =
            {
                "0.0.0",
                ". . .",
                "0.A.0",
                ". . .",
                "0.0.0",
            };

            TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid);

            List<LocRay4> rays = GridPathBranch<IGridPathTestContext, TestTile>.GetPossibleExpansions(floorPlan, branch);
            List<LocRay4> compare = new List<LocRay4>();
            if (!branch)
            {
                compare.Add(new LocRay4(new Loc(1), Dir4.Down));
                compare.Add(new LocRay4(new Loc(1), Dir4.Left));
                compare.Add(new LocRay4(new Loc(1), Dir4.Up));
                compare.Add(new LocRay4(new Loc(1), Dir4.Right));
            }

            Assert.That(rays, Is.EqualTo(compare));
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void GetPossibleExpansionsDouble(bool branch)
        {
            string[] inGrid =
            {
                "0.0.0.0",
                ". . . .",
                "0.A#B.0",
                ". . . .",
                "0.0.0.0",
            };

            TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid);

            List<LocRay4> rays = GridPathBranch<IGridPathTestContext, TestTile>.GetPossibleExpansions(floorPlan, branch);
            List<LocRay4> compare = new List<LocRay4>();
            if (!branch)
            {
                compare.Add(new LocRay4(new Loc(1), Dir4.Down));
                compare.Add(new LocRay4(new Loc(1), Dir4.Left));
                compare.Add(new LocRay4(new Loc(1), Dir4.Up));
                compare.Add(new LocRay4(new Loc(2, 1), Dir4.Down));
                compare.Add(new LocRay4(new Loc(2, 1), Dir4.Up));
                compare.Add(new LocRay4(new Loc(2, 1), Dir4.Right));
            }

            Assert.That(rays, Is.EqualTo(compare));
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void GetPossibleExpansionsAngle(bool branch)
        {
            string[] inGrid =
            {
                "0.0.0.0",
                ". . . .",
                "0.0.C.0",
                ". . # .",
                "0.A#B.0",
                ". . . .",
                "0.0.0.0",
            };

            TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid);

            List<LocRay4> rays = GridPathBranch<IGridPathTestContext, TestTile>.GetPossibleExpansions(floorPlan, branch);
            List<LocRay4> compare = new List<LocRay4>();
            if (!branch)
            {
                compare.Add(new LocRay4(new Loc(1, 2), Dir4.Down));
                compare.Add(new LocRay4(new Loc(1, 2), Dir4.Left));
                compare.Add(new LocRay4(new Loc(1, 2), Dir4.Up));
                compare.Add(new LocRay4(new Loc(2, 1), Dir4.Left));
                compare.Add(new LocRay4(new Loc(2, 1), Dir4.Up));
                compare.Add(new LocRay4(new Loc(2, 1), Dir4.Right));
            }
            else
            {
                compare.Add(new LocRay4(new Loc(2, 2), Dir4.Down));
                compare.Add(new LocRay4(new Loc(2, 2), Dir4.Right));
            }

            Assert.That(rays, Is.EqualTo(compare));
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void GetPossibleExpansionsStraight(bool branch)
        {
            string[] inGrid =
            {
                "0.0.0.0.0",
                ". . . . .",
                "0.A#B#C.0",
                ". . . . .",
                "0.0.0.0.0",
            };

            TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid);

            List<LocRay4> rays = GridPathBranch<IGridPathTestContext, TestTile>.GetPossibleExpansions(floorPlan, branch);
            List<LocRay4> compare = new List<LocRay4>();
            if (!branch)
            {
                compare.Add(new LocRay4(new Loc(1), Dir4.Down));
                compare.Add(new LocRay4(new Loc(1), Dir4.Left));
                compare.Add(new LocRay4(new Loc(1), Dir4.Up));
                compare.Add(new LocRay4(new Loc(3, 1), Dir4.Down));
                compare.Add(new LocRay4(new Loc(3, 1), Dir4.Up));
                compare.Add(new LocRay4(new Loc(3, 1), Dir4.Right));
            }
            else
            {
                compare.Add(new LocRay4(new Loc(2, 1), Dir4.Down));
                compare.Add(new LocRay4(new Loc(2, 1), Dir4.Up));
            }

            Assert.That(rays, Is.EqualTo(compare));
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void GetPossibleExpansionsT(bool branch)
        {
            string[] inGrid =
            {
                "0.0.0.0.0",
                ". . . . .",
                "0.A#B#C.0",
                ". . # . .",
                "0.0.D.0.0",
                ". . . . .",
                "0.0.0.0.0",
            };

            TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid);

            List<LocRay4> rays = GridPathBranch<IGridPathTestContext, TestTile>.GetPossibleExpansions(floorPlan, branch);
            List<LocRay4> compare = new List<LocRay4>();
            if (!branch)
            {
                compare.Add(new LocRay4(new Loc(1), Dir4.Down));
                compare.Add(new LocRay4(new Loc(1), Dir4.Left));
                compare.Add(new LocRay4(new Loc(1), Dir4.Up));
                compare.Add(new LocRay4(new Loc(3, 1), Dir4.Down));
                compare.Add(new LocRay4(new Loc(3, 1), Dir4.Up));
                compare.Add(new LocRay4(new Loc(3, 1), Dir4.Right));
                compare.Add(new LocRay4(new Loc(2, 2), Dir4.Down));
                compare.Add(new LocRay4(new Loc(2, 2), Dir4.Left));
                compare.Add(new LocRay4(new Loc(2, 2), Dir4.Right));
            }
            else
            {
                compare.Add(new LocRay4(new Loc(2, 1), Dir4.Up));
            }

            Assert.That(rays, Is.EqualTo(compare));
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void GetPossibleExpansionsCorner(bool branch)
        {
            string[] inGrid =
            {
                "A.0.0",
                ". . .",
                "0.0.0",
                ". . .",
                "0.0.0",
            };

            TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid);

            List<LocRay4> rays = GridPathBranch<IGridPathTestContext, TestTile>.GetPossibleExpansions(floorPlan, branch);
            List<LocRay4> compare = new List<LocRay4>();
            if (!branch)
            {
                compare.Add(new LocRay4(new Loc(0), Dir4.Down));
                compare.Add(new LocRay4(new Loc(0), Dir4.Right));
            }

            Assert.That(rays, Is.EqualTo(compare));
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void GetPossibleExpansionsAngleCorner(bool branch)
        {
            string[] inGrid =
            {
                "B#C.0",
                "# . .",
                "A.0.0",
                ". . .",
                "0.0.0",
            };

            TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid);

            List<LocRay4> rays = GridPathBranch<IGridPathTestContext, TestTile>.GetPossibleExpansions(floorPlan, branch);
            List<LocRay4> compare = new List<LocRay4>();
            if (!branch)
            {
                compare.Add(new LocRay4(new Loc(0, 1), Dir4.Down));
                compare.Add(new LocRay4(new Loc(0, 1), Dir4.Right));
                compare.Add(new LocRay4(new Loc(1, 0), Dir4.Down));
                compare.Add(new LocRay4(new Loc(1, 0), Dir4.Right));
            }

            Assert.That(rays, Is.EqualTo(compare));
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void GetPossibleExpansionsUCorner(bool branch)
        {
            string[] inGrid =
            {
                "A.D",
                "# #",
                "B#C",
            };

            TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid);

            List<LocRay4> rays = GridPathBranch<IGridPathTestContext, TestTile>.GetPossibleExpansions(floorPlan, branch);
            List<LocRay4> compare = new List<LocRay4>();

            Assert.That(rays, Is.EqualTo(compare));
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void GetPossibleExpansionsUNCorner(bool branch)
        {
            string[] inGrid =
            {
                "A.D#E",
                "# # #",
                "B#C.F",
            };

            TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid);

            List<LocRay4> rays = GridPathBranch<IGridPathTestContext, TestTile>.GetPossibleExpansions(floorPlan, branch);
            List<LocRay4> compare = new List<LocRay4>();

            Assert.That(rays, Is.EqualTo(compare));
        }

        [Test]
        public void CreatePath0Percent()
        {
            string[] inGrid =
            {
                "0.0.0.0",
                ". . . .",
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
                "0.0.0.0",
                ". . . .",
                "0.A.0.0",
                ". . . .",
                "0.0.0.0",
            };

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            Moq.Language.ISetupSequentialResult<int> seq = testRand.SetupSequence(p => p.Next(4));
            seq = seq.Returns(1);
            seq = seq.Returns(2);
            testRand.Setup(p => p.Next(0, 0)).Returns(0);

            var pathGen = new GridPathBranch<IGridPathTestContext, TestTile>
            {
                RoomRatio = new RandRange(0),
                BranchRatio = new RandRange(0),
                NoForcedBranches = false,
            };

            var mockHalls = new Mock<IRandPicker<PermissiveRoomGen<IGridPathTestContext, TestTile>>>(MockBehavior.Strict);
            mockHalls.Setup(p => p.Pick(testRand.Object)).Returns(new TestGridRoomGen());
            pathGen.GenericHalls = mockHalls.Object;
            var mockRooms = new Mock<IRandPicker<RoomGen<IGridPathTestContext, TestTile>>>(MockBehavior.Strict);
            Moq.Language.ISetupSequentialResult<RoomGen<IGridPathTestContext, TestTile>> roomSeq = mockRooms.SetupSequence(p => p.Pick(testRand.Object));
            roomSeq.Returns(new TestGridRoomGen('A'));
            pathGen.GenericRooms = mockRooms.Object;

            TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid);
            TestGridFloorPlan compareFloorPlan = TestGridFloorPlan.InitGridToContext(outGrid);

            pathGen.ApplyToPath(testRand.Object, floorPlan);

            TestGridFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);

            testRand.Verify(p => p.Next(4), Times.Exactly(2));

            mockHalls.Verify(p => p.Pick(testRand.Object), Times.Never);
            mockRooms.Verify(p => p.Pick(testRand.Object), Times.Exactly(1));
        }

        [Test]
        public void CreatePath100Percent()
        {
            string[] inGrid =
            {
                "0.0.0",
                ". . .",
                "0.0.0",
                ". . .",
                "0.0.0",
            };

            string[] outGrid =
            {
                "A#B#C",
                ". . #",
                "F#E#D",
                "# . .",
                "G#H#I",
            };

            TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid);
            TestGridFloorPlan compareFloorPlan = TestGridFloorPlan.InitGridToContext(outGrid);

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            Moq.Language.ISetupSequentialResult<int> seq = testRand.SetupSequence(p => p.Next(It.IsAny<int>()));
            seq = seq.Returns(0);
            seq = seq.Returns(0);
            testRand.Setup(p => p.Next(0, 0)).Returns(0);
            testRand.Setup(p => p.Next(100, 100)).Returns(100);
            seq = seq.Returns(0); // 0,0
            seq = seq.Returns(1); // Right
            seq = seq.Returns(1); // 1,0
            seq = seq.Returns(1); // Right
            seq = seq.Returns(1); // 2,0
            seq = seq.Returns(0); // Down
            seq = seq.Returns(1); // 2,1
            seq = seq.Returns(1); // Left
            seq = seq.Returns(1); // 1,1
            seq = seq.Returns(1); // Left
            seq = seq.Returns(1); // 0,1
            seq = seq.Returns(0); // Down
            seq = seq.Returns(1); // 0,2
            seq = seq.Returns(0); // Right
            seq = seq.Returns(1); // 1,2
            seq = seq.Returns(0); // Right

            var pathGen = new Mock<GridPathBranch<IGridPathTestContext, TestTile>> { CallBase = true };
            pathGen.Object.RoomRatio = new RandRange(100);
            pathGen.Object.BranchRatio = new RandRange(0);
            pathGen.Object.NoForcedBranches = false;

            var mockHalls = new Mock<IRandPicker<PermissiveRoomGen<IGridPathTestContext, TestTile>>>(MockBehavior.Strict);
            mockHalls.Setup(p => p.Pick(testRand.Object)).Returns(new TestGridRoomGen());
            pathGen.Object.GenericHalls = mockHalls.Object;
            var mockRooms = new Mock<IRandPicker<RoomGen<IGridPathTestContext, TestTile>>>(MockBehavior.Strict);
            Moq.Language.ISetupSequentialResult<RoomGen<IGridPathTestContext, TestTile>> roomSeq = mockRooms.SetupSequence(p => p.Pick(testRand.Object));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('A'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('B'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('C'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('D'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('E'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('F'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('G'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('H'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('I'));
            pathGen.Object.GenericRooms = mockRooms.Object;

            pathGen.Object.ApplyToPath(testRand.Object, floorPlan);

            TestGridFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);

            testRand.Verify(p => p.Next(It.IsAny<int>()), Times.Exactly(18));
            mockHalls.Verify(p => p.Pick(testRand.Object), Times.Exactly(8));
            mockRooms.Verify(p => p.Pick(testRand.Object), Times.Exactly(9));
        }

        [Test]
        public void CreatePath50Percent()
        {
            string[] inGrid =
            {
                "0.0.0",
                ". . .",
                "0.0.0",
                ". . .",
                "0.0.0",
            };

            string[] outGrid =
            {
                "A#B#C",
                ". . #",
                "0.0.D",
                ". . .",
                "0.0.0",
            };

            TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid);
            TestGridFloorPlan compareFloorPlan = TestGridFloorPlan.InitGridToContext(outGrid);

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            Moq.Language.ISetupSequentialResult<int> seq = testRand.SetupSequence(p => p.Next(It.IsAny<int>()));
            seq = seq.Returns(0);
            seq = seq.Returns(0);
            testRand.Setup(p => p.Next(0, 0)).Returns(0);
            testRand.Setup(p => p.Next(50, 50)).Returns(50);
            seq = seq.Returns(0); // 0,0
            seq = seq.Returns(1); // Right
            seq = seq.Returns(1); // 1,0
            seq = seq.Returns(1); // Right
            seq = seq.Returns(1); // 2,0
            seq = seq.Returns(0); // Down

            var pathGen = new Mock<GridPathBranch<IGridPathTestContext, TestTile>> { CallBase = true };
            pathGen.Object.RoomRatio = new RandRange(50);
            pathGen.Object.BranchRatio = new RandRange(0);
            pathGen.Object.NoForcedBranches = false;

            var mockHalls = new Mock<IRandPicker<PermissiveRoomGen<IGridPathTestContext, TestTile>>>(MockBehavior.Strict);
            mockHalls.Setup(p => p.Pick(testRand.Object)).Returns(new TestGridRoomGen());
            pathGen.Object.GenericHalls = mockHalls.Object;
            var mockRooms = new Mock<IRandPicker<RoomGen<IGridPathTestContext, TestTile>>>(MockBehavior.Strict);
            Moq.Language.ISetupSequentialResult<RoomGen<IGridPathTestContext, TestTile>> roomSeq = mockRooms.SetupSequence(p => p.Pick(testRand.Object));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('A'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('B'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('C'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('D'));
            pathGen.Object.GenericRooms = mockRooms.Object;

            pathGen.Object.ApplyToPath(testRand.Object, floorPlan);

            TestGridFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);

            testRand.Verify(p => p.Next(It.IsAny<int>()), Times.Exactly(8));
            mockHalls.Verify(p => p.Pick(testRand.Object), Times.Exactly(3));
            mockRooms.Verify(p => p.Pick(testRand.Object), Times.Exactly(4));
        }

        [Test]
        public void CreatePath100PercentNoFit()
        {
            // a situation in which a no-branching path
            // is forced to branch to make the room quota
            string[] inGrid =
            {
                "0.0.0",
                ". . .",
                "0.0.0",
            };

            string[] outGrid =
            {
                "A#B#E",
                ". # #",
                "D#C.F",
            };

            TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid);
            TestGridFloorPlan compareFloorPlan = TestGridFloorPlan.InitGridToContext(outGrid);

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            Moq.Language.ISetupSequentialResult<int> seq = testRand.SetupSequence(p => p.Next(It.IsAny<int>()));
            seq = seq.Returns(0);
            seq = seq.Returns(0);
            testRand.Setup(p => p.Next(0, 0)).Returns(0);
            testRand.Setup(p => p.Next(100, 100)).Returns(100);
            seq = seq.Returns(0); // 0, 0
            seq = seq.Returns(1); // Right
            seq = seq.Returns(1); // 1, 0
            seq = seq.Returns(0); // Down
            seq = seq.Returns(1); // 1, 1
            seq = seq.Returns(0); // Left
            seq = seq.Returns(1); // 0, 1
            seq = seq.Returns(0); // 0, 0
            seq = seq.Returns(0); // 1, 0
            seq = seq.Returns(0); // Right
            seq = seq.Returns(0); // 2, 0
            seq = seq.Returns(0); // Down

            var pathGen = new Mock<GridPathBranch<IGridPathTestContext, TestTile>> { CallBase = true };
            pathGen.Object.RoomRatio = new RandRange(100);
            pathGen.Object.BranchRatio = new RandRange(0);
            pathGen.Object.NoForcedBranches = false;

            var mockHalls = new Mock<IRandPicker<PermissiveRoomGen<IGridPathTestContext, TestTile>>>(MockBehavior.Strict);
            mockHalls.Setup(p => p.Pick(testRand.Object)).Returns(new TestGridRoomGen());
            pathGen.Object.GenericHalls = mockHalls.Object;
            var mockRooms = new Mock<IRandPicker<RoomGen<IGridPathTestContext, TestTile>>>(MockBehavior.Strict);
            Moq.Language.ISetupSequentialResult<RoomGen<IGridPathTestContext, TestTile>> roomSeq = mockRooms.SetupSequence(p => p.Pick(testRand.Object));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('A'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('B'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('C'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('D'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('E'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('F'));
            pathGen.Object.GenericRooms = mockRooms.Object;

            pathGen.Object.ApplyToPath(testRand.Object, floorPlan);

            TestGridFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);

            testRand.Verify(p => p.Next(It.IsAny<int>()), Times.Exactly(14));
            mockHalls.Verify(p => p.Pick(testRand.Object), Times.Exactly(5));
            mockRooms.Verify(p => p.Pick(testRand.Object), Times.Exactly(6));
        }

        [Test]
        public void CreatePath100PercentNoFitCannotBranch()
        {
            // cannot make branch quota after ten tries
            string[] inGrid =
            {
                "0.0.0",
                ". . .",
                "0.0.0",
            };

            string[] outGrid =
            {
                "A#B.0",
                ". # .",
                "D#C.0",
            };

            TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid);
            TestGridFloorPlan compareFloorPlan = TestGridFloorPlan.InitGridToContext(outGrid);

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            Moq.Language.ISetupSequentialResult<int> seq = testRand.SetupSequence(p => p.Next(It.IsAny<int>()));
            testRand.Setup(p => p.Next(0, 0)).Returns(0);
            testRand.Setup(p => p.Next(100, 100)).Returns(100);
            for (int ii = 0; ii < 10; ii++)
            {
                // start coords
                seq = seq.Returns(0);
                seq = seq.Returns(0);

                seq = seq.Returns(0); // 0, 0
                seq = seq.Returns(1); // Right
                seq = seq.Returns(1); // 1, 0
                seq = seq.Returns(0); // Down
                seq = seq.Returns(1); // 1, 1
                seq = seq.Returns(0); // Left
                seq = seq.Returns(1); // 0, 1
                seq = seq.Returns(0); // 0, 0
            }

            var pathGen = new Mock<GridPathBranch<IGridPathTestContext, TestTile>> { CallBase = true };
            pathGen.Object.RoomRatio = new RandRange(100);
            pathGen.Object.BranchRatio = new RandRange(0);
            pathGen.Object.NoForcedBranches = true;

            var mockHalls = new Mock<IRandPicker<PermissiveRoomGen<IGridPathTestContext, TestTile>>>(MockBehavior.Strict);
            mockHalls.Setup(p => p.Pick(testRand.Object)).Returns(new TestGridRoomGen());
            pathGen.Object.GenericHalls = mockHalls.Object;
            var mockRooms = new Mock<IRandPicker<RoomGen<IGridPathTestContext, TestTile>>>(MockBehavior.Strict);
            Moq.Language.ISetupSequentialResult<RoomGen<IGridPathTestContext, TestTile>> roomSeq = mockRooms.SetupSequence(p => p.Pick(testRand.Object));
            for (int ii = 0; ii < 10; ii++)
            {
                roomSeq = roomSeq.Returns(new TestGridRoomGen('A'));
                roomSeq = roomSeq.Returns(new TestGridRoomGen('B'));
                roomSeq = roomSeq.Returns(new TestGridRoomGen('C'));
                roomSeq = roomSeq.Returns(new TestGridRoomGen('D'));
            }

            pathGen.Object.GenericRooms = mockRooms.Object;

            pathGen.Object.ApplyToPath(testRand.Object, floorPlan);

            TestGridFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);

            testRand.Verify(p => p.Next(It.IsAny<int>()), Times.Exactly(100));
            mockHalls.Verify(p => p.Pick(testRand.Object), Times.Exactly(30));
            mockRooms.Verify(p => p.Pick(testRand.Object), Times.Exactly(40));
        }

        [Test]
        public void CreatePath0PercentBranch()
        {
            string[] inGrid =
            {
                "0.0.0.0.0.0.0.0",
                ". . . . . . . .",
                "0.0.0.0.0.0.0.0",
                ". . . . . . . .",
                "0.0.0.0.0.0.0.0",
            };

            string[] outGrid =
            {
                "A#B#C#D#E#F#G#H",
                ". . . . . . . .",
                "0.0.0.0.0.0.0.0",
                ". . . . . . . .",
                "0.0.0.0.0.0.0.0",
            };

            TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid);
            TestGridFloorPlan compareFloorPlan = TestGridFloorPlan.InitGridToContext(outGrid);

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            Moq.Language.ISetupSequentialResult<int> seq = testRand.SetupSequence(p => p.Next(It.IsAny<int>()));
            seq = seq.Returns(0);
            seq = seq.Returns(0);
            testRand.Setup(p => p.Next(0, 0)).Returns(0);
            testRand.Setup(p => p.Next(34, 34)).Returns(34);
            seq = seq.Returns(0); // 0, 0
            seq = seq.Returns(1); // Right
            seq = seq.Returns(1); // 1, 0
            seq = seq.Returns(1); // Right
            seq = seq.Returns(1); // 2, 0
            seq = seq.Returns(1); // Right
            seq = seq.Returns(1); // 3, 0
            seq = seq.Returns(1); // Right
            seq = seq.Returns(1); // 4, 0
            seq = seq.Returns(1); // Right
            seq = seq.Returns(1); // 5, 0
            seq = seq.Returns(1); // Right
            seq = seq.Returns(1); // 6, 0
            seq = seq.Returns(1); // Right

            var pathGen = new Mock<GridPathBranch<IGridPathTestContext, TestTile>> { CallBase = true };
            pathGen.Object.RoomRatio = new RandRange(34);
            pathGen.Object.BranchRatio = new RandRange(0);
            pathGen.Object.NoForcedBranches = false;

            var mockHalls = new Mock<IRandPicker<PermissiveRoomGen<IGridPathTestContext, TestTile>>>(MockBehavior.Strict);
            mockHalls.Setup(p => p.Pick(testRand.Object)).Returns(new TestGridRoomGen());
            pathGen.Object.GenericHalls = mockHalls.Object;
            var mockRooms = new Mock<IRandPicker<RoomGen<IGridPathTestContext, TestTile>>>(MockBehavior.Strict);
            Moq.Language.ISetupSequentialResult<RoomGen<IGridPathTestContext, TestTile>> roomSeq = mockRooms.SetupSequence(p => p.Pick(testRand.Object));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('A'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('B'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('C'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('D'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('E'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('F'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('G'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('H'));
            pathGen.Object.GenericRooms = mockRooms.Object;

            pathGen.Object.ApplyToPath(testRand.Object, floorPlan);

            TestGridFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);

            testRand.Verify(p => p.Next(It.IsAny<int>()), Times.Exactly(16));
            mockHalls.Verify(p => p.Pick(testRand.Object), Times.Exactly(7));
            mockRooms.Verify(p => p.Pick(testRand.Object), Times.Exactly(8));
        }

        [Test]
        public void CreatePath50PercentBranch()
        {
            string[] inGrid =
            {
                "0.0.0.0.0.0.0.0",
                ". . . . . . . .",
                "0.0.0.0.0.0.0.0",
                ". . . . . . . .",
                "0.0.0.0.0.0.0.0",
            };

            string[] outGrid =
            {
                "0.0.0.0.0.0.0.0",
                ". . . . . . . .",
                "A#B#C#D#F#G#I#J",
                ". # # # . . . .",
                "0.E.H.K.0.0.0.0",
            };

            TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid);
            TestGridFloorPlan compareFloorPlan = TestGridFloorPlan.InitGridToContext(outGrid);

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            Moq.Language.ISetupSequentialResult<int> seq = testRand.SetupSequence(p => p.Next(It.IsAny<int>()));
            seq = seq.Returns(0);
            seq = seq.Returns(1);
            testRand.Setup(p => p.Next(50, 50)).Returns(50);
            testRand.Setup(p => p.Next(46, 46)).Returns(46);
            seq = seq.Returns(0); // 0, 1
            seq = seq.Returns(2); // Right
            seq = seq.Returns(1); // 1, 1
            seq = seq.Returns(2); // Right
            seq = seq.Returns(1); // 2, 1
            seq = seq.Returns(2); // Right
            seq = seq.Returns(0); // 1, 1
            seq = seq.Returns(0); // Down
            seq = seq.Returns(1); // 3, 1
            seq = seq.Returns(2); // Right
            seq = seq.Returns(2); // 4, 1
            seq = seq.Returns(2); // Right
            seq = seq.Returns(0); // 2, 1
            seq = seq.Returns(0); // Down
            seq = seq.Returns(2); // 5, 1
            seq = seq.Returns(2); // Right
            seq = seq.Returns(3); // 6, 1
            seq = seq.Returns(2); // Right
            seq = seq.Returns(1); // 3, 1
            seq = seq.Returns(0); // Down

            var pathGen = new Mock<GridPathBranch<IGridPathTestContext, TestTile>> { CallBase = true };
            pathGen.Object.RoomRatio = new RandRange(46);
            pathGen.Object.BranchRatio = new RandRange(50);
            pathGen.Object.NoForcedBranches = true;

            var mockHalls = new Mock<IRandPicker<PermissiveRoomGen<IGridPathTestContext, TestTile>>>(MockBehavior.Strict);
            mockHalls.Setup(p => p.Pick(testRand.Object)).Returns(new TestGridRoomGen());
            pathGen.Object.GenericHalls = mockHalls.Object;
            var mockRooms = new Mock<IRandPicker<RoomGen<IGridPathTestContext, TestTile>>>(MockBehavior.Strict);
            Moq.Language.ISetupSequentialResult<RoomGen<IGridPathTestContext, TestTile>> roomSeq = mockRooms.SetupSequence(p => p.Pick(testRand.Object));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('A'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('B'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('C'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('D'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('E'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('F'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('G'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('H'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('I'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('J'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('K'));
            pathGen.Object.GenericRooms = mockRooms.Object;

            pathGen.Object.ApplyToPath(testRand.Object, floorPlan);

            TestGridFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);

            testRand.Verify(p => p.Next(It.IsAny<int>()), Times.Exactly(22));
            mockHalls.Verify(p => p.Pick(testRand.Object), Times.Exactly(10));
            mockRooms.Verify(p => p.Pick(testRand.Object), Times.Exactly(11));
        }

        [Test]
        public void CreatePath50PercentBranchExtend()
        {
            // to confirm that newly made branches also count as terminals
            string[] inGrid =
            {
                "0.0.0.0.0.0.0.0",
                ". . . . . . . .",
                "0.0.0.0.0.0.0.0",
                ". . . . . . . .",
                "0.0.0.0.0.0.0.0",
            };

            string[] outGrid =
            {
                "A#B#C#D#G#J.0.0",
                ". # # # . . . .",
                "0.E.H.K.0.0.0.0",
                ". # # # . . . .",
                "0.F.I.L.0.0.0.0",
            };

            TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid);
            TestGridFloorPlan compareFloorPlan = TestGridFloorPlan.InitGridToContext(outGrid);

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            Moq.Language.ISetupSequentialResult<int> seq = testRand.SetupSequence(p => p.Next(It.IsAny<int>()));
            seq = seq.Returns(0);
            seq = seq.Returns(0);
            testRand.Setup(p => p.Next(50, 50)).Returns(50);
            seq = seq.Returns(0); // 0, 0
            seq = seq.Returns(1); // Right
            seq = seq.Returns(1); // 1, 0
            seq = seq.Returns(1); // Right
            seq = seq.Returns(1); // 2, 0
            seq = seq.Returns(1); // Right
            seq = seq.Returns(0); // 1, 0
            seq = seq.Returns(0); // Down
            seq = seq.Returns(2); // 1, 1
            seq = seq.Returns(0); // Down
            seq = seq.Returns(1); // 3, 0
            seq = seq.Returns(1); // Right
            seq = seq.Returns(0); // 2, 0
            seq = seq.Returns(0); // Down
            seq = seq.Returns(3); // 2, 1
            seq = seq.Returns(0); // Down
            seq = seq.Returns(2); // 4, 0
            seq = seq.Returns(1); // Right
            seq = seq.Returns(1); // 3, 0
            seq = seq.Returns(0); // Down
            seq = seq.Returns(4); // 3, 1
            seq = seq.Returns(0); // Down

            var pathGen = new Mock<GridPathBranch<IGridPathTestContext, TestTile>> { CallBase = true };
            pathGen.Object.RoomRatio = new RandRange(50);
            pathGen.Object.BranchRatio = new RandRange(50);
            pathGen.Object.NoForcedBranches = true;

            var mockHalls = new Mock<IRandPicker<PermissiveRoomGen<IGridPathTestContext, TestTile>>>(MockBehavior.Strict);
            mockHalls.Setup(p => p.Pick(testRand.Object)).Returns(new TestGridRoomGen());
            pathGen.Object.GenericHalls = mockHalls.Object;
            var mockRooms = new Mock<IRandPicker<RoomGen<IGridPathTestContext, TestTile>>>(MockBehavior.Strict);
            Moq.Language.ISetupSequentialResult<RoomGen<IGridPathTestContext, TestTile>> roomSeq = mockRooms.SetupSequence(p => p.Pick(testRand.Object));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('A'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('B'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('C'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('D'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('E'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('F'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('G'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('H'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('I'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('J'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('K'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('L'));
            pathGen.Object.GenericRooms = mockRooms.Object;

            pathGen.Object.ApplyToPath(testRand.Object, floorPlan);

            TestGridFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);

            testRand.Verify(p => p.Next(It.IsAny<int>()), Times.Exactly(24));
            mockHalls.Verify(p => p.Pick(testRand.Object), Times.Exactly(11));
            mockRooms.Verify(p => p.Pick(testRand.Object), Times.Exactly(12));
        }

        [Test]
        public void CreatePath100PercentBranch()
        {
            string[] inGrid =
            {
                "0.0.0.0.0.0.0.0",
                ". . . . . . . .",
                "0.0.0.0.0.0.0.0",
                ". . . . . . . .",
                "0.0.0.0.0.0.0.0",
            };

            string[] outGrid =
            {
                "0.0.0.0.0.0.0.0",
                ". . . . . . . .",
                "A#B#C#E#G#I#K#M",
                ". # # # # # # .",
                "0.D.F.H.J.L.N.0",
            };

            TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid);
            TestGridFloorPlan compareFloorPlan = TestGridFloorPlan.InitGridToContext(outGrid);

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            Moq.Language.ISetupSequentialResult<int> seq = testRand.SetupSequence(p => p.Next(It.IsAny<int>()));
            seq = seq.Returns(0);
            seq = seq.Returns(1);
            testRand.Setup(p => p.Next(59, 59)).Returns(59);
            testRand.Setup(p => p.Next(100, 100)).Returns(100);
            seq = seq.Returns(0); // 0, 1
            seq = seq.Returns(2); // Right
            seq = seq.Returns(1); // 1, 1
            seq = seq.Returns(2); // Right
            seq = seq.Returns(0); // 1, 1
            seq = seq.Returns(0); // Down
            seq = seq.Returns(1); // 2, 1
            seq = seq.Returns(2); // Right
            seq = seq.Returns(1); // 2, 1
            seq = seq.Returns(0); // Down
            seq = seq.Returns(2); // 3, 1
            seq = seq.Returns(2); // Right
            seq = seq.Returns(2); // 3, 1
            seq = seq.Returns(0); // Down
            seq = seq.Returns(3); // 4, 1
            seq = seq.Returns(2); // Right
            seq = seq.Returns(3); // 4, 1
            seq = seq.Returns(0); // Down
            seq = seq.Returns(4); // 5, 1
            seq = seq.Returns(2); // Right
            seq = seq.Returns(4); // 5, 1
            seq = seq.Returns(0); // Down
            seq = seq.Returns(5); // 6, 1
            seq = seq.Returns(2); // Right
            seq = seq.Returns(5); // 6, 1
            seq = seq.Returns(0); // Down

            var pathGen = new Mock<GridPathBranch<IGridPathTestContext, TestTile>> { CallBase = true };
            pathGen.Object.RoomRatio = new RandRange(59);
            pathGen.Object.BranchRatio = new RandRange(100);
            pathGen.Object.NoForcedBranches = true;

            var mockHalls = new Mock<IRandPicker<PermissiveRoomGen<IGridPathTestContext, TestTile>>>(MockBehavior.Strict);
            mockHalls.Setup(p => p.Pick(testRand.Object)).Returns(new TestGridRoomGen());
            pathGen.Object.GenericHalls = mockHalls.Object;
            var mockRooms = new Mock<IRandPicker<RoomGen<IGridPathTestContext, TestTile>>>(MockBehavior.Strict);
            Moq.Language.ISetupSequentialResult<RoomGen<IGridPathTestContext, TestTile>> roomSeq = mockRooms.SetupSequence(p => p.Pick(testRand.Object));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('A'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('B'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('C'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('D'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('E'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('F'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('G'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('H'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('I'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('J'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('K'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('L'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('M'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('N'));
            pathGen.Object.GenericRooms = mockRooms.Object;

            pathGen.Object.ApplyToPath(testRand.Object, floorPlan);

            TestGridFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);

            testRand.Verify(p => p.Next(It.IsAny<int>()), Times.Exactly(28));
            mockHalls.Verify(p => p.Pick(testRand.Object), Times.Exactly(13));
            mockRooms.Verify(p => p.Pick(testRand.Object), Times.Exactly(14));
        }

        [Test]
        public void CreatePath200PercentBranch()
        {
            string[] inGrid =
            {
                "0.0.0.0.0.0.0.0",
                ". . . . . . . .",
                "0.0.0.0.0.0.0.0",
                ". . . . . . . .",
                "0.0.0.0.0.0.0.0",
            };

            string[] outGrid =
            {
                "0.E.H.K.N.Q.T.0",
                ". # # # # # # .",
                "A#B#C#F#I#L#O#R",
                ". # # # # # # .",
                "0.D.G.J.M.P.S.0",
            };

            TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid);
            TestGridFloorPlan compareFloorPlan = TestGridFloorPlan.InitGridToContext(outGrid);

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            Moq.Language.ISetupSequentialResult<int> seq = testRand.SetupSequence(p => p.Next(It.IsAny<int>()));
            seq = seq.Returns(0);
            seq = seq.Returns(1);
            testRand.Setup(p => p.Next(84, 84)).Returns(84);
            testRand.Setup(p => p.Next(200, 200)).Returns(200);
            seq = seq.Returns(0); // 0, 1
            seq = seq.Returns(2); // Right
            seq = seq.Returns(1); // 1, 1
            seq = seq.Returns(2); // Right
            seq = seq.Returns(0); // 1, 1
            seq = seq.Returns(0); // Down
            seq = seq.Returns(0); // 1, 1
            seq = seq.Returns(0); // Up
            seq = seq.Returns(1); // 2, 1
            seq = seq.Returns(2); // Right
            seq = seq.Returns(0); // 2, 1
            seq = seq.Returns(0); // Down
            seq = seq.Returns(0); // 2, 1
            seq = seq.Returns(0); // Up
            seq = seq.Returns(3); // 3, 1
            seq = seq.Returns(2); // Right
            seq = seq.Returns(0); // 3, 1
            seq = seq.Returns(0); // Down
            seq = seq.Returns(0); // 3, 1
            seq = seq.Returns(0); // Up
            seq = seq.Returns(5); // 4, 1
            seq = seq.Returns(2); // Right
            seq = seq.Returns(0); // 4, 1
            seq = seq.Returns(0); // Down
            seq = seq.Returns(0); // 4, 1
            seq = seq.Returns(0); // Up
            seq = seq.Returns(7); // 5, 1
            seq = seq.Returns(2); // Right
            seq = seq.Returns(0); // 5, 1
            seq = seq.Returns(0); // Down
            seq = seq.Returns(0); // 5, 1
            seq = seq.Returns(0); // Up
            seq = seq.Returns(9); // 6, 1
            seq = seq.Returns(2); // Right
            seq = seq.Returns(0); // 6, 1
            seq = seq.Returns(0); // Down
            seq = seq.Returns(0); // 6, 1
            seq = seq.Returns(0); // Up

            var pathGen = new Mock<GridPathBranch<IGridPathTestContext, TestTile>> { CallBase = true };
            pathGen.Object.RoomRatio = new RandRange(84);
            pathGen.Object.BranchRatio = new RandRange(200);
            pathGen.Object.NoForcedBranches = true;

            var mockHalls = new Mock<IRandPicker<PermissiveRoomGen<IGridPathTestContext, TestTile>>>(MockBehavior.Strict);
            mockHalls.Setup(p => p.Pick(testRand.Object)).Returns(new TestGridRoomGen());
            pathGen.Object.GenericHalls = mockHalls.Object;
            var mockRooms = new Mock<IRandPicker<RoomGen<IGridPathTestContext, TestTile>>>(MockBehavior.Strict);
            Moq.Language.ISetupSequentialResult<RoomGen<IGridPathTestContext, TestTile>> roomSeq = mockRooms.SetupSequence(p => p.Pick(testRand.Object));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('A'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('B'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('C'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('D'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('E'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('F'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('G'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('H'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('I'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('J'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('K'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('L'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('M'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('N'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('O'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('P'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('Q'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('R'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('S'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('T'));
            pathGen.Object.GenericRooms = mockRooms.Object;

            pathGen.Object.ApplyToPath(testRand.Object, floorPlan);

            TestGridFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);

            testRand.Verify(p => p.Next(It.IsAny<int>()), Times.Exactly(40));
            mockHalls.Verify(p => p.Pick(testRand.Object), Times.Exactly(19));
            mockRooms.Verify(p => p.Pick(testRand.Object), Times.Exactly(20));
        }
    }
}
