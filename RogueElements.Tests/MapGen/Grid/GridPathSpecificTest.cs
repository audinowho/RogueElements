// <copyright file="GridPathSpecificTest.cs" company="Audino">
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
    public class GridPathSpecificTest
    {
        [Test]
        [Ignore("TODO")]
        public void PlacePathWithComponent()
        {
            throw new NotImplementedException();
        }

        [Test]
        [TestCase(3, 2, 3, 3)]
        [TestCase(5, 2, 3, 3)]
        [TestCase(4, 1, 3, 3)]
        [TestCase(4, 3, 3, 3)]
        [TestCase(4, 2, 2, 3)]
        [TestCase(4, 2, 4, 3)]
        [TestCase(4, 2, 3, 2)]
        [TestCase(4, 2, 3, 4)]
        public void CreatePathWrongDimensions(int vwidth, int vheight, int hwidth, int hheight)
        {
            string[] inGrid =
            {
                "0.0.0.0",
                ". . . .",
                "0.0.0.0",
                ". . . .",
                "0.0.0.0",
            };

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);

            var pathGen = new GridPathSpecific<IGridPathTestContext>
            {
                SpecificVHalls = new PermissiveRoomGen<IGridPathTestContext>[vwidth][],
            };
            for (int ii = 0; ii < vwidth; ii++)
                pathGen.SpecificVHalls[ii] = new PermissiveRoomGen<IGridPathTestContext>[vheight];
            pathGen.SpecificHHalls = new PermissiveRoomGen<IGridPathTestContext>[hwidth][];
            for (int ii = 0; ii < hwidth; ii++)
                pathGen.SpecificHHalls[ii] = new PermissiveRoomGen<IGridPathTestContext>[hheight];

            TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid);

            Assert.Throws<InvalidOperationException>(() => { pathGen.ApplyToPath(testRand.Object, floorPlan); });
        }

        [Test]
        public void CreatePathSpecific()
        {
            // specific halls and rooms
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
                "H#I.D",
                "# . #",
                "G#F#E",
            };

            TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid);
            TestGridFloorPlan compareFloorPlan = TestGridFloorPlan.InitGridToContext(outGrid);

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);

            var pathGen = new GridPathSpecific<IGridPathTestContext>
            {
                SpecificVHalls = new PermissiveRoomGen<IGridPathTestContext>[3][],
            };
            for (int ii = 0; ii < 3; ii++)
                pathGen.SpecificVHalls[ii] = new PermissiveRoomGen<IGridPathTestContext>[2];
            pathGen.SpecificHHalls = new PermissiveRoomGen<IGridPathTestContext>[2][];
            for (int ii = 0; ii < 2; ii++)
                pathGen.SpecificHHalls[ii] = new PermissiveRoomGen<IGridPathTestContext>[3];

            pathGen.SpecificRooms.Add(new SpecificGridRoomPlan<IGridPathTestContext>(new Rect(0, 0, 1, 1), new TestGridRoomGen('A')));
            pathGen.SpecificHHalls[0][0] = new TestGridRoomGen();
            pathGen.SpecificRooms.Add(new SpecificGridRoomPlan<IGridPathTestContext>(new Rect(1, 0, 1, 1), new TestGridRoomGen('B')));
            pathGen.SpecificHHalls[1][0] = new TestGridRoomGen();
            pathGen.SpecificRooms.Add(new SpecificGridRoomPlan<IGridPathTestContext>(new Rect(2, 0, 1, 1), new TestGridRoomGen('C')));
            pathGen.SpecificVHalls[2][0] = new TestGridRoomGen();
            pathGen.SpecificRooms.Add(new SpecificGridRoomPlan<IGridPathTestContext>(new Rect(2, 1, 1, 1), new TestGridRoomGen('D')));
            pathGen.SpecificVHalls[2][1] = new TestGridRoomGen();
            pathGen.SpecificRooms.Add(new SpecificGridRoomPlan<IGridPathTestContext>(new Rect(2, 2, 1, 1), new TestGridRoomGen('E')));
            pathGen.SpecificHHalls[1][2] = new TestGridRoomGen();
            pathGen.SpecificRooms.Add(new SpecificGridRoomPlan<IGridPathTestContext>(new Rect(1, 2, 1, 1), new TestGridRoomGen('F')));
            pathGen.SpecificHHalls[0][2] = new TestGridRoomGen();
            pathGen.SpecificRooms.Add(new SpecificGridRoomPlan<IGridPathTestContext>(new Rect(0, 2, 1, 1), new TestGridRoomGen('G')));
            pathGen.SpecificVHalls[0][1] = new TestGridRoomGen();
            pathGen.SpecificRooms.Add(new SpecificGridRoomPlan<IGridPathTestContext>(new Rect(0, 1, 1, 1), new TestGridRoomGen('H')));
            pathGen.SpecificHHalls[0][1] = new TestGridRoomGen();
            pathGen.SpecificRooms.Add(new SpecificGridRoomPlan<IGridPathTestContext>(new Rect(1, 1, 1, 1), new TestGridRoomGen('I')));

            pathGen.ApplyToPath(testRand.Object, floorPlan);

            TestGridFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);
        }
    }
}
