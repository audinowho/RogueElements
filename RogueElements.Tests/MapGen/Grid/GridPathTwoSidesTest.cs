// <copyright file="GridPathTwoSidesTest.cs" company="Audino">
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
    public class GridPathTwoSidesTest
    {
        [Test]
        [Ignore("TODO")]
        public void PlacePathWithComponent()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void CreateError()
        {
            string[] inGrid =
            {
                "0",
                ".",
                "0",
            };

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);

            var pathGen = new GridPathTwoSides<IGridPathTestContext> { GapAxis = Axis4.Horiz };

            Mock<IRandPicker<PermissiveRoomGen<IGridPathTestContext>>> mockHalls = new Mock<IRandPicker<PermissiveRoomGen<IGridPathTestContext>>>(MockBehavior.Strict);
            pathGen.GenericHalls = mockHalls.Object;
            Mock<IRandPicker<RoomGen<IGridPathTestContext>>> mockRooms = new Mock<IRandPicker<RoomGen<IGridPathTestContext>>>(MockBehavior.Strict);
            pathGen.GenericRooms = mockRooms.Object;

            TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid);

            Assert.Throws<InvalidOperationException>(() => { pathGen.ApplyToPath(testRand.Object, floorPlan); });
        }

        [Test]
        public void CreatePathMinSize()
        {
            string[] inGrid =
            {
                "0.0",
            };

            string[] outGrid =
            {
                "A#B",
            };

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);

            var pathGen = new GridPathTwoSides<IGridPathTestContext> { GapAxis = Axis4.Horiz };

            Mock<IRandPicker<PermissiveRoomGen<IGridPathTestContext>>> mockHalls = new Mock<IRandPicker<PermissiveRoomGen<IGridPathTestContext>>>(MockBehavior.Strict);
            mockHalls.Setup(p => p.Pick(testRand.Object)).Returns(new TestGridRoomGen());
            pathGen.GenericHalls = mockHalls.Object;
            Mock<IRandPicker<RoomGen<IGridPathTestContext>>> mockRooms = new Mock<IRandPicker<RoomGen<IGridPathTestContext>>>(MockBehavior.Strict);
            Moq.Language.ISetupSequentialResult<RoomGen<IGridPathTestContext>> roomSeq = mockRooms.SetupSequence(p => p.Pick(testRand.Object));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('A'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('B'));
            pathGen.GenericRooms = mockRooms.Object;

            TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid);
            TestGridFloorPlan compareFloorPlan = TestGridFloorPlan.InitGridToContext(outGrid);

            pathGen.ApplyToPath(testRand.Object, floorPlan);

            TestGridFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);

            mockHalls.Verify(p => p.Pick(testRand.Object), Times.Exactly(1));
            mockRooms.Verify(p => p.Pick(testRand.Object), Times.Exactly(2));
        }

        [Test]
        public void CreatePathMedSize()
        {
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
                "A#C.C#B",
                "# . . .",
                "D#F.F#E",
                "# . . .",
                "G#I.I#H",
            };

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(2)).Returns(0);

            var pathGen = new Mock<GridPathTwoSides<IGridPathTestContext>> { CallBase = true };
            pathGen.Object.GapAxis = Axis4.Horiz;

            Moq.Language.ISetupSequentialResult<RoomGen<IGridPathTestContext>> defaultSeq = pathGen.SetupSequence(p => p.GetDefaultGen());
            defaultSeq = defaultSeq.Returns(new TestGridRoomGen('C'));
            defaultSeq = defaultSeq.Returns(new TestGridRoomGen('F'));
            defaultSeq = defaultSeq.Returns(new TestGridRoomGen('I'));

            Mock<IRandPicker<PermissiveRoomGen<IGridPathTestContext>>> mockHalls = new Mock<IRandPicker<PermissiveRoomGen<IGridPathTestContext>>>(MockBehavior.Strict);
            mockHalls.Setup(p => p.Pick(testRand.Object)).Returns(new TestGridRoomGen());
            pathGen.Object.GenericHalls = mockHalls.Object;
            Mock<IRandPicker<RoomGen<IGridPathTestContext>>> mockRooms = new Mock<IRandPicker<RoomGen<IGridPathTestContext>>>(MockBehavior.Strict);
            Moq.Language.ISetupSequentialResult<RoomGen<IGridPathTestContext>> roomSeq = mockRooms.SetupSequence(p => p.Pick(testRand.Object));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('A'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('B'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('D'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('E'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('G'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('H'));
            pathGen.Object.GenericRooms = mockRooms.Object;

            TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid);
            TestGridFloorPlan compareFloorPlan = TestGridFloorPlan.InitGridToContext(outGrid);

            pathGen.Object.ApplyToPath(testRand.Object, floorPlan);

            TestGridFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);

            testRand.Verify(p => p.Next(2), Times.Exactly(2));
            mockHalls.Verify(p => p.Pick(testRand.Object), Times.Exactly(8));
            mockRooms.Verify(p => p.Pick(testRand.Object), Times.Exactly(6));
            pathGen.Verify(p => p.GetDefaultGen(), Times.Exactly(3));
        }

        [Test]
        public void CreatePathVert()
        {
            string[] inGrid =
            {
                "0.0.0",
                ". . .",
                "0.0.0",
                ". . .",
                "0.0.0",
                ". . .",
                "0.0.0",
            };

            string[] outGrid =
            {
                "A#D#G",
                "# # #",
                "C.F.I",
                ". . .",
                "C.F.I",
                "# # #",
                "B.E.H",
            };

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(2)).Returns(0);

            var pathGen = new Mock<GridPathTwoSides<IGridPathTestContext>> { CallBase = true };
            pathGen.Object.GapAxis = Axis4.Vert;

            Moq.Language.ISetupSequentialResult<RoomGen<IGridPathTestContext>> defaultSeq = pathGen.SetupSequence(p => p.GetDefaultGen());
            defaultSeq = defaultSeq.Returns(new TestGridRoomGen('C'));
            defaultSeq = defaultSeq.Returns(new TestGridRoomGen('F'));
            defaultSeq = defaultSeq.Returns(new TestGridRoomGen('I'));

            Mock<IRandPicker<PermissiveRoomGen<IGridPathTestContext>>> mockHalls = new Mock<IRandPicker<PermissiveRoomGen<IGridPathTestContext>>>(MockBehavior.Strict);
            mockHalls.Setup(p => p.Pick(testRand.Object)).Returns(new TestGridRoomGen());
            pathGen.Object.GenericHalls = mockHalls.Object;
            Mock<IRandPicker<RoomGen<IGridPathTestContext>>> mockRooms = new Mock<IRandPicker<RoomGen<IGridPathTestContext>>>(MockBehavior.Strict);
            Moq.Language.ISetupSequentialResult<RoomGen<IGridPathTestContext>> roomSeq = mockRooms.SetupSequence(p => p.Pick(testRand.Object));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('A'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('B'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('D'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('E'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('G'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('H'));
            pathGen.Object.GenericRooms = mockRooms.Object;

            TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid);
            TestGridFloorPlan compareFloorPlan = TestGridFloorPlan.InitGridToContext(outGrid);

            pathGen.Object.ApplyToPath(testRand.Object, floorPlan);

            TestGridFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);

            testRand.Verify(p => p.Next(2), Times.Exactly(2));
            mockHalls.Verify(p => p.Pick(testRand.Object), Times.Exactly(8));
            mockRooms.Verify(p => p.Pick(testRand.Object), Times.Exactly(6));
            pathGen.Verify(p => p.GetDefaultGen(), Times.Exactly(3));
        }

        [Test]
        public void CreatePathRight()
        {
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
                "A#C.C#B",
                ". . . #",
                "D#F.F#E",
                ". . . #",
                "G#I.I#H",
            };

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(2)).Returns(1);

            var pathGen = new Mock<GridPathTwoSides<IGridPathTestContext>> { CallBase = true };
            pathGen.Object.GapAxis = Axis4.Horiz;

            Moq.Language.ISetupSequentialResult<RoomGen<IGridPathTestContext>> defaultSeq = pathGen.SetupSequence(p => p.GetDefaultGen());
            defaultSeq = defaultSeq.Returns(new TestGridRoomGen('C'));
            defaultSeq = defaultSeq.Returns(new TestGridRoomGen('F'));
            defaultSeq = defaultSeq.Returns(new TestGridRoomGen('I'));

            Mock<IRandPicker<PermissiveRoomGen<IGridPathTestContext>>> mockHalls = new Mock<IRandPicker<PermissiveRoomGen<IGridPathTestContext>>>(MockBehavior.Strict);
            mockHalls.Setup(p => p.Pick(testRand.Object)).Returns(new TestGridRoomGen());
            pathGen.Object.GenericHalls = mockHalls.Object;
            Mock<IRandPicker<RoomGen<IGridPathTestContext>>> mockRooms = new Mock<IRandPicker<RoomGen<IGridPathTestContext>>>(MockBehavior.Strict);
            Moq.Language.ISetupSequentialResult<RoomGen<IGridPathTestContext>> roomSeq = mockRooms.SetupSequence(p => p.Pick(testRand.Object));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('A'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('B'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('D'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('E'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('G'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('H'));
            pathGen.Object.GenericRooms = mockRooms.Object;

            TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid);
            TestGridFloorPlan compareFloorPlan = TestGridFloorPlan.InitGridToContext(outGrid);

            pathGen.Object.ApplyToPath(testRand.Object, floorPlan);

            TestGridFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);

            testRand.Verify(p => p.Next(2), Times.Exactly(2));
            mockHalls.Verify(p => p.Pick(testRand.Object), Times.Exactly(8));
            mockRooms.Verify(p => p.Pick(testRand.Object), Times.Exactly(6));
            pathGen.Verify(p => p.GetDefaultGen(), Times.Exactly(3));
        }

        [Test]
        public void CreatePathAlternating()
        {
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
                "A#C.C#B",
                "# . . .",
                "D#F.F#E",
                ". . . #",
                "G#I.I#H",
            };

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            Moq.Language.ISetupSequentialResult<int> seq = testRand.SetupSequence(p => p.Next(2));
            seq = seq.Returns(0);
            seq = seq.Returns(1);

            var pathGen = new Mock<GridPathTwoSides<IGridPathTestContext>> { CallBase = true };
            pathGen.Object.GapAxis = Axis4.Horiz;

            Moq.Language.ISetupSequentialResult<RoomGen<IGridPathTestContext>> defaultSeq = pathGen.SetupSequence(p => p.GetDefaultGen());
            defaultSeq = defaultSeq.Returns(new TestGridRoomGen('C'));
            defaultSeq = defaultSeq.Returns(new TestGridRoomGen('F'));
            defaultSeq = defaultSeq.Returns(new TestGridRoomGen('I'));

            Mock<IRandPicker<PermissiveRoomGen<IGridPathTestContext>>> mockHalls = new Mock<IRandPicker<PermissiveRoomGen<IGridPathTestContext>>>(MockBehavior.Strict);
            mockHalls.Setup(p => p.Pick(testRand.Object)).Returns(new TestGridRoomGen());
            pathGen.Object.GenericHalls = mockHalls.Object;
            Mock<IRandPicker<RoomGen<IGridPathTestContext>>> mockRooms = new Mock<IRandPicker<RoomGen<IGridPathTestContext>>>(MockBehavior.Strict);
            Moq.Language.ISetupSequentialResult<RoomGen<IGridPathTestContext>> roomSeq = mockRooms.SetupSequence(p => p.Pick(testRand.Object));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('A'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('B'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('D'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('E'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('G'));
            roomSeq = roomSeq.Returns(new TestGridRoomGen('H'));
            pathGen.Object.GenericRooms = mockRooms.Object;

            TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid);
            TestGridFloorPlan compareFloorPlan = TestGridFloorPlan.InitGridToContext(outGrid);

            pathGen.Object.ApplyToPath(testRand.Object, floorPlan);

            TestGridFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);

            testRand.Verify(p => p.Next(2), Times.Exactly(2));
            mockHalls.Verify(p => p.Pick(testRand.Object), Times.Exactly(8));
            mockRooms.Verify(p => p.Pick(testRand.Object), Times.Exactly(6));
            pathGen.Verify(p => p.GetDefaultGen(), Times.Exactly(3));
        }
    }
}
