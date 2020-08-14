// <copyright file="SetGridDefaultsTest.cs" company="Audino">
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
    public class SetGridDefaultsTest
    {
        [Test]
        public void StraightPath0Percent()
        {
            string[] inGrid =
            {
                "0.0.0.0.0.0.0",
                ". . . . . . .",
                "0.A#B#C#D#E.0",
                ". . . . . . .",
                "0.0.0.0.0.0.0",
            };

            TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid);

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(0, 0)).Returns(0);
            testRand.Setup(p => p.Next(It.IsAny<int>())).Returns(0);

            var pathGen = new SetGridDefaultsStep<IGridPathTestContext> { DefaultRatio = new RandRange(0) };

            pathGen.ApplyToPath(testRand.Object, floorPlan);

            // check the rooms
            Assert.That(floorPlan.RoomCount, Is.EqualTo(5));
            Assert.That(floorPlan.GetRoomPlan(0).RoomGen, Is.TypeOf<TestGridRoomGen>());
            Assert.That(floorPlan.GetRoomPlan(1).RoomGen, Is.TypeOf<TestGridRoomGen>());
            Assert.That(floorPlan.GetRoomPlan(2).RoomGen, Is.TypeOf<TestGridRoomGen>());
            Assert.That(floorPlan.GetRoomPlan(3).RoomGen, Is.TypeOf<TestGridRoomGen>());
            Assert.That(floorPlan.GetRoomPlan(4).RoomGen, Is.TypeOf<TestGridRoomGen>());
        }

        [Test]
        public void StraightPath50Percent()
        {
            string[] inGrid =
            {
                "0.0.0.0.0.0.0",
                ". . . . . . .",
                "0.A#B#C#D#E#F",
                ". . . . . . .",
                "0.0.0.0.0.0.0",
            };

            TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid);

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(50, 50)).Returns(50);
            testRand.Setup(p => p.Next(It.IsAny<int>())).Returns(0);

            var pathGen = new SetGridDefaultsStep<IGridPathTestContext> { DefaultRatio = new RandRange(50) };

            pathGen.ApplyToPath(testRand.Object, floorPlan);

            // check the rooms
            Assert.That(floorPlan.RoomCount, Is.EqualTo(6));
            Assert.That(floorPlan.GetRoomPlan(0).RoomGen, Is.TypeOf<TestGridRoomGen>());
            Assert.That(floorPlan.GetRoomPlan(1).RoomGen, Is.TypeOf<RoomGenDefault<IGridPathTestContext>>());
            Assert.That(floorPlan.GetRoomPlan(2).RoomGen, Is.TypeOf<RoomGenDefault<IGridPathTestContext>>());
            Assert.That(floorPlan.GetRoomPlan(3).RoomGen, Is.TypeOf<TestGridRoomGen>());
            Assert.That(floorPlan.GetRoomPlan(4).RoomGen, Is.TypeOf<TestGridRoomGen>());
            Assert.That(floorPlan.GetRoomPlan(5).RoomGen, Is.TypeOf<TestGridRoomGen>());
        }

        [Test]
        public void StraightPath100Percent()
        {
            string[] inGrid =
            {
                "0.0.0.0.0.0.0",
                ". . . . . . .",
                "0.A#B#C#D#E.0",
                ". . . . . . .",
                "0.0.0.0.0.0.0",
            };

            TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid);

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(100, 100)).Returns(100);
            testRand.Setup(p => p.Next(It.IsAny<int>())).Returns(0);

            var pathGen = new SetGridDefaultsStep<IGridPathTestContext> { DefaultRatio = new RandRange(100) };

            pathGen.ApplyToPath(testRand.Object, floorPlan);

            // check the rooms
            Assert.That(floorPlan.RoomCount, Is.EqualTo(5));
            Assert.That(floorPlan.GetRoomPlan(0).RoomGen, Is.TypeOf<TestGridRoomGen>());
            Assert.That(floorPlan.GetRoomPlan(1).RoomGen, Is.TypeOf<RoomGenDefault<IGridPathTestContext>>());
            Assert.That(floorPlan.GetRoomPlan(2).RoomGen, Is.TypeOf<RoomGenDefault<IGridPathTestContext>>());
            Assert.That(floorPlan.GetRoomPlan(3).RoomGen, Is.TypeOf<RoomGenDefault<IGridPathTestContext>>());
            Assert.That(floorPlan.GetRoomPlan(4).RoomGen, Is.TypeOf<TestGridRoomGen>());
        }

        [Test]
        public void BranchedPath100Percent()
        {
            string[] inGrid =
            {
                "0.0.E.F.0.0",
                ". . # # . .",
                "0.A#B#C#D.0",
                ". . # # . .",
                "0.0.G.H.0.0",
            };

            TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid);

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(100, 100)).Returns(100);
            testRand.Setup(p => p.Next(It.IsAny<int>())).Returns(0);

            var pathGen = new SetGridDefaultsStep<IGridPathTestContext> { DefaultRatio = new RandRange(100) };

            pathGen.ApplyToPath(testRand.Object, floorPlan);

            // check the rooms
            Assert.That(floorPlan.RoomCount, Is.EqualTo(8));
            Assert.That(floorPlan.GetRoomPlan(0).RoomGen, Is.TypeOf<TestGridRoomGen>());
            Assert.That(floorPlan.GetRoomPlan(1).RoomGen, Is.TypeOf<RoomGenDefault<IGridPathTestContext>>());
            Assert.That(floorPlan.GetRoomPlan(2).RoomGen, Is.TypeOf<RoomGenDefault<IGridPathTestContext>>());
            Assert.That(floorPlan.GetRoomPlan(3).RoomGen, Is.TypeOf<TestGridRoomGen>());
            Assert.That(floorPlan.GetRoomPlan(4).RoomGen, Is.TypeOf<TestGridRoomGen>());
            Assert.That(floorPlan.GetRoomPlan(5).RoomGen, Is.TypeOf<TestGridRoomGen>());
            Assert.That(floorPlan.GetRoomPlan(6).RoomGen, Is.TypeOf<TestGridRoomGen>());
            Assert.That(floorPlan.GetRoomPlan(7).RoomGen, Is.TypeOf<TestGridRoomGen>());
        }

        [Test]
        public void Immutable100Percent()
        {
            string[] inGrid =
            {
                "0.0.0.0.0.0.0",
                ". . . . . . .",
                "0.A#B#C#D#E.0",
                ". . . . . . .",
                "0.0.0.0.0.0.0",
            };

            TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid);
            GridRoomPlan roomPlan = floorPlan.GetRoomPlan(2);
            roomPlan.Components.Set(new TestComponent());

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(100, 100)).Returns(100);
            testRand.Setup(p => p.Next(It.IsAny<int>())).Returns(0);

            var pathGen = new SetGridDefaultsStep<IGridPathTestContext> { DefaultRatio = new RandRange(100) };
            pathGen.Filters.Add(new RoomFilterComponent(true, new TestComponent()));

            pathGen.ApplyToPath(testRand.Object, floorPlan);

            // check the rooms
            Assert.That(floorPlan.RoomCount, Is.EqualTo(5));
            Assert.That(floorPlan.GetRoomPlan(0).RoomGen, Is.TypeOf<TestGridRoomGen>());
            Assert.That(floorPlan.GetRoomPlan(1).RoomGen, Is.TypeOf<RoomGenDefault<IGridPathTestContext>>());
            Assert.That(floorPlan.GetRoomPlan(2).RoomGen, Is.TypeOf<TestGridRoomGen>());
            Assert.That(floorPlan.GetRoomPlan(3).RoomGen, Is.TypeOf<RoomGenDefault<IGridPathTestContext>>());
            Assert.That(floorPlan.GetRoomPlan(4).RoomGen, Is.TypeOf<TestGridRoomGen>());
        }
    }
}
