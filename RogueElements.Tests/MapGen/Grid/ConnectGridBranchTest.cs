// <copyright file="ConnectGridBranchTest.cs" company="Audino">
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
    public class ConnectGridBranchTest
    {
        [Test]
        public void StraightPath()
        {
            string[] inGrid =
            {
                "0.0.0.0",
                ". . . .",
                "0.A#B.0",
                ". . . .",
                "0.0.0.0",
            };

            string[] outGrid =
            {
                "0.0.0.0",
                ". . . .",
                "0.A#B.0",
                ". . . .",
                "0.0.0.0",
            };

            TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid);
            TestGridFloorPlan compareFloorPlan = TestGridFloorPlan.InitGridToContext(outGrid);

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(It.IsAny<int>())).Returns(0);

            var pathGen = new ConnectGridBranchStep<IGridPathTestContext> { ConnectPercent = 100 };

            Mock<IRandPicker<PermissiveRoomGen<IGridPathTestContext>>> mockHalls = new Mock<IRandPicker<PermissiveRoomGen<IGridPathTestContext>>>(MockBehavior.Strict);
            mockHalls.Setup(p => p.Pick(testRand.Object)).Returns(new TestGridRoomGen());
            pathGen.GenericHalls = mockHalls.Object;

            pathGen.ApplyToPath(testRand.Object, floorPlan);

            TestGridFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);
        }

        [Test]
        public void Wishbone()
        {
            string[] inGrid =
            {
                "0.C#D#E#F.0",
                ". # . . # .",
                "A#B.0.0.G.0",
                ". # . . . .",
                "0.H.0.0.M.0",
                ". # . . # .",
                "0.I#J#K#L.0",
            };

            string[] outGrid =
            {
                "0.C#D#E#F.0",
                ". # . . # .",
                "A#B.0.0.G.0",
                ". # . . # .",
                "0.H.0.0.M.0",
                ". # . . # .",
                "0.I#J#K#L.0",
            };

            TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid);
            TestGridFloorPlan compareFloorPlan = TestGridFloorPlan.InitGridToContext(outGrid);

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(It.IsAny<int>())).Returns(0);

            var pathGen = new ConnectGridBranchStep<IGridPathTestContext> { ConnectPercent = 100 };

            Mock<IRandPicker<PermissiveRoomGen<IGridPathTestContext>>> mockHalls = new Mock<IRandPicker<PermissiveRoomGen<IGridPathTestContext>>>(MockBehavior.Strict);
            mockHalls.Setup(p => p.Pick(testRand.Object)).Returns(new TestGridRoomGen());
            pathGen.GenericHalls = mockHalls.Object;

            pathGen.ApplyToPath(testRand.Object, floorPlan);

            TestGridFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);
        }

        [Test]
        public void SplitT()
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

            string[] outGrid =
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
            TestGridFloorPlan compareFloorPlan = TestGridFloorPlan.InitGridToContext(outGrid);

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(It.IsAny<int>())).Returns(0);

            var pathGen = new ConnectGridBranchStep<IGridPathTestContext> { ConnectPercent = 100 };

            Mock<IRandPicker<PermissiveRoomGen<IGridPathTestContext>>> mockHalls = new Mock<IRandPicker<PermissiveRoomGen<IGridPathTestContext>>>(MockBehavior.Strict);
            mockHalls.Setup(p => p.Pick(testRand.Object)).Returns(new TestGridRoomGen());
            pathGen.GenericHalls = mockHalls.Object;

            pathGen.ApplyToPath(testRand.Object, floorPlan);

            TestGridFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);
        }

        [Test]
        public void PassFork()
        {
            string[] inGrid =
            {
                "0.0.0.0",
                ". . . .",
                "0.A#B#C",
                ". # . .",
                "0.D#E.0",
            };

            string[] outGrid =
            {
                "0.0.0.0",
                ". . . .",
                "0.A#B#C",
                ". # # .",
                "0.D#E.0",
            };

            TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid);
            TestGridFloorPlan compareFloorPlan = TestGridFloorPlan.InitGridToContext(outGrid);

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(It.IsAny<int>())).Returns(0);

            var pathGen = new ConnectGridBranchStep<IGridPathTestContext> { ConnectPercent = 100 };

            Mock<IRandPicker<PermissiveRoomGen<IGridPathTestContext>>> mockHalls = new Mock<IRandPicker<PermissiveRoomGen<IGridPathTestContext>>>(MockBehavior.Strict);
            mockHalls.Setup(p => p.Pick(testRand.Object)).Returns(new TestGridRoomGen());
            pathGen.GenericHalls = mockHalls.Object;

            pathGen.ApplyToPath(testRand.Object, floorPlan);

            TestGridFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);
        }

        [Test]
        public void Flower()
        {
            string[] inGrid =
            {
                "0.0.A.0.0",
                ". . # . .",
                "F#E.B.H#I",
                ". # # # .",
                "0.D#C#G.0",
            };

            string[] outGrid =
            {
                "0.0.A.0.0",
                ". . # . .",
                "F#E#B#H#I",
                ". # # # .",
                "0.D#C#G.0",
            };

            TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid);
            TestGridFloorPlan compareFloorPlan = TestGridFloorPlan.InitGridToContext(outGrid);

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(It.IsAny<int>())).Returns(0);

            var pathGen = new ConnectGridBranchStep<IGridPathTestContext> { ConnectPercent = 100 };

            Mock<IRandPicker<PermissiveRoomGen<IGridPathTestContext>>> mockHalls = new Mock<IRandPicker<PermissiveRoomGen<IGridPathTestContext>>>(MockBehavior.Strict);
            mockHalls.Setup(p => p.Pick(testRand.Object)).Returns(new TestGridRoomGen());
            pathGen.GenericHalls = mockHalls.Object;

            pathGen.ApplyToPath(testRand.Object, floorPlan);

            TestGridFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);
        }

        [Test]
        public void Comb4A()
        {
            string[] inGrid =
            {
                "0.0.0.0.0",
                ". . . . .",
                "0.A#C#E#G",
                ". # # # #",
                "0.B.D.F.H",
                ". . . . .",
                "0.0.0.0.0",
            };

            string[] outGrid =
            {
                "0.0.0.0.0",
                ". . . . .",
                "0.A#C#E#G",
                ". # # # #",
                "0.B#D#F#H",
                ". . . . .",
                "0.0.0.0.0",
            };

            TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid);
            TestGridFloorPlan compareFloorPlan = TestGridFloorPlan.InitGridToContext(outGrid);

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(It.IsAny<int>())).Returns(0);

            var pathGen = new ConnectGridBranchStep<IGridPathTestContext> { ConnectPercent = 100 };

            Mock<IRandPicker<PermissiveRoomGen<IGridPathTestContext>>> mockHalls = new Mock<IRandPicker<PermissiveRoomGen<IGridPathTestContext>>>(MockBehavior.Strict);
            mockHalls.Setup(p => p.Pick(testRand.Object)).Returns(new TestGridRoomGen());
            pathGen.GenericHalls = mockHalls.Object;

            pathGen.ApplyToPath(testRand.Object, floorPlan);

            TestGridFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);
        }

        [Test]
        public void Comb4B()
        {
            string[] inGrid =
            {
                "0.0.0.0.0",
                ". . . . .",
                "0.A#C#E#G",
                ". # # # #",
                "0.B.D.F.H",
                ". . . . .",
                "0.0.0.0.0",
            };

            string[] outGrid =
            {
                "0.0.0.0.0",
                ". . . . .",
                "0.A#C#E#G",
                ". # # # #",
                "0.B#D.F#H",
                ". . . . .",
                "0.0.0.0.0",
            };

            TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid);
            TestGridFloorPlan compareFloorPlan = TestGridFloorPlan.InitGridToContext(outGrid);

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(It.IsNotIn<int>(2))).Returns(0);
            testRand.Setup(p => p.Next(2)).Returns(1);

            var pathGen = new ConnectGridBranchStep<IGridPathTestContext> { ConnectPercent = 100 };

            Mock<IRandPicker<PermissiveRoomGen<IGridPathTestContext>>> mockHalls = new Mock<IRandPicker<PermissiveRoomGen<IGridPathTestContext>>>(MockBehavior.Strict);
            mockHalls.Setup(p => p.Pick(testRand.Object)).Returns(new TestGridRoomGen());
            pathGen.GenericHalls = mockHalls.Object;

            pathGen.ApplyToPath(testRand.Object, floorPlan);

            TestGridFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);
        }

        [Test]
        public void Comb4Choose2A()
        {
            string[] inGrid =
            {
                "0.0.0.0.0",
                ". . . . .",
                "0.A#C#E#G",
                ". # # # #",
                "0.B.D.F.H",
                ". . . . .",
                "0.0.0.0.0",
            };

            string[] outGrid =
            {
                "0.0.0.0.0",
                ". . . . .",
                "0.A#C#E#G",
                ". # # # #",
                "0.B#D.F.H",
                ". . . . .",
                "0.0.0.0.0",
            };

            TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid);
            TestGridFloorPlan compareFloorPlan = TestGridFloorPlan.InitGridToContext(outGrid);

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(It.IsNotIn<int>(100))).Returns(0);
            Moq.Language.ISetupSequentialResult<int> seq = testRand.SetupSequence(p => p.Next(100));
            seq = seq.Returns(0);
            seq = seq.Returns(0);
            seq = seq.Returns(99);
            seq = seq.Returns(99);

            var pathGen = new ConnectGridBranchStep<IGridPathTestContext> { ConnectPercent = 50 };

            Mock<IRandPicker<PermissiveRoomGen<IGridPathTestContext>>> mockHalls = new Mock<IRandPicker<PermissiveRoomGen<IGridPathTestContext>>>(MockBehavior.Strict);
            mockHalls.Setup(p => p.Pick(testRand.Object)).Returns(new TestGridRoomGen());
            pathGen.GenericHalls = mockHalls.Object;

            pathGen.ApplyToPath(testRand.Object, floorPlan);

            TestGridFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);
        }

        [Test]
        public void Comb4Choose2B()
        {
            string[] inGrid =
            {
                "0.0.0.0.0",
                ". . . . .",
                "0.A#C#E#G",
                ". # # # #",
                "0.B.D.F.H",
                ". . . . .",
                "0.0.0.0.0",
            };

            string[] outGrid =
            {
                "0.0.0.0.0",
                ". . . . .",
                "0.A#C#E#G",
                ". # # # #",
                "0.B.D#F.H",
                ". . . . .",
                "0.0.0.0.0",
            };

            TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid);
            TestGridFloorPlan compareFloorPlan = TestGridFloorPlan.InitGridToContext(outGrid);

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(It.IsNotIn<int>(100, 4))).Returns(0);
            testRand.Setup(p => p.Next(4)).Returns(2);
            Moq.Language.ISetupSequentialResult<int> seq = testRand.SetupSequence(p => p.Next(100));
            seq = seq.Returns(0);
            seq = seq.Returns(0);
            seq = seq.Returns(99);
            seq = seq.Returns(99);

            var pathGen = new ConnectGridBranchStep<IGridPathTestContext> { ConnectPercent = 50 };

            Mock<IRandPicker<PermissiveRoomGen<IGridPathTestContext>>> mockHalls = new Mock<IRandPicker<PermissiveRoomGen<IGridPathTestContext>>>(MockBehavior.Strict);
            mockHalls.Setup(p => p.Pick(testRand.Object)).Returns(new TestGridRoomGen());
            pathGen.GenericHalls = mockHalls.Object;

            pathGen.ApplyToPath(testRand.Object, floorPlan);

            TestGridFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);
        }

        [Test]
        public void Comb4Choose3A()
        {
            string[] inGrid =
            {
                "0.0.0.0.0",
                ". . . . .",
                "0.A#C#E#G",
                ". # # # #",
                "0.B.D.F.H",
                ". . . . .",
                "0.0.0.0.0",
            };

            string[] outGrid =
            {
                "0.0.0.0.0",
                ". . . . .",
                "0.A#C#E#G",
                ". # # # #",
                "0.B#D#F.H",
                ". . . . .",
                "0.0.0.0.0",
            };

            TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid);
            TestGridFloorPlan compareFloorPlan = TestGridFloorPlan.InitGridToContext(outGrid);

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(It.IsNotIn<int>(100, 4))).Returns(0);
            testRand.Setup(p => p.Next(4)).Returns(2);
            Moq.Language.ISetupSequentialResult<int> seq = testRand.SetupSequence(p => p.Next(100));
            seq = seq.Returns(0);
            seq = seq.Returns(0);
            seq = seq.Returns(0);
            seq = seq.Returns(99);

            var pathGen = new ConnectGridBranchStep<IGridPathTestContext> { ConnectPercent = 50 };

            Mock<IRandPicker<PermissiveRoomGen<IGridPathTestContext>>> mockHalls = new Mock<IRandPicker<PermissiveRoomGen<IGridPathTestContext>>>(MockBehavior.Strict);
            mockHalls.Setup(p => p.Pick(testRand.Object)).Returns(new TestGridRoomGen());
            pathGen.GenericHalls = mockHalls.Object;

            pathGen.ApplyToPath(testRand.Object, floorPlan);

            TestGridFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);
        }

        [Test]
        public void Comb4Choose3B()
        {
            string[] inGrid =
            {
                "0.0.0.0.0",
                ". . . . .",
                "0.A#C#E#G",
                ". # # # #",
                "0.B.D.F.H",
                ". . . . .",
                "0.0.0.0.0",
            };

            string[] outGrid =
            {
                "0.0.0.0.0",
                ". . . . .",
                "0.A#C#E#G",
                ". # # # #",
                "0.B#D.F#H",
                ". . . . .",
                "0.0.0.0.0",
            };

            TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid);
            TestGridFloorPlan compareFloorPlan = TestGridFloorPlan.InitGridToContext(outGrid);

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(It.IsNotIn<int>(100, 4))).Returns(0);
            testRand.Setup(p => p.Next(4)).Returns(3);
            Moq.Language.ISetupSequentialResult<int> seq = testRand.SetupSequence(p => p.Next(100));
            seq = seq.Returns(0);
            seq = seq.Returns(0);
            seq = seq.Returns(0);
            seq = seq.Returns(99);

            var pathGen = new ConnectGridBranchStep<IGridPathTestContext> { ConnectPercent = 50 };

            Mock<IRandPicker<PermissiveRoomGen<IGridPathTestContext>>> mockHalls = new Mock<IRandPicker<PermissiveRoomGen<IGridPathTestContext>>>(MockBehavior.Strict);
            mockHalls.Setup(p => p.Pick(testRand.Object)).Returns(new TestGridRoomGen());
            pathGen.GenericHalls = mockHalls.Object;

            pathGen.ApplyToPath(testRand.Object, floorPlan);

            TestGridFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);
        }
    }
}
